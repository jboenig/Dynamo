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
    /// Comparions operators that can be used to compare
    /// two property values.
    /// </summary>
    /// <seealso cref="PropertyCompare"/>
    public enum PropertyCompareOps
    {
        /// <summary>
        /// Test for equality
        /// </summary>
        AreEqual,

        /// <summary>
        /// Test for inequality
        /// </summary>
        AreNotEqual,

        /// <summary>
        /// Test for greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Test for less than
        /// </summary>
        LessThan,

        /// <summary>
        /// Test for greater than or equal
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Test for less than or equal
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Test to see if a string contains another string
        /// </summary>
        Contains,

        /// <summary>
        /// Test to see if a string starts with another string
        /// </summary>
        StartsWith,

        /// <summary>
        /// Test to see if a string ends with another string
        /// </summary>
        EndsWith
    }
}
