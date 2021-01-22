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
using System.Collections.Generic;
using System.Transactions;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Encapsulates a collection of <see cref="Command"/> objects
    /// that are executed as a single, sequential unit.
    /// </summary>
    public class MacroCommand : Command
    {
        #region Member Variables

        private readonly List<Command> commands;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MacroCommand()
        {
            this.commands = new List<Command>();
            this.AllowAsyncExecution = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the collection of <see cref="Command"/> objects
        /// contained by this macro.
        /// </summary>
        public ICollection<Command> Commands
        {
            get { return this.commands; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the
        /// commands in this macro should be executed asynchrounously
        /// or synchronously.
        /// </summary>
        public bool AllowAsyncExecution
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes this macro command.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        public override CommandResult Execute(IServiceProvider serviceProvider, object context)
        {
            var commandRes = new MacroCommandResult();

            foreach (var command in this.Commands)
            {
                var curCommandRes = command.Execute(serviceProvider, context);
                commandRes.CommandResults.Add(curCommandRes);
            }

            return commandRes;
        }

        /// <summary>
        /// Executes this macro command asynchronously.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        public override async Task<CommandResult> ExecuteAsync(IServiceProvider serviceProvider, object context)
        {
            var commandRes = new MacroCommandResult();

            foreach (var command in this.Commands)
            {
                var curCommandRes = await command.ExecuteAsync(serviceProvider, context);
                commandRes.CommandResults.Add(curCommandRes);
            }

            return commandRes;
        }

        #endregion
    }
}
