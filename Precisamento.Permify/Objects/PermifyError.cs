using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Represent an error returned by the Permify API.
    /// </summary>
    public class PermifyError
    {
        /// <summary>
        /// The internal Permify error code. This does not correspond to an HTTP status code.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// The Permify error message.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Additional details about the error.
        /// </summary>
        [JsonPropertyName("details")]
        public List<JsonNode> Details { get; set; }
    }
}
