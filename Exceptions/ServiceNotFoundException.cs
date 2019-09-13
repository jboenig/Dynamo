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

namespace Headway.Dynamo.Exceptions
{
    /// <summary>
    /// Thrown when a service cannot be located through injection
    /// or through IServiceProvider.
    /// </summary>
    public sealed class ServiceNotFoundException : Exception
    {
        /// <summary>
        /// Constructs a <see cref="ServiceNotFoundException"/> given
        /// the type of service that was not found.
        /// </summary>
        /// <param name="serviceType">
        /// Type of service that could not be located.
        /// </param>
        public ServiceNotFoundException(Type serviceType) :
            base(string.Format("Unable to locate the service {0}", serviceType.Name))
        {
        }
    }
}
