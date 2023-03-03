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

using Headway.Dynamo.Runtime;
using Headway.Dynamo.Serialization;
using Headway.Dynamo.Exceptions;
using Newtonsoft.Json;

namespace Headway.Dynamo.Repository.Implementation;

/// <summary>
/// Implements a simple flat file repository.
/// </summary>
/// <typeparam name="TObject">
/// Type of object to store in the repository
/// </typeparam>
public class FlatFileRepo<TObject> : IObjectRepository<TObject>
    where TObject : class
{
    private readonly string filePath;
    private readonly IServiceProvider svcProvider;
    private readonly JsonSerializerSettings jsonSettings;
    private List<TObject> objects;
    private bool isRepoLoaded = false;
    private const string JsonExtension = ".json";

    /// <summary>
    /// Constructs a <see cref="FlatFileRepo{TObject}"/>
    /// </summary>
    /// <param name="filePath">
    /// Path to the file that persists the repo.
    /// </param>
    /// <param name="serializerConfigSvc">
    /// Serializer configuration service
    /// </param>
    /// <param name="svcProvider">
    /// Service provider.
    /// </param>
    public FlatFileRepo(string filePath,
        ISerializerConfigService serializerConfigSvc,
        IServiceProvider svcProvider)
    {
        if (serializerConfigSvc == null)
        {
            throw new ArgumentNullException(nameof(serializerConfigSvc));
        }

        this.filePath = ResolveFilePath(filePath);

        this.svcProvider = svcProvider;

        this.jsonSettings = serializerConfigSvc.ConfigureJsonSerializerSettings(
            typeof(TObject),
            this.svcProvider);
    }

    /// <summary>
    /// Constructs a <see cref="FlatFileRepo{TObject}"/>
    /// </summary>
    /// <param name="serializerConfigSvc">
    /// Serializer configuration service
    /// </param>
    /// <param name="svcProvider">
    /// Service provider.
    /// </param>
    public FlatFileRepo(ISerializerConfigService serializerConfigSvc,
        IServiceProvider svcProvider) :
        this(null, serializerConfigSvc, svcProvider)
    {
    }

    /// <summary>
    /// Gets a queryable object from the data source for the specified class and connection.
    /// </summary>
    /// <returns></returns>
    public IQueryable<TObject> GetQueryable()
    {
        this.EnsureRepoLoaded();

        return this.objects.AsQueryable();
    }

    /// <summary>
    /// Adds an object to the repo.
    /// </summary>
    /// <param name="obj">
    /// Object to add
    /// </param>
    public void Add(TObject obj)
    {
        this.EnsureRepoLoaded();

        var pkAccessor = obj as IPrimaryKeyAccessor;
        if (pkAccessor != null)
        {
            var pkValue = pkAccessor.PrimaryKey;
            if (pkValue != null)
            {
                var existingObj = this.GetObjectById(pkValue);
                if (existingObj != null)
                {
                    // Duplicate key
                    throw new DuplicateKeyException(pkValue);
                }
            }
        }
        this.objects.Add(obj);
    }

    /// <summary>
    /// Adds a collection of objects to the repository.
    /// </summary>
    /// <param name="objColl">Collection of objects to add</param>
    public void Add(IEnumerable<TObject> objColl)
    {
        this.EnsureRepoLoaded();

        // TODO: optimize this
        foreach (var obj in objColl)
        {
            this.Add(obj);
        }
    }

    /// <summary>
    /// Updates an object in the repo.
    /// </summary>
    /// <param name="obj">
    /// Object to update
    /// </param>
    public void Update(TObject obj)
    {
        this.EnsureRepoLoaded();

        var pkAccessor = obj as IPrimaryKeyAccessor;
        if (pkAccessor != null)
        {
            var pkValue = pkAccessor.PrimaryKey;
            if (pkValue != null)
            {
                var existingObj = this.GetObjectById(pkValue);
                if (existingObj != null)
                {
                    this.objects.Remove(existingObj);
                }
                this.objects.Add(obj);
            }
        }
    }

    /// <summary>
    /// Removes an object from the repo.
    /// </summary>
    /// <param name="obj">
    /// Object to remove.
    /// </param>
    public void Remove(TObject obj)
    {
        var pkAccessor = obj as IPrimaryKeyAccessor;
        if (pkAccessor != null)
        {
            var pkValue = pkAccessor.PrimaryKey;
            if (pkValue != null)
            {
                var existingObj = this.GetObjectById(pkValue);
                if (existingObj != null)
                {
                    this.objects.Remove(existingObj);
                }
            }
        }
    }

    /// <summary>
    /// Removes all objects from the repository.
    /// </summary>
    public void RemoveAll()
    {
        this.objects.Clear();
    }

    /// <summary>
    /// Saves all pending changes to the repo.
    /// </summary>
    public void SaveChanges()
    {
        Task.Run(() => this.SaveChangesAsync()).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Saves all pending changes to the repo asynchronously.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await this.SaveRepoAsync();
    }

    private void EnsureRepoLoaded()
    {
        if (!this.isRepoLoaded)
        {
            Task.Run(() => this.LoadRepoAsync()).GetAwaiter().GetResult();
            this.isRepoLoaded = true;
        }
    }

    private async Task LoadRepoAsync()
    {
        if (File.Exists(this.filePath))
        {
            using (var stream = File.Open(this.filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(stream))
            {
                var jsonStr = await sr.ReadToEndAsync();
                this.objects = (List<TObject>)JsonConvert.DeserializeObject<List<TObject>>(jsonStr, this.jsonSettings);
            }
        }
        else
        {
            this.objects = new List<TObject>();
        }
    }

    private async Task SaveRepoAsync()
    {
        var json = JsonConvert.SerializeObject(this.objects, this.jsonSettings);

        this.CreateDirectoryIfNeeded();

        using (var stream = File.Open(this.filePath, FileMode.Create, FileAccess.Write))
        using (StreamWriter sw = new StreamWriter(stream))
        {
            await sw.WriteLineAsync(json);
        }
    }

    private TObject GetObjectById(object pkValue)
    {
        return (from o in this.objects.Cast<IPrimaryKeyAccessor>()
                where o.PrimaryKey == pkValue
                select o).Cast<TObject>().FirstOrDefault();
    }

    private static string ResolveFilePath(string filePathIn)
    {
        string filePathOut;

        if (string.IsNullOrEmpty(filePathIn))
        {
            // Generate default path and filename
            filePathOut = Path.Combine(GetDefaultDirectory(),
                typeof(TObject).FullName + JsonExtension);
        }
        else if (!Path.IsPathRooted(filePathIn))
        {
            // Use relative filename and add default path
            filePathOut = Path.Combine(GetDefaultDirectory(),
                filePathIn);
        }
        else
        {
            // Use entire path as-is
            filePathOut = filePathIn;
        }

        // Set default file extension to .json
        if (!Path.HasExtension(filePathOut))
        {
            filePathOut += JsonExtension;
        }

        return filePathOut;
    }

    private static string GetDefaultDirectory()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var objTypeNamespaces = typeof(TObject).FullName.Split('.');
        if (objTypeNamespaces.Length > 1)
        {
            return Path.Combine(localAppData, objTypeNamespaces[0]);
        }
        return localAppData;
    }

    private void CreateDirectoryIfNeeded()
    {
        var folderPath = Path.GetDirectoryName(this.filePath);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
}
