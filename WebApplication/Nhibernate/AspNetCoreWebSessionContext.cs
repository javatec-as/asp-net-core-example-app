using System;
using System.Collections;
using Microsoft.AspNetCore.Http;
using NHibernate.Context;
using NHibernate.Engine;

namespace WebApplication.Nhibernate
{
    [Serializable]
    public class AspNetCoreWebSessionContext : MapBasedSessionContext
    {
        private const string SessionFactoryMapKey = "NHibernate.Context.AspNetCoreWebSessionContext.SessionFactoryMapKey";

        private static Lazy<IHttpContextAccessor> _httpContextAccessor;

        public static Func<IHttpContextAccessor> ContextAccessorInitializer
        {
            set => _httpContextAccessor = new Lazy<IHttpContextAccessor>(value);
        }

        public AspNetCoreWebSessionContext(ISessionFactoryImplementor factory) : base(factory)
        {
        }

        protected override IDictionary GetMap()
        {
            return _httpContextAccessor.Value.HttpContext.Items[SessionFactoryMapKey] as IDictionary;
        }

        protected override void SetMap(IDictionary value)
        {
            _httpContextAccessor.Value.HttpContext.Items[SessionFactoryMapKey] = value;
        }
    }
}