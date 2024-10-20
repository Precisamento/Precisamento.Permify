using DotNet.Testcontainers.Builders;
using Precisamento.Permify.DataService;
using Precisamento.Permify.Objects;
using Precisamento.Permify.PermissionService;
using System.Text.Json.Nodes;
using Testcontainers.PostgreSql;

namespace Precisamento.Permify.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class UnitTest1
    {
        const string SCHEMA = "entity user {}\n\nentity organization {\n\n    // organizational roles\n    relation admin @user\n    relation member @user\n\n}\n\nentity team {\n    relation parent @organization\n    relation admin @user\n    relation editor @user\n    relation member @user\n\n    attribute internal boolean\n}\n\nentity post {\n    relation author @user\n    relation team @team\n\n    attribute is_public boolean\n    attribute org_only boolean\n\n    permission view = is_public or author\n\n    action edit = author or team.editor or team.admin\n    action delete = author or team.admin\n}";
        const string SCHEMA2 = "entity user {}\n\nentity organization {\n\n    // organizational roles\n    relation admin @user\n    relation devops @user\n    relation member @user\n\n}\n\nentity team {\n    relation parent @organization\n    relation admin @user\n    relation editor @user\n    relation member @user\n}\n\nentity post {\n    relation author @user\n    relation team @team\n\n    attribute is_public boolean\n\n    permission view = is_public or author\n\n    action edit = author or team.editor or team.admin\n    action delete = author or team.admin\n}";

        private static HttpClient _httpClient;
        private static PermifyClientOptions _permifyClientOptions;

        [AssemblyInitialize]
        public static async Task AssemblySetup(TestContext context)
        {
            var postgres = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithDatabase("permify")
                .Build();


            await postgres.StartAsync();

            var conn = postgres.GetConnectionString();

            var permify = new ContainerBuilder()
                .WithImage("ghcr.io/permify/permify:latest")
                .WithPortBinding(3476, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*?http server successfully started: 3476"))
                .WithCommand("serve")
                .WithVolumeMount("PermifyConfig", "/config")
                .WithEnvironment("PERMIFY_DATABASE_URI", postgres.GetConnectionString())
                .Build();

            await permify.StartAsync();

            _permifyClientOptions = new PermifyClientOptions()
            {
                TenantId = "999",
                Secret = "THIS_IS_MY_SECRET_KEY",
                Host = $"http://{permify.Hostname}:{permify.GetMappedPublicPort(3476)}"
            };

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_permifyClientOptions.Host);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_permifyClientOptions.Secret}");
        }

        private async Task<PermifyClient> GenerateTestData(string id)
        {
            var permify = new PermifyClient(_httpClient, id);
            var tenant = await permify.CreateTenantAsync(id, "permify-test");
            var schemaVersion = await permify.WriteSchemaAsync(SCHEMA);

            var precisamento = new PermifySubject("organization", "1");
            var codeBlogTeam = new PermifySubject("team", "1");

            var mystbornAdmin = new PermifySubject("user", "1");
            var mystbornEditor = new PermifySubject("user", "2");
            var mystbornMember = new PermifySubject("user", "3");
            var sorenMember = new PermifySubject("user", "4");

            var post = new PermifySubject("post", "1");
            var post2 = new PermifySubject("post", "2");
            var post3 = new PermifySubject("post", "3");

            var relations = new List<PermifyTuple>()
            {
                new PermifyTuple(precisamento, "admin", mystbornAdmin),
                new PermifyTuple(precisamento, "member", mystbornAdmin),
                new PermifyTuple(precisamento, "member", mystbornMember),
                new PermifyTuple(precisamento, "member", sorenMember),
                new PermifyTuple(codeBlogTeam, "parent", precisamento),
                new PermifyTuple(codeBlogTeam, "admin", mystbornAdmin),
                new PermifyTuple(codeBlogTeam, "member", mystbornMember),
                new PermifyTuple(post, "author", mystbornMember),
                new PermifyTuple(post, "team", codeBlogTeam),
                new PermifyTuple(post2, "author", sorenMember),
                new PermifyTuple(post2, "team", codeBlogTeam),
                new PermifyTuple(post3, "author", sorenMember),
                new PermifyTuple(post3, "team", codeBlogTeam)
            };

            var attributes = new List<PermifyAttribute>()
            {
                new PermifyAttribute(post, "is_public", true),
                new PermifyAttribute(post2, "is_public", false),
                new PermifyAttribute(post2, "org_only", true),
                new PermifyAttribute(codeBlogTeam, "internal", false)
            };

            await permify.WriteAuthorizationDataAsync(relations, attributes);

            return permify;
        }

        private async Task<string> CreateTestBundle(PermifyClient client)
        {
            var bundle = new Bundle("CreateEditorFromScratch", ["organizationId", "teamId", "userId"], [
                new BundleOperations(
                    relationshipsWrite: [
                        "organization:{{.organizationId}}#member@user:{{.userId}}",
                        "team:{{.teamId}}#editor@user:{{.userId}}",
                        "team:{{.teamId}}#member@user:{{.userId}}",
                    ],
                    attributesWrite: [
                        "team:{{.teamId}}$internal|boolean:true"
                    ]
                ),
            ]);

            return await client.WriteBundleAsync(bundle);
        }

        [TestMethod]
        public async Task TenantCreate_NewTenant_CreatesTenant()
        {
            var client = new PermifyClient(_httpClient, "1");
            var tenant = await client.CreateTenantAsync("1", "t2");

            Assert.AreEqual("1", tenant.Id);
            Assert.AreEqual("t2", tenant.Name);

            tenant = await client.DeleteTenantAsync("1");

            Assert.AreEqual("1", tenant.Id);
            Assert.AreEqual("t2", tenant.Name);
        }

        [TestMethod]
        public async Task TenantCreate_ExistingTenant_OverwritesName()
        {
            var client = await GenerateTestData("2");
            try
            {
                var tenant = await client.CreateTenantAsync("2", "t2");

                Assert.AreEqual("2", tenant.Id);
                Assert.AreEqual("t2", tenant.Name);

                var posts = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "1"));
                Assert.AreNotEqual(0, posts.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("2");
            }
        }

        [TestMethod]
        public async Task TenantDelete_NonExistantTenant_ThrowsPermifyException()
        {
            var client = await GenerateTestData("3");
            await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
            {
                var tenant = await client.DeleteTenantAsync("99999");
            });
        }

        [TestMethod]
        public async Task WriteSchema_NewSchema_WritesSchema()
        {
            var client = new PermifyClient(_httpClient, "4");
            var tenant = await client.CreateTenantAsync("4", "t2");

            try
            {
                var schemaVersion = await client.WriteSchemaAsync(SCHEMA);

                Assert.AreNotEqual("", schemaVersion.SchemaVersion);

                var schemas = await client.ListSchemaAsync();

                Assert.AreEqual(schemas.Head, schemaVersion.SchemaVersion);
            }
            finally
            {
                await client.DeleteTenantAsync("4");
            }
        }

        [TestMethod]
        public async Task WriteSchema_ExistingSchema_KeepsData()
        {
            var client = await GenerateTestData("5");

            try
            {
                var schemaVersion2 = await client.WriteSchemaAsync(SCHEMA2);

                var posts = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "1"));
                Assert.AreNotEqual(0, posts.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("5");
            }
        }

        [TestMethod]
        public async Task ReadSchema_ExistingSchema_Succeeds()
        {
            var client = await GenerateTestData("6");
            try
            {
                var schemaList = await client.ListSchemaAsync();
                var schema = await client.ReadSchemaAsync(schemaList.Head);

                Assert.IsNotNull(schema);
            }
            finally
            {
                await client.DeleteTenantAsync("6");
            }
        }

        [TestMethod]
        public async Task ReadSchema_NonExistantSchema_ReturnsEmptySchema()
        {
            var client = await GenerateTestData("7");
            try
            {
                var schema = await client.ReadSchemaAsync("99999");

                Assert.IsTrue(schema.Entities is null || schema.Entities.Count == 0);
                Assert.IsTrue(schema.Rules is null || schema.Rules.Count == 0);
                Assert.IsTrue(schema.References is null || schema.References.Count == 0);
            }
            finally
            {
                await client.DeleteTenantAsync("7");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_NoMetadata_Succeeds()
        {
            var client = await GenerateTestData("8");

            try
            {
                var snapData = await client.WriteAuthorizationDataAsync(attributes: new List<PermifyAttribute>()
                {
                    new PermifyAttribute(new PermifyEntity("post", "2"), "is_public", true)
                });

                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post", ["2"]), ["is_public"]));
                Assert.AreEqual(true, (attributes.Attributes[0].Value as PermifyBooleanAttributeValue)!.Value);
            }
            finally
            {
                await client.DeleteTenantAsync("8");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_WithMetadata_Succeeds()
        {
            var client = await GenerateTestData("9");

            try
            {
                var schemaList = await client.ListSchemaAsync();
                var snapData = await client.WriteAuthorizationDataAsync(attributes: new List<PermifyAttribute>()
                {
                    new PermifyAttribute(new PermifyEntity("post", "2"), "is_public", true)
                }, schemaVersion: schemaList.Head);

                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post", ["2"]), ["is_public"]));
                Assert.AreEqual(true, (attributes.Attributes[0].Value as PermifyBooleanAttributeValue)!.Value);
            }
            finally
            {
                await client.DeleteTenantAsync("9");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_UpdatesSnap()
        {
            var client = await GenerateTestData("10");

            try
            {
                var schemaList = await client.ListSchemaAsync();
                var snapDataFirst = await client.WriteAuthorizationDataAsync([
                    new PermifyTuple(new PermifyEntity("post", "21"), "author", new PermifySubject("user", "1"))
                ]);

                var snapDataSecond = await client.WriteAuthorizationDataAsync([
                    new PermifyTuple(new PermifyEntity("post", "22"), "author", new PermifySubject("user", "1"))
                ]);

                Assert.AreNotEqual(snapDataFirst.SnapToken, snapDataSecond.SnapToken);
            }
            finally
            {
                await client.DeleteTenantAsync("10");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_AddNewRelation_AddsRelation()
        {
            var client = await GenerateTestData("11");

            try
            {
                var schemaList = await client.ListSchemaAsync();
                await client.WriteAuthorizationDataAsync([
                    new PermifyTuple(new PermifyEntity("post", "21"), "author", new PermifySubject("user", "1"))
                ]);

                var posts = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "1"));
                Assert.IsTrue(posts.Any(id => id == "21"));
            }
            finally
            {
                await client.DeleteTenantAsync("11");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_AddExistingRelation_AddsAdditionalRelation()
        {
            var client = await GenerateTestData("12");

            try
            {
                var schemaList = await client.ListSchemaAsync();
                await client.WriteAuthorizationDataAsync([
                    new PermifyTuple(new PermifyEntity("post", "1"), "author", new PermifySubject("user", "1"))
                ]);

                var users = await client.LookupSubjectAsync(new PermifyEntity("post", "1"), "author", new SubjectReference("user"));
                Assert.IsTrue(users.Count == 2);
            }
            finally
            {
                await client.DeleteTenantAsync("12");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_AddNewAttribute_AddsAttribute()
        {
            var client = await GenerateTestData("13");

            try
            {
                var publicPosts = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post"), ["is_public"]));
                Assert.AreEqual(2, publicPosts.Attributes.Count);

                await client.WriteAuthorizationDataAsync(attributes: new List<PermifyAttribute>()
                {
                    new PermifyAttribute(new PermifyEntity("post", "3"), "is_public", true)
                });

                publicPosts = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post"), ["is_public"]));
                Assert.AreEqual(3, publicPosts.Attributes.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("13");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_AddExistingAttribute_OverwritesAttribute()
        {
            var client = await GenerateTestData("14");

            try
            {
                var postsWithPublicAttribute = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post"), ["is_public"]));
                Assert.IsFalse(postsWithPublicAttribute.Attributes.First(a => a.Entity.Id == "2").Boolean);
                Assert.AreEqual(2, postsWithPublicAttribute.Attributes.Count);

                await client.WriteAuthorizationDataAsync(attributes: new List<PermifyAttribute>()
                {
                    new PermifyAttribute(new PermifyEntity("post", "2"), "is_public", true)
                });

                postsWithPublicAttribute = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post"), ["is_public"]));
                Assert.IsTrue(postsWithPublicAttribute.Attributes.First(a => a.Entity.Id == "2").Boolean);
                Assert.AreEqual(2, postsWithPublicAttribute.Attributes.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("14");
            }
        }

        [TestMethod]
        public async Task WriteAuthData_AddRelationWithInvalidSubject_ThrowsPermifyException()
        {
            var client = await GenerateTestData("15");

            await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
            {
                await client.WriteAuthorizationDataAsync([
                    new PermifyTuple(new PermifyEntity("post", "1"), "author", new PermifySubject("invalid", "99999"))
                ]);
            });

            await client.DeleteTenantAsync("15");
        }

        [TestMethod]
        public async Task WriteAuthData_AddAttributeWithInvalidEntity_ThrowsPermifyException()
        {
            var client = await GenerateTestData("16");

            await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
            {
                await client.WriteAuthorizationDataAsync(attributes: new List<PermifyAttribute>()
                {
                    new PermifyAttribute(new PermifyEntity("invalid", "2"), "is_public", true)
                });
            });

            await client.DeleteTenantAsync("16");
        }

        [TestMethod]
        public async Task ReadRelationships_AllPostRelations_ReturnsAllPostRelations()
        {
            var client = await GenerateTestData("17");

            try
            {
                // Returns author -> post and post -> team relations
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter()));
                Assert.AreEqual(6, relationResponse.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("17");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_SpecificRelation_ReturnsOnlyAuthors()
        {
            var client = await GenerateTestData("18");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "author", new SubjectFilter()));
                Assert.AreEqual(3, relationResponse.Tuples.Count);
                foreach (var tuple in relationResponse.Tuples)
                {
                    Assert.AreEqual("author", tuple.Relationship);
                }
            }
            finally
            {
                await client.DeleteTenantAsync("18");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_AllRelationsSpecificSubjectType_ReturnsPostAndUserRelations()
        {
            var client = await GenerateTestData("19");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter("user")));
                Assert.AreEqual(3, relationResponse.Tuples.Count);
                foreach (var tuple in relationResponse.Tuples)
                {
                    Assert.AreEqual("post", tuple.Entity.Type);
                    Assert.AreEqual("user", tuple.Subject.Type);
                }
            }
            finally
            {
                await client.DeleteTenantAsync("19");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_AllRelationsSpecificSubject_ReturnsPostAndAndUser4Relations()
        {
            var client = await GenerateTestData("20");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter("user", ["4"])));
                Assert.AreEqual(2, relationResponse.Tuples.Count);
                foreach (var tuple in relationResponse.Tuples)
                {
                    Assert.AreEqual("post", tuple.Entity.Type);
                    Assert.AreEqual("user", tuple.Subject.Type);
                    Assert.AreEqual("4", tuple.Subject.Id);
                }
            }
            finally
            {
                await client.DeleteTenantAsync("20");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_AllRelationsSpecificEntity_ReturnsPost1Relations()
        {
            var client = await GenerateTestData("21");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post", ["1"]), "", new SubjectFilter()));
                Assert.AreEqual(2, relationResponse.Tuples.Count);
                foreach (var tuple in relationResponse.Tuples)
                {
                    Assert.AreEqual("post", tuple.Entity.Type);
                    Assert.AreEqual("1", tuple.Entity.Id);
                }
            }
            finally
            {
                await client.DeleteTenantAsync("21");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_InvalidEntityType_ReturnEmptyList()
        {
            var client = await GenerateTestData("22");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("invalid"), "", new SubjectFilter()));
                Assert.AreEqual(0, relationResponse.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("22");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_InvalidRelationType_ReturnEmptyList()
        {
            var client = await GenerateTestData("23");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "invalid", new SubjectFilter()));
                Assert.AreEqual(0, relationResponse.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("23");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_InvalidSubjectType_ReturnEmptyList()
        {
            var client = await GenerateTestData("24");

            try
            {
                var relationResponse = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("invalid")));
                Assert.AreEqual(0, relationResponse.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("24");
            }
        }

        [TestMethod]
        public async Task ReadRelationships_Paginate_GetsAllTuples()
        {
            var client = await GenerateTestData("25");
            var tuples = new List<PermifyTuple>();
            var trips = 0;

            try
            {
                var allRelations = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter()));
                Assert.AreEqual(6, allRelations.Tuples.Count);

                ReadRelationshipsResponse paginatedRelations;
                var token = "";
                do
                {
                    trips++;
                    paginatedRelations = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter()), pageSize: 3, continuousToken: token);
                    tuples.AddRange(paginatedRelations.Tuples);
                    token = paginatedRelations.ContinuousToken;
                }
                while (token != "");

                Assert.AreEqual(2, trips);
                Assert.AreEqual(6, tuples.Count);

                foreach (var tuple in allRelations.Tuples)
                {
                    Assert.IsTrue(tuples.Any(
                        t => t.Entity.Id == tuple.Entity.Id 
                        && t.Relationship == tuple.Relationship
                        && t.Subject.Id == tuple.Subject.Id));
                }
            }
            finally
            {
                await client.DeleteTenantAsync("25");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_AllAttributes_GetPostAndTeamAttributes()
        {
            var client = await GenerateTestData("26");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter());
                Assert.AreEqual(4, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "post"));
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "team"));
            }
            finally
            {
                await client.DeleteTenantAsync("26");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_AllAttributesSpecificEntity_GetPostAttributes()
        {
            var client = await GenerateTestData("27");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post")));
                Assert.AreEqual(3, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "post"));
                Assert.IsTrue(attributes.Attributes.Any(a => a.Name == "is_public"));
                Assert.IsTrue(attributes.Attributes.Any(a => a.Name == "org_only"));
                Assert.IsFalse(attributes.Attributes.Any(a => a.Entity.Type == "team"));
            }
            finally
            {
                await client.DeleteTenantAsync("27");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_AllAttributesSpecificEntityId_GetPost1And2Attributes()
        {
            var client = await GenerateTestData("28");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post", ["1"])));
                Assert.AreEqual(1, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "post" && a.Name == "is_public"));

                attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post", ["2"])));
                Assert.AreEqual(2, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "post" && a.Name == "is_public"));
                Assert.IsTrue(attributes.Attributes.Any(a => a.Entity.Type == "post" && a.Name == "org_only"));
            }
            finally
            {
                await client.DeleteTenantAsync("28");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_SpecificAttributeAllEntities_GetIsPublicAttributes()
        {
            var client = await GenerateTestData("29");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter(), ["is_public"]));
                Assert.AreEqual(2, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.All(a => a.Name == "is_public"));
            }
            finally
            {
                await client.DeleteTenantAsync("29");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_SpecificAttributeSpecificEntity_GetPost1IsPublic()
        {
            var client = await GenerateTestData("30");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("post", ["1"]), ["is_public"]));
                Assert.AreEqual(1, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes.Any(a => a.Name == "is_public" && a.Boolean));
            }
            finally
            {
                await client.DeleteTenantAsync("30");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_InvalidEntity_ReturnEmptyList()
        {
            var client = await GenerateTestData("31");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("invalid", ["9999"])));
                Assert.AreEqual(0, attributes.Attributes.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("31");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_InvalidAttribute_ReturnEmptyList()
        {
            var client = await GenerateTestData("32");

            try
            {
                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter(), ["invalid"]));
                Assert.AreEqual(0, attributes.Attributes.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("32");
            }
        }

        [TestMethod]
        public async Task ReadAttributes_Paginate_GetsAllAttributes()
        {
            var client = await GenerateTestData("32");
            var attributes = new List<PermifyAttribute>();
            var trips = 0;

            try
            {
                var attributesResponse = await client.ReadAttributesAsync(new AttributeFilter());
                Assert.AreEqual(4, attributesResponse.Attributes.Count);

                var token = "";
                do
                {
                    var paginatedResponse = await client.ReadAttributesAsync(new AttributeFilter(), pageSize: 3, continuousToken: token);
                    attributes.AddRange(paginatedResponse.Attributes);
                    token = paginatedResponse.ContinuousToken;
                    trips++;
                }
                while (token != "");

                Assert.AreEqual(2, trips);
                Assert.AreEqual(attributes.Count, attributesResponse.Attributes.Count);

                foreach (var attr in attributesResponse.Attributes)
                {
                    Assert.IsTrue(attributes.Any(a => a.Entity.Id == attr.Entity.Id && a.Name == attr.Name));
                }
            }
            finally
            {
                await client.DeleteTenantAsync("32");
            }
        }

        [TestMethod]
        public async Task ReadBundle_ExistingBundle_ReturnsBundle()
        {
            var client = await GenerateTestData("33");

            try
            {
                var bundleName = await CreateTestBundle(client);

                var bundle = await client.ReadBundleAsync(bundleName);
                Assert.AreEqual(bundleName, bundle.Name);
                Assert.AreEqual(3, bundle.Operations[0].RelationshipsWrite.Count);
                Assert.AreEqual(1, bundle.Operations[0].AttributesWrite.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("33");
            }
        }

        [TestMethod]
        public async Task ReadBundle_MissingBundle_ThrowsPermifyException()
        {
            var client = await GenerateTestData("34");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    await client.ReadBundleAsync("invalid");
                });
            }
            finally
            {
                await client.DeleteTenantAsync("34");
            }
        }

        [TestMethod]
        public async Task RunBundle_ValidArguments_ExecutesAllOperations()
        {
            var client = await GenerateTestData("35");

            try
            {
                var bundleName = await CreateTestBundle(client);
                var args = new Dictionary<string, string>()
                {
                    { "organizationId", "1" },
                    { "teamId", "1" },
                    { "userId", "3000" }
                };

                await client.RunBundleAsync(bundleName, args);
                var relations = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(""), "", new SubjectFilter(ids: ["3000"])));
                Assert.AreEqual(3, relations.Tuples.Count);
                Assert.IsTrue(relations.Tuples.Any(t => t.Entity.Type == "organization" && t.Relationship == "member"));
                Assert.IsTrue(relations.Tuples.Any(t => t.Entity.Type == "team" && t.Relationship == "member"));
                Assert.IsTrue(relations.Tuples.Any(t => t.Entity.Type == "team" && t.Relationship == "editor"));

                var attributes = await client.ReadAttributesAsync(new AttributeFilter(new EntityFilter("team", ["1"]), ["internal"]));
                Assert.AreEqual(1, attributes.Attributes.Count);
                Assert.IsTrue(attributes.Attributes[0].Boolean);
            }
            finally
            {
                await client.DeleteTenantAsync("35");
            }
        }

        [TestMethod]
        public async Task RunBundle_MissingBundle_ThrowsPermifyException()
        {
            var client = await GenerateTestData("36");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    await client.RunBundleAsync("invalid", new Dictionary<string, string>());
                });
            }
            finally
            {
                await client.DeleteTenantAsync("36");
            }
        }

        [TestMethod]
        public async Task DeleteData_AllEntitiesAllSubject_DeletesAllPostTeams()
        {
            var client = await GenerateTestData("37");

            try
            {
                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "team", new SubjectFilter()));

                await client.DeleteDataAsync(new TupleFilter(new EntityFilter("post"), "team", new SubjectFilter()));

                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "team", new SubjectFilter()));

                Assert.IsTrue(postTeamsPreDelete.Tuples.Count > 0);
                Assert.AreEqual(0, postTeamsPostDelete.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("37");
            }
        }

        [TestMethod]
        public async Task DeleteData_OneEntityAllSubjects_DeletesAllPost1Teams()
        {
            var client = await GenerateTestData("38");

            try
            {
                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "team", new SubjectFilter()));

                await client.DeleteDataAsync(new TupleFilter(new EntityFilter("post", ["1"]), "team", new SubjectFilter()));

                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "team", new SubjectFilter()));

                Assert.IsTrue(postTeamsPreDelete.Tuples.Count > 0);
                Assert.AreEqual(postTeamsPreDelete.Tuples.Count - 1, postTeamsPostDelete.Tuples.Count);
                Assert.IsTrue(postTeamsPostDelete.Tuples.All(t => t.Entity.Id != "1"));

            }
            finally
            {
                await client.DeleteTenantAsync("38");
            }
        }

        [TestMethod]
        public async Task DeleteData_AllEntitiesOneSubject_DeletesAllUser3()
        {
            var client = await GenerateTestData("39");

            try
            {
                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter(type: "user", ids: ["4"])));

                await client.DeleteDataAsync(new TupleFilter(new EntityFilter(""), "", new SubjectFilter("user", ["4"])));

                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter(type: "user", ids: ["4"])));

                Assert.IsTrue(postTeamsPreDelete.Tuples.Count > 0);
                Assert.AreEqual(0, postTeamsPostDelete.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("39");
            }
        }

        [TestMethod]
        public async Task DeleteData_OneEntityTypeOneSubject_DeletesPostAuthorUser3()
        {
            var client = await GenerateTestData("40");

            try
            {
                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter(type: "user")));

                await client.DeleteDataAsync(new TupleFilter(new EntityFilter("post"), "author", new SubjectFilter("user", ["4"])));

                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter(type: "user")));
                Assert.IsTrue(postTeamsPreDelete.Tuples.Count > 0);
                Assert.IsTrue(postTeamsPostDelete.Tuples.Count < postTeamsPreDelete.Tuples.Count);
                Assert.IsTrue(postTeamsPostDelete.Tuples.All(t => t.Subject.Id != "4"));
            }
            finally
            {
                await client.DeleteTenantAsync("40");
            }
        }

        [TestMethod]
        public async Task DeleteData_OneEntityOneSubject_DeletesPost2AuthorUser3()
        {
            var client = await GenerateTestData("41");

            try
            {
                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter(type: "user")));

                await client.DeleteDataAsync(new TupleFilter(new EntityFilter("post", ["2"]), "author", new SubjectFilter("user", ["4"])));

                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter("post"), "", new SubjectFilter(type: "user")));
                Assert.IsTrue(postTeamsPreDelete.Tuples.Count > 0);
                Assert.IsTrue(postTeamsPostDelete.Tuples.Count < postTeamsPreDelete.Tuples.Count);
                Assert.IsTrue(postTeamsPostDelete.Tuples.Any(t => t.Entity.Id == "3" && t.Subject.Id == "4"));
            }
            finally
            {
                await client.DeleteTenantAsync("41");
            }
        }

        [TestMethod]
        public async Task DeleteData_InvalidEntity_DoesNothing()
        {
            var client = await GenerateTestData("42");

            try
            {

                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "author", new SubjectFilter("user")));
                await client.DeleteDataAsync(new TupleFilter(new EntityFilter("cookbook"), "author", new SubjectFilter("user")));
                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "author", new SubjectFilter("user")));

                Assert.IsTrue(postTeamsPostDelete.Tuples.Count > 0);
                Assert.AreEqual(postTeamsPreDelete.Tuples.Count, postTeamsPostDelete.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("42");
            }
        }

        [TestMethod]
        public async Task DeleteData_InvalidRelation_DoesNothing()
        {
            var client = await GenerateTestData("43");

            try
            {

                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("user")));
                await client.DeleteDataAsync(new TupleFilter(new EntityFilter(), "founder", new SubjectFilter()));
                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("user")));

                Assert.IsTrue(postTeamsPostDelete.Tuples.Count > 0);
                Assert.AreEqual(postTeamsPreDelete.Tuples.Count, postTeamsPostDelete.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("43");
            }
        }

        [TestMethod]
        public async Task DeleteData_InvalidSubject_DoesNothing()
        {
            var client = await GenerateTestData("44");

            try
            {

                var postTeamsPreDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("user")));
                await client.DeleteDataAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("invalid")));
                var postTeamsPostDelete = await client.ReadRelationshipsAsync(new TupleFilter(new EntityFilter(), "", new SubjectFilter("user")));

                Assert.IsTrue(postTeamsPostDelete.Tuples.Count > 0);
                Assert.AreEqual(postTeamsPreDelete.Tuples.Count, postTeamsPostDelete.Tuples.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("44");
            }
        }

        [TestMethod]
        public async Task DeleteData_AllEntityOneAttribute_DeletesIsPublic()
        {
            var client = await GenerateTestData("45");

            try
            {
                var attrsPreDelete = await client.ReadAttributesAsync(new AttributeFilter());

                await client.DeleteDataAsync(attributeFilter: new AttributeFilter(["is_public"]));

                var attrsPostDelete = await client.ReadAttributesAsync(new AttributeFilter());

                Assert.IsTrue(attrsPreDelete.Attributes.Any(a => a.Name == "is_public"));
                Assert.IsFalse(attrsPostDelete.Attributes.Any(a => a.Name == "is_public"));
            }
            finally
            {
                await client.DeleteTenantAsync("45");
            }
        }

        [TestMethod]
        public async Task DeleteData_OneEntityTypeOneAttribute_DeletesPostIsPublic()
        {
            var client = await GenerateTestData("46");

            try
            {
                var attrsPreDelete = await client.ReadAttributesAsync(new AttributeFilter());

                await client.DeleteDataAsync(attributeFilter: new AttributeFilter(new EntityFilter("post"), ["is_public"]));

                var attrsPostDelete = await client.ReadAttributesAsync(new AttributeFilter());

                Assert.IsTrue(attrsPreDelete.Attributes.Any(a => a.Name == "is_public"));
                Assert.IsFalse(attrsPostDelete.Attributes.Any(a => a.Name == "is_public"));
            }
            finally
            {
                await client.DeleteTenantAsync("46");
            }
        }

        [TestMethod]
        public async Task CheckAccess_AdminForPostEdit_ReturnsAllowed()
        {
            var client = await GenerateTestData("47");

            try
            {
                var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "edit", new PermifySubject("user", "1"));
                Assert.IsTrue(access.Access == PermifyAccess.Allowed);
            }
            finally
            {
                await client.DeleteTenantAsync("47");
            }
        }

        [TestMethod]
        public async Task CheckAccess_EditorForPostTeamEdit_ReturnsAllowed()
        {
            var client = await GenerateTestData("48");

            try
            {
                var bundle = await CreateTestBundle(client);
                await client.RunBundleAsync(bundle, new Dictionary<string, string>()
                {
                    { "organizationId", "1" },
                    { "teamId", "1" },
                    { "userId", "1000" }
                });

                var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "edit", new PermifySubject("user", "1000"));
                Assert.IsTrue(access.Access == PermifyAccess.Allowed);
            }
            finally
            {
                await client.DeleteTenantAsync("48");
            }
        }

        [TestMethod]
        public async Task CheckAccess_EditorForOtherPostEdit_ReturnsDenied()
        {
            var client = await GenerateTestData("49");

            try
            {
                var bundle = await CreateTestBundle(client);
                await client.RunBundleAsync(bundle, new Dictionary<string, string>()
                {
                    { "organizationId", "1" },
                    { "teamId", "2" },
                    { "userId", "1000" }
                });

                var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "edit", new PermifySubject("user", "1000"));
                Assert.IsTrue(access.Access == PermifyAccess.Denied);
            }
            finally
            {
                await client.DeleteTenantAsync("49");
            }
        }

        [TestMethod]
        public async Task CheckAccess_AuthorForPostEdit_ReturnsAllowed()
        {
            var client = await GenerateTestData("50");

            try
            {
                var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "edit", new PermifySubject("user", "3"));
                Assert.IsTrue(access.Access == PermifyAccess.Allowed);
            }
            finally
            {
                await client.DeleteTenantAsync("50");
            }
        }

        [TestMethod]
        public async Task CheckAccess_UnkownPermission_ThrowsPermifyException()
        {
            var client = await GenerateTestData("51");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "unknown", new PermifySubject("user", "3"));
                });
            }
            finally
            {
                await client.DeleteTenantAsync("51");
            }
        }

        [TestMethod]
        public async Task CheckAccess_UnknownUserPostEdit_ReturnsDenied()
        {
            var client = await GenerateTestData("52");

            try
            {
                var access = await client.CheckAccessAsync(new PermifyEntity("post", "1"), "edit", new PermifySubject("user", "50"));
                Assert.IsTrue(access.Access == PermifyAccess.Denied);
            }
            finally
            {
                await client.DeleteTenantAsync("52");
            }
        }

        [TestMethod]
        public async Task Expand_Works()
        {
            var client = await GenerateTestData("53");

            try
            {
                var tree = await client.ExpandAsync(new PermifyEntity("post", "2"), "edit");
            }
            finally
            {
                await client.DeleteTenantAsync("53");
            }
        }

        [TestMethod]
        public async Task LookupSubject_EditPost_ReturnsAuthorAndAdmin()
        {

            var client = await GenerateTestData("54");

            try
            {
                var subjects = await client.LookupSubjectAsync(new PermifyEntity("post", "1"), "edit", new SubjectReference("user"));
                Assert.AreEqual(2, subjects.Count);
                Assert.IsTrue(subjects.Any(s => s == "1"));
                Assert.IsTrue(subjects.Any(s => s == "3"));
            }
            finally
            {
                await client.DeleteTenantAsync("54");
            }
        }

        [TestMethod]
        public async Task LookupSubject_EditInvalidPost_ReturnsEmptyList()
        {

            var client = await GenerateTestData("54");

            try
            {
                var subjects = await client.LookupSubjectAsync(new PermifyEntity("post", "4"), "edit", new SubjectReference("user"));
                Assert.AreEqual(0, subjects.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("54");
            }
        }

        [TestMethod]
        public async Task LookupSubject_InvalidEntityEditInvalidPost_ReturnsEmptyList()
        {

            var client = await GenerateTestData("54");

            try
            {
                var subjects = await client.LookupSubjectAsync(new PermifyEntity("post", "1"), "edit", new SubjectReference("moo"));
                Assert.AreEqual(0, subjects.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("54");
            }
        }

        [TestMethod]
        public async Task LookupEntity_AdminCanEditAllPosts_ReturnsAllPosts()
        {

            var client = await GenerateTestData("55");

            try
            {
                var entities = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "1"));
                Assert.AreEqual(3, entities.Count);
                Assert.IsTrue(entities.Any(e => e == "1"));
                Assert.IsTrue(entities.Any(e => e == "2"));
                Assert.IsTrue(entities.Any(e => e == "3"));
            }
            finally
            {
                await client.DeleteTenantAsync("55");
            }
        }

        [TestMethod]
        public async Task LookupEntity_UserCanEditTheirPosts_ReturnsPostsWithUserAsAuthor()
        {

            var client = await GenerateTestData("56");

            try
            {
                var entities = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "4"));
                Assert.AreEqual(2, entities.Count);
                Assert.IsTrue(entities.Any(e => e == "2"));
                Assert.IsTrue(entities.Any(e => e == "3"));
            }
            finally
            {
                await client.DeleteTenantAsync("56");
            }
        }

        [TestMethod]
        public async Task LookupEntity_InvalidPermission_ThrowsPermifyException()
        {

            var client = await GenerateTestData("57");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    await client.LookupEntityAsync("post", "invalid", new PermifySubject("user", "4"));
                });
            }
            finally
            {
                await client.DeleteTenantAsync("57");
            }
        }

        [TestMethod]
        public async Task LookupEntity_InvalidEntity_ThrowsPermifyException()
        {

            var client = await GenerateTestData("58");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    await client.LookupEntityAsync("invalid", "edit", new PermifySubject("user", "4"));
                });
            }
            finally
            {
                await client.DeleteTenantAsync("58");
            }
        }

        [TestMethod]
        public async Task LookupEntity_InvalidSubject_ReturnsEmptyList()
        {

            var client = await GenerateTestData("59");

            try
            {
                var entities = await client.LookupEntityAsync("post", "edit", new PermifySubject("invalid", "4"));
                Assert.AreEqual(0, entities.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("59");
            }
        }

        [TestMethod]
        public async Task LookupEntity_InvalidSubjectId_ReturnsEmptyList()
        {
            var client = await GenerateTestData("60");

            try
            {
                var entities = await client.LookupEntityAsync("post", "edit", new PermifySubject("user", "99"));
                Assert.AreEqual(0, entities.Count);
            }
            finally
            {
                await client.DeleteTenantAsync("60");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_AdminPublicPost_ReturnsAllAllow()
        {
            var client = await GenerateTestData("61");

            try
            {
                var permissions = await client.SubjectPermissionListAsync(new PermifyEntity("post", "1"), new PermifySubject("user", "1"));
                Assert.AreEqual(PermifyAccess.Allowed, permissions["view"]);
                Assert.AreEqual(PermifyAccess.Allowed, permissions["edit"]);
                Assert.AreEqual(PermifyAccess.Allowed, permissions["delete"]);
            }
            finally
            {
                await client.DeleteTenantAsync("61");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_AdminPrivatePost_EditDeleteAllowedViewDenied()
        {
            var client = await GenerateTestData("61");

            try
            {
                var permissions = await client.SubjectPermissionListAsync(new PermifyEntity("post", "3"), new PermifySubject("user", "1"));
                Assert.AreEqual(PermifyAccess.Denied, permissions["view"]);
                Assert.AreEqual(PermifyAccess.Allowed, permissions["edit"]);
                Assert.AreEqual(PermifyAccess.Allowed, permissions["delete"]);
            }
            finally
            {
                await client.DeleteTenantAsync("61");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_InvalidEntityType_ThrowsPermifyException()
        {
            var client = await GenerateTestData("62");

            try
            {
                await Assert.ThrowsExceptionAsync<PermifyException>(async () =>
                {
                    await client.SubjectPermissionListAsync(new PermifyEntity("invalid", "3"), new PermifySubject("user", "1"));
                });
            }
            finally
            {
                await client.DeleteTenantAsync("62");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_InvalidEntityId_ReturnsAllDenied()
        {
            var client = await GenerateTestData("62");

            try
            {
                var permissions = await client.SubjectPermissionListAsync(new PermifyEntity("post", "20"), new PermifySubject("user", "1"));
                Assert.AreEqual(PermifyAccess.Denied, permissions["view"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["edit"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["delete"]);
            }
            finally
            {
                await client.DeleteTenantAsync("62");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_InvalidSubjectType_ReturnsViewAllowed()
        {
            var client = await GenerateTestData("63");

            try
            {
                var permissions = await client.SubjectPermissionListAsync(new PermifyEntity("post", "1"), new PermifySubject("invalid", "1"));
                Assert.AreEqual(PermifyAccess.Allowed, permissions["view"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["edit"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["delete"]);
            }
            finally
            {
                await client.DeleteTenantAsync("63");
            }
        }

        [TestMethod]
        public async Task SubjectPermissionList_InvalidSubjectId_ReturnsViewAllowed()
        {
            var client = await GenerateTestData("64");

            try
            {
                var permissions = await client.SubjectPermissionListAsync(new PermifyEntity("post", "1"), new PermifySubject("user", "20"));
                Assert.AreEqual(PermifyAccess.Allowed, permissions["view"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["edit"]);
                Assert.AreEqual(PermifyAccess.Denied, permissions["delete"]);
            }
            finally
            {
                await client.DeleteTenantAsync("64");
            }
        }
    }
}
