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

using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata;

/// <summary>
/// Encapsulates an relationship between two <see cref="ObjectType"/> objects.
/// </summary>
public abstract class Relationship : INamedObject
{
	/// <summary>
	/// Gets the name of the relationship.
	/// </summary>
	public abstract string Name
	{
		get;
	}

	/// <summary>
	/// Gets the namespace of the relationship.
	/// </summary>
	public abstract string Namespace
	{
		get;
	}

	/// <summary>
	/// Gets the fully qualified name of the relationship.
	/// </summary>
	public abstract string FullName
	{
		get;
	}

	/// <summary>
	/// Gets the <see cref="ObjectType"/> that is the source of the relationship.
	/// </summary>
	public abstract ObjectType Source
	{
		get;
	}

	/// <summary>
	/// Gets the <see cref="ObjectType"/> that is the target of the relationship.
	/// </summary>
	public abstract ObjectType Target
	{
		get;
	}

	/// <summary>
	/// Gets the cardinality of the relationship.
	/// </summary>
	public abstract RelationshipCardinality Cardinality
	{
		get;
	}

	/// <summary>
	/// Gets an attribute of the relationship by name.
	/// </summary>
	/// <typeparam name="T">Type of attribute to return.</typeparam>
	/// <param name="attributeName">Name of attribute to return.</param>
	/// <returns>Attribute matching the specified name or null if it is not found.</returns>
	public abstract T GetAttribute<T>(string attributeName);
}
