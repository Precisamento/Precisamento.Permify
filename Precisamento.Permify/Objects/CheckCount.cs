using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Contains the recursive depth used to determine the result of a Permify operation.
    /// </summary>
    public class CheckCount
    {
        /// <summary>
        /// The recursive depth used to determine the result of a Permify operation.
        /// </summary>
        [JsonPropertyName("check_count")]
        public int Count { get; set; }
    }
}
