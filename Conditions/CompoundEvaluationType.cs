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

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Determines whether all <see cref="Condition"/> objects
    /// associated with a <see cref="CompoundCondition"/> must
    /// evaluate to true in order for the condition to be true
    /// or any conditions can be true.
    /// </summary>
    public enum CompoundEvaluationType
    {
        /// <summary>
        /// Any one condition evaluating to true makes the
        /// <see cref="CompoundCondition"/> true.
        /// </summary>
        Any,

        /// <summary>
        /// All conditions must be true for the
        /// <see cref="CompoundCondition"/> true.
        /// </summary>
        All
    };
}
