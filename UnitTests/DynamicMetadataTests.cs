﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.UnitTests.Mockdata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFileRepo;
using System.ComponentModel.Design;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamicMetadataTests
    {
        [TestMethod]
        public void VerifyReflectionPropertiesOnDynamicObjectType()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.DynamicProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
            Assert.IsNotNull(dynamicPersonObjectType);

            // Verify that DerivesFrom is System.Object
            Assert.AreEqual(dynamicPersonObjectType.DerivesFrom.FullName, "System.Object");

            // Verify that compile-time properties can be successfully found
            var firstNameProp = dynamicPersonObjectType.GetPropertyByName("FirstName");
            Assert.IsNotNull(firstNameProp);
            var lastNameProp = dynamicPersonObjectType.GetPropertyByName("LastName");
            Assert.IsNotNull(lastNameProp);
        }

        [TestMethod]
        public void AddPropertyToDynamicObjectType()
        {
            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();

            // Register Person as a dynamic object type and verify return value
            var dynamicPersonObjectType = metadataProvider.DynamicProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
            Assert.IsNotNull(dynamicPersonObjectType);

            // Add a new string property
            dynamicPersonObjectType.AddProperty("Foo", IntegralType.String);

            var fooProp = dynamicPersonObjectType.GetPropertyByName("Foo");
            Assert.AreEqual(fooProp.FullName, "Headway.Dynamo.UnitTests.Mockdata.Person.Foo");
            Assert.AreEqual(fooProp.DataType.FullName, "System.String");
        }

        [TestMethod]
        public void LoadMetadataFromJson()
        {
            var svcProvider = new ServiceContainer();

            // Use StandardMetadataProvider
            var metadataProvider = new StandardMetadataProvider();
            svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
            svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                @"MockData/SuperheroMetadata.json",
                svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService,
                svcProvider
                );

            var dynamicMetadataProvider = new DynamicMetadataProvider();
            dynamicMetadataProvider.Load(metadataRepo);

            var metahumanObjType = dynamicMetadataProvider.GetDataType<ObjectType>("Headway.Dynamo.UnitTests.Mockdata.Metahuman");
            Assert.IsNotNull(metahumanObjType);
            var propSuperpower1 = metahumanObjType.GetPropertyByName("Superpower1");
            Assert.IsNotNull(propSuperpower1);
        }
    }
}
