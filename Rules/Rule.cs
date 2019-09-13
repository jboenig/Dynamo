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
        public bool Apply(IServiceProvider serviceProvider, object context)
        {
            try
            {
                this.OnBeforeApply(serviceProvider, context);
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
                evalRes = this.Condition.Evaluate(serviceProvider, context);
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
                        cmdRes = this.WhenTrue.Execute(serviceProvider, context);
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
                        cmdRes = this.WhenFalse.Execute(serviceProvider, context);
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
        protected virtual void OnBeforeApply(IServiceProvider serviceProvider, dynamic context)
        {
        }
    }
}
