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
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Conditions;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Exceptions;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class PropertyCompareTests
    {
        private ServiceContainer svcProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.svcProvider = new ServiceContainer();
            var metadataProvider = new StandardMetadataProvider();
            this.svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property is equal
        /// to a string constant.
        /// </summary>
        [TestMethod]
        public void StringPropEqualToConstant()
        {
            var person = new Person()
            {
                FirstName = "Fred"
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "FirstName",
                PropertyValue = "Fred",
                Operator = PropertyCompareOps.AreEqual
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property contains
        /// string constant.
        /// </summary>
        [TestMethod]
        public void StringPropContainsConstant()
        {
            var person = new Person()
            {
                FirstName = "Andrew"
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "FirstName",
                PropertyValue = "dr",
                Operator = PropertyCompareOps.Contains
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property starts with
        /// a string constant.
        /// </summary>
        [TestMethod]
        public void StringPropBeginsWithConstant()
        {
            var person = new Person()
            {
                FirstName = "Andrew"
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "FirstName",
                PropertyValue = "An",
                Operator = PropertyCompareOps.Contains
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property ends with
        /// a string constant.
        /// </summary>
        [TestMethod]
        public void StringPropEndsWithConstant()
        {
            var person = new Person()
            {
                FirstName = "Andrew"
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "FirstName",
                PropertyValue = "ew",
                Operator = PropertyCompareOps.Contains
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property is equal to
        /// a string variable.
        /// </summary>
        [TestMethod]
        public void StringPropEqualToVariable()
        {
            var person = new Person()
            {
                FirstName = "Johnson",
                LastName = "Johnson"
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "FirstName",
                PropertyValue = "$(LastName)",
                Operator = PropertyCompareOps.AreEqual
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if an integer property is greater than
        /// an integer constant.
        /// </summary>
        [TestMethod]
        public void IntPropGreaterThanConstant()
        {
            var person = new Person()
            {
                FirstName = "Bugs",
                LastName = "Bunny",
                DateOfBirth = new DateTime(1965, 2, 17)
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "AgeInYears",
                PropertyValue = 50,
                Operator = PropertyCompareOps.GreaterThan
            };
            var evalRes = propCompareCondition.Evaluate(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// verify that an exception is thrown when the property
        /// name does not exist in the context.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PropertyNotFoundException))]
        public void ComparePropertyNotFound()
        {
            var person = new Person()
            {
                FirstName = "Bugs",
                LastName = "Bunny",
                DateOfBirth = new DateTime(1965, 2, 17)
            };

            var propCompareCondition = new PropertyCompare()
            {
                PropertyName = "Age",
                PropertyValue = 50,
                Operator = PropertyCompareOps.GreaterThan
            };
            propCompareCondition.Evaluate(this.svcProvider, person);
        }
    }
}
