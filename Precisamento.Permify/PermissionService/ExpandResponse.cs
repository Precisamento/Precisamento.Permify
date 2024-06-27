using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class ExpandResponse
    {
        [JsonPropertyName("tree")]
        public ExpandTree Tree { get; set; }
    }
}
