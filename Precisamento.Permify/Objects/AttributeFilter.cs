using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Filter attributes based on an entity filter and list of attribute names.
    /// </summary>
    public class AttributeFilter
    {
        /// <summary>
        /// Filter entities based on their type and ids. Either or both fields can be empty.
        /// </summary>
        [JsonPropertyName("entity")]
        public EntityFilter Entity { get; set; }
        
        /// <summary>
        /// The list of attribute names to include in the operation.
        /// </summary>
        [JsonPropertyName("attributes")]
        public List<string> Attributes { get; set; }

        public AttributeFilter()
        {
            Entity = new EntityFilter();
            Attributes = new List<string>();
        }

        public AttributeFilter(EntityFilter entity)
        {
            Entity = entity;
            Attributes = new List<string>();
        }

        public AttributeFilter(List<string> attributes)
        {
            Entity = new EntityFilter();
            Attributes = attributes;
        }

        public AttributeFilter(EntityFilter entity, List<string> attributes)
        {
            Entity = entity;
            Attributes = attributes;
        }
    }
}
