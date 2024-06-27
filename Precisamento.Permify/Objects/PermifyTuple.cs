using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Represents a relationship between an entity and a subject.
    /// </summary>
    public class PermifyTuple
    {
        /// <summary>
        /// The entity in the relationship.
        /// </summary>
        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        /// <summary>
        /// The relationship the subject has with the entity.
        /// </summary>
        [JsonPropertyName("relation")]
        public string Relationship { get; set; }

        /// <summary>
        /// The subject in the relationship.
        /// </summary>
        [JsonPropertyName("subject")]
        public PermifySubject Subject { get; set; }

        public PermifyTuple(PermifyEntity entity, string relationship, PermifySubject subject)
        {
            Entity = entity;
            Relationship = relationship;
            Subject = subject;
        }

        public override string ToString() => $"{Entity}#{Relationship}@{Subject}";
    }
}
