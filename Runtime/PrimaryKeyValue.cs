namespace Headway.AppCore.Runtime
{
    public sealed class PrimaryKeyValue
    {
        public PrimaryKeyValue()
        {
        }

        public PrimaryKeyValue(object value)
        {
            this.Value = value;
        }

        public object Value
        {
            get;
            set;
        }
    }
}
