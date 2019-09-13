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

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Encapsulates the metadata for a type of object.
	/// </summary>
	public abstract class ObjectType : ComplexType
	{
		#region Public Properties

		/// <summary>
		/// Gets a reference to the base class for this class.
		/// </summary>
		public abstract ObjectType DerivesFrom
		{
			get;
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="paramList"></param>
        /// <returns>A new object of type T.</returns>
        public abstract T CreateInstance<T>(params object[] paramList) where T : class;

        #endregion
    }
}
