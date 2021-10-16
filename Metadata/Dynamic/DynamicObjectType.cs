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
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Headway.Dynamo.Collections;
using Headway.Dynamo.Metadata.Reflection;

namespace Headway.Dynamo.Metadata.Dynamic
{
    /// <summary>
    /// Implements an <see cref="ObjectType"/> that is dynamically
    /// configurable at run-time.
    /// </summary>
    [Serializable]
    public class DynamicObjectType : ObjectTypeBaseImpl, ISerializable
    {
        #region Member Variables

        private string fullName;
        private Type clrType;
        private List<DynamicProperty> dynamicProperties;
        private ObjectType derivesFrom;

        #endregion

        #region Constructors

        private DynamicObjectType() :
            base(null)
        {
            this.clrType = typeof(object);
            this.fullName = this.clrType.FullName;
            this.dynamicProperties = new List<DynamicProperty>();
            this.SetDerivesFrom();
        }

        private DynamicObjectType(IMetadataProvider metadataProvider = null) :
            base(metadataProvider)
        {
            this.clrType = typeof(object);
            this.fullName = this.clrType.FullName;
            this.dynamicProperties = new List<DynamicProperty>();
            this.SetDerivesFrom();
        }

        private DynamicObjectType(IMetadataProvider metadataProvider,
            string fullName,
            Type clrType,
            ObjectType derivesFrom) :
            base(metadataProvider)
        {
            if (clrType == null)
            {
                throw new ArgumentNullException("clrType");
            }
            this.clrType = clrType;

            if (string.IsNullOrEmpty(fullName))
            {
                this.fullName = this.clrType.FullName;
            }
            else
            {
                this.fullName = fullName;
            }

            this.dynamicProperties = new List<DynamicProperty>();
            this.SetDerivesFrom(derivesFrom);
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a <see cref="DynamicObjectType"/> given
        /// a <see cref="IMetadataProvider"/>, a CLR type,
        /// and optional derives from <see cref="ObjectType"/>.
        /// </summary>
        /// <param name="metadataProvider">
        /// <see cref="IMetadataProvider"/> service used to resolve
        /// data type information.
        /// </param>
        /// <param name="fullName">
        /// Fully qualified name of new <see cref="DynamicObjectType"/> to create.
        /// </param>
        /// <param name="clrType">
        /// CLR type associated with this dynamic object type.
        /// </param>
        /// <param name="derivesFrom">
        /// <see cref="ObjectType"/> this new <see cref="DynamicObjectType"/>
        /// derives from.
        /// </param>
        /// <returns>
        /// A new <see cref="DynamicObjectType"/>.
        /// </returns>
        public static DynamicObjectType Create(IMetadataProvider metadataProvider,
            string fullName,
            Type clrType,
            ObjectType derivesFrom = null)
        {
            if (metadataProvider == null)
            {
                throw new ArgumentNullException(nameof(metadataProvider));
            }

            if (clrType == null)
            {
                throw new ArgumentNullException(nameof(clrType));
            }

            if (derivesFrom == null)
            {
                derivesFrom = metadataProvider.GetDataType<ObjectType>(clrType.BaseType.FullName);
                if (derivesFrom == null)
                {
                    derivesFrom = metadataProvider.GetDataType<ObjectType>(typeof(System.Object));
                }
            }

            return new DynamicObjectType(metadataProvider, fullName, clrType, derivesFrom);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public override string FullName
        {
            get { return this.fullName; }
        }

        /// <summary>
        /// Gets the fully qualified CLR type that this type maps to.
        /// </summary>
        public override string AssemblyQualifiedTypeName
        {
            get { return this.clrType.AssemblyQualifiedName; }
        }

        /// <summary>
        /// Gets a reference to the base class for this class.
        /// </summary>
        public override ObjectType DerivesFrom
        {
            get
            {
                return this.derivesFrom;
            }
        }

        /// <summary>
        /// Gets the default value for the data type.
        /// </summary>
        public override object DefaultValue
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the version number of this <see cref="DataType"/>.
        /// </summary>
        public override long VersionNumber
        {
            get { return -1; }
        }

        /// <summary>
        /// Gets the collection of properties associated with this class.
        /// </summary>
        public override IEnumerable<Property> Properties
        {
            get
            {
                return new AggregateCollection<Property>(this.GetReflectionProperties(),
                    this.dynamicProperties);
            }
        }

        /// <summary>
        /// Gets the CLR type that this type maps to.
        /// </summary>
        public override Type CLRType
        {
            get
            {
                return this.clrType;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeName"></param>
        /// <returns></returns>
        public override IEnumerable<T> GetAttributes<T>(string attributeName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a list of the names of all available attributes.
        /// </summary>
        /// <returns>Enumerable collection of attribute names.</returns>
        public override IEnumerable<string> GetAttributeNames()
        {
            return new string[] { };
        }

        /// <summary>
        /// Gets the list of <see cref="DataType"/>s that this data type depends on.
        /// </summary>
        /// <param name="dependencies">
        /// Collection of <see cref="DataType"/>s that this data type depends on.
        /// </param>
        public override void GetDependencies(IList<DataType> dependencies)
        {
        }

        /// <summary>
        /// Adds a property to this type.
        /// </summary>
        /// <param name="propertyName">Name of property to add</param>
        /// <param name="dataType">Data type of property to add</param>
        /// <returns>Returns the new property created</returns>
        public override Property AddProperty(string propertyName, DataType dataType)
        {
            var existingProp = (from p in this.dynamicProperties
                                where p.Name == propertyName
                                select p).FirstOrDefault();

            if (existingProp != null)
            {
                var msg = string.Format("Object type {0} already has a property named {1}", this.Name, propertyName);
                throw new ArgumentException(msg, "propertyName");
            }

            var dynamicProp = DynamicProperty.Create(this, propertyName, dataType);
            this.dynamicProperties.Add(dynamicProp);
            return dynamicProp;
        }

        #endregion

        #region Serialization

        private DynamicObjectType(SerializationInfo info, StreamingContext context)
        {
            this.fullName = info.GetString("fullName");
            var clrTypeName = info.GetString("clrType");
            if (!string.IsNullOrEmpty(clrTypeName))
            {
                this.clrType = Type.GetType(clrTypeName);
            }

            this.dynamicProperties = info.GetValue("dynamicProperties", typeof(List<DynamicProperty>)) as List<DynamicProperty>;

            // TODO: Deserialize the DerivesFrom property
            // This is a temporary hack!  This currently only works for a DerivesFrom that
            // is loaded through reflection!!! Still need to implement a real serialization
            // scheme for DerivesFrom if we need to support dynamic base classes.
            this.SetDerivesFrom(null);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("fullName", this.fullName);
            info.AddValue("clrType", this.clrType.AssemblyQualifiedName);
            info.AddValue("dynamicProperties", this.dynamicProperties);

            // TODO: Serialize the DerivesFrom property
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            // IServiceProvider is attached to the StreamingContext
            var svcProvider = context.Context as IServiceProvider;
            if (svcProvider != null)
            {
                // Get the IMetadataProvider service
                var metadataProvider = svcProvider.GetService(typeof(IMetadataProvider)) as IMetadataProvider;
                if (metadataProvider != null)
                {
                    this.AttachMetadataProvider(metadataProvider);
                }
            }

            // Ensure that properties have back-pointer to this
            // object type
            foreach (var curprop in this.dynamicProperties)
            {
                curprop.SetOwner(this);
            }
        }

        #endregion

        #region Implementation

        private void SetDerivesFrom(ObjectType value = null)
        {
            this.derivesFrom = value;
        }

        #endregion
    }
}
