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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.UnitTests.Mockdata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.Implementation;
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
