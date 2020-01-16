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
using Headway.Dynamo.Commands;

namespace Headway.Dynamo.Exceptions
{
    /// <summary>
    /// Exception thrown when a call to <see cref="Command.Execute(IServiceProvider, object)"/>
    /// fails due to an exception or a failure indicated by the <see cref="CommandResult"/>.
    /// </summary>
    public sealed class CommandExecuteException : Exception
    {
        private string messagePrefix;

        /// <summary>
        /// Constructs a <see cref="CommandExecuteException"/> given
        /// a message prefix and the exception that caused the error. The
        /// message prefix is prepended to the error message from the exception
        /// and is optional.
        /// </summary>
        /// <param name="messagePrefix">Optional message prefix.</param>
        /// <param name="innerException">Exception that caused the error.</param>
        public CommandExecuteException(string messagePrefix, Exception innerException) :
            base("", innerException)
        {
            this.messagePrefix = messagePrefix;
            this.CommandResult = new BoolCommandResult(false, innerException.Message);
        }

        /// <summary>
        /// Constructs a <see cref="CommandExecuteException"/> given
        /// a message prefix and the <see cref="CommandResult"/> that caused the error.
        /// The message prefix is prepended to the error message from the exception
        /// and is optional.
        /// </summary>
        /// <param name="messagePrefix">Optional message prefix.</param>
        /// <param name="commandRes">Result returned by the command execution.</param>
        public CommandExecuteException(string messagePrefix, CommandResult commandRes)
        {
            this.messagePrefix = messagePrefix;
            this.CommandResult = commandRes;
        }

        /// <summary>
        /// Gets the <see cref="CommandResult"/> returned by the command.
        /// </summary>
        public CommandResult CommandResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error message for this exception.
        /// </summary>
        public override string Message
        {
            get
            {
                if (this.CommandResult.IsSuccess)
                {
                    // This should never be the case
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(this.messagePrefix))
                {
                    // No message prefix given
                    return string.Format("Command failed due to the following error - {0}", this.CommandResult.Description);
                }

                return string.Format("{0} - {1}", this.messagePrefix, this.CommandResult.Description);
            }
        }
    }
}
