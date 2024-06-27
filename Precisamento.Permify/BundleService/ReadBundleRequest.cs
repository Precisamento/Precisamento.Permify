using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.BundleService
{
    public class ReadBundleRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public ReadBundleRequest(string name)
        {
            Name = name;
        }
    }
}
