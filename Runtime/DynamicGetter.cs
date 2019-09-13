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

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// This class provides an abstract interface to objects that
    /// can be used to retrieve a piece of data from a dynamic
    /// context.
    /// </summary>
    /// <typeparam name="T">Type of data to retrieve</typeparam>
    /// <remarks>
    /// The purpose of dynamic getters is to allow the consumer of
    /// the getter to retrieve a piece of data from the dynamic
    /// context without any knowledge of how or where it is stored
    /// in the context.
    /// </remarks>
    public abstract class DynamicGetter<T>
    {
        /// <summary>
        /// Returns the value in the dynamic context that the
        /// getter is designed to retrieve.
        /// </summary>
        /// <param name="serviceProvider">Interface to service provider</param>
        /// <param name="context">User defined context data</param>
        /// <returns>
        /// A value from the dynamic context of type T.
        /// </returns>
        public abstract T Get(IServiceProvider serviceProvider, dynamic context);
    }
}
