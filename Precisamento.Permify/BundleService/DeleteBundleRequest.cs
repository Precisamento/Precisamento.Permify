using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.BundleService
{
    public class DeleteBundleRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public DeleteBundleRequest(string name)
        {
            Name = name;
        }
    }
}
