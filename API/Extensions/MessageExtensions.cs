using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;

namespace API.Extensions;

public static class MessageExtensions
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            Content = message.Content,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientId = message.Recipient.Id,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            RecipientImageUrl = message.Recipient.ImageUrl,
            SenderImageUrl = message.Sender.ImageUrl
        };
    }

    // Func<input-type, output-type>
    // THIS IS NOT A PROJECTION - we need to access it via a static method call
    public static Expression<Func<Message, MessageDto>> ToDtoProjection()
    {
        return message => new MessageDto
        {
             Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            Content = message.Content,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientId = message.Recipient.Id,
            DateRead = message.DateRead,
            MessageSent = message.MessageSent,
            RecipientImageUrl = message.Recipient.ImageUrl,
            SenderImageUrl = message.Sender.ImageUrl
        };
    }
}
