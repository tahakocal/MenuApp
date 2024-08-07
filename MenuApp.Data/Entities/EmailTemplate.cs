using MenuApp.Data.Entities.Base;
using Microsoft.AspNetCore.Http;

namespace MenuApp.Data.Entities;

public class EmailTemplate : IEntity
{
    public string? To { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? CC { get; set; }
    public string? AttachFromFilename { get; set; }
}

public class MailData
{
    public MailData(List<string> to, string subject, string? body = null, string? from = null,
        string? displayName = null, string? replyTo = null, string? replyToName = null, List<string>? bcc = null,
        List<string>? cc = null)
    {
        // Receiver
        To = to;
        Bcc = bcc ?? new List<string>();
        Cc = cc ?? new List<string>();

        // Sender
        From = from;
        DisplayName = displayName;
        ReplyTo = replyTo;
        ReplyToName = replyToName;

        // Content
        Subject = subject;
        Body = body;
    }

    // Receiver
    public List<string> To { get; }
    public List<string> Bcc { get; }
    public List<string> Cc { get; }

    // Sender
    public string? From { get; }
    public string? DisplayName { get; }
    public string? ReplyTo { get; }
    public string? ReplyToName { get; }

    // Content
    public string Subject { get; }
    public string? Body { get; }
}

public class MailDataWithAttachments
{
    // Receiver
    public List<string>? To { get; set; }
    public List<string>? Bcc { get; set; }
    public List<string>? Cc { get; set; }

    // Sender
    public string? From { get; set; }
    public string? DisplayName { get; set; }
    public string? ReplyTo { get; set; }
    public string? ReplyToName { get; set; }

    // Content
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public IFormFileCollection? Attachments { get; set; }
}