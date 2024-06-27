using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class SubjectPermissionListResponse
    {
        [JsonPropertyName("results")]
        public Dictionary<string, PermifyAccess> Permissions { get; set; }
    }
}
