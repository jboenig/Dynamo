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
using System.Text;
using System.Reflection;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// 
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
        public override CommandResult Execute(IServiceProvider serviceProvider, object context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var propertyAccessor = context as IPropertyAccessor;
            if (propertyAccessor != null)
            {
                propertyAccessor.SetPropertyValue<object>(this.PropertyName, this.Value);
            }
            else
            {
                // Use reflection
                var propInfo = context.GetType().GetProperty(this.PropertyName);
                if (propInfo == null)
                {
                    var msg = string.Format("Property {0} does not exist", this.PropertyName);
                    throw new InvalidOperationException(msg);
                }
                propInfo.SetValue(context, this.Value);
            }

            return CommandResult.Success;
        }
    }
}
