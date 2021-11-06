using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.OrderItemReserver
{
    public class OrderItemsReserverFunction
    {
        private const string ContainerName = "reserved-orders";
        private readonly Settings _settings;

        public OrderItemsReserverFunction(Settings settings)
        {
            _settings = settings;
        }

        [FunctionName("OrderItemsReserverFunction")]
        [FixedDelayRetry(3, "00:00:05")]
        public async Task Run([ServiceBusTrigger("order-reserver", Connection = "ServiceBusConnection")]
            Message message, ILogger log)
        {
            var payload = Encoding.UTF8.GetString(message.Body);
            // Create the container and return a container client object
            var blobClient = new BlobServiceClient(_settings.BlobStorageConnectionString);
            var containerClient = blobClient.GetBlobContainerClient(ContainerName);
            await containerClient.CreateIfNotExistsAsync();
            var blob = containerClient.GetBlobClient($"{Guid.NewGuid()}.json");

            await using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
            {
                await blob.UploadAsync(ms);
            }

            //add to blob
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {payload}");
        }
    }
}