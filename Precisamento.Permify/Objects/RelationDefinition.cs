using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    public class RelationDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("relation_references")]
        public List<RelationDefinitionReference> References { get; set; }
    }

    public class RelationDefinitionReference
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("relation")]
        public string Relation { get; set; }
    }
}
