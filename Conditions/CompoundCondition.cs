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
        /// Default <see cref="EvaluationType"/> is
        /// <see cref="EvaluationType.All"/>.
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
        public override bool Evaluate(IServiceProvider serviceProvider, object context)
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
                    result = curCondition.Evaluate(serviceProvider, context);
                }
            }
            else
            {
                // All conditions must be true
                result = true;
                while (result && enumConditions.MoveNext())
                {
                    var curCondition = enumConditions.Current;
                    result = curCondition.Evaluate(serviceProvider, context);
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
