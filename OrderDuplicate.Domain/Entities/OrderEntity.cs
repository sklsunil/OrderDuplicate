using OrderDuplicate.Domain.Common;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderDuplicate.Domain.Entities
{
    [Table("Orders")]
    public class OrderEntity : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string CounterPersonId { get; set; }
        public bool IsCheckOut { get; set; }
        public ICollection<OrderLineItemEntity> Items { get; set; }
    }
}
