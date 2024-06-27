using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.PermissionService
{
    public class SubjectFilterRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionMetadata? Metadata { get; set; }

        [JsonPropertyName("entity")]
        public PermifyEntity Entity { get; set; }

        [JsonPropertyName("permission")]
        public string Permission { get; set; }

        [JsonPropertyName("subject_reference")]
        public SubjectReference SubjectReference { get; set; }

        [JsonPropertyName("context")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PermissionContext? Context { get; set; }

        public SubjectFilterRequest()
        {
        }

        public SubjectFilterRequest(PermifyEntity entity, string permission, SubjectReference subjectReference)
        {
            Entity = entity;
            Permission = permission;
            SubjectReference = subjectReference;
        }

        public SubjectFilterRequest(PermifyEntity entity, string permission, SubjectReference subjectReference, PermissionMetadata? metadata)
            : this(entity, permission, subjectReference)
        {
            Metadata = metadata;
        }

        public SubjectFilterRequest(PermifyEntity entity, string permission, SubjectReference subjectReference, PermissionMetadata? metadata, PermissionContext? context)
            : this(entity, permission, subjectReference)
        {
            Metadata = metadata;
            Context = context;
        }
    }
}
