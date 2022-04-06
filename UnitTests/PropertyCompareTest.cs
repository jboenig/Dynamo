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
using System.Threading.Tasks;
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
        public async Task StringPropEqualToConstant()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property contains
        /// string constant.
        /// </summary>
        [TestMethod]
        public async Task StringPropContainsConstant()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property starts with
        /// a string constant.
        /// </summary>
        [TestMethod]
        public async Task StringPropBeginsWithConstant()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property ends with
        /// a string constant.
        /// </summary>
        [TestMethod]
        public async Task StringPropEndsWithConstant()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if a string property is equal to
        /// a string variable.
        /// </summary>
        [TestMethod]
        public async Task StringPropEqualToVariable()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// test to see if an integer property is greater than
        /// an integer constant.
        /// </summary>
        [TestMethod]
        public async Task IntPropGreaterThanConstant()
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
            var evalRes = await propCompareCondition.EvaluateAsync(this.svcProvider, person);
            Assert.IsTrue(evalRes);
        }

        /// <summary>
        /// Test a <see cref="PropertyCompare"/> to
        /// verify that an exception is thrown when the property
        /// name does not exist in the context.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(PropertyNotFoundException))]
        public async Task ComparePropertyNotFound()
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
            await propCompareCondition.EvaluateAsync(this.svcProvider, person);
        }
    }
}
