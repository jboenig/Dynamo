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

namespace Headway.Dynamo.Exceptions
{
    /// <summary>
    /// Exception thrown when a call to
    /// <see cref="Headway.Dynamo.Conditions.Condition.Evaluate(IServiceProvider, object)"/>
    /// fails.
    /// </summary>
    public sealed class ConditionEvalException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="ConditionEvalException"/> given an
        /// inner exception.
        /// </summary>
        /// <param name="innerException">Exception that caused the failure.</param>
        public ConditionEvalException(Exception innerException) :
            base(string.Format("Condition evaluation failed due to the following error - {0}", innerException.Message), innerException)
        {
        }

        /// <summary>
        /// Constructs a <see cref="ConditionEvalException"/> given a message.
        /// </summary>
        /// <param name="msg">Message explaining the exception.</param>
        public ConditionEvalException(string msg) : base(msg)
        {
        }
    }
}
