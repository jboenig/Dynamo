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
        public override Task<bool> EvaluateAsync(IServiceProvider serviceProvider, object context)
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
                return Task.FromResult(true);
            }
            else if (curValue == null || this.PropertyValue == null)
            {
                return Task.FromResult(false);
            }

            if (this.compareFunc == null)
            {
                var msg = string.Format("{0} is not a supported comparison operator for a {1}",
                    this.Operator, nameof(PropertyCompare));
                throw new InvalidOperationException(msg);
            }

            return Task.FromResult(this.compareFunc(curValue, actualValue));
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
        /// <remarks>
        /// If the value assigned is a string, it may optionally
        /// contain variable references that are resolved at run-time.
        /// The syntax for a variable can be either $(VariableName)
        /// or {VariableName}.  The variable must reference a property
        /// that exists on the context object passed to the
        /// <see cref="PropertyCompare.EvaluateAsync(IServiceProvider, object)"/>
        /// method.
        /// </remarks>
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
