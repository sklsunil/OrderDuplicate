using System.ComponentModel;

namespace OrderDuplicate.Application.Model.Order
{
    public class OrderModel
    {
        public string Id { get; set; }

        [Description("Order Number")]
        public string OrderNumber { get; set; }

        [Description("Counter Person Id")]
        public string CounterPersonId { get; set; }

        public List<OrderItemModel> OrderItems { get; set; }
    }
    public class UpdateOrderModel : OrderModel
    {
        [Description("Id")]
        public int Id { get; set; }
    }
    public class OrderItemModel
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
