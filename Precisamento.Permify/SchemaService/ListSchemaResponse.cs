using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Precisamento.Permify.Objects;

namespace Precisamento.Permify.SchemaService
{
    public class ListSchemaResponse
    {
        [JsonPropertyName("head")]
        public string Head { get; set; }

        [JsonPropertyName("schemas")]
        public List<SchemaInfo> Schemas { get; set; }

        [JsonPropertyName("continuous_token")]
        public string ContinuousToken { get; set; }
    }
}
