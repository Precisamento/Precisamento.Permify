using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    public class EntityDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("relations")]
        public Dictionary<string, RelationDefinition> Relations { get; set; }

        [JsonPropertyName("permissions")]
        public Dictionary<string, JsonNode> Permissions { get; set; }

        [JsonPropertyName("attributes")]
        public Dictionary<string, AttributeDefinition> Attributes { get; set; }

        [JsonPropertyName("references")]
        public Dictionary<string, ReferenceType> References { get; set; }
    }
}
