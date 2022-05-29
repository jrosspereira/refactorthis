namespace RefactorThis.Persistence.Interfaces
{
    public interface IRepository<in T>
    {
        void Save(T entity);
    }
}
