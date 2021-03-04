using System.Collections.Generic;
using WebApplication.Nhibernate;

namespace WebApplication.Repository
{
    public class Customer : AbstractModelClass<int>
    {
        public virtual string Name { get; set; }

        protected internal virtual ISet<Contact> ContactsHbm { get; set; } = new HashSet<Contact>();
        public virtual IList<Contact> Contacts => new List<Contact>(ContactsHbm).AsReadOnly();

    }
}