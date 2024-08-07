using System.Reflection;
using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using MenuApp.Data;
using MenuApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using RazorLight;

namespace MenuApp.Core.Mailling;

public class MailService : IMailService
{
    private readonly ApplicationDbContext _dbContext;

    public MailService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> SendWithAttachmentsAsync(MailDataWithAttachments mailData,
        CancellationToken ct = default)
    {
        try
        {
            var mailSetting = await _dbContext.MailSetting.FirstOrDefaultAsync();

            // Initialize a new instance of the MimeKit.MimeMessage class
            var mail = new MimeMessage();

            #region Sender / Receiver

            // Sender
            mail.From.Add(new MailboxAddress(mailSetting.Name, mailData.From ?? mailSetting.SenderMail));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? mailSetting.Name,
                mailData.From ?? mailSetting.SenderMail);

            // Receiver
            foreach (var mailAddress in mailData.To)
                mail.To.Add(MailboxAddress.Parse(mailAddress));

            // Set Reply to if specified in mail data
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

            // BCC
            // Check if a BCC was supplied in the request
            if (mailData.Bcc != null)
                // Get only addresses where value is not null or with whitespace. x = value of address
                foreach (var mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            // CC
            // Check if a CC address was supplied in the request
            if (mailData.Cc != null)
                foreach (var mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            #endregion

            #region Content

            // Add Content to Mime Message
            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;

            // Check if we got any attachments and add the to the builder for our message
            if (mailData.Attachments != null)
            {
                byte[] attachmentFileByteArray;
                foreach (var attachment in mailData.Attachments)
                    if (attachment.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            attachment.CopyTo(memoryStream);
                            attachmentFileByteArray = memoryStream.ToArray();
                        }

                        body.Attachments.Add(attachment.FileName, attachmentFileByteArray,
                            ContentType.Parse(attachment.ContentType));
                    }
            }

            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            #endregion

            #region Send Mail

            using var smtp = new SmtpClient();

            if (mailSetting.UseSSL)
                await smtp.ConnectAsync(mailSetting.Host, mailSetting.Port, SecureSocketOptions.SslOnConnect, ct);
            else if (mailSetting.UseStartTls)
                await smtp.ConnectAsync(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls, ct);

            await smtp.AuthenticateAsync(mailSetting.SenderMail, mailSetting.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            return true;

            #endregion
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendAsync(MailData mailData, CancellationToken ct = default)
    {
        try
        {
            var mailSetting = await _dbContext.MailSetting.FirstOrDefaultAsync();

            // Initialize a new instance of the MimeKit.MimeMessage class
            var mail = new MimeMessage();

            #region Sender / Receiver

            // Sender
            mail.From.Add(new MailboxAddress(mailSetting.Name, mailData.From ?? mailSetting.SenderMail));
            mail.Sender = new MailboxAddress(mailData.DisplayName ?? mailSetting.Name,
                mailData.From ?? mailSetting.SenderMail);

            // Receiver
            foreach (var mailAddress in mailData.To)
                mail.To.Add(MailboxAddress.Parse(mailAddress));

            // Set Reply to if specified in mail data
            if (!string.IsNullOrEmpty(mailData.ReplyTo))
                mail.ReplyTo.Add(new MailboxAddress(mailData.ReplyToName, mailData.ReplyTo));

            // BCC
            // Check if a BCC was supplied in the request
            if (mailData.Bcc != null)
                // Get only addresses where value is not null or with whitespace. x = value of address
                foreach (var mailAddress in mailData.Bcc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Bcc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            // CC
            // Check if a CC address was supplied in the request
            if (mailData.Cc != null)
                foreach (var mailAddress in mailData.Cc.Where(x => !string.IsNullOrWhiteSpace(x)))
                    mail.Cc.Add(MailboxAddress.Parse(mailAddress.Trim()));

            #endregion

            #region Content

            // Add Content to Mime Message
            var body = new BodyBuilder();
            mail.Subject = mailData.Subject;
            body.HtmlBody = mailData.Body;
            mail.Body = body.ToMessageBody();

            #endregion

            #region Send Mail

            using var smtp = new SmtpClient();

            if (mailSetting.UseSSL)
                await smtp.ConnectAsync(mailSetting.Host, mailSetting.Port, SecureSocketOptions.SslOnConnect, ct);
            else if (mailSetting.UseStartTls)
                await smtp.ConnectAsync(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls, ct);

            await smtp.AuthenticateAsync(mailSetting.SenderMail, mailSetting.Password, ct);
            await smtp.SendAsync(mail, ct);
            await smtp.DisconnectAsync(true, ct);

            return true;

            #endregion
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<string> GetEmailTemplate<T>(string emailTemplate, T emailTemplateModel)
    {
        var mailTemplate = LoadTemplate(emailTemplate);

        // IRazorEngine razorEngine = new RazorEngine();
        // var modifiedMailTemplate = razorEngine.Compile(mailTemplate);

        var engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(Assembly.GetEntryAssembly())
            .Build();
        var result = await engine.CompileRenderStringAsync(
            emailTemplate,
            mailTemplate,
            emailTemplateModel
        );

        return result;
    }

    private static string LoadTemplate(string messageTemplate)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var templateDirectory = Path.Combine(baseDirectory, "Models", "Mail", "Templates");

        var filePath = Path.Combine(templateDirectory, $"{messageTemplate}", $"{messageTemplate}.cshtml");

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(fileStream, Encoding.Default);

        var mailTemplate = streamReader.ReadToEnd();
        streamReader.Close();

        return mailTemplate;
    }
}