using Precisamento.Permify.BundleService;
using Precisamento.Permify.DataService;
using Precisamento.Permify.Objects;
using Precisamento.Permify.PermissionService;
using Precisamento.Permify.SchemaService;
using Precisamento.Permify.TenantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Precisamento.Permify
{
    public interface IPermifyClientFull : 
        IPermifyClient, 
        IPermifyClientMultiTenant, 
        IPermifyClientRaw, 
        IPermifyClientRawMultiTenant
    {
    }

    public interface IPermifyClientBase
    {
        /// <summary>
        /// Gets a paginated list of tenants defined in the Permify instance.
        /// </summary>
        /// <param name="pageSize">The number of tenants to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing tenants from a previous request.</param>
        /// <returns>A list of tenants and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListTenantsResponse> ListTenantsAsync(int pageSize = 10, string? continuousToken = null);

        /// <summary>
        /// Creates a new tenant with the given ID and name.
        /// </summary>
        /// <param name="id">The id of the tenant to create.</param>
        /// <param name="name">The name of the new tenant.</param>
        /// <returns>The created tenant.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<PermifyTenant> CreateTenantAsync(string id, string name);

        /// <summary>
        /// Deletes the tenant with the given ID.
        /// </summary>
        /// <param name="id">The id of the tenant to delete.</param>
        /// <returns>The deleted tenant.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<PermifyTenant> DeleteTenantAsync(string id);
    }

    public interface IPermifyClient : IPermifyClientBase
    {
        /// <summary>
        /// Creates or updates the permify schema for a tenant.
        /// </summary>
        /// <param name="schema">The Permify schema to write. See https://docs.permify.co/getting-started/modeling for details.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SchemaMetadata> WriteSchemaAsync(string schema);

        /// <summary>
        /// Returns a paginated list of schema updates that have been applied to the tenant, similar to `git log`.
        /// </summary>
        /// <param name="pageSize">The number of updates to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing schema updates from a previous request.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListSchemaResponse> ListSchemaAsync(int pageSize = 10, string? continuousToken = null);

        /// <summary>
        /// Returns the schema with the given version.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="schemaVersion">The version of the schema to retrieve.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(string schemaVersion);

        /// <summary>
        /// Returns the schema with the given version, or the latest schema if no version is provided.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="schemaVersion">The version of the schema to retrieve.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(SchemaMetadata schemaVersion);

        /// <summary>
        /// Adds the listed relationships and attributes to the Permify tenant.
        /// </summary>
        /// <param name="relations">The relations to add.</param>
        /// <param name="attributes">The attributes to add.</param>
        /// <param name="schemaVersion">The schema version these changes apply to for caching purposes.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> WriteAuthorizationDataAsync(List<PermifyTuple>? relations = null, List<PermifyAttribute>? attributes = null, string? schemaVersion = null);

        /// <summary>
        /// Get a paginated list of subjects that have a relationship with a set of entities based on a series of filters.
        /// </summary>
        /// <param name="filter">Defines the filters used to retrieve the subjects.</param>
        /// <param name="snapToken">A token representing the last known version of the data for caching purposes.</param>
        /// <param name="pageSize">The number of subjects to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing subjects from a previous request.</param>
        /// <returns>A list of subjects that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(
            TupleFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null);

        /// <summary>
        /// Get a paginated list of attributes that match a set of filters.
        /// </summary>
        /// <param name="filter">Defines the filters used to retrieve the attributes.</param>
        /// <param name="snapToken">A token representing the last known version of the data for caching purposes.</param>
        /// <param name="pageSize">The number of attributes to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing subjects from a previous request.</param>
        /// <returns>A list of attributes that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadAttributesResponse> ReadAttributesAsync(
            AttributeFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the operation to run.</param>
        /// <param name="arguments">A JSON object containing the named arguments used to run the bundle.</param>
        /// <remarks>
        /// <see cref="RunBundleAsync(string, IDictionary{string, string})"/> should be favored over this method unless some of the arguments can't be strings.
        /// </remarks>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(string name, JsonNode arguments);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="name">The name of the operation to run.</param>
        /// <param name="arguments">A set of the named arguments used to run the bundle.</param>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(string name, IDictionary<string, string> arguments);

        /// <summary>
        /// Deletes a set of relationships and attributes based on a set of filters.
        /// </summary>
        /// <param name="tupleFilter">A filter used to determine which relationships to delete.</param>
        /// <param name="attributeFilter">A filter used to determine which attributes to delete.</param>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> DeleteDataAsync(TupleFilter? tupleFilter = null, AttributeFilter? attributeFilter = null);

        /// <summary>
        /// Determines if a subject can perform an action on an entity.
        /// </summary>
        /// <param name="entity">The entity or resource to check if the subject has permission for.</param>
        /// <param name="permission">The permission or action to check for.</param>
        /// <param name="subject">The subject or user to check has the permission on the entity.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <param name="arguments">Additional arguments used in the check.</param>
        /// <returns>The access level and the depth searched to get the result.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<CheckAccessResponse> CheckAccessAsync(
            PermifyEntity entity,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null);

        /// <summary>
        /// Get all subjects (users and user sets) that have a relationship or attribute with the given entity and permission.
        /// </summary>
        /// <param name="entity">The entity to get related subjects to.</param>
        /// <param name="permission">The permission or action to check the entity for.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <param name="arguments">Additional arguments used in the check.</param>
        /// <returns>A recursive tree containing the users and user sets.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ExpandTree> ExpandAsync(
            PermifyEntity entity,
            string permission,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null);

        /// <summary>
        /// Gets a list of all subjects that can perform an action on an entity.
        /// </summary>
        /// <param name="entity">The entity to check for permission for.</param>
        /// <param name="permission">The permission or action to check.</param>
        /// <param name="subject">Information about the subjects to fetch, such as the subject type and relatoion with the entity.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A list of subjects that have the given permission for the entity.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> LookupSubjectAsync(
            PermifyEntity entity,
            string permission,
            SubjectReference subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="entityType">The type of the entity to query.</param>
        /// <param name="permission">The permission or action to check for.</param>
        /// <param name="subject">The subject to check permission for.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> LookupEntityAsync(
            string entityType,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Gets a list of all permissions/relations that a subject has for an entity.
        /// By default, only permissions are returned. If relations are needed, set <see cref="PermissionMetadata.OnlyPermissions"/> to false.
        /// </summary>
        /// <param name="entity">The entity to check the permissions for.</param>
        /// <param name="subject">The subject to get the permissions for.</param>
        /// <param name="metadata">
        /// Metadata determining if only permissions should be checked, or if all relations are included.
        /// Additionally contains tokens used for caching purposes and the recursive check depth limit.
        /// </param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A dictionary that maps permission names to the access level for the given subject.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<Dictionary<string, PermifyAccess>> SubjectPermissionListAsync(
            PermifyEntity entity,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Define a named program (bundle) that will execute a series of operations when run.
        /// </summary>
        /// <param name="bundle">The bundle containing the operations to run, and the arguments used to call the program.</param>
        /// <returns>The bundle name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<string> WriteBundleAsync(Bundle bundle);

        /// <summary>
        /// Defines multiple named programs (bundles) that will execute a series of operations when run.
        /// </summary>
        /// <param name="bundles">A list of bundles containing the operations to run, and the arguments used to call the programs.</param>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <returns>A list of the added bundle names.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> WriteBundlesAsync(List<Bundle> bundles);

        /// <summary>
        /// Gets the bundle definition with the given name.
        /// </summary>
        /// <param name="bundleName">The name of the bundle to get.</param>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <returns>The bundle with the given name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<Bundle> ReadBundleAsync(string bundleName);

        /// <summary>
        /// Deletes a bundle with the given name.
        /// </summary>
        /// <param name="bundleName">The name of the bundle to delete.</param>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <returns>The name of the deleted bundle.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<string> DeleteBundleAsync(string bundleName);
    }

    public interface IPermifyClientRaw : IPermifyClient
    {
        /// <summary>
        /// Creates or updates the permify schema for a tenant.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SchemaMetadata> WriteSchemaAsync(WriteSchemaRequest request);

        /// <summary>
        /// Returns a paginated list of schema updates that have been applied to the tenant, similar to `git log`.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListSchemaResponse> ListSchemaAsync(ListSchemaRequest request);

        /// <summary>
        /// Returns the schema with the given version, or the latest schema if no version is provided.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(ReadSchemaRequest request);

        /// <summary>
        /// Creates the listed relationships and attributes.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> WriteAuthorizationDataAsync(WriteAuthorizationDataRequest request);

        /// <summary>
        /// Get a paginated list of subjects that have a relationship with a set of entities based on a series of filters.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of subjects that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(ReadRelationshipsRequest request);

        /// <summary>
        /// Get a paginated list of attributes that match a set of filters.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of attributes that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadAttributesResponse> ReadAttributesAsync(ReadAttributesRequest request);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(RunBundleRequest request);

        /// <summary>
        /// Deletes a set of relationships and attributes based on a set of filters.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> DeleteDataAsync(DeleteDataRequest request);

        /// <summary>
        /// Determines if a subject can perform an action on an entity.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The access level and the depth searched to get the result.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request);

        /// <summary>
        /// Get all subjects (users and user sets) that have a relationship or attribute with the given entity and permission.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A recursive tree containing the users and user sets.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ExpandResponse> ExpandAsync(ExpandRequest request);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SubjectFilterResponse> LookupSubjectAsync(SubjectFilterRequest request);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<LookupEntityResponse> LookupEntityAsync(LookupEntityRequest request);

        /// <summary>
        /// Gets a list of all permissions/relations that a subject has for an entity.
        /// By default, only permissions are returned. If relations are needed, set <see cref="PermissionMetadata.OnlyPermissions"/> to false.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A dictionary that maps permission names to the access level for the given subject.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SubjectPermissionListResponse> SubjectPermissionListAsync(SubjectPermissionListRequest request);

        /// <summary>
        /// Defines multiple named programs (bundles) that will execute a series of operations when run.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of the added bundle names.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<WriteBundleResponse> WriteBundlesAsync(WriteBundleRequest request);

        /// <summary>
        /// Gets the bundle definition with the given name.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The bundle with the given name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadBundleResponse> ReadBundleAsync(ReadBundleRequest request);

        /// <summary>
        /// Deletes a bundle with the given name.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The name of the deleted bundle.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<DeleteBundleResponse> DeleteBundleAsync(DeleteBundleRequest request);

        /// <summary>
        /// Gets a paginated list of tenants defined in the Permify instance.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>A list of tenants and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListTenantsResponse> ListTenantsAsync(ListTenantsRequest request);

        /// <summary>
        /// Creates a new tenant with the given ID and name.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The created tenant.</returns>
        public Task<CreateTenantResponse> CreateTenantAsync(CreateTenantRequest request);

        /// <summary>
        /// Deletes the tenant with the given ID.
        /// </summary>
        /// <param name="request">The request data.</param>
        /// <returns>The deleted tenant.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<DeleteTenantResponse> DeleteTenantAsync(DeleteTenantRequest request);
    }

    public interface IPermifyClientMultiTenant : IPermifyClientBase
    {
        /// <summary>
        /// Creates or updates the permify schema for a tenant.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="schema">The Permify schema to write. See https://docs.permify.co/getting-started/modeling for details.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SchemaMetadata> WriteSchemaAsync(string tenant, string schema);

        /// <summary>
        /// Returns a paginated list of schema updates that have been applied to the tenant, similar to `git log`.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="pageSize">The number of updates to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing schema updates from a previous request.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListSchemaResponse> ListSchemaAsync(string tenant, int pageSize = 10, string? continuousToken = null);

        /// <summary>
        /// Returns the schema with the given version.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="schemaVersion">The version of the schema to retrieve.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(string tenant, string schemaVersion);

        /// <summary>
        /// Returns the schema with the given version, or the latest schema if no version is provided.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="schemaVersion">The version of the schema to retrieve.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(string tenant, SchemaMetadata schemaVersion);

        /// <summary>
        /// Adds the listed relationships and attributes to the Permify tenant.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="relations">The relations to add.</param>
        /// <param name="attributes">The attributes to add.</param>
        /// <param name="schemaVersion">The schema version these changes apply to for caching purposes.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> WriteAuthorizationDataAsync(
            string tenant,
            List<PermifyTuple>? relations = null,
            List<PermifyAttribute>? attributes = null,
            string? schemaVersion = null);

        /// <summary>
        /// Get a paginated list of subjects that have a relationship with a set of entities based on a series of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="filter">Defines the filters used to retrieve the subjects.</param>
        /// <param name="snapToken">A token representing the last known version of the data for caching purposes.</param>
        /// <param name="pageSize">The number of subjects to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing subjects from a previous request.</param>
        /// <returns>A list of subjects that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(
            string tenant,
            TupleFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null);

        /// <summary>
        /// Get a paginated list of attributes that match a set of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="filter">Defines the filters used to retrieve the attributes.</param>
        /// <param name="snapToken">A token representing the last known version of the data for caching purposes.</param>
        /// <param name="pageSize">The number of attributes to return in the current request.</param>
        /// <param name="continuousToken">A token to continue listing subjects from a previous request.</param>
        /// <returns>A list of attributes that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadAttributesResponse> ReadAttributesAsync(
            string tenant,
            AttributeFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="name">The name of the operation to run.</param>
        /// <param name="arguments">A JSON object containing the named arguments used to run the bundle.</param>
        /// <remarks>
        /// <see cref="RunBundleAsync(string, IDictionary{string, string})"/> should be favored over this method unless some of the arguments can't be strings.
        /// </remarks>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(string tenant, string name, JsonNode arguments);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="name">The name of the operation to run.</param>
        /// <param name="arguments">A set of the named arguments used to run the bundle.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(string tenant, string name, IDictionary<string, string> arguments);

        /// <summary>
        /// Deletes a set of relationships and attributes based on a set of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="tupleFilter">A filter used to determine which relationships to delete.</param>
        /// <param name="attributeFilter">A filter used to determine which attributes to delete.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> DeleteDataAsync(string tenant, TupleFilter? tupleFilter = null, AttributeFilter? attributeFilter = null);

        /// <summary>
        /// Determines if a subject can perform an action on an entity.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="entity">The entity or resource to check if the subject has permission for.</param>
        /// <param name="permission">The permission or action to check for.</param>
        /// <param name="subject">The subject or user to check has the permission on the entity.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <param name="arguments">Additional arguments used in the check.</param>
        /// <returns>The access level and the depth searched to get the result.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<CheckAccessResponse> CheckAccessAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null);

        /// <summary>
        /// Get all subjects (users and user sets) that have a relationship or attribute with the given entity and permission.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="entity">The entity to get related subjects to.</param>
        /// <param name="permission">The permission or action to check the entity for.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <param name="arguments">Additional arguments used in the check.</param>
        /// <returns>A recursive tree containing the users and user sets.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ExpandTree> ExpandAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null);

        /// <summary>
        /// Gets a list of all subjects that can perform an action on an entity.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="entity">The entity to check for permission for.</param>
        /// <param name="permission">The permission or action to check.</param>
        /// <param name="subject">Information about the subjects to fetch, such as the subject type and relatoion with the entity.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A list of subjects that have the given permission for the entity.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> LookupSubjectAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            SubjectReference subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="entityType">The type of the entity to query.</param>
        /// <param name="permission">The permission or action to check for.</param>
        /// <param name="subject">The subject to check permission for.</param>
        /// <param name="metadata">Metadata containing tokens used for caching purposes, and a recursive check depth limit.</param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> LookupEntityAsync(
            string tenant,
            string entityType,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Gets a list of all permissions/relations that a subject has for an entity.
        /// By default, only permissions are returned. If relations are needed, set <see cref="PermissionMetadata.OnlyPermissions"/> to false.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="entity">The entity to check the permissions for.</param>
        /// <param name="subject">The subject to get the permissions for.</param>
        /// <param name="metadata">
        /// Metadata determining if only permissions should be checked, or if all relations are included.
        /// Additionally contains tokens used for caching purposes and the recursive check depth limit.
        /// </param>
        /// <param name="context">Additional data used in the check.</param>
        /// <returns>A dictionary that maps permission names to the access level for the given subject.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<Dictionary<string, PermifyAccess>> SubjectPermissionListAsync(
            string tenant,
            PermifyEntity entity,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null);

        /// <summary>
        /// Define a named program (bundle) that will execute a series of operations when run.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="bundle">The bundle containing the operations to run, and the arguments used to call the program.</param>
        /// <returns>The bundle name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<string> WriteBundleAsync(string tenant, Bundle bundle);

        /// <summary>
        /// Defines multiple named programs (bundles) that will execute a series of operations when run.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="bundles">A list of bundles containing the operations to run, and the arguments used to call the programs.</param>
        /// <returns>A list of the added bundle names.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<List<string>> WriteBundlesAsync(string tenant, List<Bundle> bundles);

        /// <summary>
        /// Gets the bundle definition with the given name.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="bundleName">The name of the bundle to get.</param>
        /// <returns>The bundle with the given name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<Bundle> ReadBundleAsync(string tenant, string bundleName);

        /// <summary>
        /// Deletes a bundle with the given name.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="bundleName">The name of the bundle to delete.</param>
        /// <returns>The name of the deleted bundle.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<string> DeleteBundleAsync(string tenant, string bundleName);
    }

    public interface IPermifyClientRawMultiTenant : IPermifyClientBase
    {
        /// <summary>
        /// Creates or updates the permify schema for a tenant.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SchemaMetadata> WriteSchemaAsync(string tenant, WriteSchemaRequest request);

        /// <summary>
        /// Returns a paginated list of schema updates that have been applied to the tenant, similar to `git log`.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ListSchemaResponse> ListSchemaAsync(string tenant, ListSchemaRequest request);

        /// <summary>
        /// Returns the schema with the given version, or the latest schema if no version is provided.
        /// </summary>
        /// <remarks>
        /// Can be used to revert to an earlier schema version if needed.
        /// </remarks>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<JsonNode> ReadSchemaAsync(string tenant, ReadSchemaRequest request);

        /// <summary>
        /// Creates the listed relationships and attributes.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> WriteAuthorizationDataAsync(string tenant, WriteAuthorizationDataRequest request);

        /// <summary>
        /// Get a paginated list of subjects that have a relationship with a set of entities based on a series of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A list of subjects that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(string tenant, ReadRelationshipsRequest request);

        /// <summary>
        /// Get a paginated list of attributes that match a set of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A list of attributes that match the filter and a continuation token to retrieve additional results.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadAttributesResponse> ReadAttributesAsync(string tenant, ReadAttributesRequest request);

        /// <summary>
        /// Runs a previously defined operation (bundle) with the specified arguments.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> RunBundleAsync(string tenant, RunBundleRequest request);

        /// <summary>
        /// Deletes a set of relationships and attributes based on a set of filters.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A SnapToken that can be used in future requests for caching purposes.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SnapMetadata> DeleteDataAsync(string tenant, DeleteDataRequest request);

        /// <summary>
        /// Determines if a subject can perform an action on an entity.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>The access level and the depth searched to get the result.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<CheckAccessResponse> CheckAccessAsync(string tenant, CheckAccessRequest request);

        /// <summary>
        /// Get all subjects (users and user sets) that have a relationship or attribute with the given entity and permission.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A recursive tree containing the users and user sets.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ExpandResponse> ExpandAsync(string tenant, ExpandRequest request);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SubjectFilterResponse> LookupSubjectAsync(string tenant, SubjectFilterRequest request);

        /// <summary>
        /// Gets a list of all entities that the given subject has permission for/can perform an action on.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A list of entities the subject has the given permission for.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<LookupEntityResponse> LookupEntityAsync(string tenant, LookupEntityRequest request);

        /// <summary>
        /// Gets a list of all permissions/relations that a subject has for an entity.
        /// By default, only permissions are returned. If relations are needed, set <see cref="PermissionMetadata.OnlyPermissions"/> to false.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A dictionary that maps permission names to the access level for the given subject.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<SubjectPermissionListResponse> SubjectPermissionListAsync(string tenant, SubjectPermissionListRequest request);

        /// <summary>
        /// Defines multiple named programs (bundles) that will execute a series of operations when run.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>A list of the added bundle names.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<WriteBundleResponse> WriteBundlesAsync(string tenant, WriteBundleRequest request);

        /// <summary>
        /// Gets the bundle definition with the given name.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>The bundle with the given name.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<ReadBundleResponse> ReadBundleAsync(string tenant, ReadBundleRequest request);

        /// <summary>
        /// Deletes a bundle with the given name.
        /// </summary>
        /// <param name="tenant">The tenant to target for this request.</param>
        /// <param name="request">The request data.</param>
        /// <returns>The name of the deleted bundle.</returns>
        /// <exception cref="PermifyException">Permify returned an error. Inspect <see cref="PermifyException.Error"/> for additional details.</exception>
        /// <exception cref="HttpRequestException">An error occurred with the HTTP request.</exception>
        public Task<DeleteBundleResponse> DeleteBundleAsync(string tenant, DeleteBundleRequest request);
    }
}
