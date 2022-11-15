using Kerbero.Identity.Modules.Email.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Kerbero.Identity.Modules.Email.Services;

public class EmailSenderService: IEmailSenderService
{
	private readonly ISendGridClient _sendGridClient;
	private readonly EmailSenderServiceOptions _options;
	private readonly ILogger _logger;

	public EmailSenderService(ISendGridClient sendGridClient, IOptions<EmailSenderServiceOptions> options,
		ILogger<EmailSenderService> logger)
	{
		_sendGridClient = sendGridClient;
		_options = options.Value;
		_logger = logger;
	}

	public async Task SendEmailAsSystem(string toEmail, string subject, string message)
	{
		if (string.IsNullOrWhiteSpace(_options.FromEmail) || string.IsNullOrWhiteSpace(_options.SenderName))
		{
			throw new Exception("From email or password not specified");
		}
		
		var msg = new SendGridMessage()
		{
			From = new EmailAddress(_options.FromEmail, _options.SenderName),
			Subject = subject,
			HtmlContent = message
		};
		msg.AddTo(new EmailAddress(toEmail));

		// Disable click tracking.
		// See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
		msg.SetClickTracking(false, false);
		var response = await _sendGridClient.SendEmailAsync(msg);
		if (response.IsSuccessStatusCode)
			_logger.LogInformation("Email to {ToEmail} queued successfully!", toEmail);
		else
			_logger.LogInformation("Failure Email to {ToEmail}", toEmail);
	}
}
