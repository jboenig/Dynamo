////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.
//
// Headway.Dynamo is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// Headway.Dynamo is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.UnitTests.Mockdata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFile;
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
            var extraStuffProp = dynamicPersonObjectType.GetPropertyByName("ExtraStuff");
            Assert.IsNotNull(extraStuffProp);
            Assert.IsNotNull(extraStuffProp.DataType);
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

            var metadataRepoFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"MockData/SuperheroMetadata.json");

            var metadataRepo = new FlatFileRepo<DynamicObjectType>(
                metadataRepoFilePath,
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
