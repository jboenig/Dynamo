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

using System.Text;

namespace Headway.Dynamo.Runtime
{
	/// <summary>
	/// This class contains helper methods for parsing and contatenating
	/// names that contain delimeters.
	/// </summary>
	public static class NameHelpers
	{
        /// <summary>
        /// Gets the name portion of a fully qualified name.
        /// </summary>
        /// <param name="fullName">
        /// Fully qualified name.
        /// </param>
        /// <returns>
        /// Name portion (sans namespace) of the fully qualified name.
        /// </returns>
		public static string GetName(string fullName)
		{
			var delimiter = fullName.LastIndexOf('.');
			if (delimiter > 0 && delimiter < fullName.Length - 1)
			{
				return fullName.Substring(delimiter + 1);
			}
			return null;
		}

        /// <summary>
        /// Gets the namespace portion of a fully qualified name.
        /// </summary>
        /// <param name="fullName">
        /// Fully qualified name.
        /// </param>
        /// <returns>
        /// Namespace portion (sans name) of the fully qualified name.
        /// </returns>
		public static string GetNamespace(string fullName)
		{
			var delimiter = fullName.LastIndexOf('.');
			if (delimiter > 0)
			{
				return fullName.Substring(0, delimiter);
			}
			return null;
		}

        /// <summary>
        /// Creates a fully qualified name given a namespace and
        /// name.
        /// </summary>
        /// <param name="nameSpace">
        /// Namespace for the fully qualified name.
        /// </param>
        /// <param name="name">
        /// Name for the fully qualified name.
        /// </param>
        /// <returns>
        /// Fully qualified name that concatenates the namespace
        /// and name using the correct delimiter.
        /// </returns>
		public static string CreateFullName(string nameSpace, string name)
		{
			var sb = new StringBuilder();
			sb.Append(nameSpace);
			if (!nameSpace.EndsWith("."))
			{
				sb.Append(".");
			}
			sb.Append(name);
			return sb.ToString();
		}

        /// <summary>
        /// Parses a fully qualified name into tokens.
        /// </summary>
        /// <param name="fullName">
        /// Fully qualified name to parse.
        /// </param>
        /// <returns>
        /// Array of strings containing each part of the
        /// fully qualified name.
        /// </returns>
        public static string[] ParseName(string fullName)
        {
            return fullName.Split(new char[] { '.' });
        }
	}
}
