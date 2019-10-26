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
            this.ExecuteAsync = true;
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
        public bool ExecuteAsync
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
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            return new Task<CommandResult>(() =>
            {
                var commandRes = new MacroCommandResult();

                var commandTasks = new List<Task<CommandResult>>();

                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        foreach (var command in this.Commands)
                        {
                            var curCommandTask = command.Execute(serviceProvider, context);

                            if (this.ExecuteAsync)
                            {
                                commandTasks.Add(curCommandTask);
                                curCommandTask.Start();
                            }
                            else
                            {
                                curCommandTask.RunSynchronously();
                                commandRes.CommandResults.Add(curCommandTask.Result);
                            }
                        }

                        Task.WaitAll(commandTasks.ToArray());

                        foreach(var curCommandTask in commandTasks)
                        {
                            commandRes.CommandResults.Add(curCommandTask.Result);
                        }

                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                return commandRes;
            });
        }        

        #endregion
    }
}
