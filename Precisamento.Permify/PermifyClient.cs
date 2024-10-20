using Precisamento.Permify.BundleService;
using Precisamento.Permify.DataService;
using Precisamento.Permify.Objects;
using Precisamento.Permify.PermissionService;
using Precisamento.Permify.SchemaService;
using Precisamento.Permify.TenantService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Precisamento.Permify
{
    public class PermifyClient : IPermifyClientFull
    {
        private HttpClient _client;
        private string _tenantId;

        private const string WRITE_SCHEMA = "schemas/write";
        private const string LIST_SCHEMA = "schemas/list";
        private const string PARTIAL_SCHEMA_UPDATE = "schemas/partial-write";
        private const string READ_SCHEMA = "schemas/read";
        private const string WRITE_AUTHORIZATION_DATA = "data/write";
        private const string READ_RELATIONSHIPS = "data/relationships/read";
        private const string READ_ATTRIBUTES = "data/attributes/read";
        private const string RUN_BUNDLE = "data/run-bundle";
        private const string DELETE_DATA = "data/delete";
        private const string CHECK_ACCESS = "permissions/check";
        private const string EXPAND = "permissions/expand";
        private const string SUBJECT_FILTER = "permissions/lookup-subject";
        private const string LOOKUP_ENTITY = "permissions/lookup-entity";
        private const string LOOKUP_ENTITY_STREAM = "permissions/lookup-entity-stream";
        private const string SUBJECT_PERMISSION_LIST = "permissions/subject-permission";
        private const string WRITE_BUNDLE = "bundle/write";
        private const string READ_BUNDLE = "bundle/read";
        private const string DELETE_BUNDLE = "bundle/delete";

        private const string LIST_TENANTS = "v1/tenants/list";
        private const string CREATE_TENANT = "v1/tenants/create";
        private const string DELETE_TENANT = "v1/tenants";

        public PermifyClient(HttpClient client, PermifyClientOptions options)
        {
            _client = client;
            _tenantId = options.TenantId;
        }

        public PermifyClient(HttpClient client, string tenantId)
        {
            _client = client;
            _tenantId = tenantId;
        }

        /// <summary>
        /// Helper method to create the URL for the API requests.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetUrl(string tenant, string url)
        {
            return $"v1/tenants/{tenant}/{url}";
        }

        /// <summary>
        /// Helper method to deserialize the responses from the Permify API.
        /// </summary>
        /// <typeparam name="T">The concrete of the response JSON content.</typeparam>
        /// <param name="response">The HTTP response from the Permify API.</param>
        private static async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var test = await response.Content.ReadAsStringAsync();
                return (await response.Content!.ReadFromJsonAsync<T>())!;
            }
            else if (response.Content != null && response.Content.Headers.ContentType?.MediaType == "application/json")
            {
                PermifyError? error = null;

                try
                {
                    error = await response.Content.ReadFromJsonAsync<PermifyError>();
                }
                catch
                {
                }

                if (error is null)
                {
                    // Deinitely throws here
                    response.EnsureSuccessStatusCode();
                    return default;
                }
                else
                {
                    throw new PermifyException(error);
                }
            }
            else
            {
                response.EnsureSuccessStatusCode();
                return default;
            }
        }

        /// <inheritdoc />
        public Task<SchemaMetadata> WriteSchemaAsync(string schema)
            => WriteSchemaAsync(new WriteSchemaRequest(schema));

        /// <inheritdoc />
        public Task<SchemaMetadata> WriteSchemaAsync(string tenant, string schema)
            => WriteSchemaAsync(tenant, new WriteSchemaRequest(schema));

        /// <inheritdoc />
        public Task<SchemaMetadata> WriteSchemaAsync(WriteSchemaRequest request)
            => WriteSchemaAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SchemaMetadata> WriteSchemaAsync(string tenant, WriteSchemaRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, WRITE_SCHEMA), request);
            return await HandleResponse<SchemaMetadata>(response);
        }

        /// <inheritdoc />
        public Task<ListSchemaResponse> ListSchemaAsync(int pageSize = 10, string? continuousToken = null)
            => ListSchemaAsync(_tenantId, new ListSchemaRequest(pageSize, continuousToken));

        /// <inheritdoc />
        public Task<ListSchemaResponse> ListSchemaAsync(string tenant, int pageSize = 10, string? continuousToken = null)
            => ListSchemaAsync(tenant, new ListSchemaRequest(pageSize, continuousToken));

        /// <inheritdoc />
        public Task<ListSchemaResponse> ListSchemaAsync(ListSchemaRequest request)
            => ListSchemaAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<ListSchemaResponse> ListSchemaAsync(string tenant, ListSchemaRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, LIST_SCHEMA), request);
            return await HandleResponse<ListSchemaResponse>(response);
        }

        /// <inheritdoc />
        public Task<SchemaMetadata> PartialSchemaUpdateAsync(Dictionary<string, EntityUpdate> updates, string? schemaVersion = null)
            => PartialSchemaUpdateAsync(_tenantId, new PartialSchemaUpdateRequest(updates, schemaVersion == null ? null : new SchemaMetadata(schemaVersion)));

        /// <inheritdoc />
        public Task<SchemaMetadata> PartialSchemaUpdateAsync(string tenant, Dictionary<string, EntityUpdate> updates, string? schemaVersion = null)
            => PartialSchemaUpdateAsync(tenant, new PartialSchemaUpdateRequest(updates, schemaVersion == null ? null : new SchemaMetadata(schemaVersion)));

        /// <inheritdoc />
        public Task<SchemaMetadata> PartialSchemaUpdateAsync(PartialSchemaUpdateRequest request)
            => PartialSchemaUpdateAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SchemaMetadata> PartialSchemaUpdateAsync(string tenantId, PartialSchemaUpdateRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenantId, PARTIAL_SCHEMA_UPDATE), request);
            return await HandleResponse<SchemaMetadata>(response);
        }

        /// <inheritdoc />
        public Task<SchemaDefinition> ReadSchemaAsync(string schemaVersion)
            => ReadSchemaAsync(_tenantId, new ReadSchemaRequest(new SchemaMetadata(schemaVersion)));

        /// <inheritdoc />
        public Task<SchemaDefinition> ReadSchemaAsync(string tenant, string schemaVersion)
            => ReadSchemaAsync(tenant, new ReadSchemaRequest(new SchemaMetadata(schemaVersion)));

        /// <inheritdoc />
        public Task<SchemaDefinition> ReadSchemaAsync(SchemaMetadata schemaVersion)
            => ReadSchemaAsync(new ReadSchemaRequest(schemaVersion));

        /// <inheritdoc />
        public Task<SchemaDefinition> ReadSchemaAsync(string tenant, SchemaMetadata schemaVersion)
            => ReadSchemaAsync(tenant, new ReadSchemaRequest(schemaVersion));

        /// <inheritdoc />
        public Task<SchemaDefinition> ReadSchemaAsync(ReadSchemaRequest request)
            => ReadSchemaAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SchemaDefinition> ReadSchemaAsync(string tenant, ReadSchemaRequest request)
        {
            request.Metadata = EnsureSchemaMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, READ_SCHEMA), request);
            var result = await HandleResponse<ReadSchemaResponse>(response);
            return result.Schema;
        }

        /// <inheritdoc />
        public Task<SnapMetadata> WriteAuthorizationDataAsync(List<PermifyTuple>? relations = null, List<PermifyAttribute>? attributes = null, string? schemaVersion = null)
        {
            // SchemaMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing in an empty string as the schemaVersion.
            var metadata = new SchemaMetadata(schemaVersion ?? "");
            return WriteAuthorizationDataAsync(new WriteAuthorizationDataRequest(relations, attributes, metadata));
        }

        /// <inheritdoc />
        public Task<SnapMetadata> WriteAuthorizationDataAsync(string tenant, List<PermifyTuple>? relations = null, List<PermifyAttribute>? attributes = null, string? schemaVersion = null)
        {
            // SchemaMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing in an empty string as the schemaVersion.
            var metadata = new SchemaMetadata(schemaVersion ?? "");
            return WriteAuthorizationDataAsync(tenant, new WriteAuthorizationDataRequest(relations, attributes, metadata));
        }

        /// <inheritdoc />
        public Task<SnapMetadata> WriteAuthorizationDataAsync(WriteAuthorizationDataRequest request)
            => WriteAuthorizationDataAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SnapMetadata> WriteAuthorizationDataAsync(string tenant, WriteAuthorizationDataRequest request)
        {
            if (request.Relations is null && request.Attributes is null)
                throw new ArgumentNullException(nameof(request), "At least one of relation or attributes must be provided.");
            request.Metadata = EnsureSchemaMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, WRITE_AUTHORIZATION_DATA), request);
            return await HandleResponse<SnapMetadata>(response);
        }

        /// <inheritdoc />
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(
            TupleFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null)
        {
            // SnapMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing an empty string as the snapToken.
            var metadata = new SnapMetadata(snapToken ?? "");
            return ReadRelationshipsAsync(new ReadRelationshipsRequest(filter, metadata, pageSize, continuousToken));
        }

        /// <inheritdoc />
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(
            string tenant,
            TupleFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null)
        {
            // SnapMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing an empty string as the snapToken.
            var metadata = new SnapMetadata(snapToken ?? "");
            return ReadRelationshipsAsync(tenant, new ReadRelationshipsRequest(filter, metadata, pageSize, continuousToken));
        }

        /// <inheritdoc />
        public Task<ReadRelationshipsResponse> ReadRelationshipsAsync(ReadRelationshipsRequest request)
            => ReadRelationshipsAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<ReadRelationshipsResponse> ReadRelationshipsAsync(string tenant, ReadRelationshipsRequest request)
        {
            request.Metadata = EnsureSnapMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, READ_RELATIONSHIPS), request);
            return await HandleResponse<ReadRelationshipsResponse>(response);
        }

        /// <inheritdoc />
        public Task<ReadAttributesResponse> ReadAttributesAsync(
            AttributeFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null)
        {
            // SnapMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing an empty string as the snapToken.
            var metadata = new SnapMetadata(snapToken ?? "");
            return ReadAttributesAsync(new ReadAttributesRequest(filter, metadata, pageSize, continuousToken));
        }

        /// <inheritdoc />
        public Task<ReadAttributesResponse> ReadAttributesAsync(
            string tenant,
            AttributeFilter filter,
            string? snapToken = null,
            int pageSize = 10,
            string? continuousToken = null)
        {
            // SnapMetadata is a required parameter in the request. Instead of being null, the parameter can be ignored
            // by passing an empty string as the snapToken.
            var metadata = new SnapMetadata(snapToken ?? "");
            return ReadAttributesAsync(tenant, new ReadAttributesRequest(filter, metadata, pageSize, continuousToken));
        }

        /// <inheritdoc />
        public Task<ReadAttributesResponse> ReadAttributesAsync(ReadAttributesRequest request)
            => ReadAttributesAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<ReadAttributesResponse> ReadAttributesAsync(string tenant, ReadAttributesRequest request)
        {
            request.Metadata = EnsureSnapMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, READ_ATTRIBUTES), request);
            return await HandleResponse<ReadAttributesResponse>(response);
        }

        /// <inheritdoc />
        public Task<SnapMetadata> RunBundleAsync(string name, JsonNode arguments)
            => RunBundleAsync(new RunBundleRequest(name, arguments));

        /// <inheritdoc />
        public Task<SnapMetadata> RunBundleAsync(string tenant, string name, JsonNode arguments)
            => RunBundleAsync(tenant, new RunBundleRequest(name, arguments));

        /// <inheritdoc />
        public Task<SnapMetadata> RunBundleAsync(string name, IDictionary<string, string> arguments)
            => RunBundleAsync(new RunBundleRequest(name, arguments));

        /// <inheritdoc />
        public Task<SnapMetadata> RunBundleAsync(string tenant, string name, IDictionary<string, string> arguments)
            => RunBundleAsync(tenant, new RunBundleRequest(name, arguments));

        /// <inheritdoc />
        public Task<SnapMetadata> RunBundleAsync(RunBundleRequest request)
            => RunBundleAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SnapMetadata> RunBundleAsync(string tenant, RunBundleRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, RUN_BUNDLE), request);
            return await HandleResponse<SnapMetadata>(response);
        }

        /// <inheritdoc />
        public Task<SnapMetadata> DeleteDataAsync(TupleFilter? tupleFilter = null, AttributeFilter? attributeFilter = null)
            => DeleteDataAsync(new DeleteDataRequest(tupleFilter, attributeFilter));

        /// <inheritdoc />
        public Task<SnapMetadata> DeleteDataAsync(string tenant, TupleFilter? tupleFilter = null, AttributeFilter? attributeFilter = null)
            => DeleteDataAsync(tenant, new DeleteDataRequest(tupleFilter, attributeFilter));

        /// <inheritdoc />
        public Task<SnapMetadata> DeleteDataAsync(DeleteDataRequest request)
            => DeleteDataAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SnapMetadata> DeleteDataAsync(string tenant, DeleteDataRequest request)
        {
            if (request.TupleFilter is null && request.AttributeFilter is null)
                throw new ArgumentNullException(nameof(request), "At least one of tupleFilter or attributeFilter must be provided.");

            // TupleFilter and AttributeFilter can't be null for the request,
            // but they can be empty.
            request.TupleFilter ??= new TupleFilter();
            request.AttributeFilter ??= new AttributeFilter();

            var response = await _client.PostAsJsonAsync(GetUrl(tenant, DELETE_DATA), request);
            return await HandleResponse<SnapMetadata>(response);
        }

        /// <inheritdoc />
        public Task<CheckAccessResponse> CheckAccessAsync(
            PermifyEntity entity,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null)
        {
            return CheckAccessAsync(new CheckAccessRequest(entity, permission, subject, metadata, context, arguments));
        }

        /// <inheritdoc />
        public Task<CheckAccessResponse> CheckAccessAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null)
        {
            return CheckAccessAsync(tenant, new CheckAccessRequest(entity, permission, subject, metadata, context, arguments));
        }

        /// <inheritdoc />
        public Task<CheckAccessResponse> CheckAccessAsync(CheckAccessRequest request)
            => CheckAccessAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<CheckAccessResponse> CheckAccessAsync(string tenant, CheckAccessRequest request)
        {
            request.Metadata = EnsurePermissionMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, CHECK_ACCESS), request);
            return await HandleResponse<CheckAccessResponse>(response);
        }

        /// <inheritdoc />
        public async Task<ExpandTree> ExpandAsync(
            PermifyEntity entity,
            string permission,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null)
        {
            var response = await ExpandAsync(new ExpandRequest(entity, permission, metadata, context, arguments));
            return response.Tree;
        }

        /// <inheritdoc />
        public async Task<ExpandTree> ExpandAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null,
            List<PermissionArgument>? arguments = null)
        {
            var response = await ExpandAsync(tenant, new ExpandRequest(entity, permission, metadata, context, arguments));
            return response.Tree;
        }

        /// <inheritdoc />
        public Task<ExpandResponse> ExpandAsync(ExpandRequest request)
            => ExpandAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<ExpandResponse> ExpandAsync(string tenant, ExpandRequest request)
        {
            request.Metadata = EnsurePermissionMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, EXPAND), request);
            return await HandleResponse<ExpandResponse>(response);
        }

        /// <inheritdoc />
        public async Task<List<string>> LookupSubjectAsync(
            PermifyEntity entity,
            string permission,
            SubjectReference subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await LookupSubjectAsync(new SubjectFilterRequest(entity, permission, subject, metadata, context));
            return response.SubjectIds;
        }

        /// <inheritdoc />
        public async Task<List<string>> LookupSubjectAsync(
            string tenant,
            PermifyEntity entity,
            string permission,
            SubjectReference subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await LookupSubjectAsync(tenant, new SubjectFilterRequest(entity, permission, subject, metadata, context));
            return response.SubjectIds;
        }

        /// <inheritdoc />
        public Task<SubjectFilterResponse> LookupSubjectAsync(SubjectFilterRequest request)
            => LookupSubjectAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SubjectFilterResponse> LookupSubjectAsync(string tenant, SubjectFilterRequest request)
        {
            request.Metadata = EnsurePermissionMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, SUBJECT_FILTER), request);
            return await HandleResponse<SubjectFilterResponse>(response);
        }

        /// <inheritdoc />
        public async Task<List<string>> LookupEntityAsync(
            string entityType,
            string permission,
            PermifySubject subject,
            LookupScope? scope = null,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await LookupEntityAsync(new LookupEntityRequest(entityType, permission, subject, scope, metadata, context));
            return response.EntityIds;
        }

        /// <inheritdoc />
        public async Task<List<string>> LookupEntityAsync(
            string tenant,
            string entityType,
            string permission,
            PermifySubject subject,
            LookupScope? scope = null,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await LookupEntityAsync(tenant, new LookupEntityRequest(entityType, permission, subject, scope, metadata, context));
            return response.EntityIds;
        }

        /// <inheritdoc />
        public Task<LookupEntityResponse> LookupEntityAsync(LookupEntityRequest request)
            => LookupEntityAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<LookupEntityResponse> LookupEntityAsync(string tenant, LookupEntityRequest request)
        {
            request.Metadata = EnsurePermissionMetadata(request.Metadata);
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, LOOKUP_ENTITY), request);
            return await HandleResponse<LookupEntityResponse>(response);
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, PermifyAccess>> SubjectPermissionListAsync(
            PermifyEntity entity,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await SubjectPermissionListAsync(new SubjectPermissionListRequest(entity, subject, metadata, context));
            return response.Permissions;
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, PermifyAccess>> SubjectPermissionListAsync(
            string tenant,
            PermifyEntity entity,
            PermifySubject subject,
            PermissionMetadata? metadata = null,
            PermissionContext? context = null)
        {
            var response = await SubjectPermissionListAsync(tenant, new SubjectPermissionListRequest(entity, subject, metadata, context));
            return response.Permissions;
        }

        /// <inheritdoc />
        public Task<SubjectPermissionListResponse> SubjectPermissionListAsync(SubjectPermissionListRequest request)
            => SubjectPermissionListAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<SubjectPermissionListResponse> SubjectPermissionListAsync(string tenant, SubjectPermissionListRequest request)
        {
            request.Metadata = EnsurePermissionMetadata(request.Metadata);
            if (request.Metadata.OnlyPermissions is null)
                request.Metadata.OnlyPermissions = true;
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, SUBJECT_PERMISSION_LIST), request);
            return await HandleResponse<SubjectPermissionListResponse>(response);
        }

        /// <inheritdoc />
        public async Task<string> WriteBundleAsync(Bundle bundle)
        {
            var response = await WriteBundlesAsync(new WriteBundleRequest(new List<Bundle>() { bundle }));
            return response.Names[0];
        }

        /// <inheritdoc />
        public async Task<string> WriteBundleAsync(string tenant, Bundle bundle)
        {
            var response = await WriteBundlesAsync(tenant, new WriteBundleRequest(new List<Bundle>() { bundle }));
            return response.Names[0];
        }

        /// <inheritdoc />
        public async Task<List<string>> WriteBundlesAsync(List<Bundle> bundles)
        {
            var response = await WriteBundlesAsync(new WriteBundleRequest(bundles));
            return response.Names;
        }

        /// <inheritdoc />
        public async Task<List<string>> WriteBundlesAsync(string tenant, List<Bundle> bundles)
        {
            var response = await WriteBundlesAsync(tenant, new WriteBundleRequest(bundles));
            return response.Names;
        }

        /// <inheritdoc />
        public Task<WriteBundleResponse> WriteBundlesAsync(WriteBundleRequest request)
            => WriteBundlesAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<WriteBundleResponse> WriteBundlesAsync(string tenant, WriteBundleRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, WRITE_BUNDLE), request);
            return await HandleResponse<WriteBundleResponse>(response);
        }

        /// <inheritdoc />
        public async Task<Bundle> ReadBundleAsync(string bundleName)
        {
            var response = await ReadBundleAsync(new ReadBundleRequest(bundleName));
            return response.Bundle;
        }

        /// <inheritdoc />
        public async Task<Bundle> ReadBundleAsync(string tenant, string bundleName)
        {
            var response = await ReadBundleAsync(tenant, new ReadBundleRequest(bundleName));
            return response.Bundle;
        }

        /// <inheritdoc />
        public Task<ReadBundleResponse> ReadBundleAsync(ReadBundleRequest request)
            => ReadBundleAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<ReadBundleResponse> ReadBundleAsync(string tenant, ReadBundleRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, READ_BUNDLE), request);
            return await HandleResponse<ReadBundleResponse>(response);
        }

        /// <inheritdoc />
        public async Task<string> DeleteBundleAsync(string bundleName)
        {
            var response = await DeleteBundleAsync(new DeleteBundleRequest(bundleName));
            return response.Name;
        }

        /// <inheritdoc />
        public async Task<string> DeleteBundleAsync(string tenant, string bundleName)
        {
            var response = await DeleteBundleAsync(tenant, new DeleteBundleRequest(bundleName));
            return response.Name;
        }

        /// <inheritdoc />
        public Task<DeleteBundleResponse> DeleteBundleAsync(DeleteBundleRequest request)
            => DeleteBundleAsync(_tenantId, request);

        /// <inheritdoc />
        public async Task<DeleteBundleResponse> DeleteBundleAsync(string tenant, DeleteBundleRequest request)
        {
            var response = await _client.PostAsJsonAsync(GetUrl(tenant, DELETE_BUNDLE), request);
            return await HandleResponse<DeleteBundleResponse>(response);
        }

        /// <inheritdoc />
        public Task<ListTenantsResponse> ListTenantsAsync(int pageSize = 10, string? continuousToken = null)
        {
            return ListTenantsAsync(new ListTenantsRequest(pageSize, continuousToken));
        }

        /// <inheritdoc />
        public async Task<ListTenantsResponse> ListTenantsAsync(ListTenantsRequest request)
        {
            var response = await _client.PostAsJsonAsync($"{LIST_TENANTS}", request);
            return await HandleResponse<ListTenantsResponse>(response);
        }

        /// <inheritdoc />
        public async Task<PermifyTenant> CreateTenantAsync(string id, string name)
        {
            var response = await CreateTenantAsync(new CreateTenantRequest(id, name));
            return response.Tenant;
        }

        /// <inheritdoc />
        public async Task<CreateTenantResponse> CreateTenantAsync(CreateTenantRequest request)
        {
            //var test = await _client.PostAsync(CREATE_TENANT,  request);
            var response = await _client.PostAsJsonAsync($"{CREATE_TENANT}", request);
            return await HandleResponse<CreateTenantResponse>(response);
        }

        /// <inheritdoc />
        public async Task<PermifyTenant> DeleteTenantAsync(string id)
        {
            var response = await DeleteTenantAsync(new DeleteTenantRequest(id));
            return response.Tenant;
        }

        /// <inheritdoc />
        public async Task<DeleteTenantResponse> DeleteTenantAsync(DeleteTenantRequest request)
        {
            var response = await _client.DeleteAsync($"{DELETE_TENANT}/{request.Id}");
            return await HandleResponse<DeleteTenantResponse>(response);
        }

        private SnapMetadata EnsureSnapMetadata(SnapMetadata? metadata)
        {
            return metadata ?? new SnapMetadata("");
        }

        private PermissionMetadata EnsurePermissionMetadata(PermissionMetadata? metadata)
        {
            metadata ??= new PermissionMetadata();
            if (metadata.Depth < 3)
                metadata.Depth = 3;
            return metadata;
        }

        private SchemaMetadata EnsureSchemaMetadata(SchemaMetadata? metadata)
        {
            return metadata ?? new SchemaMetadata("");
        }
    }
}
