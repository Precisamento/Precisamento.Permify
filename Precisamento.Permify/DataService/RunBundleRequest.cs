using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class RunBundleRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("arguments")]
        public JsonNode Arguments { get; set; }

        public RunBundleRequest()
        {
        }

        public RunBundleRequest(string name, JsonNode arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public RunBundleRequest(string name, IDictionary<string, string> arguments)
        {
            Name = name;
            var obj = new JsonObject();
            foreach (var arg in arguments)
            {
                obj.Add(arg.Key, JsonValue.Create(arg.Value));
            }
            Arguments = obj;
        }
    }
}
