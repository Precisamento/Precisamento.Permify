using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class LookupEntityResponse
    {
        [JsonPropertyName("entity_ids")]
        public List<string> EntityIds { get; set; }
    }
}
