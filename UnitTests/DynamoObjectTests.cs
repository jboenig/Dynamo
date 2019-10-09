using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFileRepo;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamoObjectTests
    {
        [TestMethod]
        [ExpectedException(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))]
        public void TrySetDynamicPropOnStaticClass()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.DynamicProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
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
            var dynamicPersonObjectType = metadataProvider.DynamicProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(null, new object[] { metadataProvider });

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
            var dynamicPersonObjectType = metadataProvider.DynamicProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            //dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(null, new object[] { metadataProvider });

            // Attempt to access added property Foo - should fail since Person
            // is not a dynamic type
            person.Foo = "Hello";
            Assert.AreEqual(person.Foo, "Hello");

            var fooProp = dynamicPersonObjectType.GetPropertyByName("Foo");
            Assert.IsNotNull(fooProp);
        }

        [TestMethod]
        public void CreateDynamicObject()
        {
            // Use StandardMetadataProvider
            var svcProvider = new ServiceContainer();
            var metadataProvider = new StandardMetadataProvider();
            svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
            svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                metadataProvider.GetDataType<ObjectType>(typeof(DynamicObjectType)),
                @"MockData/SuperheroMetadata.json",
                svcProvider
                );
            metadataProvider.DynamicProvider.Load(metadataRepo);

            dynamic metaHuman = metadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman);
            metaHuman.Superpower1 = "Invisibility";
        }
    }
}
