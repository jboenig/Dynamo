using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Metadata.Dynamic;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class DynamicMetadataTests
    {
        [TestMethod]
        public void RegisterDynamicPersonClass()
        {
            var metadataProvider = new StandardMetadataProvider();
            var dynamicPersonObjectType = metadataProvider.RegisterObjectType(typeof(Person).FullName, typeof(Person));
            Assert.IsNotNull(dynamicPersonObjectType);
            Assert.AreEqual(dynamicPersonObjectType.DerivesFrom.FullName, "System.Object");
        }
    }
}
