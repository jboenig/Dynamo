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
/// Base class for members of an <see cref="ObjectType"/>.
/// </summary>
public abstract class Member : INamedObject, IAttributeContainer
{
	/// <summary>
	/// Gets the owner of this member.
	/// </summary>
	public abstract ComplexType Owner
	{
		get;
	}

	/// <summary>
	/// Gets the name of the member.
	/// </summary>
	public abstract string Name
	{
		get;
	}

	/// <summary>
	/// Gets the namespace.
	/// </summary>
	public string Namespace
	{
		get
		{
			var owner = this.Owner;
			if (owner == null)
			{
				var msg = string.Format("Member named {0} is not associated with an Owner", this.Name);
				throw new InvalidOperationException(msg);
			}
			return owner.FullName;
		}
	}

	/// <summary>
	/// Gets the fullname.
	/// </summary>
	public string FullName
	{
		get
		{
			return Headway.Dynamo.Runtime.NameHelpers.CreateFullName(this.Namespace, this.Name);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="attributeName"></param>
	/// <returns></returns>
	public abstract IEnumerable<T> GetAttributes<T>(string attributeName) where T : Attribute;

	/// <summary>
	/// Gets a list of the names of all available attributes.
	/// </summary>
	/// <returns>Enumerable collection of attribute names.</returns>
	public abstract IEnumerable<string> GetAttributeNames();
}
