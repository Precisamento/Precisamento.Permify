using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class SubjectPermissionListRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionMetadata? Metadata { get; set; }

        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        [JsonPropertyName("subject")]
        public PermifySubject Subject { get; set; }

        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionContext? Context { get; set; }

        public SubjectPermissionListRequest()
        {
        }

        public SubjectPermissionListRequest(PermifyEntity entity, PermifySubject subject)
        {
            Entity = entity;
            Subject = subject;
        }

        public SubjectPermissionListRequest(PermifyEntity entity, PermifySubject subject, PermissionMetadata? metadata)
            : this(entity, subject)
        {
            Metadata = metadata;
        }

        public SubjectPermissionListRequest(PermifyEntity entity, PermifySubject subject, PermissionMetadata? metadata, PermissionContext? context)
            : this(entity, subject, metadata)
        {
            Context = context;
        }
    }
}
