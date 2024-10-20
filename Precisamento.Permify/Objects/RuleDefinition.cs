using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace Precisamento.Permify.Objects
{
    public class RuleDefinition
    {
        public string Name { get; set; }
        public Dictionary<string, AttributeType> Arguments { get; set; }
        public JsonNode Expression { get; set; }
    }
}
