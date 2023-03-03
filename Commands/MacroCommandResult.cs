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

namespace Headway.Dynamo.Commands;

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
