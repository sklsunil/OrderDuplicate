using OrderDuplicate.Domain.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDuplicate.Domain.Entities
{
    internal class GroupEntity : AuditableEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CounterId { get; set; }
        public string GroupName { get; set; }
        public CounterEntity CounterEntity { get; set; }
    }
}
