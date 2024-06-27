using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.DataService
{
    public class DeleteDataRequest
    {
        [JsonPropertyName("tuple_filter")]
        public TupleFilter? TupleFilter { get; set; }

        [JsonPropertyName("attribute_filter")]
        public AttributeFilter? AttributeFilter { get; set; }

        public DeleteDataRequest()
        {
        }

        public DeleteDataRequest(TupleFilter? tupleFilter, AttributeFilter? attributeFilter)
        {
            TupleFilter = tupleFilter;
            AttributeFilter = attributeFilter;
        }

        public DeleteDataRequest(TupleFilter tupleFilter)
        {
            TupleFilter = tupleFilter;
        }

        public DeleteDataRequest(AttributeFilter attributeFilter)
        {
            AttributeFilter = attributeFilter;
        }
    }
}
