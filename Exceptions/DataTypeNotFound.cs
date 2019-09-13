using System;

namespace Headway.Dynamo.Exceptions
{
    public sealed class DataTypeNotFound : Exception
    {
        public DataTypeNotFound(string dataTypeName) :
            base(string.Format("Data type {0} not found", dataTypeName))
        {
        }
    }
}
