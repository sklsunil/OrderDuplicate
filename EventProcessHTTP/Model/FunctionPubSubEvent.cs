namespace EventProcessHTTP.Model
{
    public class PubSubEvent
    {
        public EventType EventType { get; set; }
        public string Content { get; set; }
        public string Identifier { get; set; }
        public ControlType ControlType { get; set; } = ControlType.Default;
    }

    public enum EventType
    {
        HTML = 1,
        Text = 2,
        Image = 3,
        Custom = 4 // Added Custom event type
    }

    public enum ControlType
    {
        Input,
        TextArea,
        Default
    }

    public class FunctionPubSubEvent : PubSubEvent
    {
        public int SesstionId { get; set; }
    }
}
