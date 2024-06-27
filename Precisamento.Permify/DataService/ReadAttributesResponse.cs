using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class ReadAttributesResponse
    {
        [JsonPropertyName("attributes")]
        public List<PermifyAttribute> Attributes { get; set; }

        [JsonPropertyName("continuous_token")]
        public string ContinuousToken { get; set; }
    }
}
