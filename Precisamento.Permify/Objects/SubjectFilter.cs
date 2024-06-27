using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Filters a set of entities based on their type, id, and/or relation with an entity.
    /// </summary>
    public class SubjectFilter
    {
        /// <summary>
        /// The type of subject to include in the result.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The IDs of the subjects to include in the result.
        /// </summary>
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }

        /// <summary>
        /// A specific relation that included subjects must have with the entity.
        /// </summary>
        [JsonPropertyName("relation")]
        public string Relation { get; set; }

        public SubjectFilter()
            : this("", null, "")
        {
        }

        public SubjectFilter(string type)
            : this(type, null, "")
        {
        }

        public SubjectFilter(string type, List<string> ids)
            : this(type, ids, "")
        {
        }

        public SubjectFilter(string type, string relation)
            : this(type, null, relation)
        {
        }

        public SubjectFilter(List<string> ids)
            : this("", ids, "")
        {
        }

        public SubjectFilter(List<string> ids, string relation)
            : this("", ids, relation)
        {
        }

        public SubjectFilter(string type = "", List<string>? ids = null, string relation = "")
        {
            Type = type;
            Ids = ids ?? new List<string>();
            Relation = relation;
        }
    }
}
