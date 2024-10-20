using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class LookupEntityRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionMetadata? Metadata { get; set; }

        [JsonPropertyName("entity_type")]
        public string EntityType { get; set; }

        [JsonPropertyName("permission")]
        public string Permission { get; set; }

        [JsonPropertyName("subject")]
        public PermifySubject Subject { get; set; }

        [JsonPropertyName("scope")]
        public LookupScope? Scope { get; set; }

        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionContext? Context { get; set; }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject)
            : this(entityType, permission, subject, null, null, null)
        {
        }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject, LookupScope scope)
            : this(entityType, permission, subject, scope, null, null)
        {
        }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject, PermissionMetadata? metadata)
            : this(entityType, permission, subject, null, metadata, null)
        {
        }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject, LookupScope scope, PermissionMetadata? metadata)
            : this(entityType, permission, subject, scope, metadata, null)
        {
        }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject, PermissionMetadata? metadata, PermissionContext? context)
            : this(entityType, permission, subject, null, metadata, context)
        {
        }

        public LookupEntityRequest(string entityType, string permission, PermifySubject subject, LookupScope? scope, PermissionMetadata? metadata, PermissionContext? context)
        {
            EntityType = entityType;
            Permission = permission;
            Subject = subject;
            Metadata = metadata;
            Context = context;
        }
    }
}
