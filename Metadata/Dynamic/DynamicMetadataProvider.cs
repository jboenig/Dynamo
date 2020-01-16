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

using System;
using System.Collections.Generic;
using Headway.Dynamo.Repository;

namespace Headway.Dynamo.Metadata.Dynamic
{
    /// <summary>
    /// Implementation of <see cref="IMetadataProvider"/> service
    /// that allows data types to be registered manually.
    /// </summary>
    public class DynamicMetadataProvider : ChainedMetadataProvider
    {
        private readonly Dictionary<string, DynamicObjectType> objTypes;
        private readonly IMetadataProvider nextProvider;

        /// <summary>
        /// Constructs a <see cref="DynamicMetadataProvider"/> given a
        /// reference to the next <see cref="IMetadataProvider"/> in
        /// a chain.
        /// </summary>
        /// <param name="nextProvider">
        /// Next metadata provider in the chain
        /// </param>
        public DynamicMetadataProvider(IMetadataProvider nextProvider = null) :
            base(nextProvider)
        {
            this.nextProvider = nextProvider;
            this.objTypes = new Dictionary<string, DynamicObjectType>();
        }

        /// <summary>
        /// Gets a <see cref="DataType"/> by fully
        /// qualified name.
        /// </summary>
        /// <param name="fullName">Full name of the data type to retrieve</param>
        /// <returns>
        /// The <see cref="DataType"/> matching the specified name or null
        /// if the data type does not exist.
        /// </returns>
        protected override T GetDataTypeInternal<T>(string fullName)
        {
            T result = default(T);

            if (this.objTypes.ContainsKey(fullName))
            {
                result = this.objTypes[fullName] as T;
            }

            return result;
        }

        /// <summary>
        /// Loads all <see cref="DynamicObjectType"/> objects from
        /// the specified repository.
        /// </summary>
        /// <param name="repo">
        /// Repository from which to load <see cref="DynamicObjectType"/>
        /// entities.
        /// </param>
        public void Load(IObjectRepository<DynamicObjectType> repo)
        {
            if (repo == null)
            {
                throw new ArgumentNullException(nameof(repo));
            }
            this.Load(repo.GetQueryable());
        }

        /// <summary>
        /// Loads <see cref="DynamicObjectType"/> objects from
        /// the specified collection.
        /// </summary>
        /// <param name="dynamicObjTypes">
        /// Collection from which to load <see cref="DynamicObjectType"/>
        /// entities.
        /// </param>
        public void Load(IEnumerable<DynamicObjectType> dynamicObjTypes)
        {
            if (dynamicObjTypes == null)
            {
                throw new ArgumentNullException(nameof(dynamicObjTypes));
            }

            foreach (var curObjType in dynamicObjTypes)
            {
                this.objTypes.Add(curObjType.FullName, curObjType);
            }
        }

        /// <summary>
        /// Registers a new <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="clrType"></param>
        /// <returns>
        /// Returns a new <see cref="ObjectType"/>
        /// </returns>
        public ObjectType RegisterObjectType(string fullName,
            Type clrType)
        {
            var objType = DynamicObjectType.Create(this, fullName, clrType);
            this.RegisterObjectType(this, objType);
            return objType;
        }

        /// <summary>
        /// Registers a new <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="fullName"></param>
        /// <param name="clrType"></param>
        /// <returns>
        /// Returns a new <see cref="ObjectType"/>
        /// </returns>
        public ObjectType RegisterObjectType(IMetadataProvider metadataProvider,
            string fullName,
            Type clrType)
        {
            var objType = DynamicObjectType.Create(metadataProvider, fullName, clrType);
            this.RegisterObjectType(metadataProvider, objType);
            return objType;
        }

        /// <summary>
        /// Registers the given <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="objType"></param>
        public void RegisterObjectType(DynamicObjectType objType)
        {
            this.RegisterObjectType(this, objType);
        }

        /// <summary>
        /// Registers the given <see cref="DynamicObjectType"/> with this
        /// <see cref="DynamicMetadataProvider"/>.
        /// </summary>
        /// <param name="metadataProvider"></param>
        /// <param name="objType"></param>
        public void RegisterObjectType(IMetadataProvider metadataProvider, DynamicObjectType objType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (this.objTypes.ContainsKey(objType.FullName))
            {
                var msg = string.Format("{0} is already registered with this dynamic metadata provider", objType.FullName);
                throw new InvalidOperationException(msg);
            }

            // Attach the metadata provider
            if (metadataProvider == null)
            {
                metadataProvider = this;
            }
            objType.AttachMetadataProvider(metadataProvider);

            this.objTypes.Add(objType.FullName, objType);
        }
    }
}
