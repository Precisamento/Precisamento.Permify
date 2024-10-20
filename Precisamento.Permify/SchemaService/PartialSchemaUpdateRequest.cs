using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.SchemaService
{
    public class PartialSchemaUpdateRequest
    {
        [JsonPropertyName("metadata")]
        public SchemaMetadata? Metadata { get; set; }

        [JsonPropertyName("entities")]
        public Dictionary<string, EntityUpdate> EntitiesToUpdate { get; set; }

        public PartialSchemaUpdateRequest()
        {
        }

        public PartialSchemaUpdateRequest(Dictionary<string, EntityUpdate> entitiesToUpdate)
        {
            EntitiesToUpdate = entitiesToUpdate;
        }

        public PartialSchemaUpdateRequest(Dictionary<string, EntityUpdate> entitiesToUpdate, SchemaMetadata? metadata)
        {
            EntitiesToUpdate = entitiesToUpdate;
            Metadata = metadata;
        }
    }
}
