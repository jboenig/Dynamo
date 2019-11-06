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

using System.Collections.Generic;
using System.Linq;

namespace Headway.Dynamo.Commands
{
    /// <summary>
    /// This class implements a <see cref="CommandResult"/> that aggregates
    /// other <see cref="CommandResult"/> objects.
    /// </summary>
    public sealed class MacroCommandResult : CommandResult
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public MacroCommandResult()
        {
            this.CommandResults = new List<CommandResult>();
        }

        /// <summary>
        /// Gets the collection of <see cref="CommandResult"/>
        /// objects in this <see cref="MacroCommandResult"/>.
        /// </summary>
        public List<CommandResult> CommandResults
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the description for this <see cref="MacroCommandResult"/>
        /// </summary>
        public override string Description
        {
            get
            {
                int numSuccessful = 0;
                int numFailed = 0;
                int numTotal = 0;
                foreach (var curRes in this.CommandResults)
                {
                    if (curRes.IsSuccess)
                    {
                        numSuccessful++;
                    }
                    else
                    {
                        numFailed++;
                    }
                    numTotal++;
                }
                return string.Format("{0} commands executed - {1} successful and {2} failed", numTotal, numSuccessful, numFailed);
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
                return !(from cr in this.CommandResults
                         where cr.IsSuccess == false
                         select cr).Any();
            }
        }
    }
}
