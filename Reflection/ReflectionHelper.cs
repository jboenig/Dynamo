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
using System.Linq;
using System.Reflection;

namespace Headway.Dynamo.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTypeName"></param>
        /// <param name="methodName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetDatafromGenericMethod<T>(string dataTypeName, string methodName, object obj)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).FirstOrDefault(x => x.FullName == dataTypeName);
            MethodInfo method = typeof(T).GetMethod(methodName);
            if (types == null)
                throw new InvalidOperationException("Unable to identity the dataType");
            if (method == null)
                throw new InvalidOperationException("Unable to identity the Generic method.");
            MethodInfo generic = method.MakeGenericMethod(types);
            return generic.Invoke(obj, null);

        }
    }
}
