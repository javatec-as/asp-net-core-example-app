using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Nhibernate;
using NHibernate.Linq;

namespace WebApplication.Repository
{
    public interface ICustomerRepo : IReadWriteRepo<Customer, int>
    {
        Task<IList<Customer>> GetCustomersWithContactsAsync();
    }

    public class CustomerRepo : AbstractReadWriteRepo<Customer, int>, ICustomerRepo
    {
        public CustomerRepo(ISessionManager sessionManager) : base(sessionManager) { }

        public async Task<IList<Customer>> GetCustomersWithContactsAsync()
        {
            return await CurrentSession
                .Query<Customer>()
                .Where( customer => customer.ContactsHbm.Any())
                .FetchMany(customer => customer.ContactsHbm)
                .ToListAsync();
        }
    }
}