using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Defines a reference to a relation for a subject type.
    /// </summary>
    public class SubjectReference
    {
        /// <summary>
        /// The type of the subject entity.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// The referenced relation.
        /// </summary>
        [JsonPropertyName("relation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Relation { get; set; }

        public SubjectReference()
        {
        }

        public SubjectReference(string type)
        {
            Type = type;
        }

        public SubjectReference(string type, string? relation = null)
        {
            Type = type;
            Relation = relation;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Relation))
            {
                return Type;
            }
            else
            {
                return $"{Type}#{Relation}";
            }
        }
    }
}
