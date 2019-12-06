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
    /// Interface to service that creates objects of a
    /// given type.
    /// </summary>
    /// <typeparam name="TObject">
    /// Type of objects this factory creates.
    /// </typeparam>
    public interface IObjectFactory<TObject> where TObject : class
    {
        /// <summary>
        /// Creates an instance of type TObject.
        /// </summary>
        /// <param name="svcProvider">
        /// Service provider used during object creation.
        /// </param>
        /// <param name="context">
        /// Context object used during creation.
        /// </param>
        /// <returns>
        /// A new instance of type TObject.
        /// </returns>
        TObject CreateInstance(IServiceProvider svcProvider, object context);
    }
}
