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
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Implementation of <see cref="Condition"/> that retrieves the
    /// value of a property by name using the <see cref="IPropertyAccessor"/>
    /// interface and compares it for equality to a constant value.
    /// </summary>
    public class PropertyCompare : Condition
    {
        private Func<object, object, bool> compareFunc;
        private PropertyCompareOps compareOp;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PropertyCompare()
        {
        }

        /// <summary>
        /// Construct a <see cref="PropertyCompare"/> given a property
        /// name and value.
        /// </summary>
        /// <param name="propertyName">Name of property to compare</param>
        /// <param name="propertyValue">Value to compare property to</param>
        /// <param name="compareOp">Function used to compare the property value</param>
        public PropertyCompare(string propertyName,
            object propertyValue,
            PropertyCompareOps compareOp = PropertyCompareOps.AreEqual)
        {
            this.PropertyName = propertyName;
            this.PropertyValue = propertyValue;
            this.Operator = compareOp;
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
            object actualValue;

            if (this.PropertyValue is string)
            {
                // Provide the opportunity to resolve any
                // variables in the string
                actualValue = PropertyResolver.ResolvePropertyValues(context, (string)this.PropertyValue);
            }
            else
            {
                actualValue = this.PropertyValue;
            }

            object curValue = null;
            if (!PropertyResolver.TryGetPropertyValue<object>(context, this.PropertyName, out curValue))
            {
                throw new PropertyNotFoundException(context.GetType(),
                    this.PropertyName);
            }

            if (curValue == null && this.PropertyValue == null)
            {
                return true;
            }
            else if (curValue == null || this.PropertyValue == null)
            {
                return false;
            }

            if (this.compareFunc == null)
            {
                var msg = string.Format("{0} is not a supported comparison operator for a {1}",
                    this.Operator, nameof(PropertyCompare));
                throw new InvalidOperationException(msg);
            }

            return this.compareFunc(curValue, actualValue);
        }

        /// <summary>
        /// Gets or sets the name of the property to retrieve from the
        /// context.
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to compare against.
        /// </summary>
        public object PropertyValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        /// <seealso cref="PropertyCompareOps"/>
        public PropertyCompareOps Operator
        {
            get
            {
                return this.compareOp;
            }
            set
            {
                this.compareOp = value;
                this.InitCompareFunc();
            }
        }

        private void InitCompareFunc()
        {
            if (this.Operator == PropertyCompareOps.AreEqual)
            {
                this.compareFunc = (v1, v2) => v1.Equals(v2);
            }
            else if (this.Operator == PropertyCompareOps.AreNotEqual)
            {
                this.compareFunc = (v1, v2) => !(v1.Equals(v2));
            }
            else if (this.Operator == PropertyCompareOps.Contains)
            {
                this.compareFunc = (v1, v2) =>
                {
                    var v1Str = v1.ToString();
                    var v2Str = v2.ToString();
                    return v1Str.Contains(v2Str);
                };
            }
            else if (this.Operator == PropertyCompareOps.StartsWith)
            {
                this.compareFunc = (v1, v2) =>
                {
                    var v1Str = v1.ToString();
                    var v2Str = v2.ToString();
                    return v1Str.StartsWith(v2Str);
                };
            }
            else if (this.Operator == PropertyCompareOps.EndsWith)
            {
                this.compareFunc = (v1, v2) =>
                {
                    var v1Str = v1.ToString();
                    var v2Str = v2.ToString();
                    return v1Str.EndsWith(v2Str);
                };
            }
            else if (this.Operator == PropertyCompareOps.GreaterThan)
            {
                this.compareFunc = (v1, v2) =>
                {
                    return Convert.ToDouble(v1) > Convert.ToDouble(v2);
                };
            }
            else if (this.Operator == PropertyCompareOps.LessThan)
            {
                this.compareFunc = (v1, v2) =>
                {
                    return Convert.ToDouble(v1) < Convert.ToDouble(v2);
                };
            }
            else if (this.Operator == PropertyCompareOps.GreaterThanOrEqual)
            {
                this.compareFunc = (v1, v2) =>
                {
                    return Convert.ToDouble(v1) >= Convert.ToDouble(v2);
                };
            }
            else if (this.Operator == PropertyCompareOps.LessThanOrEqual)
            {
                this.compareFunc = (v1, v2) =>
                {
                    return Convert.ToDouble(v1) <= Convert.ToDouble(v2);
                };
            }
        }
    }
}
