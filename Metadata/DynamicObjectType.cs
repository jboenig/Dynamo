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
using System.Collections.Generic;
using System.Runtime.Serialization;
using Headway.Dynamo.Collections;
using Headway.Dynamo.Metadata.Reflection;

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class DynamicObjectType : ObjectTypeBaseImpl, ISerializable
    {
        #region Member Variables

        private string fullName;
        private Type clrType;
        private List<DynamicProperty> dynamicProperties;
        private ObjectType derivesFrom;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataProvider"></param>
        public DynamicObjectType(IMetadataProvider metadataProvider) :
            base(metadataProvider)
        {
            this.clrType = typeof(object);
            this.fullName = this.clrType.FullName;
            this.dynamicProperties = new List<DynamicProperty>();
            this.SetDerivesFrom();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="derivesFrom"></param>
        public DynamicObjectType(IMetadataProvider metadataProvider, Type clrType, ObjectType derivesFrom = null) :
            base(metadataProvider)
        {
            if (clrType == null)
            {
                throw new ArgumentNullException("clrType");
            }
            this.clrType = clrType;
            this.fullName = this.clrType.FullName;
            this.dynamicProperties = new List<DynamicProperty>();
            this.SetDerivesFrom(derivesFrom);
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
                return new AggregateCollection<Property>(this.GetReflectionProperties(), this.dynamicProperties);
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
            if (value != null)
            {
                this.derivesFrom = value;
            }
            else
            {
                throw new NotImplementedException();
                //if (this.clrType != null)
                //{
                //    var baseType = this.clrType.BaseType;
                //    if (baseType != null)
                //    {
                //        this.derivesFrom = ReflectionObjectType.Get(baseType.FullName);
                //    }
                //}
            }
        }

        #endregion
    }
}
