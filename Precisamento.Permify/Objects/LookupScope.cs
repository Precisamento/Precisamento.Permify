using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Filter for looking up entities in specific organizations or repositories.
    /// </summary>
    public class LookupScope
    {
        /// <summary>
        /// A list of repositories to search in.
        /// </summary>
        [JsonPropertyName("repository")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LookupScopeFilter? Repository { get; set; }

        /// <summary>
        /// A list of organizations to search in.
        /// </summary>
        [JsonPropertyName("organization")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public LookupScopeFilter? Organization { get; set; }

        public LookupScope()
        {
        }

        public LookupScope(LookupScopeFilter? repository, LookupScopeFilter? organization)
        {
            Repository = repository;
            Organization = organization;
        }

        public LookupScope(IEnumerable<string>? repository, IEnumerable<string>? organization)
        {
            Repository = repository == null ? null : new LookupScopeFilter(repository.ToList());
            Organization = organization == null ? null : new LookupScopeFilter(organization.ToList());
        }
    }

    /// <summary>
    /// A specific filter containing a list of objects inside of a lookup scope.
    /// </summary>
    public class LookupScopeFilter
    {
        [JsonPropertyName("data")]
        public List<string> Data { get; set; }

        public LookupScopeFilter()
        {
        }

        public LookupScopeFilter(List<string> data)
        {
            Data = data;
        }
    }
}
