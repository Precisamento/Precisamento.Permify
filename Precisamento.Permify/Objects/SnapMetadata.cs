using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Represents a snapshot of the data in a Permify tenant.
    /// </summary>
    public class SnapMetadata
    {
        /// <summary>
        /// The token representing the snapshot. Typically used for caching purposes, and can optionally be an empty string in most requests.
        /// </summary>
        [JsonPropertyName("snap_token")]
        public string SnapToken { get; set; } = "";

        public SnapMetadata()
        {
        }

        public SnapMetadata(string snapToken)
        {
            SnapToken = snapToken;
        }
    }
}
