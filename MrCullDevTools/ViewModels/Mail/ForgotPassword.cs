using System;

namespace MrCullDevTools.ViewModels.Mail
{
	public class ForgotPassword : EmailBase
	{
		public Guid Token { get; set; }
	}
}