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
	/// Implementation of the <see cref="IAssemblyLoader"/>
    /// interface based on all assemblies in the current
    /// AppDomain.
	/// </summary>
	public sealed class AppDomainAssemblyLoader : IAssemblyLoader
	{
		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public AppDomainAssemblyLoader()
		{
		}

        #endregion

        #region IAssemblyLoader Interface

        /// <summary>
        /// Loads the specified assembly into the current app domain.
        /// </summary>
        /// <param name="assemblyName">Fully qualified name of the assembly.</param>
        /// <returns>
        /// Returns the Assembly that was loaded or null if not found.
        /// </returns>
        /// <remarks>
        /// This implementation iterates through all assemblies returned
        /// by the <see cref="AppDomainAssemblyLoader.GetAllAssemblies"/>
        /// method and returns the one matching the specified assembly name
        /// parameter.  If not found, returns null.
        /// </remarks>
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
		/// Returns all assemblies in the current AppDomain.
		/// </summary>
		/// <returns>
        /// Collection of assemblies in the current AppDomain.
        /// </returns>
		public IEnumerable<Assembly> GetAllAssemblies()
		{
			var appDomain = AppDomain.CurrentDomain;
			return appDomain.GetAssemblies();
		}

		#endregion
	}
}
