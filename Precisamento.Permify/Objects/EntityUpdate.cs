using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    public class EntityUpdate
    {
        [JsonPropertyName("write")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Write { get; set; }

        [JsonPropertyName("delete")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Delete { get; set; }

        [JsonPropertyName("update")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Update { get; set; }

        public EntityUpdate()
        {
        }

        public EntityUpdate(List<string>? write, List<string>? delete, List<string>? update)
        {
            Write = write;
            Delete = delete;
            Update = update;
        }

        public EntityUpdate(IEnumerable<string>? write, IEnumerable<string>? delete, IEnumerable<string>? update)
        {
            Write = write?.ToList();
            Delete = delete?.ToList();
            Update = update?.ToList();
        }
    }
}
