using System;

namespace API.Entities;

public class Message
{

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime? MessageSent { get; set; } = DateTime.UtcNow;
    public required bool SenderDeleted { get; set; }
    public required bool RecipientDeleted { get; set; }

    // nav properties
    public required string SenderId { get; set; }
    public Member Sender { get; set; } = null!;
    public required string RecipientId { get; set; }
    public Member Recipient { get; set; } = null!;

}
