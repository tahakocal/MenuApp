using MenuApp.Data.Entities.Base;

namespace MenuApp.Data.Entities.System;

public class MailSetting : IEntity
{
    public string Name { get; set; }
    public string Domain { get; set; }
    public string Host { get; set; }
    public string InfoMail { get; set; }
    public string? BccMail { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string SenderMail { get; set; }
    public bool UseSSL { get; set; }
    public bool UseStartTls { get; set; }
}