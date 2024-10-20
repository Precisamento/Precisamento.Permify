using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    public class SchemaDefinition
    {
        [JsonPropertyName("entityDefinitions")]
        public Dictionary<string, EntityDefinition> Entities { get; set; }

        [JsonPropertyName("ruleDefinitions")]
        public Dictionary<string, RuleDefinition> Rules { get; set; }

        [JsonPropertyName("references")]
        public Dictionary<string, SchemaReference> References { get; set; }
    }
}
