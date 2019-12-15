using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Reflection;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestClass]
    public class ReflectionMetadataTests
    {
        private ReflectionMetadataProvider metadataProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.metadataProvider = new ReflectionMetadataProvider();
        }

        [TestMethod]
        public void VerifyBasicClassInfo()
        {
            var personMetadata = this.metadataProvider.GetDataType<ObjectType>(typeof(Person).FullName);
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
            var bugsBunny = new Person()
            {
                FirstName = "Bugs",
                LastName = "Bunny"
            };

            var personMetadata = this.metadataProvider.GetDataType<ObjectType>(typeof(Person).FullName);

            var firstNameProp = personMetadata.GetPropertyByName("FirstName");
            Assert.IsNotNull(firstNameProp);
            var firstNameValue = firstNameProp.GetValue<string>(bugsBunny);
            Assert.AreEqual(firstNameValue, "Bugs");

            var lastNameProp = personMetadata.GetPropertyByName("LastName");
            Assert.IsNotNull(lastNameProp);
            var lastNameValue = lastNameProp.GetValue<string>(bugsBunny);
            Assert.AreEqual(lastNameValue, "Bunny");
        }

        [TestMethod]
        public void GetBaseClassProperty()
        {
            var employeeMetadata = this.metadataProvider.GetDataType<ObjectType>(typeof(Employee).FullName);

            var employeeIdProp = employeeMetadata.GetPropertyByName("EmployeeId");
            Assert.IsNotNull(employeeIdProp);
            Assert.AreEqual(employeeIdProp.DataType.Name, "String");
        }

        [TestMethod]
        public void GetBaseClassPropertyValue()
        {
            var personMetadata = this.metadataProvider.GetDataType<ObjectType>(typeof(Employee).FullName);

            var daffyDuck = new Employee()
            {
                FirstName = "Daffy",
                LastName = "Duck",
                EmployeeId = "L112"
            };

            var employeeIdProp = personMetadata.GetPropertyByName("EmployeeId");
            Assert.IsNotNull(employeeIdProp);

            var employeeIdValue = employeeIdProp.GetValue<string>(daffyDuck);
            Assert.AreEqual(employeeIdValue, "L112");
        }
    }
}
