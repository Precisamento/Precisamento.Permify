using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.BundleService
{
    public class WriteBundleRequest
    {
        [JsonPropertyName("bundles")]
        public List<Bundle> Bundles { get; set; }

        public WriteBundleRequest(List<Bundle> bundles)
        {
            Bundles = bundles;
        }
    }
}
