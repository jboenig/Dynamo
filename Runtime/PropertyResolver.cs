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
using System.Text;
using Headway.Dynamo.Exceptions;

namespace Headway.Dynamo.Runtime
{
    /// <summary>
    /// Contains methods for resolving properties.
    /// </summary>
    public static class PropertyResolver
    {
        /// <summary>
        /// Used to specify the type of delimiter used to
        /// represent properties in the input string.
        /// </summary>
        public enum DelimiterMode
        {
            /// <summary>
            /// Specifies that curly braces are used - {PropertyName}
            /// </summary>
            CurlyBraces,

            /// <summary>
            /// Specifies that dollar sign plus parenthesis are
            /// used - $(PropertyName)
            /// </summary>
            DollarParen,

            /// <summary>
            /// Specifies that the input string may contain any combination
            /// of delimiters for property names
            /// </summary>
            Any
        }

        /// <summary>
        /// Return the value of the specified property on
        /// the object.
        /// </summary>
        /// <typeparam name="T">
        /// Data type of the property to get.
        /// </typeparam>
        /// <param name="obj">
        /// Object containing the property.
        /// </param>
        /// <param name="propertyName">
        /// Name of the property to retrieve.
        /// </param>
        /// <returns>
        /// Value of the property
        /// </returns>
        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            T res = default(T);

