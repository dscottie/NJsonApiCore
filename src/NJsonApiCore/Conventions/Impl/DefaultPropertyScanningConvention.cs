﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using NJsonApi.Serialization.Representations;

namespace NJsonApi.Conventions.Impl
{
    internal class DefaultPropertyScanningConvention : IPropertyScanningConvention
    {
        public DefaultPropertyScanningConvention()
        {
            ThrowOnUnmappedLinkedType = true;
        }

        /// <summary>
        /// Determines if the given PropertyInfo is the primary ID of the currently scanned resource.
        /// </summary>
        public virtual bool IsPrimaryId(PropertyInfo propertyInfo)
        {
            return propertyInfo.Name == "Id";
        }

        /// <summary>
        /// Used to distinguish simple properties (serialized in-line) from linked resources (side-loaded in "linked" section).
        /// </summary>
        public virtual bool IsLinkedResource(PropertyInfo pi)
        {
            var type = pi.PropertyType;
            bool isPrimitiveType = type.GetTypeInfo().IsPrimitive || type.GetTypeInfo().IsValueType || (type == typeof(string) || (type == typeof(DateTime)) || (type == typeof(TimeSpan)) || (type == typeof(DateTimeOffset))) || IsLink(pi);
            return !isPrimitiveType;
        }

        public virtual bool IsLink(PropertyInfo pi)
        {
            var type = pi.PropertyType;
            var implementsILink = typeof(ILink).IsAssignableFrom(type);
            return implementsILink;
        }

        /// <summary>
        /// Determines if the property should be ignored during scanning.
        /// </summary>
        public virtual bool ShouldIgnore(PropertyInfo pi)
        {
            return pi.GetCustomAttribute<JsonIgnoreAttribute>() != null;
        }

        /// <summary>
        /// If set to true, any scanned property that is discovered to be a linked resource, but is never registered in the builder,
        /// will cause an exception to be thrown during build time.
        ///
        /// If set to false, scanned properties that are discovered to be linked resources are silently removed from the mapping during build
        /// and ignored.
        /// </summary>
        public bool ThrowOnUnmappedLinkedType { get; set; }
    }
}