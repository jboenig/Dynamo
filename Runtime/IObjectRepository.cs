namespace Headway.AppCore.Runtime
{
    public interface IObjectRepository<TObject> where TObject : class
    {
        IQueryable<TObject> Objects { get; }
        void Add(TObject obj);
        void Update(TObject obj);
        void Remove(TObject obj);
        void SaveChanges();
    }
}
