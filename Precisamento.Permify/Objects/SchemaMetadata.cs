using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Metadata about a Permify schema.
    /// </summary>
    public class SchemaMetadata
    {
        /// <summary>
        /// Contains the schema version of a Permify schema. Typically used for caching purposes, and can optionally be an empty string in most requests.
        /// </summary>
        [JsonRequired]
        [JsonPropertyName("schema_version")]
        public string SchemaVersion { get; set; } = "";

        public SchemaMetadata()
        {
        }

        public SchemaMetadata(string schemaVersion)
        {
            SchemaVersion = schemaVersion;
        }
    }
}
