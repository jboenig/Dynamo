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
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    foreach (var command in this.Commands)
                    {
                        var result = command.Execute(serviceProvider, context);
                        if (!result.IsSuccess)
                        {
                            return result;
                        }
                    }

                    scope.Complete();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            return CommandResult.Success;
        }        

        #endregion
    }
}
