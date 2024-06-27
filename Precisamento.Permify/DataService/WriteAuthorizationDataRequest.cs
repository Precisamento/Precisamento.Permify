using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class WriteAuthorizationDataRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SchemaMetadata Metadata { get; set; }

        [JsonPropertyName("tuples")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermifyTuple>? Relations { get; set; }

        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermifyAttribute>? Attributes { get; set; }

        public WriteAuthorizationDataRequest()
        {
            Metadata = new SchemaMetadata();
        }

        public WriteAuthorizationDataRequest(List<PermifyTuple> relations)
        {
            Relations = relations;
            Metadata = new SchemaMetadata();
        }

        public WriteAuthorizationDataRequest(List<PermifyTuple> relations, SchemaMetadata metadata)
        {
            Metadata = metadata;
            Relations = relations;;
        }

        public WriteAuthorizationDataRequest(List<PermifyAttribute> attributes, SchemaMetadata metadata)
        {
            Metadata = metadata;
            Attributes = attributes;
        }

        public WriteAuthorizationDataRequest(List<PermifyTuple>? relations, List<PermifyAttribute>? attributes, SchemaMetadata metadata)
        {
            Metadata = metadata;
            Relations = relations;
            Attributes = attributes;
        }
    }
}
