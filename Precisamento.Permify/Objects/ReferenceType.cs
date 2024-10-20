using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ReferenceType
    {
        [EnumMember(Value = "REFERENCE_UNSPECIFIED")]
        Unspecified,

        [EnumMember(Value = "REFERENCE_RELATION")]
        Relation,

        [EnumMember(Value = "REFERENCE_PERMISSION")]
        Permission,

        [EnumMember(Value = "REFERENCE_ATTRIBUTE")]
        Attribute
    }
}
