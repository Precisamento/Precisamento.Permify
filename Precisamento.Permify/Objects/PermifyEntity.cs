using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Represents a Permify entity or resource.
    /// </summary>
    public class PermifyEntity
    {
        /// <summary>
        /// The type of the entity.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The id of the entity.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonConstructor]
        public PermifyEntity(string type, string id)
        {
            Type = type;
            Id = id;
        }

        public override string ToString() => $"{Type}:{Id}";
    }
}
