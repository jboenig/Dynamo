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

using System;

namespace Headway.Dynamo.Conditions
{
    /// <summary>
    /// Implements a <see cref="Condition"/> using a
    /// lambda expression.
    /// </summary>
    public class LambdaCondition : Condition
    {
        private Func<object, bool> expr;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LambdaCondition()
        {
        }

        /// <summary>
        /// Constructs a <see cref="LambdaCondition"/> given
        /// an expression.
        /// </summary>
        /// <param name="expr">
        /// Expression used to implement <see cref="Evaluate(IServiceProvider, object)"/>
        /// </param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected void SetExpression(Func<object, bool> value)
        {
            this.expr = value;
        }
    }
}
