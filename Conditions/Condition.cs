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

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Encapsulates a condition that can be evaluated
    /// as TRUE or FALSE.
    /// </summary>
    public abstract class Condition
    {
        /// <summary>
        /// Evaluates the condition.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// Returns TRUE or FALSE based on evaluation
        /// of the condition.
        /// </returns>
        public abstract bool Evaluate(IServiceProvider serviceProvider, object context);

        /// <summary>
        /// Singleton <see cref="Condition"/> that always evaluates to true.
        /// </summary>
        public static Condition True = new True();

        /// <summary>
        /// Singleton <see cref="Condition"/> that always evaluates to false.
        /// </summary>
        public static Condition False = new False();
    }
}
