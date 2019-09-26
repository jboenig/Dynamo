using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamoObjectTests
    {
        [TestMethod]
        public void GetSetDynamicProp()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var personObjType = metadataProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
            Assert.IsNotNull(personObjType);

            // Add a dynamic string property call Foo
            personObjType.AddProperty("Foo", IntegralType.String);

            // Create a new instance
            //            var obj = new DynamoObject(personObjType);
            dynamic obj = new DynamoPerson(metadataProvider);
            obj.FirstName = "Dude";
            obj.Foo = "hello world";
            var fooProp = personObjType.GetPropertyByName("Foo");
            Assert.AreEqual(fooProp.FullName, "Headway.Dynamo.UnitTests.Mockdata.Person.Foo");
            Assert.AreEqual(fooProp.DataType.FullName, "System.String");
        }
    }
}
