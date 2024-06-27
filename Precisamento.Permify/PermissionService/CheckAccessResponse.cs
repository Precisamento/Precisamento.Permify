using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class CheckAccessResponse
    {
        [JsonPropertyName("can")]
        public PermifyAccess Access { get; set; }

        [JsonPropertyName("check_count")]
        public CheckCount CheckCount { get; set; }
    }
}
