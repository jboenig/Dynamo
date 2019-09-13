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

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Encapsulates the result of executing a command.
    /// </summary>
    public abstract class CommandResult
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CommandResult()
        {
        }

        /// <summary>
        /// Constructs a result given the context object
        /// using during command execution.
        /// </summary>
        /// <param name="context">
        /// Context used during command execution.
        /// </param>
        public CommandResult(object context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Gets a flag indicating whether result is successful or not.
        /// </summary>
        public abstract bool IsSuccess
        {
            get;
        }

        /// <summary>
        /// Gets the context object used during command execution.
        /// </summary>
        public object Context
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a singleton <see cref="CommandResult"/> object that indicates
        /// successful execution of a command.
        /// </summary>
        public static CommandResult Success = new BoolCommandResult(true, string.Empty);
    }

    /// <summary>
    /// Implements a simple <see cref="CommandResult"/> that indicates success
    /// or failure and a message.
    /// </summary>
    public sealed class BoolCommandResult : CommandResult
    {
        private readonly bool isSuccess;
        private readonly string description;

        /// <summary>
        /// Constructs a <see cref="BoolCommandResult"/> given a boolean
        /// flag that indicates success/failure and a description.
        /// </summary>
        /// <param name="context">
        /// Context used during command execution.
        /// </param>
        /// <param name="isSuccess">
        /// Flag indicating whether or not the command was successful
        /// </param>
        /// <param name="description">Description of the result</param>
        public BoolCommandResult(bool isSuccess, string description, object context = null) :
            base(context)
        {
            this.isSuccess = isSuccess;
            this.description = description;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public override string Description
        {
            get { return this.description; }
        }

        /// <summary>
        /// Gets a flag indicating whether result is successful or not.
        /// </summary>
        public override bool IsSuccess
        {
            get { return this.isSuccess; }
        }
    }
}
