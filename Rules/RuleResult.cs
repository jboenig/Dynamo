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
using Headway.Dynamo.Commands;

namespace Headway.Dynamo.Rules
{
    /// <summary>
    /// Result codes that can occur when a <see cref="Rule"/>
    /// is applied.
    /// </summary>
    public enum RuleResultCode
    {
        /// <summary>
        /// Rule applied successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Indicates that the evaluation of the condition failed.
        /// </summary>
        ConditionEvalFailed = 1,

        /// <summary>
        /// Indicates that the action failed.
        /// </summary>
        ActionFailed = 2,

        /// <summary>
        /// Indicates that an exception occurred while applying the rule.
        /// </summary>
        Exception = 3
    }

    /// <summary>
    /// Encapsulates results that can occur an attempt is made to
    /// apply a <see cref="Rule"/>.
    /// </summary>
    public class RuleResult
    {
        /// <summary>
        /// Constructs a <see cref="RuleResult"/> given a result
        /// code and a description.
        /// </summary>
        /// <param name="resultCode">Code describing the result.</param>
        /// <param name="conditionRes">Value returned by evaluation of the <see cref="Rule.Condition"/>.</param>
        /// <param name="description">Text description of the result.</param>
        public RuleResult(RuleResultCode resultCode, bool? conditionRes, string description = null)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = resultCode;
            this.Description = description;
            this.ConditionResult = conditionRes;
        }

        /// <summary>
        /// Constructs a <see cref="RuleResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="ex">Exception that occurred during the transition.</param>
        /// <param name="conditionRes">Value returned by evaluation of the <see cref="Rule.Condition"/>.</param>
        public RuleResult(Exception ex, bool? conditionRes = null)
        {
            this.ActionResult = CommandResult.Success;
            this.ResultCode = RuleResultCode.Exception;
            this.Description = ex.Message;
            this.ConditionResult = conditionRes;
        }

        /// <summary>
        /// Constructs a <see cref="RuleResult"/> given an
        /// exception object.
        /// </summary>
        /// <param name="actionRes">
        /// Result of the command executed.
        /// </param>
        /// <param name="conditionRes">Value returned by evaluation of the <see cref="Rule.Condition"/>.</param>
        public RuleResult(CommandResult actionRes, bool? conditionRes = null)
        {
            this.ConditionResult = conditionRes;

            if (actionRes == null)
            {
                this.ResultCode = RuleResultCode.Success;
                this.ActionResult = CommandResult.Success;
            }
            else
            {
                if (actionRes.IsSuccess)
                {
                    this.ResultCode = RuleResultCode.Success;
                }
                else
                {
                    this.ResultCode = RuleResultCode.ActionFailed;
                }
                this.Description = actionRes.Description;
                this.ActionResult = actionRes;
            }
        }

        /// <summary>
        /// Gets the boolean value returned by the evaluation of the
        /// <see cref="Rule.Condition"/> associated with the <see cref="Rule"/>.
        /// This value is only set if the <see cref="Rule.Condition"/> was
        /// successfully evaluated.
        /// </summary>
        public bool? ConditionResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the result code.
        /// </summary>
        public RuleResultCode ResultCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Exception that occurred during a transition.
        /// </summary>
        /// <remarks>
        /// Null if no exception occurred.
        /// </remarks>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the <see cref="CommandResult"/> from the
        /// actions executed.
        /// </summary>
        /// <remarks>
        /// If a failure occurs, this should be the first
        /// error encountered, since processing stops when
        /// an error occurs.
        /// </remarks>
        public CommandResult ActionResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a flag indicating whether or not the rule was successfully applied.
        /// </summary>
        public bool IsSuccess
        {
            get { return (this.ResultCode == RuleResultCode.Success); }
        }

        /// <summary>
        /// Singleton object used to indicate that the condition on the rule evaluated
        /// to true and the rule was successfully applied.
        /// and the 
        /// </summary>
        public static RuleResult SuccessConditionTrue = new RuleResult(RuleResultCode.Success, true);

        /// <summary>
        /// Singleton object used to indicate that the condition on the rule evaluated
        /// to false and the rule was successfully applied.
        /// </summary>
        public static RuleResult SuccessConditionFalse = new RuleResult(RuleResultCode.Success, false);
    }
}
