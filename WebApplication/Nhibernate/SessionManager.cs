using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHibernate;
using NHibernate.Context;
using NHibernate.Impl;
using ISession = NHibernate.ISession;

namespace WebApplication.Nhibernate
{
    public interface ISessionManager
    {
        ISession GetOrCreateSession();
        Task FinalizeSessionAsync(bool doRollback = false);
    }

    public class SessionManager : ISessionManager
    {
        private const int LockTimeoutInMilliseconds = 100;
        private static readonly object FallbackLockObject = new();

        private readonly ISessionFactory _sessionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(ISessionFactory sessionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _sessionFactory = sessionFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public ISession GetOrCreateSession()
        {
            if (TryGetSession(out var session))
            {
                return session;
            }

            var lockObject = GetLockObject();

            if (!Monitor.TryEnter(lockObject, LockTimeoutInMilliseconds))
            {
                return null;
            }

            try
            {
                return CreateSession(true);
            }
            finally
            {
                Monitor.Exit(lockObject); // ensure that the lock is released.
            }
        }

        public async Task FinalizeSessionAsync(bool doRollback = false)
        {
            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                return;
            }

            var session = CurrentSessionContext.Unbind(_sessionFactory);
            var transaction = session?.GetCurrentTransaction();

            try
            {
                // if true, next step will be the finally block
                if (transaction?.IsActive != true || doRollback)
                {
                    return;
                }

                await session.FlushAsync(); // will throw exception if any errors
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                doRollback = true;
                throw;
            }
            finally
            {
                if (transaction?.IsActive == true && doRollback)
                {
                    await transaction.RollbackAsync();
                }
                session?.Dispose();
            }
        }

        private bool TryGetSession(out ISession session)
        {
            if (CurrentSessionContext.HasBind(_sessionFactory))
            {
                session = _sessionFactory.GetCurrentSession();
                return true;
            }

            if (_sessionFactory is SessionFactoryImpl sessionFactoryImpl 
                && (sessionFactoryImpl.CurrentSessionContext is ThreadStaticSessionContext 
                    || sessionFactoryImpl.CurrentSessionContext is CallSessionContext))
            {
                session = CreateSession();
                return true;
            }

            session = null;
            return false;
        }

        private object GetLockObject()
        {
            return _sessionFactory is SessionFactoryImpl {CurrentSessionContext: AspNetCoreWebSessionContext} 
                ? _httpContextAccessor.HttpContext 
                : FallbackLockObject;
        }

        private ISession CreateSession(bool doDoubleCheck = false)
        {
            // double check locking in cases where method is called inside a lock
            if (doDoubleCheck && CurrentSessionContext.HasBind(_sessionFactory))
            {
                return _sessionFactory.GetCurrentSession();
            }

            CurrentSessionContext.Bind(_sessionFactory.OpenSession());
            _sessionFactory.GetCurrentSession().BeginTransaction(IsolationLevel.ReadCommitted);
            return _sessionFactory.GetCurrentSession();
        }
    }
}