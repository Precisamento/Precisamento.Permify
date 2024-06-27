using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Information about a schema including it's version creation date.
    /// </summary>
    public class SchemaInfo
    {
        /// <summary>
        /// The schema version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>
        /// The date and time the schema was created.
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
}
