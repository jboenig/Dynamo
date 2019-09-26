using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamicExtObjectTests
    {
        [TestMethod]
        [ExpectedException(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))]
        public void TrySetDynamicPropOnStaticClass()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<Person>(null);

            // Attempt to access added property Foo
            person.Foo = "Hello";
        }

        [TestMethod]
        public void TrySetGetDeclaredDynamicPropOnDynamoDerivedClass()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(new object[] { metadataProvider });

            // Attempt to access added property Foo - should fail since Person
            // is not a dynamic type
            person.Foo = "Hello";
            Assert.AreEqual(person.Foo, "Hello");
        }

        [TestMethod]
        public void TrySetGetUndeclaredDynamicPropOnDynamoDerivedClass()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            //dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(new object[] { metadataProvider });

            // Attempt to access added property Foo - should fail since Person
            // is not a dynamic type
            person.Foo = "Hello";
            Assert.AreEqual(person.Foo, "Hello");

            var fooProp = dynamicPersonObjectType.GetPropertyByName("Foo");
            Assert.IsNotNull(fooProp);
        }
    }
}
