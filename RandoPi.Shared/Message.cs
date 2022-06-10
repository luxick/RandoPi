namespace RandoPi.Shared;

[Serializable]
public class Message
{
    public Guid MsgId { get; set; } = Guid.NewGuid();

    public MessageType MessageType { get; set; } = MessageType.None;

    public string Text { get; set; } = "";
}