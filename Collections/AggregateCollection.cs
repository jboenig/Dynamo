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
using System.Collections.Generic;

namespace Headway.Dynamo.Collections
{
    /// <summary>
    /// Implements an enumerable collection that is composed of
    /// two collections.
    /// </summary>
    /// <typeparam name="T">Data type stored in the collection</typeparam>
	public class AggregateCollection<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T>[] collections = new IEnumerable<T>[2];

        /// <summary>
        /// Constructs an <see cref="AggregateCollection{T}"/> given
        /// two collections.
        /// </summary>
        /// <param name="collection1">
        /// First collection to aggregate
        /// </param>
        /// <param name="collection2">
        /// Second collection to aggregate
        /// </param>
		public AggregateCollection(IEnumerable<T> collection1, IEnumerable<T> collection2)
		{
			if (collection1 == null)
			{
				throw new ArgumentNullException("collection1");
			}

			if (collection2 == null)
			{
				throw new ArgumentNullException("collection2");
			}

			this.collections[0] = collection1;
			this.collections[1] = collection2;
		}

        /// <summary>
        /// Gets an enumerator for accessing the collection.
        /// </summary>
        /// <returns>Enumerator for accessing collection</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return new AggregateCollection<T>.Enumerator(this);
		}

        /// <summary>
        /// Gets an enumerator for accessing the collection.
        /// </summary>
        /// <returns>Enumerator for accessing collection</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new AggregateCollection<T>.Enumerator(this);
		}

		private sealed class Enumerator : IEnumerator<T>
		{
			private readonly AggregateCollection<T> masterColl;
			private IEnumerator<T> currentEnumerator;
			private int collIdx;

			public Enumerator(AggregateCollection<T> coll)
			{
				this.masterColl = coll;
			}

			public T Current
			{
				get { return this.currentEnumerator.Current; }
			}

			public void Dispose()
			{
				if (this.currentEnumerator != null)
				{
					this.currentEnumerator.Dispose();
					this.currentEnumerator = null;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get { return this.currentEnumerator.Current; }
			}

			public bool MoveNext()
			{
				bool found = false;
				bool done = false;

				while (!found && !done)
				{
					if (this.currentEnumerator == null && this.collIdx < 2)
					{
						this.currentEnumerator = this.masterColl.collections[this.collIdx++].GetEnumerator();
					}

					if (this.currentEnumerator != null)
					{
						found = this.currentEnumerator.MoveNext();
						if (!found)
						{
							this.currentEnumerator.Dispose();
							this.currentEnumerator = null;
							done = (this.collIdx >= 2);
						}
					}
				}

				return found;
			}

			public void Reset()
			{
				this.collIdx = 0;

				if (this.currentEnumerator != null)
				{
					this.currentEnumerator.Dispose();
					this.currentEnumerator = null;
				}
			}
		}
	}
}
