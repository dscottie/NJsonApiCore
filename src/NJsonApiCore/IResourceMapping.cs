using NJsonApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations;

namespace NJsonApi
{
    public interface IResourceMapping
    {
        Func<object, object> IdGetter { get; set; }
        Action<object, string> IdSetter { get; set; }
        Type ResourceRepresentationType { get; set; }
        Type Controller { get; }
        string ResourceType { get; set; }
        List<IRelationshipMapping> Relationships { get; set; }
        Dictionary<string, Func<object, object>> PropertyGetters { get; set; }
        Dictionary<string, Action<object, object>> PropertySetters { get; }
        Dictionary<string, Expression<Action<object, object>>> PropertySettersExpressions { get; }


        Dictionary<string, Func<object, ILink>> LinkGetters { get; set; }
        Dictionary<string, Action<object, ILink>> LinkSetters { get; }
        Dictionary<string, Expression<Action<object, object>>> LinkSettersExpressions { get; }

        bool ValidateIncludedRelationshipPaths(string[] includedPaths);

        Dictionary<string, object> GetAttributes(object objectGraph, JsonSerializerSettings settings);

        Dictionary<string, object> GetValuesFromAttributes(Dictionary<string, object> attributes);
        Dictionary<string, ILink> GetValuesFromLinks(ILinkData links);
    }
}