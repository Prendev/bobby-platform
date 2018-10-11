using System.Configuration;
using System.Threading.Tasks;
using AegisImplicitMail;

namespace QvaDev.Common.Services
{
	public interface IEmailService
	{
		void Send(string subject, string body);
	}

	public class EmailService : IEmailService
	{
		private readonly object _syncRoot = new object();

		public void Send(string subject, string body)
		{
			Task.Run(() =>
			{
				if (!bool.TryParse(ConfigurationManager.AppSettings["EmailService.IsEnabled"], out bool isEnabled) ||
				    !isEnabled) return;

				lock (_syncRoot)
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

						mailer.Send(mail);
					}
				}
			});
		}
	}
}
