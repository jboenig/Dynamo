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

namespace Headway.Dynamo.Repository;

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
    /// Adds a collection of objects to the repository.
    /// </summary>
    /// <param name="objColl">Collection of objects to add</param>
    void Add(IEnumerable<TObject> objColl);

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
    /// Removes all objects from the repository.
    /// </summary>
    void RemoveAll();

    /// <summary>
    /// Save changes made to the repository.
    /// </summary>
    void SaveChanges();

    /// <summary>
    /// Save changes made to the repository asynchronously.
    /// </summary>
    Task SaveChangesAsync();
}
