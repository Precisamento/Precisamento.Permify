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
            if (pageSize < 1 || pageSize > 100)
            {
                throw new PermifyException("Page size must be between 1 and 100");
            }

            PageSize = pageSize;
            ContinuousToken = continuousToken;
        }
    }
}
