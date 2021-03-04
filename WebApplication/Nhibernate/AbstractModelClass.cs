namespace WebApplication.Nhibernate
{
    public abstract class AbstractModelClass<T>
    {
        public virtual T Id { get; set; }
    }
}