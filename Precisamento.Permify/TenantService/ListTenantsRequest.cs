using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.TenantService
{
    public class ListTenantsRequest
    {
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("continuous_token")]
        public string? ContinuousToken { get; set; }

        public ListTenantsRequest()
        {
        }

        public ListTenantsRequest(int pageSize, string? continuousToken)
        {
            PageSize = pageSize;
            ContinuousToken = continuousToken;
        }

        public ListTenantsRequest(int pageSize)
        {
            PageSize = pageSize;
        }

        public ListTenantsRequest(string? continuousToken)
        {
            ContinuousToken = continuousToken;
        }
    }
}
