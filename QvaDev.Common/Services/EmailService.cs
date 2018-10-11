using System.Configuration;
using AegisImplicitMail;

namespace QvaDev.Common.Services
{
	public interface IEmailService
	{
		void Send(string subject, string body);
	}

	public class EmailService : IEmailService
	{
		public void Send(string subject, string body)
		{
			var host = ConfigurationManager.AppSettings["EmailService.Host"];
			var user = ConfigurationManager.AppSettings["EmailService.User"];
			var password = ConfigurationManager.AppSettings["EmailService.Password"];
			var to = ConfigurationManager.AppSettings["EmailService.To"];

			using (var mailer = new MimeMailer(host))
			{
				mailer.User = user;
				mailer.Password = password;
				mailer.SslType = SslMode.Ssl;
				mailer.AuthenticationMode = AuthenticationType.Base64;

				var mail = new MimeMailMessage();
				mail.From = new MimeMailAddress(mailer.User);
				mail.To.Add(to);
				mail.Subject = subject;
				mail.Body = body;

				mailer.SendMailAsync(mail);
			}
		}
	}
}
