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
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.UnitTests.Mockdata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.Implementation;
using System.ComponentModel.Design;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class FlatFileRepoTests
    {
        private ServiceContainer svcProvider;
        private IEnumerable<Person> personColl;

        [TestInitialize]
        public void TestInitialize()
        {
            this.svcProvider = new ServiceContainer();
            var metadataProvider = new StandardMetadataProvider();
            this.svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
            var standardSerializerConfig = new StandardSerializerConfigService();
            this.svcProvider.AddService(typeof(ISerializerConfigService), standardSerializerConfig);

            var personArray = new Person[]
            {
                new Person()
                {
                    Id = 1,
                    FirstName = "Fred",
                    LastName = "Flintstone",
                    DateOfBirth = new DateTime(200)
                },
                new Person()
                {
                    Id = 1,
                    FirstName = "Wilma",
                    LastName = "Flintstone",
                    DateOfBirth = new DateTime(201)
                },
                new Person()
                {
                    Id = 1,
                    FirstName = "Barney",
                    LastName = "Rubble",
                    DateOfBirth = new DateTime(300)
                },
                new Person()
                {
                    Id = 1,
                    FirstName = "Betty",
                    LastName = "Rubble",
                    DateOfBirth = new DateTime(301)
                }
            };

            this.personColl = personArray;
        }

        [TestMethod]
        public void ReadWriteDefaultLocation()
        {
            var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            Assert.IsNotNull(serializerConfigSvc);

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/" + "Headway",
                typeof(Person).FullName + ".json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Do not specify file path and allow default
            var repo = new JsonFileRepo<Person>(serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new JsonFileRepo<Person>(serializerConfigSvc, this.svcProvider);
            Assert.AreEqual(repo2.GetQueryable().Count(), this.personColl.Count());
        }

        [TestMethod]
        public void ReadWriteRelativePath()
        {
            var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            Assert.IsNotNull(serializerConfigSvc);

            var relativePath = "foo.json";

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/" + "Headway",
                relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Do not specify file path and allow default
            var repo = new JsonFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new JsonFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
            Assert.AreEqual(repo2.GetQueryable().Count(), this.personColl.Count());
        }

        [TestMethod]
        public void ReadWriteExplicitPath()
        {
            var serializerConfigSvc = this.svcProvider.GetService(typeof(ISerializerConfigService)) as ISerializerConfigService;
            Assert.IsNotNull(serializerConfigSvc);

            var relativePath = "foo.json";

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/" + "Headway",
                relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Do not specify file path and allow default
            var repo = new JsonFileRepo<Person>(filePath, serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new JsonFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
            Assert.AreEqual(repo2.GetQueryable().Count(), this.personColl.Count());
        }
    }
}
