using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Precisamento.Permify
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPermify(this IServiceCollection services, PermifyClientOptions options)
        {
            services.AddHttpClient("Permify", client =>
            {
                client.BaseAddress = new Uri(options.Host);

                if (options.Secret != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {options.Secret}");
                }
            });

            services.AddHttpClient<IPermifyClient, PermifyClient>("Permify", client => new PermifyClient(client, options));
            services.AddHttpClient<IPermifyClientMultiTenant, PermifyClient>("Permify", client => new PermifyClient(client, options));
            services.AddHttpClient<IPermifyClientRaw, PermifyClient>("Permify", client => new PermifyClient(client, options));
            services.AddHttpClient<IPermifyClientRawMultiTenant, PermifyClient>("Permify", client => new PermifyClient(client, options));
            services.AddHttpClient<IPermifyClientFull, PermifyClient>("Permify", client => new PermifyClient(client, options));

            return services;
        }
    }
}
