using System;
using System.Web.Mvc;
using MrCullDevTools.ViewModels.Mail;

namespace MrCullDevTools.Infrastructure.Extensions
{
	public static class UrlHelperExtensions
	{
		public static UrlHelperAdminUrls Admin(this UrlHelper helper)
		{
			return new UrlHelperAdminUrls(helper);
		}

		#region Home

		public static HomeUrls Home(this UrlHelper helper)
		{
			return new HomeUrls(helper);
		}

		public class HomeUrls
		{
			public UrlHelper Url { get; set; }

			public HomeUrls(UrlHelper url)
			{
				Url = url;
			}

			public string Index()
			{
				return Url.RouteUrl("Home_Index");
			}

            public string ContIdGen()
            {
                return Url.RouteUrl("Home_ContIdGen");
            }

            public string RandomNumbers()
            {
                return Url.RouteUrl("Home_RandomNumbers");
            }

            public string SqlFormatter()
            {
                return Url.RouteUrl("Home_SqlFormatter");
            }

            public string JsonFormatter()
            {
                return Url.RouteUrl("Home_JsonFormatter");
            }

            public string ClientDetails()
            {
                return Url.RouteUrl("Home_ClientDetails");
            }
        }

		#endregion

		#region Account

		public static AccountUrls Account(this UrlHelper helper)
		{
			return new AccountUrls(helper);
		}

		public class AccountUrls
		{
			public UrlHelper Url { get; set; }

			public AccountUrls(UrlHelper url)
			{
				Url = url;
			}

			public string Index()
			{
				return Url.RouteUrl("Account_Index");
			}
			public string Login()
			{
				return Url.RouteUrl("Account_Login");
			}
			public string Login(string returnUrl)
			{
				return Url.RouteUrl("Account_Login", new { returnUrl });
			}
			public string Logout()
			{
				return Url.RouteUrl("Account_Logout");
			}

			public string Register()
			{
				return Url.RouteUrl("Account_Register");
			}
			public string ForgotPassword()
			{
				return Url.RouteUrl("Account_ForgotPassword");
			}
			public string ResetPassword(Guid token)
			{
				return Url.RouteUrl("Account_ResetPassword", new { token });
			}
			public string ValidateUsername()
			{
				return Url.RouteUrl("Account_ValidateUsername");
			}
		}

		#endregion
	}
}