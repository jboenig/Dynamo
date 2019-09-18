﻿////////////////////////////////////////////////////////////////////////////////
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

using System.Text;

namespace Headway.Dynamo.Runtime
{
	/// <summary>
	/// This class contains helper methods for parsing and contatenating
	/// names that contain delimeters.
	/// </summary>
	public static class NameHelpers
	{
		public static string GetName(string fullName)
		{
			var delimiter = fullName.LastIndexOf('.');
			if (delimiter > 0 && delimiter < fullName.Length - 1)
			{
				return fullName.Substring(delimiter + 1);
			}
			return null;
		}

		public static string GetNamespace(string fullName)
		{
			var delimiter = fullName.LastIndexOf('.');
			if (delimiter > 0)
			{
				return fullName.Substring(0, delimiter);
			}
			return null;
		}

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

        public static string[] ParseName(string fullName)
        {
            return fullName.Split(new char[] { '.' });
        }
	}
}