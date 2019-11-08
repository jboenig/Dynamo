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
    public abstract class ConditionalCommand : Command
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
            return null;
        }
    }
}
