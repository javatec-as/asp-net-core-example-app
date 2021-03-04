using System.Threading.Tasks;

namespace WebApplication.Nhibernate
{
    public interface IReadWriteRepo<T, in TId> : IReadOnlyRepo<T, TId>
    {
        Task DeleteAsync(T entity);
        Task<T> SaveOrUpdateAsync(T entity);
        Task<T> SaveAsync(T entity);
        Task<T> UpdateAsync(T entity);
    }

    public abstract class AbstractReadWriteRepo<T, TId> : AbstractReadOnlyRepo<T, TId>, IReadWriteRepo<T, TId>
        where T : AbstractModelClass<TId>
    {
        protected AbstractReadWriteRepo(ISessionManager sessionManager) : base(sessionManager) { }

        public virtual async Task DeleteAsync(T entity)
        {
            await CurrentSession.DeleteAsync(entity);
        }

        public virtual async Task<T> SaveOrUpdateAsync(T entity)
        {
            await CurrentSession.SaveOrUpdateAsync(entity);
            return entity;
        }

        public virtual async Task<T> SaveAsync(T entity)
        {
            await CurrentSession.SaveAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            await CurrentSession.UpdateAsync(entity);
            return entity;
        }
    }
}