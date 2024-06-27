using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class ReadRelationshipsRequest
    {
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SnapMetadata? Metadata { get; set; }

        [JsonPropertyName("filter")]
        public TupleFilter Filter { get; set; }

        [JsonPropertyName("page_size")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? PageSize { get; set; }

        [JsonPropertyName("continuous_token")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ContinuousToken { get; set; }

        public ReadRelationshipsRequest()
        {
        }

        public ReadRelationshipsRequest(TupleFilter filter)
        {
            Filter = filter;
        }

        public ReadRelationshipsRequest(TupleFilter filter, SnapMetadata? metadata)
        {
            Metadata = metadata;
            Filter = filter;
        }

        public ReadRelationshipsRequest(TupleFilter filter, SnapMetadata? metadata, int pageSize)
        {
            Metadata = metadata;
            Filter = filter;
            PageSize = pageSize;
        }

        public ReadRelationshipsRequest(TupleFilter filter, SnapMetadata? metadata, int pageSize, string? continuousToken)
        {
            Metadata = metadata;
            Filter = filter;
            PageSize = pageSize;
            ContinuousToken = continuousToken;
        }
    }
}
