using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    public class RuleDefinition
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("arguments")]
        public Dictionary<string, AttributeType> Arguments { get; set; }

        [JsonPropertyName("expression")]
        public JsonNode Expression { get; set; }
    }
}
