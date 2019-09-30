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

using System.Linq;

namespace Headway.Dynamo.Repository
{
	/// <summary>
	/// Interface to an object that stores and retrieves objects
    /// of a specified type.
	/// </summary>
    public interface IObjectRepository<TObject> where TObject : class
    {
		/// <summary>
		/// Gets a queryable object for accessing the repository.
		/// </summary>
		/// <returns></returns>
		IQueryable<TObject> GetQueryable();

        /// <summary>
        /// Adds an object to the repository.
        /// </summary>
        /// <param name="obj">Object to add</param>
        void Add(TObject obj);

        /// <summary>
        /// Updates an object in the repository.
        /// </summary>
        /// <param name="obj">
        /// Object to update.
        /// </param>
        void Update(TObject obj);

        /// <summary>
        /// Removes the specified object from the repository.
        /// </summary>
        /// <param name="obj">
        /// Object to remove.
        /// </param>
        void Remove(TObject obj);

        /// <summary>
        /// Save changes made to the repository.
        /// </summary>
        void SaveChanges();
    }
}
