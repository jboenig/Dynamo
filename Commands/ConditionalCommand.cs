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
using Headway.Dynamo.Conditions;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Implements a command that wraps another <see cref="Command"/>
    /// with a <see cref="Condition"/>.  The <see cref="ExecuteWhen"/>
    /// optionally contains a <see cref="Condition"/> that is evaluated
    /// in order to determine if the <see cref="ConditionalCommand.Command"/>
    /// should execute.  If <see cref="ExecuteWhen"/> is null, then the
    /// command is always executed.
    /// </summary>
    public class ConditionalCommand : Command
    {
        /// <summary>
        /// Gets or sets the <see cref="Condition"/> that determines
        /// if this command will be executed.
        /// </summary>
        public Condition ExecuteWhen
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the <see cref="Command"/> to execute
        /// when the <see cref="ExecuteWhen"/> condition evaluates
        /// to true.
        /// </summary>
        public Command Command
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        /// <remarks>
        /// If there is no <see cref="ExecuteWhen"/> condition then the
        /// command is executed.  Also, if <see cref="ExecuteWhen"/> evaluates
        /// true, then the command is also execute.
        /// </remarks>
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            if (this.ExecuteWhen == null || this.ExecuteWhen.Evaluate(serviceProvider, context) == true)
            {
                return this.Command.Execute(serviceProvider, context);
            }
            return new Task<CommandResult>(() => CommandResult.Success);
        }
    }
}
