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
using System.Collections.Generic;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata
{
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
}
