using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.SchemaService
{
    public class WriteSchemaRequest
    {
        [JsonPropertyName("schema")]
        public string Schema { get; set; }

        public WriteSchemaRequest()
        {
        }

        public WriteSchemaRequest(string schema)
        {
            Schema = schema;
        }
    }
}
