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
/// Encapsulates a collection of <see cref="Command"/> objects
/// that are executed as a single, sequential unit.
/// </summary>
public class MacroCommand : Command
{
    #region Member Variables

    private readonly List<Command> commands;

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MacroCommand()
    {
        this.commands = new List<Command>();
        this.AllowParallelExecution = false;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the collection of <see cref="Command"/> objects
    /// contained by this macro.
    /// </summary>
    public ICollection<Command> Commands
    {
        get { return this.commands; }
    }

    /// <summary>
    /// Gets or sets a flag indicating whether or not the
    /// commands in this macro should be executed in parallel.
    /// </summary>
    public bool AllowParallelExecution
    {
        get;
        set;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Executes this macro command asynchronously.
    /// </summary>
    /// <param name="serviceProvider">Interface to service provider</param>
    /// <param name="context">User defined context data</param>
    /// <returns>
    /// Returns a <see cref="CommandResult"/> object that describes
    /// the result.
    /// </returns>
    public override async Task<CommandResult> Execute(IServiceProvider serviceProvider, object context)
    {
        if (this.AllowParallelExecution)
        {
            return await this.ExecuteParallel(serviceProvider, context);
        }
        return await this.ExecuteSequential(serviceProvider, context);
    }

    #endregion

    #region Implementation

    private async Task<CommandResult> ExecuteSequential(IServiceProvider serviceProvider, object context)
    {
        var commandRes = new MacroCommandResult();

        foreach (var command in this.Commands)
        {
            var curCommandRes = await command.Execute(serviceProvider, context);
            commandRes.CommandResults.Add(curCommandRes);
        }

        return commandRes;
    }

    private async Task<CommandResult> ExecuteParallel(IServiceProvider serviceProvider, object context)
    {
        var commandRes = new MacroCommandResult();

        List<Task<CommandResult>> cmdTasks = new List<Task<CommandResult>>();

        foreach (var command in this.Commands)
        {
            cmdTasks.Add(command.Execute(serviceProvider, context));
        }

        await Task.WhenAll(cmdTasks);

        commandRes.CommandResults.AddRange(cmdTasks.Select(t => t.Result));

        return commandRes;
    }

    #endregion
}
