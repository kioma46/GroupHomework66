namespace GroupHomework66.Models;

public class Message
{
    public int Id { get; set; }
    public string MessageText { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public int SenderUserId { get; set; }
    public User? User { get; set; }
    
    public int ReceiverUserId { get; set; }
    public User? ReceiverUser { get; set; }
}