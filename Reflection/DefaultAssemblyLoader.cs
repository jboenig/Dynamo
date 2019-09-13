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
using System.Reflection;

namespace Headway.Dynamo.Reflection
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class DefaultAssemblyLoader : IAssemblyLoader
	{
		#region Constructors

		/// <summary>
		/// 
		/// </summary>
		public DefaultAssemblyLoader()
		{
		}

		#endregion

		#region IAssemblyLoader Interface

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public Assembly LoadAssembly(string assemblyName)
		{
			foreach (var curAssembly in this.GetAllAssemblies())
			{
				if (curAssembly.FullName.CompareTo(assemblyName) == 0)
				{
					return curAssembly;
				}
			}

			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="assemblyName"></param>
		/// <returns></returns>
		public byte[] GetAssemblyBytes(string assemblyName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Assembly> GetAllAssemblies()
		{
			var appDomain = AppDomain.CurrentDomain;
			return appDomain.GetAssemblies();
		}

		#endregion
	}
}
