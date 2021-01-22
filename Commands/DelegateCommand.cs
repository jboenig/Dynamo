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

        private readonly CommandFuncNoParams cmdFuncNoParams;
        private readonly CommandFuncFull cmdFuncFull;
        private readonly CommandFuncAllParamsBoolResult cmdFuncAllParamsBoolRes;

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
        public override CommandResult Execute(IServiceProvider serviceProvider, object context)
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
        }
    }
}
