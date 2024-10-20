using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Distinguishes if a name refers to an entity or a rule.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SchemaReference
    {
        /// <summary>
        /// Default, unspecified reference.
        /// </summary>
        [EnumMember(Value = "REFERENCE_UNSPECIFIED")]
        Unspecified,

        /// <summary>
        /// Indicates that a name refers to an entity.
        /// </summary>
        [EnumMember(Value = "REFERENCE_ENTITY")]
        Entity,

        /// <summary>
        /// Indicates that a name refers to a rule.
        /// </summary>
        [EnumMember(Value = "REFERENCE_RULE")]
        Rule
    }
}
