public class ChatMessage{
    public string Id { get; set; } = Guid.NewGuid().ToString(); 
    public string? SenderId { get; set; }
    public string? ReceiverId { get; set; }
    public string? Message { get; set; }
    public DateTime CreaetedAt { get; set; }
    public bool IsSeen { get; set; } = false;
}