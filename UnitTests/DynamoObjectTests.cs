using System;
using System.IO;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Exceptions;
using Headway.Dynamo.Repository.FlatFileRepo;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamoObjectTests
    {
        private StandardMetadataProvider MetadataProvider
        {
            get;
            set;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.MetadataProvider = new StandardMetadataProvider();
        }

        [TestMethod]
        [ExpectedException(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException))]
        public void TrySetDynamicPropOnStaticClass()
        {
            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = this.MetadataProvider.DynamicProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
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
            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = this.MetadataProvider.DynamicProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(null, null);

            // Attempt to access added property Foo - should fail since Person
            // is not a dynamic type
            person.Foo = "Hello";
            Assert.AreEqual(person.Foo, "Hello");
        }

        [TestMethod]
        public void TrySetGetUndeclaredDynamicPropOnDynamoDerivedClass()
        {
            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = this.MetadataProvider.DynamicProvider.RegisterObjectType(typeof(DynamoPerson).FullName, typeof(DynamoPerson));
            Assert.IsNotNull(dynamicPersonObjectType);
            // Add a new string property
            //dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            // Create instance of registered dynamic type
            dynamic person = dynamicPersonObjectType.CreateInstance<DynamoPerson>(null, null);

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
            svcProvider.AddService(typeof(IMetadataProvider), this.MetadataProvider);
            svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                this.MetadataProvider.GetDataType<ObjectType>(typeof(DynamicObjectType)),
                @"MockData/SuperheroMetadata.json",
                svcProvider
                );
            this.MetadataProvider.DynamicProvider.Load(metadataRepo);

            dynamic metaHuman = this.MetadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman);
            metaHuman.Superpower1 = "Invisibility";
            var sp1 = metaHuman.Superpower1;
            Assert.AreEqual(sp1, "Invisibility");
            metaHuman.FirstName = "Fred";
        }

        [TestMethod]
        public void SerializeDynamicObject()
        {
            // Use StandardMetadataProvider
            var svcProvider = new ServiceContainer();
            svcProvider.AddService(typeof(IMetadataProvider), this.MetadataProvider);
            svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                this.MetadataProvider.GetDataType<ObjectType>(typeof(DynamicObjectType)),
                @"MockData/SuperheroMetadata.json",
                svcProvider
                );
            this.MetadataProvider.DynamicProvider.Load(metadataRepo);

            dynamic metaHuman = this.MetadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman);
            metaHuman.Superpower1 = "Invisibility";
            var sp1 = metaHuman.Superpower1;
            Assert.AreEqual(sp1, "Invisibility");
            metaHuman.FirstName = "Fred";

            // Serialize object
            var serializerConfigSvc = svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            if (serializerConfigSvc == null)
            {
                throw new ServiceNotFoundException(typeof(ISerializerConfigService));
            }

            var objType = this.MetadataProvider.GetDataType<ObjectType>("Headway.Dynamo.UnitTests.Mockdata.Metahuman");

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                objType,
                svcProvider);

            var jsonMetaHuman = JsonConvert.SerializeObject(metaHuman, jsonSettings);

            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.WriteLine(jsonMetaHuman);
                }
                buffer = stream.GetBuffer();
            }

            using (var stream = new MemoryStream(buffer))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    dynamic metaHuman_2 = JsonConvert.DeserializeObject<DynamoPerson>(sr.ReadToEnd(), jsonSettings);
                    Assert.IsNotNull(metaHuman_2);
                    var sp1_2 = metaHuman_2.Superpower1;
                    Assert.AreEqual(sp1_2, "Invisibility");
                }
            }
        }
    }
}
