namespace EventProcessHTTP.Model
{
    public class FunctionPubSubEvent
    {
        public int SesstionId { get; set; }
        public EventType EventType { get; set; }
        public string Content { get; set; }
        public string Identifier { get; set; }
    }
    public enum EventType
    {
        HTML = 1,
        Text = 2,
        Image = 3
    }
}
