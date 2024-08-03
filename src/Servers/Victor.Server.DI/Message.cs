namespace Victor.Server.DI;
    
    public enum MessageType
    {
        DETECT
    }
    public record Message
    {
        public MessageType Type { get; set; }
    }

