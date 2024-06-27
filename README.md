# Precisamento.Permify

Precisamento.Permify is a C# SDK for the Permify API.

# Usage

## General

There are few interfaces available that have slightly different uses, but they all share the same default implementation (PermifyClient).

1. IPermifyClient
  * Best for single tenant applications, or applications that only access a single tenant.
  * The methods have descriptive parameters with sane defaults where expected.
2. IPermifyClientMultiTenant
  * For applications that need to interact with mutliple tenants.
  * All methods take a specific tenant as their first argument.
  * The methods have descriptive parameters with sane defaults where expected.
3. IPermifyClientRaw
  * Best for single tenant applications, or applications that only access a single tenant.
  * The methods take and return API model objects that directly mirror the JSON requests.
4. IPermifyClientRawMultiTenant
  * For applications that need to interact with mutliple tenants.
  * All methods take a specific tenant as their first argument.
  * The methods take and return API model objects that directly mirror the JSON requests.
5. IPermifyClientFull
  * Useful for hybrid applications with one main tenant, but occasionally needing to interact with others.
  * Union of the above interfaces.

### Example

```cs
var schema = "..."; // Permify schema string

// Typically this would be loaded in from a config file or environment variables.
var options = new PermifyClientOptions()
{
  TenantId = "my-tenant", // Defaults to "t1"
  Secret = "MY_SECRET_KEY", // One of the keys when using the preshared keys authn config.
  Host = "http://localhost:3476" // URL of the Permify server
};

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(options.Host);
httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.Secret}");

var permifyClient = new PermifyClient(httpClient, options.TenantId);

await permifyClient.CreateTenantAsync(options.TenantId, "My Tenant");
await permifyClient.WriteSchemaAsync(schema);
```

## DependencyInjection

If this library is being used in an application using the C# DependencyInjection functionality (ASP.NET for example), the client can be registered using the `AddPermify` extension method. It will add all of the interface types to the `IServiceCollection`.

```cs
// App startup

// Typically this would be loaded in from a config file or environment variables.
var options = new PermifyClientOptions()
{
  TenantId = "my-tenant", // Defaults to "t1"
  Secret = "MY_SECRET_KEY", // One of the keys when using the preshared keys authn config.
  Host = "http://localhost:3476" // URL of the Permify server
};

var services = new ServiceCollection();

services.AddPermify(options);



// Get IPermifyClient via constructor

class MyController : Controller {
  private IPermifyClient _permify;

  public MyController(IPermifyClient permify) {
    _permify = permify;
  }
}
```

## Install

This library can be installed from nuget under the name `Precisamento.Permify`.

```sh
dotnet add package Precisamento.Permify
```

# Docs

The docs site is currently under construction. The library is reasonably well-commented with doc blocks, and the methods on `IPermifyClient` should be easily identifiable based on the [Permify API documentation](https://docs.permify.co/api-reference/introduction).
