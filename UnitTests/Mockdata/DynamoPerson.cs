using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.UnitTests.Mockdata
{
    public class DynamoPerson : DynamoObject
    {
        public DynamoPerson(ObjectType objType) :
            base(objType)
        {

        }
    }
}
