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
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Implementation of <see cref="Condition"/> that retrieves the
    /// value of a property by name using the <see cref="IPropertyAccessor"/>
    /// interface and compares it for equality to a constant value.
    /// </summary>
    public class PropertyEqualsCondition : Condition
    {
        private string propertyName;
        private object propertyValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PropertyEqualsCondition()
        {
        }

        /// <summary>
        /// Construct a <see cref="PropertyEqualsCondition"/> given a property
        /// name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to compare</param>
        /// <param name="propertyValue">Value to compare property to</param>
        public PropertyEqualsCondition(string propertyName, object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
        }

        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns TRUE or FALSE based on evaluation
        /// of the condition.
        /// </returns>
        public override bool Evaluate(IServiceProvider serviceProvider, object context)
        {
            var propertyAccessor = context as IPropertyAccessor;
            if (propertyAccessor == null)
            {
                throw new ArgumentException();
            }

            var curValue = propertyAccessor.GetPropertyValue<object>(this.propertyName);

            if (curValue == null && this.PropertyValue == null)
            {
                return true;
            }
            else if (curValue == null || this.PropertyValue == null)
            {
                return false;
            }

            return curValue.Equals(this.PropertyValue);
        }

        /// <summary>
        /// Gets or sets the name of the property to retrieve from the
        /// context.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                this.propertyName = value;
            }
        }

        /// <summary>
        /// Gets or sets the value to compare against.
        /// </summary>
        public object PropertyValue
        {
            get
            {
                return this.propertyValue;
            }
            set
            {
                this.propertyValue = value;
            }
        }
    }
}
