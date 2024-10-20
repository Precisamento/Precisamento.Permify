using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AttributeType
    {
        [EnumMember(Value = "ATTRIBUTE_TYPE_UNSPECIFIED")]
        Unspecified,

        [EnumMember(Value = "ATTRIBUTE_TYPE_BOOLEAN")]
        Boolean,

        [EnumMember(Value = "ATTRIBUTE_TYPE_BOOLEAN_ARRAY")]
        BooleanArray,

        [EnumMember(Value = "ATTRIBUTE_TYPE_STRING")]
        String,

        [EnumMember(Value = "ATTRIBUTE_TYPE_STRING_ARRAY")]
        StringArray,

        [EnumMember(Value = "ATTRIBUTE_TYPE_INTEGER")]
        Integer,

        [EnumMember(Value = "ATTRIBUTE_TYPE_INTEGER_ARRAY")]
        IntegerArray,

        [EnumMember(Value = "ATTRIBUTE_TYPE_DOUBLE")]
        Double,

        [EnumMember(Value = "ATTRIBUTE_TYPE_DOUBLE_ARRAY")]
        DoubleArray,
    }
}
