using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Reflection;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class ReflectionMetadataTests
    {
        [TestMethod]
        public void VerifyBasicClassInfo()
        {
            var metadataProvider = new ReflectionMetadataProvider();
            var personMetadata = metadataProvider.GetDataType<ObjectType>(typeof(Person).FullName);
            Assert.AreEqual(personMetadata.FullName, "Headway.Dynamo.UnitTests.Mockdata.Person");
            Assert.AreEqual(personMetadata.Name, "Person");
            Assert.AreEqual(personMetadata.Namespace, "Headway.Dynamo.UnitTests.Mockdata");
            Assert.AreEqual(personMetadata.IsEnumerable, false);
            Assert.AreEqual(personMetadata.ItemDataType, null);
            Assert.AreEqual(personMetadata.DefaultValue, default(Mockdata.Person));

            var firstNameProp = personMetadata.GetPropertyByName("FirstName");
            Assert.IsNotNull(firstNameProp);
            Assert.AreEqual(firstNameProp.DataType.Name, "String");
        }

        [TestMethod]
        public void GetStringPropertyValues()
        {
            var metadataProvider = new ReflectionMetadataProvider();

            var person = new Person()
            {
                FirstName = "Bugs",
                LastName = "Bunny"
            };

            var personMetadata = metadataProvider.GetDataType<ObjectType>(typeof(Person).FullName);

            var firstNameProp = personMetadata.GetPropertyByName("FirstName");
            Assert.IsNotNull(firstNameProp);
            var firstNameValue = firstNameProp.GetValue<string>(person);
            Assert.AreEqual(firstNameValue, "Bugs");

            var lastNameProp = personMetadata.GetPropertyByName("LastName");
            Assert.IsNotNull(lastNameProp);
            var lastNameValue = lastNameProp.GetValue<string>(person);
            Assert.AreEqual(lastNameValue, "Bunny");
        }
    }
}
