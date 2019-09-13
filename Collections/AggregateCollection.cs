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
	public class AggregateCollection<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T>[] collections = new IEnumerable<T>[2];

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

		public IEnumerator<T> GetEnumerator()
		{
			return new AggregateCollection<T>.Enumerator(this);
		}

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
