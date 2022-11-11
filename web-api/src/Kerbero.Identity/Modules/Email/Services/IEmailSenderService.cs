namespace Kerbero.Identity.Modules.Email.Services;

public interface IEmailSenderService
{
	Task SendEmailAsSystem(string email, string subject, string htmlMessage);
}
