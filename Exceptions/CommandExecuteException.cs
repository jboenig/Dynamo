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
