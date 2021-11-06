using System.Collections.Generic;
using Newtonsoft.Json;

namespace Functions.DeliveryOrderProcessor.Models
{
    public class Order
    {
        public Order()
        {
        }


        //[JsonProperty(PropertyName = "orderId")]
        public string OrderId { get; set; }

        //[JsonProperty(PropertyName = "id")] 
        public int Id { get; set; }

        public string BuyerId { get; set; }
        public Address ShipToAddress { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}