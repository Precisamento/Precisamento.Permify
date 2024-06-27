using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class ReadRelationshipsResponse
    {
        [JsonPropertyName("tuples")]
        public List<PermifyTuple> Tuples { get; set; }

        [JsonPropertyName("continuous_token")]
        public string ContinuousToken { get; set; }
    }
}
