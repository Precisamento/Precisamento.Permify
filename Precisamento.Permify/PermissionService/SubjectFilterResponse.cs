using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class SubjectFilterResponse
    {
        [JsonPropertyName("subject_ids")]
        public List<string> SubjectIds { get; set; }
    }
}
