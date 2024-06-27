using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class ExpandRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionMetadata? Metadata { get; set; }

        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        [JsonPropertyName("permission")]
        public string Permission { get; set; }

        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionContext? Context { get; set; }

        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermissionArgument>? Arguments { get; set; }

        public ExpandRequest()
        {
        }

        public ExpandRequest(PermifyEntity entity, string permission)
        {
            Entity = entity;
            Permission = permission;
        }

        public ExpandRequest(PermifyEntity entity, string permission, PermissionMetadata? metadata)
            : this(entity, permission)
        {
            Metadata = metadata;
        }

        public ExpandRequest(PermifyEntity entity, string permission, PermissionMetadata? metadata, PermissionContext? context)
            : this(entity, permission, metadata)
        {
            Context = context;
        }

        public ExpandRequest(PermifyEntity entity, string permission, PermissionMetadata? metadata, PermissionContext? context, List<PermissionArgument>? arguments)
            : this(entity, permission, metadata, context)
        {
            Arguments = arguments;
        }
    }
}
