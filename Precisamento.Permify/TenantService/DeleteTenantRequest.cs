using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.TenantService
{
    public class DeleteTenantRequest
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        public DeleteTenantRequest(string id)
        {
            Id = id;
        }
    }
}
