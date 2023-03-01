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

using Headway.Dynamo.Conditions;
using Headway.Dynamo.Commands;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Rules
{
    /// <summary>
    /// A Rule consists of a <see cref="Condition"/>,
    /// a <see cref="Command"/> to execute when the condition
    /// is true and a <see cref="Command"/> to execute
    /// when the condition is false.
    /// </summary>
    public abstract class Rule
    {
        /// <summary>
        /// Gets or sets the condition that the rule
        /// tests.
        /// </summary>
        public abstract Condition Condition
        {
            get;
            set;
        }

        /// <summary>
        /// The command that is executed when the condition
        /// is true.
        /// </summary>
        public abstract Command WhenTrue
        {
            get;
            set;
        }

        /// <summary>
        /// The command that is executed when the condition
        /// is false.
        /// </summary>
        public abstract Command WhenFalse
        {
            get;
            set;
        }

        /// <summary>
        /// Applies the rule to the specified context.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">
        /// Context containing data used to apply the rule.
        /// </param>
        /// <returns>
        /// Returns the result of evaluating the <see cref="Rule.Condition"/>
        /// associated with this rule.
        /// </returns>
        /// <remarks>
        /// This method evaluates the <see cref="Rule.Condition"/> and fires
        /// either the <see cref="Rule.WhenTrue"/> or <see cref="Rule.WhenFalse"/>
        /// command based on the result.
        /// 
        /// This method can throw the following exceptions -
        ///   <see cref="RuleInitException"/>
        ///   <see cref="ConditionEvalException"/>
        ///   <see cref="CommandExecuteException"/>
        /// </remarks>
        public async Task<bool> Apply(IServiceProvider serviceProvider, object context)
        {
            try
            {
                await this.OnBeforeApply(serviceProvider, context);
            }
            catch (Exception ex)
            {
                throw new RuleInitException(ex);
            }

            if (this.Condition == null)
            {
                return false;
            }

            bool evalRes;

            try
            {
                evalRes = await this.Condition.Evaluate(serviceProvider, context);
            }
            catch (Exception ex)
            {
                throw new ConditionEvalException(ex);
            }

            if (evalRes)
            {
                if (this.WhenTrue != null)
                {
                    CommandResult cmdRes;

                    try
                    {
                        cmdRes = await this.WhenTrue.Execute(serviceProvider, context);
                    }
                    catch (Exception ex)
                    {
                        throw new CommandExecuteException("An exception was thrown while attempting to execute the WhenTrue command on a rule", ex);
                    }

                    if (!cmdRes.IsSuccess)
                    {
                        throw new CommandExecuteException("Execution of the WhenTrue command failed", cmdRes);
                    }
                }
            }
            else
            {
                if (this.WhenFalse != null)
                {
                    CommandResult cmdRes;

                    try
                    {
                        cmdRes = await this.WhenFalse.Execute(serviceProvider, context);
                    }
                    catch (Exception ex)
                    {
                        throw new CommandExecuteException("An exception was thrown while attempting to execute the WhenFalse command on a rule", ex);
                    }

                    if (!cmdRes.IsSuccess)
                    {
                        throw new CommandExecuteException("Execution of the WhenFalse command failed", cmdRes);
                    }
                }
            }

            return evalRes;
        }

        /// <summary>
        /// This method is called by the <see cref="Apply"/> method before it
        /// does anything. It allows derived classes to perform initialization
        /// on the service provider and/or context.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">
        /// Context containing data used to apply the rule.
        /// </param>
        /// <remarks>
        /// Base class implementation is a no-op and does not need to be called
        /// by derived class implementations.
        /// </remarks>
        protected virtual Task OnBeforeApply(IServiceProvider serviceProvider, dynamic context)
        {
            return Task.CompletedTask;
        }
    }
}
