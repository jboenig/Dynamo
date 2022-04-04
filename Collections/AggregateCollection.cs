////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

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

        #region Enumerator Class

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

        #endregion
    }
}
