using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Precisamento.Permify.Objects
{
    /// <summary>
    /// Bundle a set relation and attribute creation/deletion operations to be executed when a specific action occurs.
    /// </summary>
    public class Bundle
    {
        /// <summary>
        /// The name of the bundle. Can be seen as the name of a program.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The names of the arguments that are passed to the operations when the bundle is executed.
        /// </summary>
        [JsonPropertyName("arguments")]
        public List<string> Arguments { get; set; }

        /// <summary>
        /// A list of operations to run when the bundle is executed.
        /// </summary>
        [JsonPropertyName("operations")]
        public List<BundleOperations> Operations { get; set; }

        public Bundle(string name, List<string> arguments, BundleOperations operations)
        {
            Name = name;
            Arguments = arguments;
            Operations = new List<BundleOperations>() { operations };
        }

        [JsonConstructor]
        public Bundle(string name, List<string> arguments, List<BundleOperations> operations)
        {
            Name = name;
            Arguments = arguments;
            Operations = operations;
        }
    }

    /// <summary>
    /// Defines the operations to be run when a bundle is executed. The bundle arguments can be accessed using interpolation {{.argument_name}}. Make sure the period before the argument name is included.
    /// </summary>
    public class BundleOperations
    {
        /// <summary>
        /// The relationships to create or update.
        /// </summary>
        /// <remarks>
        /// "organization:{{.organizationId}}#member@user:{{.userId}}"
        /// </remarks>
        [JsonPropertyName("relationships_write")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? RelationshipsWrite { get; set; }

        /// <summary>
        /// The relationships to delete.
        /// </summary>
        /// <remarks>
        /// "organization:{{.organizationId}}#member@user:{{.userId}}"
        /// </remarks>
        [JsonPropertyName("relationships_delete")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? RelationshipsDelete { get; set; }

        /// <summary>
        /// The attributes to create or update.
        /// </summary>
        /// <remarks>
        /// "organization:{{.organizationID}}$public|boolean:false"
        /// </remarks>
        [JsonPropertyName("attributes_write")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? AttributesWrite { get; set; }

        /// <summary>
        /// The attributes to delete.
        /// </summary>
        /// <remarks>
        /// "organization:{{.organizationID}}$public|boolean:false"
        /// </remarks>
        [JsonPropertyName("attributes_delete")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? AttributesDelete { get; set; }

        public BundleOperations(
            List<string>? relationshipsWrite = null,
            List<string>? relationshipsDelete = null,
            List<string>? attributesWrite = null,
            List<string>? attributesDelete = null)
        {
            if (relationshipsWrite is null && relationshipsDelete is null && attributesWrite is null && attributesDelete is null)
                throw new ArgumentNullException(nameof(relationshipsWrite), "At least one operation must be provided");

            RelationshipsWrite = relationshipsWrite;
            RelationshipsDelete = relationshipsDelete;
            AttributesWrite = attributesWrite;
            AttributesDelete = attributesDelete;
        }
    }
}
