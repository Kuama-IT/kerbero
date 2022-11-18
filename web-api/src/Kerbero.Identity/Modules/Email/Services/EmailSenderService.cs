using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Kerbero.Identity.Modules.Email.Services;

public class EmailSenderService: IEmailSenderService
{
	private readonly ISendGridClient _sendGridClient;
	private readonly IConfiguration _configuration;
	private readonly ILogger _logger;

	public EmailSenderService(ISendGridClient sendGridClient, IConfiguration configuration,
		ILogger<EmailSenderService> logger)
	{
		_sendGridClient = sendGridClient;
		_configuration = configuration;
		_logger = logger;
	}

	public async Task SendEmailAsSystem(string toEmail, string subject, string message)
	{
		var fromEmail = _configuration["SENDGRID_FROM_EMAIL"];
		var senderName = _configuration["SENDGRID_SENDER_NAME"];
		if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(senderName))
		{
			throw new Exception("From email or password not specified");
		}
		
		var msg = new SendGridMessage()
		{
			From = new EmailAddress(fromEmail, senderName),
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
