namespace Functions.DeliveryOrderProcessor.Models
{
    public class OrderItem
    {
        public CatalogItemOrdered ItemOrdered { get; set; }
        public decimal UnitPrice { get; set; }
        public int Units { get; set; }
    }
}