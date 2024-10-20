using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.SchemaService
{
    public class ReadSchemaResponse
    {
        [JsonPropertyName("schema")]
        public SchemaDefinition Schema { get; set; }
    }
}
