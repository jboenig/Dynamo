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

using System.Reflection;

namespace Headway.Dynamo.Reflection;

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
