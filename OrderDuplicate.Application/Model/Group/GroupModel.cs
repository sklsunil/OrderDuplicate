using System.ComponentModel;

namespace OrderDuplicate.Application.Model.Group
{
    public class GroupModel
    {
        [Description("GroupName")]
        public string GroupName { get; set; }
    }
    public class UpdateGroupModel : GroupModel
    {
        [Description("Id")]
        public int Id { get; set; }
    }
}