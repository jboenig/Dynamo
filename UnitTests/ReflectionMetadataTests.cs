////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

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
