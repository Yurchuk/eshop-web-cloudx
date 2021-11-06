using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Functions.DeliveryOrderProcessor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Functions.DeliveryOrderProcessor
{
    public class DeliveryOrderProcessorFunction
    {
        private readonly CosmosClient _cosmosClient;

        public DeliveryOrderProcessorFunction(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        [FunctionName("DeliveryOrderProcessorFunction")]
        public async Task Run([ServiceBusTrigger("delivery-order-processor", Connection = "ServiceBusConnection")]
            Message message, ILogger log)
        {
            var orderString = Encoding.UTF8.GetString(message.Body);
            var order = JsonSerializer.Deserialize<Order>(orderString);

            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync("eShop");
            var targetDatabase = databaseResponse.Database;

            var indexingPolicy = new IndexingPolicy
            {
                IndexingMode = IndexingMode.Consistent,
                Automatic = true,
                IncludedPaths =
                {
                    new IncludedPath
                    {
                        Path = "/*"
                    }
                }
            };
            var containerProperties = new ContainerProperties("Orders", "/orderId")
            {
                IndexingPolicy = indexingPolicy
            };
            var containerResponse = await targetDatabase.CreateContainerIfNotExistsAsync(containerProperties, 10000);
            var customContainer = containerResponse.Container;
            var cosmosOrder = new CosmosOrder()
            {
                BuyerId = order.BuyerId,
                Id = order.Id.ToString(),
                OrderId = order.ToString(),
                OrderItems = order.OrderItems,
                ShipToAddress = order.ShipToAddress
            };
            var createOrderResponse = await customContainer.CreateItemAsync(cosmosOrder, new PartitionKey(cosmosOrder.OrderId));

            //add to blob
            log.LogInformation("DeliveryOrderProcessorFunction function processed a request.");
        }
    }
}