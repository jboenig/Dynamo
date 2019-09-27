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
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.Repository
{
	/// <summary>
	/// Interface to an object that stores and retrieves objects on behalf
	/// of a session.
	/// </summary>
    public interface IObjectRepository<TObject> where TObject : class
    {
        /// <summary>
        /// Gets the name of the repository.
        /// </summary>
        string Name
		{
			get;
		}

		/// <summary>
		/// Creates a connection to the data source.
		/// </summary>
		/// <returns></returns>
		IConnection CreateConnection();

		/// <summary>
		/// Gets a queryable object from the data source for the specified class and connection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection"></param>
		/// <param name="objType"></param>
		/// <returns></returns>
		IQueryable<T> GetQueryable<T>(IConnection connection, ObjectType objType);

//		public abstract IQueryable<T> GetQueryable<T>(IConnection connection, ObjectType objType, Expression expression);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		ITransaction BeginTransaction(IConnection connection);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Add(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Update(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        void Remove(TObject obj);

        /// <summary>
        /// 
        /// </summary>
        void SaveChanges();
    }
}