            if (obj != null)
            {
                var propAccessor = obj as IPropertyAccessor;
                if (propAccessor != null)
                {
                    res = propAccessor.GetPropertyValue<T>(propertyName);
                }
                else
                {
                    // Attempt to use reflection
                    var prop = obj.GetType().GetProperty(propertyName);
                    if (prop != null)
                    {
                        res = (T)prop.GetValue(obj);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Attempts to resolve the source string as a single
        /// variable name. This only works if the entire source
        /// string is a single variable name.
        /// </summary>
        /// <param name="obj">
        /// Object containing the property value.
        /// </param>
        /// <param name="source">
        /// String containing property name as a variable.
        /// </param>
        /// <param name="value">Value of the property found</param>
        /// <returns>
        /// Returns true if the source string consists of a single variable name
        /// and the variable name is resolved.
        /// </returns>
        public static bool TryResolveSinglePropertyValue(object obj, string source, out object propValue)
        {
            return TryResolveSinglePropertyValue(DelimiterMode.Any, obj, source, out propValue);
        }

        /// <summary>
        /// Attempts to resolve the source string as a single
        /// variable name. This only works if the entire source
        /// string is a single variable name.
        /// </summary>
        /// <param name="delimiterMode">
        /// Specifies which delimiter or delimiters to use for parsing
        /// property names.
        /// </param>
        /// <param name="obj">
        /// Object containing the property value.
        /// </param>
        /// <param name="source">
        /// String containing property name as a variable.
        /// </param>
        /// <param name="propValue">Value of the property found</param>
        /// <returns>
        /// Returns true if the source string consists of a single variable name
        /// and the variable name is resolved.
        /// </returns>
        public static bool TryResolveSinglePropertyValue(DelimiterMode delimiterMode, object obj, string source, out object propValue)
        {
            if (delimiterMode == DelimiterMode.CurlyBraces)
            {
                return TryResolveSinglePropertyValue("{", "}", obj, source, out propValue);
            }
            else if (delimiterMode == DelimiterMode.DollarParen)
            {
                return TryResolveSinglePropertyValue("$(", ")", obj, source, out propValue);
            }
            else if (delimiterMode == DelimiterMode.Any)
            {
                if (TryResolveSinglePropertyValue("$(", ")", obj, source, out propValue))
                {
                    return true;
                }
                return TryResolveSinglePropertyValue("{", "}", obj, source, out propValue);
            }

            var msg = string.Format("Delimiter mode {0} is not currently supported", Enum.GetName(typeof(DelimiterMode), delimiterMode));
            throw new ArgumentException(msg, "delimiterMode");
        }

        /// <summary>
        /// Attempts to resolve the source string as a single
        /// variable name. This only works if the entire source
        /// string is a single variable name.
        /// </summary>
        /// <param name="startPropDelimiter">Start delimiter, usually $(</param>
        /// <param name="endPropDelimiter">End delimiter, usually )</param>
        /// <param name="obj">
        /// Object containing the property value.
        /// </param>
        /// <param name="source">
        /// String containing property name as a variable.
        /// </param>
        /// <param name="propValue">Value of the property found</param>
        /// <returns>
        /// Returns true if the source string consists of a single variable name
        /// and the variable name is resolved.
        /// </returns>
        private static bool TryResolveSinglePropertyValue(string startPropDelimiter, string endPropDelimiter, object obj, string source,
            out object propValue)
        {
            bool success = false;
            propValue = null;

            var idxStartPropDelimiter = source.IndexOf(startPropDelimiter);
            if (idxStartPropDelimiter == 0)
            {
                // This index points to the first character in the property name
                var idxStartPropName = idxStartPropDelimiter + startPropDelimiter.Length;
                var idxEndPropDelimiter = source.IndexOf(endPropDelimiter, idxStartPropName);
                if (idxEndPropDelimiter >= 0)
                {
                    // This index points to last character in property name
                    var idxEndPropName = idxEndPropDelimiter - endPropDelimiter.Length;

                    // We found a property name
                    var propName = source.Substring(idxStartPropName, idxEndPropName - idxStartPropName + 1);
                    var propNameWithDelimiters = source.Substring(idxStartPropDelimiter, idxEndPropDelimiter - idxStartPropDelimiter + 1);
                    string formatSpec = null;

                    var idxFormatSpec = propName.IndexOf(':');
                    if (idxFormatSpec > 0 && idxFormatSpec < propName.Length - 1)
                    {
                        // Format spec included, so parse it out
                        formatSpec = propName.Substring(idxFormatSpec + 1);
                        propName = propName.Substring(0, idxFormatSpec);
                    }

                    // Get the property value and append it to the result
                    try
                    {
                        propValue = GetPropertyValue<object>(obj, propName);
                        success = true;
                    }
                    catch (PropertyNotFoundException)
                    {
                        propValue = null;
                        success = false;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Replaces property names in the source string with the actual
        /// values of the properties.
        /// </summary>
        /// <param name="source">
        /// String containing property references.
        /// </param>
        /// <returns>
        /// The source string with all property references replaced with
        /// actual property values.
        /// </returns>
        /// <remarks>
        /// This method calls
        /// <see cref="ResolvePropertyValues(DelimiterMode, object, string)"/>
        /// with <see cref="DelimiterMode"/> set to <see cref="DelimiterMode.Any"/>.
        /// This means that properties can be specified using either $(PropertyName)
        /// or {PropertyName} syntax.
        /// </remarks>
        public static string ResolvePropertyValues(object obj, string source)
        {
            return ResolvePropertyValues(DelimiterMode.Any, obj, source);
        }

        public static string ResolvePropertyName(string source, out string propNameWithDelimiter)
        {
            var name = ResolvePropertyName("$(", ")", source, out propNameWithDelimiter);

            return name;
        }

        /// <summary>
        /// Replaces property names in the source string with the actual
        /// values of the properties.
        /// </summary>
        /// <param name="delimiterMode">
        /// Specifies which delimiter or delimiters to use for parsing
        /// property names.
        /// </param>
        /// <param name="source">
        /// String containing property references in the format
        /// specified by the <see cref="DelimiterMode"/>
        /// </param>
        /// <returns>
        /// The source string with all property references replaced with
        /// actual property values.
        /// </returns>
        public static string ResolvePropertyValues(DelimiterMode delimiterMode, object obj, string source)
        {
            if (delimiterMode == DelimiterMode.CurlyBraces)
            {
                return ResolvePropertyValues("{", "}", obj, source);
            }
            else if (delimiterMode == DelimiterMode.DollarParen)
            {
                return ResolvePropertyValues("$(", ")", obj, source);
            }
            else if (delimiterMode == DelimiterMode.Any)
            {
                var res = ResolvePropertyValues("{", "}", obj, source);
                res = ResolvePropertyValues("$(", ")", obj, res);
                return res;
            }

            var msg = string.Format("Delimiter mode {0} is not currently supported", Enum.GetName(typeof(DelimiterMode), delimiterMode));
            throw new ArgumentException(msg, "delimiterMode");
        }

        /// <summary>
        /// ResolvePropertyName : This method resolves the property name from the input source string.
        /// </summary>
        /// <param name="startPropDelimiter">Start delimiter, usually $(</param>
        /// <param name="endPropDelimiter">End delimiter, usually )</param>
        /// <param name="source">The source string to find the property name between the delimiter</param>
        /// <param name="propNameWithDelimiter">Output the property name along with the delimiter</param>
        /// <returns></returns>
        private static string ResolvePropertyName(string startPropDelimiter, string endPropDelimiter,
            string source, out string propNameWithDelimiter)
        {
            var propName = string.Empty;
            propNameWithDelimiter = string.Empty;

            if (!string.IsNullOrEmpty(source))
            {
                // Look for opening delimiter
                var idxStartPropDelimiter = source.IndexOf(startPropDelimiter);

                if (idxStartPropDelimiter >= 0)
                {
                    // This index points to the first character in the property name
                    var idxStartPropName = idxStartPropDelimiter + startPropDelimiter.Length;

                    // Look for closing delimiter
                    var idxEndPropDelimiter = source.IndexOf(endPropDelimiter, idxStartPropName);

                    if (idxEndPropDelimiter >= 0)
                    {
                        // This index points to last character in property name
                        var idxEndPropName = idxEndPropDelimiter - endPropDelimiter.Length;

                        // We found a property name
                        propName = source.Substring(idxStartPropName, idxEndPropName - idxStartPropName + 1);
                        propNameWithDelimiter = source.Substring(idxStartPropDelimiter, idxEndPropDelimiter - idxStartPropDelimiter + 1);
                    }
                }          
            }

            return propName;
        }

        /// <summary>
        /// Replaces $(PropertyName) in the source string with the actual
        /// value of the property.
        /// </summary>
        /// <param name="startPropDelimiter">
        /// Delimiter used to mark the start of a property name
        /// </param>
        /// <param name="endPropDelimiter">
        /// Delimiter used to mark the end of a property name
        /// </param>
        /// <param name="source">
        /// String containing property references in the form
        /// of $(PropertyName)
        /// </param>
        /// <returns>
        /// The source string with all property references replaced with
        /// actual property values.
        /// </returns>
        private static string ResolvePropertyValues(string startPropDelimiter, string endPropDelimiter,
            object obj, string source)
        {
            var sbResult = new StringBuilder();

            while (!string.IsNullOrEmpty(source))
            {
                // Look for opening delimiter
                var idxStartPropDelimiter = source.IndexOf(startPropDelimiter);
                if (idxStartPropDelimiter >= 0)
                {
                    // This index points to the first character in the property name
                    var idxStartPropName = idxStartPropDelimiter + startPropDelimiter.Length;

                    // Look for closing delimiter
                    var idxEndPropDelimiter = source.IndexOf(endPropDelimiter, idxStartPropName);
                    if (idxEndPropDelimiter >= 0)
                    {
                        // This index points to last character in property name
                        var idxEndPropName = idxEndPropDelimiter - endPropDelimiter.Length;

                        // We found a property name
                        var propName = source.Substring(idxStartPropName, idxEndPropName - idxStartPropName + 1);
                        var propNameWithDelimiters = source.Substring(idxStartPropDelimiter, idxEndPropDelimiter - idxStartPropDelimiter + 1);
                        string formatSpec = null;

                        // Append literal portion of source up to the opening delimiter
                        // for the property name
                        if (idxStartPropDelimiter > 0)
                        {
                            sbResult.Append(source.Substring(0, idxStartPropDelimiter));
                        }

                        // Adjust source string to start after the closing delimiter
                        if (idxEndPropDelimiter + 1 < source.Length)
                        {
                            source = source.Substring(idxEndPropDelimiter + endPropDelimiter.Length);
                        }
                        else
                        {
                            source = string.Empty;
                        }

                        var idxFormatSpec = propName.IndexOf(':');
                        if (idxFormatSpec > 0 && idxFormatSpec < propName.Length - 1)
                        {
                            // Format spec included, so parse it out
                            formatSpec = propName.Substring(idxFormatSpec + 1);
                            propName = propName.Substring(0, idxFormatSpec);
                        }

                        object propValue = null;

                        // Get the property value and append it to the result
                        try
                        {
                            propValue = GetPropertyValue<object>(obj, propName);

                            if (propValue != null)
                            {
                                if (!string.IsNullOrEmpty(formatSpec))
                                {
                                    if (propValue.GetType() == typeof(Int32))
                                    {
                                        sbResult.Append(((Int32)propValue).ToString(formatSpec));
                                    }
                                    else
                                    {
                                        throw new InvalidOperationException("Format specification not supported for this type of property");
                                    }
                                }
                                else
                                {
                                    sbResult.Append(propValue.ToString());
                                }
                            }
                        }
                        catch (PropertyNotFoundException)
                        {
                            sbResult.Append(propNameWithDelimiters);
                        }
                    }
                    else
                    {
                        // Property name not found so append entire
                        // source string and exit loop
                        sbResult.Append(source);
                        source = string.Empty;
                    }
                }
                else
                {
                    // Property name not found so append entire
                    // source string and exit loop
                    sbResult.Append(source);
                    source = string.Empty;
                }
            }

            return sbResult.ToString();
        }
    }
}
