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

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Implements a command based on a delgate function.
    /// </summary>
    public class DelegateCommand : Command
    {
        /// <summary>
        /// Signature for a command function with no
        /// parameters.
        /// </summary>
        /// <returns>
        /// Returns true if successful, otherwise returns false.
        /// </returns>
        public delegate bool CommandFuncNoParams();

        /// <summary>
        /// Signature for a command function that takes all
        /// <see cref="Command.Execute(IServiceProvider, object)"/>
        /// parameters and returns a <see cref="CommandResult"/>.
        /// </summary>
        /// <param name="svcProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns a <see cref="CommandResult"/> object that describes
        /// the result.
        /// </returns>
        public delegate CommandResult CommandFuncFull(IServiceProvider svcProvider, object context);

        /// <summary>
        /// Signature for a command function that takes all
        /// <see cref="Command.Execute(IServiceProvider, object)"/>
        /// parameters and returns a <see cref="CommandResult"/>.
        /// </summary>
        /// <param name="svcProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns true if successful, otherwise returns false.
        /// </returns>
        public delegate bool CommandFuncAllParamsBoolResult(IServiceProvider svcProvider, object context);

        private CommandFuncNoParams cmdFuncNoParams;
        private CommandFuncFull cmdFuncFull;
        private CommandFuncAllParamsBoolResult cmdFuncAllParamsBoolRes;

        /// <summary>
        /// Construct a <see cref="DelegateCommand"/> with a
        /// command function that only returns a bool.
        /// </summary>
        /// <param name="cmdFunc">
        /// Delegate to invoke when this command is executed.
        /// </param>
        public DelegateCommand(CommandFuncNoParams cmdFunc)
        {
            this.cmdFuncNoParams = cmdFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdFunc">
        /// Delegate to invoke when this command is executed.
        /// </param>
        public DelegateCommand(CommandFuncFull cmdFunc)
        {
            this.cmdFuncFull = cmdFunc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdFunc">
        /// Delegate to invoke when this command is executed.
        /// </param>
        public DelegateCommand(CommandFuncAllParamsBoolResult cmdFunc)
        {
            this.cmdFuncAllParamsBoolRes = cmdFunc;
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
        /// The implementation invokes the delegate function associated
        /// with this command.
        /// </remarks>
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            return new Task<CommandResult>(() =>
            {
                CommandResult res = CommandResult.Success;

                if (this.cmdFuncFull != null)
                {
                    res = this.cmdFuncFull(serviceProvider, context);
                }
                else if (this.cmdFuncAllParamsBoolRes != null)
                {
                    if (!this.cmdFuncAllParamsBoolRes(serviceProvider, context))
                    {
                        res = CommandResult.Fail;
                    }
                }
                else if (this.cmdFuncNoParams != null)
                {
                    if (!this.cmdFuncNoParams())
                    {
                        res = CommandResult.Fail;
                    }
                }

                return res;
            });
        }
    }
}
