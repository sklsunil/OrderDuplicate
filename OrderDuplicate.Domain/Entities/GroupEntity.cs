using OrderDuplicate.Domain.Common;

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderDuplicate.Domain.Entities
{
    [Table("Groups")]
    public class GroupEntity : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CounterId { get; set; }
        public string GroupName { get; set; }
        public CounterEntity CounterEntity { get; set; }
    }
}
