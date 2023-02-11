namespace Headway.AppCore.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PrimaryKeyValue
    {
        /// <summary>
        /// 
        /// </summary>
        public PrimaryKeyValue()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PrimaryKeyValue(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get;
            set;
        }
    }
}
