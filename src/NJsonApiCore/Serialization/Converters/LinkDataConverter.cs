﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NJsonApi.Infrastructure;

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
                default:
                    throw new InvalidOperationException("When updating a resource 'links' needs to contain data which is an object.");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            serializer.Serialize(writer, value);
        }
    }
}
