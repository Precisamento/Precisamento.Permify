using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class CheckAccessRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionMetadata? Metadata { get; set; }

        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        [JsonPropertyName("permission")]
        public string Permission { get; set; }

        [JsonPropertyName("subject")]
        public PermifySubject Subject { get; set; }

        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionContext? Context { get; set; }

        [JsonPropertyName("arguments")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<PermissionArgument>? Arguments { get; set; }

        public CheckAccessRequest()
        {
        }

        public CheckAccessRequest(PermifyEntity entity, string permission, PermifySubject subject)
        {
            Entity = entity;
            Permission = permission;
            Subject = subject;
        }

        public CheckAccessRequest(PermifyEntity entity, string permission, PermifySubject subject, PermissionMetadata? metadata)
        {
            Entity = entity;
            Permission = permission;
            Subject = subject;
            Metadata = metadata;
        }

        public CheckAccessRequest(PermifyEntity entity, string permission, PermifySubject subject, PermissionMetadata? metadata, PermissionContext? context)
        {
            Entity = entity;
            Permission = permission;
            Subject = subject;
            Metadata = metadata;
            Context = context;
        }

        public CheckAccessRequest(PermifyEntity entity, string permission, PermifySubject subject, PermissionMetadata? metadata, PermissionContext? context, List<PermissionArgument>? arguments)
        {
            Entity = entity;
            Permission = permission;
            Subject = subject;
            Metadata = metadata;
            Context = context;
            Arguments = arguments;
        }
    }
}
