using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;

namespace WebApplication.Nhibernate
{
    public interface IReadOnlyRepo<T, in TId>
    {
        Task<T> GetAsync(TId id);
        Task<T> LoadAsync(TId id);
        Task<IList<T>> ListAsync();
    }

    public abstract class AbstractReadOnlyRepo<T, TId> : IReadOnlyRepo<T, TId>
    {
        private readonly ISessionManager _sessionManager;
        protected ISession CurrentSession => _sessionManager.GetOrCreateSession();

        protected AbstractReadOnlyRepo(ISessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public virtual async Task<T> GetAsync(TId id)
        {
            return await CurrentSession.GetAsync<T>(id);
        }

        public virtual async Task<T> LoadAsync(TId id)
        {
            return await CurrentSession.LoadAsync<T>(id);
        }

        public virtual async Task<IList<T>> ListAsync()
        {
            return await CurrentSession
                .CreateCriteria(typeof(T))
                .ListAsync<T>();
        }
    }
}