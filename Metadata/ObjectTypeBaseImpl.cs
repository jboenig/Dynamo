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
using System.Reflection;
using Headway.Dynamo.Metadata.Reflection;
using Headway.Dynamo.Runtime;

namespace Headway.Dynamo.Metadata
{
	/// <summary>
	/// Provides a base implementation of <see cref="ObjectType"/>.
	/// </summary>
	public abstract class ObjectTypeBaseImpl : ObjectType
	{
        #region Member Variables

        private IMetadataProvider metadataProvider;
		private List<Property> reflectionProperties;
        private bool hasObjTypeConstructor = true;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs on <see cref="ObjectTypeBaseImpl"/> given
        /// a <see cref="IMetadataProvider"/> service reference.
        /// </summary>
        /// <param name="metadataProvider">
        /// Metadata provider service.
        /// </param>
		public ObjectTypeBaseImpl(IMetadataProvider metadataProvider = null)
		{
            this.AttachMetadataProvider(metadataProvider);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets the CLR type that this type maps to.
		/// </summary>
		public override Type CLRType
		{
			get
			{
#if false
				var assembly = Headway.Dynamo.AssemblyManager.LoadAssembly(this.AssemblyName);
				if (assembly == null)
				{
					var msg = string.Format("Unable to load assembly {0} for class {1}", this.AssemblyName, this.Name);
					throw new InvalidOperationException(msg);
				}
				return assembly.GetType(this.CLRTypeName);
#else
				// If the assembly is not loaded yet at this point, then the AssemblyResolve event on the
				// AppDomain should fire. A handler should be hooked up to this event that will use
				// the appropriate IAssemblyLoader to load the assembly.
				return Type.GetType(this.CLRTypeName);
#endif
			}
		}

		/// <summary>
		/// Determines if this object type is assignable from the specified <see cref="DataType"/>.
		/// </summary>
		/// <param name="otherDataType"><see cref="DataType"/> to test.</param>
		/// <returns>
		/// Returns true if the specified <see cref="DataType"/> is compatible with this
		/// <see cref="ObjectType"/>, otherwise returns false.
		/// </returns>
		public override bool IsAssignableFrom(DataType otherDataType)
		{
			var found = false;
			var curObjType = otherDataType as ObjectType;
			while (curObjType != null && !found)
			{
				found = this.FullName.Equals(curObjType.FullName);
				curObjType = curObjType.DerivesFrom;
			}

			return found;
		}

		/// <summary>
		/// Gets a flag indicating whether or not this data type is enumerable.
		/// </summary>
		public override bool IsEnumerable
		{
			get
			{
				return typeof(IEnumerable<object>).IsAssignableFrom(this.CLRType) ||
				       typeof(System.Collections.IEnumerable).IsAssignableFrom(this.CLRType);
			}
		}

		/// <summary>
		/// Gets the <see cref="DataType"/> of items when the <see cref="IsEnumerable"/>
		/// flag is true.
		/// </summary>
		public override DataType ItemDataType
		{
			get
			{
				DataType value = null;

				if (this.IsEnumerable)
				{
					var clrType = this.CLRType;

					if (clrType.IsGenericType && clrType.GenericTypeArguments.GetLength(0) > 0)
					{
						var itemCLRType = clrType.GenericTypeArguments[0];
						if (itemCLRType != null)
						{
                            value = this.metadataProvider.GetDataType<DataType>(itemCLRType.FullName);
						}
					}
				}

				return value;
			}
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates an instance of the class.
        /// </summary>
        /// <typeparam name="T">CLR type to return</typeparam>
        /// <param name="svcProvider">
        /// Service provider.
        /// </param>
        /// <param name="paramList">
        /// Array of parameters for constructor.
        /// </param>
        /// <returns>A new object of type T.</returns>
        public override T CreateInstance<T>(IServiceProvider svcProvider, params object[] paramList)
		{
            T instance = default(T);

            var clrType = this.CLRType;
			if (clrType == null)
			{
				throw new InvalidOperationException(this.ErrorMessageCLRTypeNotFound);
			}

            if (this.hasObjTypeConstructor)
            {
                List<object> paramsWithObjectType = new List<object>();
                if (paramList != null && paramList.Length > 0)
                {
                    paramsWithObjectType.AddRange(paramList);
                }
                paramsWithObjectType.Insert(0, this);

                try
                {
                    instance = Activator.CreateInstance(clrType, paramsWithObjectType.ToArray()) as T;
                }
                catch (MissingMethodException)
                {
                    this.hasObjTypeConstructor = false;
                    instance = Activator.CreateInstance(clrType, paramList) as T;
                }
            }
            else
            {
                instance = Activator.CreateInstance(clrType, paramList) as T;
            }

            if (instance != null)
            {
                this.InitInstance<T>(instance, svcProvider);
            }

            return instance;
		}

		/// <summary>
		/// Gets the <see cref="Property"/> matching the specified name.
		/// </summary>
		public override Property GetPropertyByName(string propertyName)
		{
			return (from prop in this.FindAllProperties()
					where prop.Name == propertyName
					select prop).FirstOrDefault();
		}

		/// <summary>
		/// Returns all properties in the inheritance hierarchy.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<Property> FindAllProperties()
		{
			return this.FindProperties(null);
		}

		/// <summary>
		/// Returns all properties in the inheritance hiearchy that match the
		/// specified filter.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public override IEnumerable<Property> FindProperties(Func<Property, bool> filter)
		{
			var propsFound = new List<Property>();
			var propNamesVisited = new List<string>();

			ObjectType curClass = this;
			while (curClass != null)
			{
				foreach (var curProperty in curClass.Properties)
				{
					var curPropName = curProperty.Name;
					var alreadyVisited = ((from propName in propNamesVisited
										   where (propName.CompareTo(curPropName) == 0)
									       select propName).FirstOrDefault() != null);

					if (!alreadyVisited)
					{
						propNamesVisited.Add(curPropName);

						if (filter == null || filter(curProperty))
						{
							propsFound.Add(curProperty);
						}
					}
				}
				curClass = curClass.DerivesFrom;
			}

			return propsFound;
		}

#if false
		/// <summary>
		/// Gets the relationship matching the specified name.
		/// </summary>
		/// <param name="relationshipName">Name of relationship to get.</param>
		/// <returns><see cref="Relationship"/> matching the specified name.</returns>
		public override Relationship GetRelationshipByName(string relationshipName)
		{
			return (from rel in this.Relationships
					where rel.Name == relationshipName
					select rel).FirstOrDefault();
		}

		/// <summary>
		/// Gets all relationships of this class including inherited relationships.
		/// </summary>
		/// <returns>List of <see cref="Property"/> objects.</returns>
		public override IEnumerable<Relationship> FindAllRelationships()
		{
			return this.FindRelationships(null);
		}

		/// <summary>
		/// Gets all relationships matching the specified filter - includes inherited relationships.
		/// </summary>
		/// <param name="filter">Filter criteria.</param>
		/// <returns>List of <see cref="Property"/> objects.</returns>
		public override IEnumerable<Relationship> FindRelationships(Func<Relationship, bool> filter)
		{
			var relsFound = new List<Relationship>();
			var relNamesVisited = new List<string>();

			objType curClass = this;
			while (curClass != null)
			{
				foreach (var curRelationship in curClass.Relationships)
				{
					var curRelName = curRelationship.Name;
					var alreadyVisited = ((from relName in relNamesVisited
										   where (relName.CompareTo(curRelName) == 0)
										   select relName).FirstOrDefault() != null);

					if (!alreadyVisited)
					{
						relNamesVisited.Add(curRelName);

						if (filter == null || filter(curRelationship))
						{
							relsFound.Add(curRelationship);
						}
					}
				}
				curClass = curClass.DerivesFrom;
			}

			return relsFound;
		}
#endif

        #endregion

        #region Metadata Provider

        /// <summary>
        /// Called by implementations in this assembly to attach
        /// the <see cref="IMetadataProvider"/> object.
        /// </summary>
        /// <param name="metadataProvider"></param>
        internal void AttachMetadataProvider(IMetadataProvider metadataProvider)
        {
            this.metadataProvider = metadataProvider;
        }

        /// <summary>
        /// Gets the <see cref="IMetadataProvider"/> service
        /// used to resolve data type information.
        /// </summary>
        protected IMetadataProvider MetadataProvider
        {
            get
            {
                if (this.metadataProvider == null)
                {
                    var msg = string.Format("ObjectType {0} has no metadata provider. Make sure that an IMetadataProvider service is attached to the ObjectType before invoking this operation.", this.FullName);
                    throw new InvalidOperationException(msg);
                }
                return this.metadataProvider;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Provides a hook to initialize new instances.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="svcProvider"></param>
        protected virtual void InitInstance<T>(T instance, IServiceProvider svcProvider)
        {
            var initObj = instance as IObjectInit;
            if (initObj != null)
            {
                initObj.Init(svcProvider);
            }
        }

        /// <summary>
        /// Gets a list of <see cref="ReflectionProperty"/> objects that encapsulate
        /// CLR properties that can be access by this <see cref="ObjectType"/>.
        /// </summary>
        /// <returns>A list of <see cref="ReflectionProperty"/> objects.</returns>
        protected IEnumerable<Property> GetReflectionProperties()
		{
			if (this.reflectionProperties != null)
			{
				return this.reflectionProperties;
			}

			this.reflectionProperties = new List<Property>();

			var clrType = this.CLRType;
			if (clrType == null)
			{
				throw new InvalidOperationException(this.ErrorMessageCLRTypeNotFound);
			}

			if (this.IsCLRTypeBaseClass)
			{
				// Get properties through reflection and iterate through each one
				var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
				var propInfos = clrType.GetProperties(bindingFlags);
				foreach (var propInfo in propInfos)
				{
					Property prop = null;

					// If property is an integral type then create a ReflectionProperty
					var integralType = IntegralType.Get(propInfo.PropertyType);
					if (integralType != null)
					{
						prop = Reflection.ReflectionProperty.Create(this.MetadataProvider, this, propInfo);
					}
					else
					{
#if false
                        // Property is not an integral type. Get a reference to the
                        // metadata provider and find the metadata.

						var metadataProvider = this.GetMetadataProvider();
						if (metadataProvider == null)
						{
							var msg = string.Format("objType {0} is not associated with a metadata provider", this.FullName);
							throw new InvalidOperationException(msg);
						}

                        // Find related objType
                        var relatedobjType = metadataProvider.GetDataType<ObjectType>(propInfo.PropertyType.FullName);
						if (relatedobjType != null)
						{
                            // This is a relationship to another objType. Check to see if there
                            // is a RelationshipAttribute on the property.
                            string relName = null;
							var relAttr = RelationshipAttribute.TryGet(propInfo);
							if (relAttr != null)
							{
								relName = relAttr.Name;
							}

							if (string.IsNullOrEmpty(relName))
							{
								// There is no explicit RelationshipAttribute on this property,
								// so generate a default relationship name for it.
								relName = string.Format("{0}_{1}", this.Name, relatedobjType.Name);
							}

							// Query the metadata provider for the relationship
							var relationship = metadataProvider.GetRelationship(relName);
							if (relationship == null)
							{
								var msg = string.Format("Unable to find relationship {0}", relName);
								throw new InvalidOperationException(msg);
							}

							prop = ReflectionNavigationProperty.Create(propInfo, relationship, true);
                        }
#else
                        prop = Reflection.ReflectionProperty.Create(this.MetadataProvider, this, propInfo);
#endif
                    }

                    if (prop != null)
					{
						this.reflectionProperties.Add(prop);
					}
				}
			}

			return this.reflectionProperties;
		}

		private string ErrorMessageCLRTypeNotFound
		{
			get
			{
				return string.Format("Unable to resolve CLRType for class {0}", this.Name);
			}
		}

		private bool IsCLRTypeBaseClass
		{
			get
			{
				var clrType = this.CLRType;
				var curClass = this.DerivesFrom;
				if (curClass != null && curClass.CLRType == clrType)
				{
					return false;
				}
				return true;
			}
		}

        #endregion
	}
}
