using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
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
                @"MockData/SuperheroMetadata.json",
                svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService,
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
                @"MockData/SuperheroMetadata.json",
                svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService,
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
            metaHuman.Addr = new Address()
            {
                Line1 = "123 Somewhere Pl",
                City = "Tulsa",
                State = "OK"
            };

            // Serialize object
            var serializerConfigSvc = svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            if (serializerConfigSvc == null)
            {
                throw new ServiceNotFoundException(typeof(ISerializerConfigService));
            }

            var objType = this.MetadataProvider.GetDataType<ObjectType>("Headway.Dynamo.UnitTests.Mockdata.Metahuman");

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                typeof(DynamoPerson),
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


        [TestMethod]
        public void SerializeDynamicObjectCollection()
        {
            // Use StandardMetadataProvider
            var svcProvider = new ServiceContainer();
            svcProvider.AddService(typeof(IMetadataProvider), this.MetadataProvider);
            svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                @"MockData/SuperheroMetadata.json",
                svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService,
                svcProvider
                );
            this.MetadataProvider.DynamicProvider.Load(metadataRepo);

            var metaHumans = new List<DynamoPerson>();
            
            dynamic metaHuman1 = this.MetadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman1);
            metaHuman1.Id = 1;
            metaHuman1.Superpower1 = "Kindness";
            var sp1 = metaHuman1.Superpower1;
            Assert.AreEqual(sp1, "Kindness");
            metaHuman1.FirstName = "Fred";
            metaHuman1.LastName = "Rogers";
            metaHuman1.DateOfBirth = new DateTime(1928, 3, 20);
            metaHuman1.Addr = new Address()
            {
                Line1 = "4802 Fifth Ave",
                City = "Pittsburgh",
                State = "PA"
            };

            metaHumans.Add(metaHuman1);

            dynamic metaHuman2 = this.MetadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman2);
            metaHuman2.Id = 2;
            metaHuman2.Superpower1 = "Future Vision";
            metaHuman2.Superpower2 = "Money";
            metaHuman2.FirstName = "Elon";
            metaHuman2.LastName = "Musk";
            metaHuman2.DateOfBirth = new DateTime(1971, 6, 28);
            metaHuman2.Addr = new Address()
            {
                Line1 = "3500 Deer Creek Road",
                City = "Palo Alto",
                State = "CA"
            };

            metaHumans.Add(metaHuman2);

            dynamic metaHuman3 = this.MetadataProvider.CreateInstance<DynamoPerson>(null,
                "Headway.Dynamo.UnitTests.Mockdata.Metahuman",
                null);
            Assert.IsNotNull(metaHuman2);
            metaHuman3.Id = 3;
            metaHuman3.Superpower1 = "Egomania";
            metaHuman3.Superpower2 = "Lying";
            metaHuman3.FirstName = "Donald";
            metaHuman3.LastName = "Trump";
            metaHuman3.DateOfBirth = new DateTime(1946, 6, 14);
            metaHuman3.Addr = new Address()
            {
                Line1 = "1600 Pennsylvania Ave NW",
                City = "Washington",
                State = "DC",
                PostalCode = "20500"
            };

            metaHumans.Add(metaHuman3);

            // Serialize object
            var serializerConfigSvc = svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            if (serializerConfigSvc == null)
            {
                throw new ServiceNotFoundException(typeof(ISerializerConfigService));
            }

            var objType = this.MetadataProvider.GetDataType<ObjectType>("Headway.Dynamo.UnitTests.Mockdata.Metahuman");

            var jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
                typeof(DynamoPerson),
                svcProvider);

            var jsonMetaHumans = JsonConvert.SerializeObject(metaHumans, jsonSettings);

            byte[] buffer;

            using (var stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.WriteLine(jsonMetaHumans);
                }
                buffer = stream.GetBuffer();
            }

            using (var stream = new MemoryStream(buffer))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    var deserializedMetaHumans = JsonConvert.DeserializeObject<List<DynamoPerson>>(sr.ReadToEnd(), jsonSettings);
                    Assert.IsNotNull(deserializedMetaHumans);

                    int deserializedMetaHumanCount = 0;

                    foreach (dynamic curDeserializedMetaHuman in deserializedMetaHumans)
                    {
                        var originalMetaHuman = (from mh in metaHumans
                                                 where mh.Id == curDeserializedMetaHuman.Id
                                                 select mh).FirstOrDefault();

                        Assert.IsNotNull(originalMetaHuman);
                        Assert.AreEqual(curDeserializedMetaHuman.FirstName,
                            originalMetaHuman.FirstName);
                        Assert.AreEqual(curDeserializedMetaHuman.MiddleName,
                            originalMetaHuman.MiddleName);
                        Assert.AreEqual(curDeserializedMetaHuman.LastName,
                            originalMetaHuman.LastName);
                        Assert.AreEqual(curDeserializedMetaHuman.DateOfBirth,
                            originalMetaHuman.DateOfBirth);
                        if (curDeserializedMetaHuman.Addr != null &&
                            originalMetaHuman.Addr != null)
                        {
                            Assert.AreEqual(curDeserializedMetaHuman.Addr.Line1,
                                originalMetaHuman.Addr.Line1);
                            Assert.AreEqual(curDeserializedMetaHuman.Addr.Line2,
                                originalMetaHuman.Addr.Line2);
                            Assert.AreEqual(curDeserializedMetaHuman.Addr.City,
                                originalMetaHuman.Addr.City);
                            Assert.AreEqual(curDeserializedMetaHuman.Addr.State,
                                originalMetaHuman.Addr.State);
                            Assert.AreEqual(curDeserializedMetaHuman.Addr.PostalCode,
                                originalMetaHuman.Addr.PostalCode);
                        }
                        else
                        {
                            Assert.AreEqual(curDeserializedMetaHuman.Addr,
                                originalMetaHuman.Addr);
                        }

                        deserializedMetaHumanCount++;
                    }

                    Assert.AreEqual(deserializedMetaHumanCount, metaHumans.Count);
                }
            }
        }
    }
}
