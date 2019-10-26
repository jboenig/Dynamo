using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MacroCommandResult : CommandResult
    {
        /// <summary>
        /// 
        /// </summary>
        public MacroCommandResult()
        {
            this.CommandResults = new List<CommandResult>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<CommandResult> CommandResults
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string Description
        {
            get
            {
                return "TODO";
            }
        }

        /// <summary>
        /// Gets a flag indicating whether or not the command
        /// was successful.
        /// </summary>
        /// <remarks>
        /// For a macro command to be considered successful, all
        /// commands it executes must have executed successfully.
        /// </remarks>
        public override bool IsSuccess
        {
            get
            {
                return (from cr in this.CommandResults
                        where cr.IsSuccess == false
                        select cr).Any();
            }
        }
    }
}
