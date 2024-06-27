using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class ReadAttributesRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SnapMetadata? Metadata { get; set; }

        [JsonPropertyName("filter")]
        public AttributeFilter Filter { get; set; }

        [JsonPropertyName("page_size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageSize { get; set; }

        [JsonPropertyName("continuous_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContinuousToken { get; set; }

        public ReadAttributesRequest()
        {
        }

        public ReadAttributesRequest(AttributeFilter filter)
        {
            Filter = filter;
        }

        public ReadAttributesRequest(AttributeFilter filter, SnapMetadata? metadata)
        {
            Metadata = metadata;
            Filter = filter;
        }

        public ReadAttributesRequest(AttributeFilter filter, SnapMetadata? metadata, int pageSize)
        {
            Metadata = metadata;
            Filter = filter;
            PageSize = pageSize;
        }

        public ReadAttributesRequest(AttributeFilter filter, SnapMetadata? metadata, int pageSize, string? continuousToken)
        {
            Metadata = metadata;
            Filter = filter;
            PageSize = pageSize;
            ContinuousToken = continuousToken;
        }
    }
}
