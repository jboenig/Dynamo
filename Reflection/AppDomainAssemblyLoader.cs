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
