using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderDuplicate.Domain.Entities
{
    [Table("GroupCounters")]
    public class GroupCounterEntity
    {
        public int GroupId { get; set; }
        public GroupEntity Group { get; set; }

        public int CounterId { get; set; }
        public CounterEntity Counter { get; set; }
    }
}
