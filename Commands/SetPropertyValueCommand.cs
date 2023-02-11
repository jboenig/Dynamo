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

using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Implements a <see cref="Command"/> that sets the value
    /// of a property on the context object passed to the command.
    /// </summary>
    public sealed class SetPropertyValueCommand : Command
    {
        /// <summary>
        /// Gets or sets the name of the property to set.
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to assign to the property.
        /// </summary>
        /// <remarks>
        /// If the value assigned is a string, it may optionally
        /// contain variable references that are resolved at run-time.
        /// The syntax for a variable can be either $(VariableName)
        /// or {VariableName}.  The variable must reference a property
        /// that exists on the context object passed to the
        /// <see cref="SetPropertyValueCommand.ExecuteAsync(IServiceProvider, object)"/>
        /// method.
        /// </remarks>
        public object Value
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
        /// <para>
        /// Assigns the <see cref="SetPropertyValueCommand.Value"/>
        /// to the property in the context object.
        /// </para>
        /// <para>
        /// If the<see cref="SetPropertyValueCommand.Value"/> is a
        /// string, it may optionally contain variable references that
        /// are resolved at run-time. The syntax for a variable can be
        /// either $(VariableName) or {VariableName}.  The variable must
        /// reference a property that exists on the context object passed
        /// to this method.
        /// </para>
        /// </remarks>
        public override Task<CommandResult> ExecuteAsync(IServiceProvider serviceProvider, object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var actualValue = this.Value;
            if (this.Value is string strValue)
            {
                // Value is a string. Attempt to resolve any variables
                // it may contain.
                actualValue = PropertyResolver.ResolvePropertyValues(context, strValue);
            }
            PropertyResolver.SetPropertyValue<object>(context, this.PropertyName, actualValue);

            return Task.FromResult(CommandResult.Success);
        }
    }
}
