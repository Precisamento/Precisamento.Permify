using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    public class PermissionContext
    {
        [JsonPropertyName("tuples")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermifyTuple>? Tuples { get; set; }

        [JsonPropertyName("attributes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermifyAttribute>? Attributes { get; set; }

        /// <summary>
        /// Additional data to pass to the Permify operation. Can typically be accessed through the `request` object in the Permify schema.
        /// </summary>
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonNode? Data { get; set; }

        public PermissionContext()
        {
        }

        public PermissionContext(List<PermifyTuple>? tuples = null, List<PermifyAttribute>? attributes = null, JsonNode? data = null)
        {
            Tuples = tuples;
            Attributes = attributes;
            Data = data;
        }

        public PermissionContext(List<PermifyTuple>? tuples = null, List<PermifyAttribute>? attributes = null, Dictionary<string, string>? data = null)
        {
            Tuples = tuples;
            Attributes = attributes;
            if (data != null)
            {
                var obj = new JsonObject();
                foreach (var item in data)
                {
                    obj.Add(item.Key, item.Value);
                }
                Data = obj;
            }
        }
    }
}
