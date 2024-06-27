using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Filters a set of entities based on their type and/or IDs.
    /// </summary>
    public class EntityFilter
    {
        /// <summary>
        /// The type of entity to include in the result.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The IDs of the entities to include in the result.
        /// </summary>
        [JsonPropertyName("ids")]
        public List<string> Ids { get; set; }

        public EntityFilter()
        {
            Type = string.Empty;
            Ids = new List<string>();
        }

        public EntityFilter(string type)
        {
            Type = type;
            Ids = new List<string>();
        }

        public EntityFilter(List<string> ids)
        {
            Type = string.Empty;
            Ids = ids;
        }

        public EntityFilter(string type, List<string> ids)
        {
            Type = type;
            Ids = ids;
        }
    }
}
