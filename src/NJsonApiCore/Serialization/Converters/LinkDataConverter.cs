using System;
using Newtonsoft.Json;
using NJsonApi.Serialization.Representations;
using Newtonsoft.Json.Linq;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Representations.Relationships;

namespace NJsonApi.Serialization.Converters
{
    public class LinkDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ILinkData);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken obj = JToken.Load(reader);
            switch (obj.Type)
            {
                case JTokenType.Object:
                    return obj.ToObject<LinkData>(serializer);
                case JTokenType.Array:
                    return obj.ToObject<LinkData>();
                default:
                    throw new InvalidOperationException("When updating a resource, each relationship needs to contain data the is either an array or an object.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.Serialize(writer, value);
        }
    }

    public class LinkConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ILink);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken obj = JToken.Load(reader);
            switch (obj.Type)
            {
                case JTokenType.Object:
                    return new LinkObject { Link = new SimpleLink(new Uri(obj.Value<string>("href"))) };
                    //return obj.ToObject<LinkObject>(serializer);
                case JTokenType.String:
                    return new LinkObject { Link = new SimpleLink(new Uri(obj.Value<string>())) };
                default:
                    throw new InvalidOperationException("When updating a resource, each link needs to contain data the is either a link object or string.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.Serialize(writer, value);
        }
    }
}
