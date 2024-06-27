using Precisamento.Permify.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.Permify
{
    public class PermifyClientOptions
    {
        public string TenantId { get; set; } = "t1";
        public string? Secret { get; set; }
        public string Host { get; set; }
    }
}
