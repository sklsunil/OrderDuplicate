using OrderDuplicate.Domain.Entities;
using System.ComponentModel;

namespace OrderDuplicate.Application.Model.Group
{
    public class GroupModel
    {
        [Description("CounterId")]
        public int CounterId { get; set; }
        [Description("GroupName")]
        public string GroupName { get; set; }
        public CounterEntity CounterEntity { get; set; }
    }
    public class UpdateGroupModel : GroupModel
    {
        [Description("Id")]
        public int Id { get; set; }
    }
}