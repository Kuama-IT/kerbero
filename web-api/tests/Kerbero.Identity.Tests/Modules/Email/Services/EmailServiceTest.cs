using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kerbero.Identity.Modules.Email.Options;
using Kerbero.Identity.Modules.Email.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Email.Services;

public class EmailServiceTest
{
	private readonly EmailSenderService _emailSenderService;
	private readonly Mock<ISendGridClient> _sendGridClient;

	public EmailServiceTest()
	{
		_sendGridClient = new Mock<ISendGridClient>();
		var logger = new Mock<ILogger<EmailSenderService>>();
		var options = new Mock<IOptions<EmailSenderServiceOptions>>();
		options.Setup(optionsMock => optionsMock.Value)
			.Returns(new EmailSenderServiceOptions()
			{
				FromEmail = "kerbero@kerbero.com",
				FromPassword = "kerbero",
				SendGridKey = "KEY"
			});
		_emailSenderService = new EmailSenderService(_sendGridClient.Object, options.Object, logger.Object);
	}

	[Fact]
	public async Task SendEmailAsyncTest()
	{
		_sendGridClient.Setup(client =>
				client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((SendGridMessage _, CancellationToken _) =>
				new Response(HttpStatusCode.Accepted, new StringContent("something"), null));
		
		await _emailSenderService.SendEmailAsSystem("email@email.com", "Email confirmation", "Please, confirm you're email");
		var expectMsg = new SendGridMessage()
		{
			From = new EmailAddress("kerbero@kerbero.com", "kerbero"),
			Subject = "Email confirmation",
			PlainTextContent = "Please, confirm you're email",
			HtmlContent = "Please, confirm you're email"
		};
		expectMsg.AddTo(new EmailAddress("email@email.com"));
		expectMsg.SetClickTracking(false, false);
		_sendGridClient.Verify(client =>
			client.SendEmailAsync(It.Is<SendGridMessage>(msg => msg.From == expectMsg.From &&
			                                                    msg.Subject == expectMsg.Subject &&
			                                                    msg.HtmlContent == expectMsg.HtmlContent &&
			                                                    msg.Personalizations[0].Tos[0].Email == 
																	expectMsg.Personalizations[0].Tos[0].Email),
				It.IsAny<CancellationToken>()));
	}
}
