namespace Headway.AppCore.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public interface IObjectRepository<TObject> where TObject : class
    {
        /// <summary>
        /// 
        /// </summary>
        IQueryable<TObject> Objects { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Add(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Update(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Remove(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        void SaveChanges();
    }
}
