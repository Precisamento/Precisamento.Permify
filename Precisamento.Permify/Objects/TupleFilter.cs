using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Filter tuples based on the entity, relation, and subject. 
    /// </summary>
    public class TupleFilter
    {
        /// <summary>
        /// Filter tuples based on the entity type and ids. Either or both fields can be empty.
        /// </summary>
        [JsonPropertyName("entity")]
        public EntityFilter Entities { get; set; }

        /// <summary>
        /// Filter tuples with the specified relation.
        /// </summary>
        [JsonPropertyName("relation")]
        public string Relation { get; set; }

        /// <summary>
        /// Filter subjects based on their type, ids, and relation. Any or all fields can be empty.
        /// </summary>
        [JsonPropertyName("subject")]
        public SubjectFilter Subjects { get; set; }

        public TupleFilter()
            : this(null, null, null)
        {
        }

        public TupleFilter(EntityFilter entities)
            : this(entities, null, null)
        { }

        public TupleFilter(EntityFilter entities, string relation)
            : this(entities, relation, null)
        { }

        public TupleFilter(SubjectFilter subjects)
            : this(null, null, subjects)
        { }

        public TupleFilter(string relation, SubjectFilter subjects)
            : this(null, relation, subjects)
        { }

        public TupleFilter(EntityFilter? entities = null, string? relation = null, SubjectFilter? subjects = null)
        {
            Entities = entities ?? new EntityFilter();
            Relation = relation ?? string.Empty;
            Subjects = subjects ?? new SubjectFilter();
        }
    }
}
