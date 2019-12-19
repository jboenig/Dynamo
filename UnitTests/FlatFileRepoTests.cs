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
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.UnitTests.Mockdata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFile;
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
            var repo = new FlatFileRepo<Person>(serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new FlatFileRepo<Person>(serializerConfigSvc, this.svcProvider);
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
            var repo = new FlatFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new FlatFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
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
            var repo = new FlatFileRepo<Person>(filePath, serializerConfigSvc, this.svcProvider);
            repo.Add(this.personColl);
            repo.SaveChanges();

            Assert.IsTrue(File.Exists(filePath));

            var repo2 = new FlatFileRepo<Person>(relativePath, serializerConfigSvc, this.svcProvider);
            Assert.AreEqual(repo2.GetQueryable().Count(), this.personColl.Count());
        }
    }
}
