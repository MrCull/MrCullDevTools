using System.Configuration;
using ActionMailer.Net.Mvc;
using MrCullDevTools.ViewModels.Mail;

namespace MrCullDevTools.Controllers
{
	public interface IMailController
	{
		EmailResult Welcome(Welcome model);
		EmailResult ForgotPassword(ForgotPassword model);
	}

	public class MailController : MailerBase, IMailController
	{
		public EmailResult Welcome(Welcome model)
		{
			SetToAndFromValues(model);

			Subject = "Welcome to MrCullDevTools";
			return Email("Welcome", model);
		}
		public EmailResult ForgotPassword(ForgotPassword model)
		{
			SetToAndFromValues(model);

			Subject = "[MrCullDevTools] Forgot password";
			return Email("ForgotPassword", model);
		}

		private void SetToAndFromValues(EmailBase model)
		{
			if (!string.IsNullOrEmpty(model.To))
			{
				To.Add(model.To);
			}
			From = model.From ?? ConfigurationManager.AppSettings["Email:Support"];
			model.SiteTitle = ConfigurationManager.AppSettings["Site:Title"];
		}
	}
}