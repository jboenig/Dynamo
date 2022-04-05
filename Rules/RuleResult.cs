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
