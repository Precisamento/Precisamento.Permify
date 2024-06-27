using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// The access level a subject has for an entity/resource.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumMemberConverter))]
    public enum PermifyAccess
    {
        [EnumMember(Value = "CHECK_RESULT_UNSPECIFIED")]
        Unspecified,

        [EnumMember(Value = "CHECK_RESULT_ALLOWED")]
        Allowed,

        [EnumMember(Value = "CHECK_RESULT_DENIED")]
        Denied
    }
}
