using Newtonsoft.Json;
using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NJsonApi.Serialization.Representations
{
    [JsonConverter(typeof(SerializationAwareConverter))]
    public class SimpleLink : ISimpleLink, ISerializationAware
    {
        public SimpleLink()
        {
        }

        public SimpleLink(Uri href)
        {
            this.Href = href.AbsoluteUri;
        }

        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
        public string Href { get; set; }

        public virtual void Serialize(JsonWriter writer) => writer.WriteValue(Href);

        public override string ToString() => Href;
    }

    public class LinkObject : SimpleLink, ILinkObject, IObjectMetaDataContainer
    {
        private MetaData _meta = new MetaData();

//        [JsonProperty(PropertyName = "href", NullValueHandling = NullValueHandling.Ignore)]
//        public ISimpleLink Link { get; set; }

        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public MetaData Meta { get { return _meta; } set { _meta = value; } }

        public LinkObject()
        {
        }

        public LinkObject(Uri href) : base(href)
        {
            //this.Link = new SimpleLink(href);
        }

        public MetaData GetMetaData()
        {
            return _meta;
        }

        public override void Serialize(JsonWriter writer)
        {
            if (_meta?.Any() ?? false)
            {
                //writer.WriteRaw(JsonConvert.SerializeObject(this));
                writer.WriteStartObject();
                writer.WritePropertyName("href");
                writer.WriteValue(Href);
                writer.WritePropertyName("meta");
                writer.WriteRawValue(JsonConvert.SerializeObject(_meta));
                writer.WriteEndObject();
            }
            else
            {
                base.Serialize(writer);
            }
        }
    }
}