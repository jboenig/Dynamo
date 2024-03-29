﻿////////////////////////////////////////////////////////////////////////////////
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

namespace Headway.Dynamo.Rules;

/// <summary>
/// Extension methods for collections of <see cref="Rule"/>
/// objects.
/// </summary>
public static class RuleCollectionExtensions
{
    /// <summary>
    /// Applies a collection of rules.
    /// </summary>
    /// <param name="rules">
    /// Collection of rules to apply.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <param name="context">
    /// Context object.
    /// </param>
    public static async Task Apply(this IEnumerable<Rule> rules,
        IServiceProvider serviceProvider,
        object context)
    {
        foreach (var curRule in rules)
        {
            await curRule.Apply(serviceProvider, context);
        }
    }
}
