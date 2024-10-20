using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.SchemaService
{
    public class ReadSchemaResponse
    {
        [JsonPropertyName("scema")]
        public SchemaDefinition Schema { get; set; }
    }
}
