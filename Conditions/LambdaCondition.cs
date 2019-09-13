﻿////////////////////////////////////////////////////////////////////////////////
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
    public class LambdaCondition : Condition
    {
        private Func<object, bool> expr;

        public LambdaCondition()
        {
        }

        public LambdaCondition(Func<object, bool> expr)
        {
            this.expr = expr;
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
            if (this.expr == null)
            {
                return false;
            }
            return this.expr(context);
        }

        protected void SetExpression(Func<object, bool> value)
        {
            this.expr = value;
        }
    }
}
