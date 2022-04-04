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

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Aggregates multiple <see cref="Condition"/> objects into
    /// a single condition using a <see cref="CompoundEvaluationType"/>,
    /// which determines whether the multiple conditions are AND'd or
    /// OR'd together.
    /// </summary>
    public class CompoundCondition : Condition
    {
        #region Member Variables

        private List<Condition> conditions;
        private CompoundEvaluationType evalType = CompoundEvaluationType.All;

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// Default <see cref="CompoundEvaluationType"/> is
        /// <see cref="CompoundEvaluationType.All"/>.
        /// </remarks>
        public CompoundCondition()
        {
            this.evalType = CompoundEvaluationType.All;
            this.conditions = new List<Condition>();
        }

        /// <summary>
        /// Constructs a <see cref="CompoundCondition"/> given
        /// an <see cref="EvaluationType"/>.
        /// </summary>
        /// <param name="evalType">
        /// Determines whether all inner conditions must be true
        /// or any one condition must be true for this condition
        /// to be true.
        /// </param>
        public CompoundCondition(CompoundEvaluationType evalType)
        {
            this.conditions = new List<Condition>();
        }

        /// <summary>
        /// Gets or sets the <see cref="EvaluationType"/> for
        /// this compound condition.
        /// </summary>
        public CompoundEvaluationType EvaluationType
        {
            get { return this.evalType; }
            set { this.evalType = value; }
        }

        /// <summary>
        /// Evaluate all of the <see cref="Condition"/> objects in the
        /// <see cref="CompoundCondition.Conditions"/> collection based on
        /// the <see cref="EvaluationType"/> associated with this
        /// <see cref="CompoundCondition"/>.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Boolean flag indicating whether the
        /// condition is true or false.
        /// </returns>
        public override async Task<bool> EvaluateAsync(IServiceProvider serviceProvider, object context)
        {
            bool result;

            var enumConditions = this.conditions.GetEnumerator();

            if (this.evalType == CompoundEvaluationType.Any)
            {
                // Any one condition must be true
                result = false;
                while (!result && enumConditions.MoveNext())
                {
                    var curCondition = enumConditions.Current;
                    result = await curCondition.EvaluateAsync(serviceProvider, context);
                }
            }
            else
            {
                // All conditions must be true
                result = true;
                while (result && enumConditions.MoveNext())
                {
                    var curCondition = enumConditions.Current;
                    result = await curCondition.EvaluateAsync(serviceProvider, context);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the collection of <see cref="Condition"/> objects
        /// that this compound condition evaluates.
        /// </summary>
        public ICollection<Condition> Conditions
        {
            get { return this.conditions; }
        }
    }
}
