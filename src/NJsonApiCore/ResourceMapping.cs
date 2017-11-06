﻿using NJsonApi.Infrastructure;
using NJsonApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations;

namespace NJsonApi
{
    public class ResourceMapping<TEntity, TController> : IResourceMapping
    {
        public Func<object, object> IdGetter { get; set; }
        public Action<object, string> IdSetter { get; set; }
        public Type ResourceRepresentationType { get; set; }
        public string ResourceType { get; set; }
        public Dictionary<string, Func<object, object>> PropertyGetters { get; set; }
        public Dictionary<string, Action<object, object>> PropertySetters { get; private set; }
        public Dictionary<string, Expression<Action<object, object>>> PropertySettersExpressions { get; private set; }
        public Dictionary<string, Func<object, ILink>> LinkGetters { get; set; }
        public Dictionary<string, Action<object, ILink>> LinkSetters { get; private set; }
        public Dictionary<string, Expression<Action<object, object>>> LinkSettersExpressions { get; private set; }
        public List<IRelationshipMapping> Relationships { get; set; }
        public Type Controller { get; set; }
        
        public ResourceMapping()
        {
            ResourceRepresentationType = typeof(TEntity);
            Controller = typeof(TController);
            PropertyGetters = new Dictionary<string, Func<object, object>>();
            PropertySetters = new Dictionary<string, Action<object, object>>();
            PropertySettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            LinkGetters = new Dictionary<string, Func<object, ILink>>();
            LinkSetters = new Dictionary<string, Action<object, ILink>>();
            LinkSettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            Relationships = new List<IRelationshipMapping>();
        }

        public ResourceMapping(Expression<Func<TEntity, object>> idPointer)
        {
            IdGetter = ExpressionUtils.CompileToObjectTypedFunction(idPointer);
            ResourceRepresentationType = typeof(TEntity);
            PropertyGetters = new Dictionary<string, Func<object, object>>();
            PropertySetters = new Dictionary<string, Action<object, object>>();
            PropertySettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            LinkGetters = new Dictionary<string, Func<object, ILink>>();
            LinkSetters = new Dictionary<string, Action<object, ILink>>();
            LinkSettersExpressions = new Dictionary<string, Expression<Action<object, object>>>();
            Relationships = new List<IRelationshipMapping>();
        }

        public bool ValidateIncludedRelationshipPaths(string[] includedPaths)
        {
            foreach (var relationshipPath in includedPaths)
            {
                IResourceMapping currentMapping = this;

                var parts = relationshipPath.Split('.');
                foreach (var part in parts)
                {
                    var relationship = currentMapping.Relationships.SingleOrDefault(x => x.RelationshipName == part);
                    if (relationship == null)
                        return false;

                    currentMapping = relationship.ResourceMapping;
                }
            }
            return true;
        }

        public Dictionary<string, object> GetAttributes(object objectGraph, JsonSerializerSettings settings)
        {
            if (settings.NullValueHandling == NullValueHandling.Ignore)
            {
                return PropertyGetters
                    .Where(x => x.Value(objectGraph) != null)
                    .ToDictionary(kvp => CamelCaseUtil.ToCamelCase(kvp.Key), kvp => kvp.Value(objectGraph));
            }
            else
            {
                return PropertyGetters.ToDictionary(kvp => CamelCaseUtil.ToCamelCase(kvp.Key), kvp => kvp.Value(objectGraph));
            }
        }

        //public Dictionary<string, ILink> GetLinks(object objectGraph, JsonSerializerSettings settings)
        public ILinkData GetLinks(object objectGraph, JsonSerializerSettings settings)
        {
            Dictionary<string, ILink> links;
//            if (settings.NullValueHandling == NullValueHandling.Ignore)
//            {
                links = LinkGetters
                    .Where(x => x.Value(objectGraph) != null)
                    .ToDictionary(kvp => CamelCaseUtil.ToCamelCase(kvp.Key), kvp => kvp.Value(objectGraph));
//            }
//            else
//            {
//                links = LinkGetters.ToDictionary(kvp => CamelCaseUtil.ToCamelCase(kvp.Key),
//                    kvp => kvp.Value(objectGraph));
//            }

            var linkData = new LinkData();
            foreach (var link in links)
            {
                linkData.Add(link.Key, link.Value);
            }
            return linkData;
        }

        // TODO ROLA - type handling must be better in here
        public Dictionary<string, object> GetValuesFromAttributes(Dictionary<string, object> attributes)
        {
            var values = new Dictionary<string, object>();

            // For each property setter that we found on the concrete resource class
            foreach (var propertySetter in PropertySettersExpressions)
            {
                // Try to find a matching attribute on the provided document
                object value;
                attributes.TryGetValue(CamelCaseUtil.ToCamelCase(propertySetter.Key), out value);
                if (value != null)
                {
                    // If we found one, then add to the new dictionary
                    values.Add(CamelCaseUtil.ToCamelCase(propertySetter.Key), value);
                }
            }

            return values;
        }

        public Dictionary<string, ILink> GetValuesFromLinks(ILinkData links)
        {
            var values = new Dictionary<string, ILink>();
            // For each property setter that we found on the concrete resource class
            foreach (var linkSetter in LinkSettersExpressions)
            {
                // Try to find a matching attribute on the provided document
                links.TryGetValue(CamelCaseUtil.ToCamelCase(linkSetter.Key), out var value);
                if (value != null)
                {
                    // If we found one, then add to the new dictionary
                    values.Add(CamelCaseUtil.ToCamelCase(linkSetter.Key), value);
                }
            }
            return values;
        }
    }
}