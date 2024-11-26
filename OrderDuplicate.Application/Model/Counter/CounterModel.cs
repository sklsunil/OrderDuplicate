using System.ComponentModel;

namespace OrderDuplicate.Application.Model.Counter
{
    public class CounterModel
    {   
        [Description("PersonId Id")]
        public int PersonId { get; set; }

        [Description("Counter Name")]
        public string CounterName { get; set; }        
    }
    public class UpdateCounterModel : CounterModel
    {
        [Description("Id")]
        public int Id { get; set; }
    }
}
