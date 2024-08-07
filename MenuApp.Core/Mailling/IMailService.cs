using MenuApp.Data.Entities;

namespace MenuApp.Core.Mailling;

public interface IMailService
{
    Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    Task<bool> SendWithAttachmentsAsync(MailDataWithAttachments mailData, CancellationToken ct);
    Task<string> GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel);
}