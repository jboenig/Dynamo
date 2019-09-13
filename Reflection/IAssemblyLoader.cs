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
	/// Interface to objects that retrieve and load assemblies.
	/// </summary>
	public interface IAssemblyLoader
	{
		/// <summary>
		/// Loads the specified assembly into the current app domain.
		/// </summary>
		/// <param name="assemblyName">Fully qualified name of the assembly.</param>
		/// <returns>Returns the Assembly that was loaded or null if not found.</returns>
		Assembly LoadAssembly(string assemblyName);

		/// <summary>
		/// Returns the assembly as a byte array.
		/// </summary>
		/// <param name="assemblyName">Fully qualified name of the assembly.</param>
		/// <returns>Contents of the assembly as a byte array.</returns>
		byte[] GetAssemblyBytes(string assemblyName);

		/// <summary>
		/// Returns a list of all available assemblies.
		/// </summary>
		/// <returns>Enumerable collection of Assembly objects.</returns>
		IEnumerable<Assembly> GetAllAssemblies();
	}
}
