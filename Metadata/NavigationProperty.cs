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

namespace Headway.Dynamo.Metadata;

/// <summary>
/// Encapsulates a property that can be used to navigate
/// a <see cref="Relationship"/>.
/// </summary>
public abstract class NavigationProperty : Property
{
	/// <summary>
	/// Gets the <see cref="Relationship"/> bound to this property.
	/// </summary>
	public abstract Relationship Relationship
	{
		get;
	}

	/// <summary>
	/// Gets a flag indicating whether this <see cref="NavigationProperty"/> is
	/// the source or target of the <see cref="Relationship"/>.
	/// </summary>
	public abstract bool IsSource
	{
		get;
	}

	/// <summary>
	/// Gets the <see cref="DataType"/> of this property.
	/// </summary>
	public override DataType DataType
	{
		get
		{
			var rel = this.Relationship;
			if (rel == null)
			{
				var msg = string.Format("Navigation property {0} is not bound to a Relationship", this.Name);
				throw new InvalidOperationException(msg);
			}
			return (this.IsSource) ? (rel.Source) : (rel.Target);
		}
	}
}
