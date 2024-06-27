using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    public class PermissionArgument
    {
        [JsonPropertyName("computedAttribute")]
        public ComputedAttribute ComputedAttribute { get; set; }

        [JsonPropertyName("contextAttribute")]
        public ContextAttribute ContextAttribute { get; set; }
    }

    public class ComputedAttribute
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class ContextAttribute
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
