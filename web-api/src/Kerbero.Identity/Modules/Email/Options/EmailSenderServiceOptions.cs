namespace Kerbero.Identity.Modules.Email.Options;

public class EmailSenderServiceOptions
{
	public string? SendGridKey { get; set; }
	public string? FromEmail { get; set; }
	public string? SenderName { get; set; }
}
