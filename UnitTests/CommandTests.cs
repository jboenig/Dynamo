﻿////////////////////////////////////////////////////////////////////////////////
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
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Conditions;
using Headway.Dynamo.Metadata;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Repository.FlatFile;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.UnitTests.Mockdata;

namespace Headway.Dynamo.UnitTests
{
    [TestClass]
    public class CommandTests
    {
        private ServiceContainer svcProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.svcProvider = new ServiceContainer();

            var metadataProvider = new StandardMetadataProvider();
            this.svcProvider.AddService(typeof(IMetadataProvider), metadataProvider);
            this.svcProvider.AddService(typeof(ISerializerConfigService), new StandardSerializerConfigService(null));
        }

        /// <summary>
        /// Test a <see cref="ConditionalCommand"/> using
        /// a null <see cref="ConditionalCommand.ExecuteWhen"/>
        /// value and verify that the command executes.
        /// </summary>
        [TestMethod]
        public void NullConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = null,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, "Dude");
        }

        /// <summary>
        /// Test a <see cref="ConditionalCommand"/> using
        /// a <see cref="Condition"/> that evaluates to false
        /// and verify that the command does not execute.
        /// </summary>
        [TestMethod]
        public void FalseConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = Condition.False,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, null);
        }

        /// <summary>
        /// Test a <see cref="ConditionalCommand"/> using
        /// a <see cref="Condition"/> that evaluates to true
        /// and verify that the command executes successfully.
        /// </summary>
        [TestMethod]
        public void TrueConditionalSynchronousCommand()
        {
            var person = new Person();
            var cmdConditional = new ConditionalCommand()
            {
                ExecuteWhen = Condition.True,
                Command = new SetPropertyValueCommand()
                {
                    PropertyName = "FirstName",
                    Value = "Dude"
                }
            };
            var cmdTask = cmdConditional.Execute(this.svcProvider, person);
            cmdTask.RunSynchronously();
            Assert.IsTrue(cmdTask.Result.IsSuccess);
            Assert.AreEqual(person.FirstName, "Dude");
        }

        /// <summary>
        /// Executes a <see cref="MacroCommand"/> with several
        /// commands synchronously and verifies successful
        /// execution.
        /// </summary>
        [TestMethod]
        public void SyncronousMacroCommand()
        {
            var cmdNoop = new DelegateCommand(() => true);
            var cmdAddNumbers = new DelegateCommand((s,c) =>
            {
                var v1 = PropertyResolver.GetPropertyValue<int>(c, "Val1");
                var v2 = PropertyResolver.GetPropertyValue<int>(c, "Val2");
                var sum = v1 + v2;
                return new BoolCommandResult(true, string.Format("Sum of {0} + {1} = {2}", v1, v2, sum));
            });
            var cmdMacro = new MacroCommand();
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.ExecuteAsync = false;
            var contextObj = new
            {
                Val1 = 2,
                Val2 = 5,
                Result = -1
            };
            var cmdMacroTask = cmdMacro.Execute(this.svcProvider, contextObj);
            cmdMacroTask.RunSynchronously();
            var cmdRes = cmdMacroTask.Result;
            Assert.IsTrue(cmdRes.IsSuccess);
            Assert.AreEqual(cmdRes.Description, "4 commands executed - 4 successful and 0 failed");
        }

        /// <summary>
        /// Executes a <see cref="MacroCommand"/> with several
        /// commands asynchronously and verifies successful
        /// execution.
        /// </summary>
        [TestMethod]
        public void AsyncronousMacroCommand()
        {
            var cmdNoop = new DelegateCommand(() =>
            {
                System.Threading.Thread.Sleep(20);
                return true;
            });
            var cmdAddNumbers = new DelegateCommand((s, c) =>
            {
                var v1 = PropertyResolver.GetPropertyValue<int>(c, "Val1");
                var v2 = PropertyResolver.GetPropertyValue<int>(c, "Val2");
                var sum = v1 + v2;
                return new BoolCommandResult(true, string.Format("Sum of {0} + {1} = {2}", v1, v2, sum));
            });
            var cmdMacro = new MacroCommand();
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.Commands.Add(cmdNoop);
            cmdMacro.Commands.Add(cmdAddNumbers);
            cmdMacro.ExecuteAsync = true;
            var contextObj = new
            {
                Val1 = 2,
                Val2 = 5,
                Result = -1
            };
            var cmdMacroTask = cmdMacro.Execute(this.svcProvider, contextObj);
            cmdMacroTask.RunSynchronously();
            var cmdRes = cmdMacroTask.Result;
            Assert.IsTrue(cmdRes.IsSuccess);
            Assert.AreEqual(cmdRes.Description, "4 commands executed - 4 successful and 0 failed");
        }
    }
}
