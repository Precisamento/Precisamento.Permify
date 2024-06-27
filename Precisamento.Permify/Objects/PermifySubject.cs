using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    public class PermifySubject : PermifyEntity
    {
        [JsonPropertyName("relation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Relation { get; set; }

        public PermifySubject()
            : base("", "")
        {
        }

        public PermifySubject(string type, string id)
            : base(type, id)
        {
        }

        public PermifySubject(string type, string id, string relation)
            : base(type, id)
        {
            Relation = relation;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Relation))
            {
                return $"{Type}:{Id}";
            }
            else
            {
                return $"{Type}:{Id}#{Relation}";
            }
        } 
    }
}
