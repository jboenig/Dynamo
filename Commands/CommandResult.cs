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

        /// <summary>
        /// Gets a singleton <see cref="CommandResult"/> object that indicates
        /// failed execution of a command.
        /// </summary>
        public static CommandResult Fail = new BoolCommandResult(false, string.Empty);
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
