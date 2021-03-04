using WebApplication.Nhibernate;

namespace WebApplication.Repository
{
    public class Contact : AbstractModelClass<int>
    {
        public virtual string Name { get; set; }
        public virtual Customer Customer { get; set; }
    }
}