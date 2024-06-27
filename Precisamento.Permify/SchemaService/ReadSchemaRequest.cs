using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Precisamento.Permify.Objects;

namespace Precisamento.Permify.SchemaService
{
    public class ReadSchemaRequest
    {
        [JsonPropertyName("metadata")]
        public SchemaMetadata Metadata { get; set; }

        public ReadSchemaRequest()
        {
        }

        public ReadSchemaRequest(SchemaMetadata metadata)
        {
            Metadata = metadata;
        }

        public ReadSchemaRequest(string schemaVersion)
        {
            Metadata = new SchemaMetadata(schemaVersion);
        }
    }
}
