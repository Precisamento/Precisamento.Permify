using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.TenantService
{
    public class DeleteTenantResponse
    {
        [JsonPropertyName("tenant")]
        public PermifyTenant Tenant { get; set; }
    }
}
