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
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// Implements a <see cref="Command"/> that sets a 
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
        public override Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new Task<CommandResult>(() =>
            {
                var actualValue = this.Value;
                var strValue = this.Value as string;
                if (strValue != null)
                {
                    // Value is a string. Attempt to resolve any variables
                    // it may contain.
                    actualValue = PropertyResolver.ResolvePropertyValues(context, strValue);
                }
                PropertyResolver.SetPropertyValue<object>(context, this.PropertyName, actualValue);
                return CommandResult.Success;
            });
        }
    }
}
