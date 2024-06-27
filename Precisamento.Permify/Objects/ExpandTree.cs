using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    public class ExpandTree
    {
        [JsonPropertyName("entity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermifyEntity? Entity { get; set; }

        [JsonPropertyName("permission")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Permission { get; set; }

        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermissionArgument>? Arguments { get; set; }

        [JsonPropertyName("expand")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ExpandTreeNode? Expand { get; set; }

        [JsonPropertyName("leaf")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ExpandTreeLeaf? Leaf { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ExpandTreeOperation
    {
        [EnumMember(Value = "OPERATION_UNSPECIFIED")]
        Unspecified,

        [EnumMember(Value = "OPERATION_UNION")]
        Union,

        [EnumMember(Value = "OPERATION_INTERSECTION")]
        Intersection,

        [EnumMember(Value = "OPERATION_EXCLUSION")]
        Exclusion
    }

    public class ExpandTreeNode
    {
        [JsonPropertyName("operation")]
        public ExpandTreeOperation Operation = ExpandTreeOperation.Unspecified;

        [JsonPropertyName("children")]
        public List<ExpandTree> Children { get; set; }
    }


    public class ExpandTreeLeaf
    {
        [JsonPropertyName("subjects")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ExpandTreeSubjects? Subjects { get; set; }

        [JsonPropertyName("values")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ExpandTreeValues? Values { get; set; }

        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IAttributeValue? Value { get; set; }
    }

    public class ExpandTreeSubjects
    {
        [JsonPropertyName("subjects")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermifySubject>? Subjects { get; set; }
    }

    public class ExpandTreeValues
    {
        [JsonPropertyName("values")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public JsonNode? Values { get; set; }
    }
}
