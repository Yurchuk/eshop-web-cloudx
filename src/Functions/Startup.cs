using Functions;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            var blobStorageConnectionString = configuration["AZURE_STORAGE_CONNECTION_STRING"];
            var serviceBusConnection = configuration["ServiceBusConnection"];
            var cosmosDbUri = configuration["COSMOS_DB_URI"];
            var cosmosDbKey = configuration["COSMOS_DB_KEY"];

            builder.Services.AddScoped(c => new CosmosClient(cosmosDbUri, cosmosDbKey));
            builder.Services.AddScoped(c => new Settings
            {
                BlobStorageConnectionString = blobStorageConnectionString
            });
        }
    }

    public class Settings
    {
        public string BlobStorageConnectionString { get; set; }
    }
}