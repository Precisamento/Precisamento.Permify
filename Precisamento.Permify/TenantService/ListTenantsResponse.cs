using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.TenantService
{
    public class ListTenantsResponse
    {
        [JsonPropertyName("tenants")]
        public List<PermifyTenant> Tenants { get; set; }

        [JsonPropertyName("continuous_token")]
        public string ContinuousToken { get; set; }
    }
}
