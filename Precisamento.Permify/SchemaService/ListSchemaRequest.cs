using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.SchemaService
{
    public class ListSchemaRequest
    {
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("continuous_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContinuousToken { get; set; }

        public ListSchemaRequest()
        {
        }

        public ListSchemaRequest(int pageSize, string? continuousToken)
        {
            PageSize = pageSize;
            ContinuousToken = continuousToken;
        }
    }
}
