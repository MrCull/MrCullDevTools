﻿using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using CacheStack;
using CacheStack.DonutCaching;
using Dapper;
using MrCullDevTools.Infrastructure;
using MrCullDevTools.Infrastructure.Attributes;
using MrCullDevTools.Infrastructure.Extensions;
using MrCullDevTools.Models.Users;
using MrCullDevTools.Services;
using MrCullDevTools.ViewModels.Account;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using ServiceStack.CacheAccess;
using ServiceStack.Text;
using Spruce;

namespace MrCullDevTools.Controllers
{
    public class AccountController : BaseController
    {
		private readonly IMailController _mailController;
	    private readonly IUserService _userService;
	    private readonly IUserAuthenticationService _authenticationService;

		public AccountController(IDbConnection db, ICacheClient cache, IMetricTracker metrics, IMailController mailController, IUserService userService, IUserAuthenticationService authenticationService) : base (db, cache, metrics)
		{
			_mailController = mailController;
			_userService = userService;
			_authenticationService = authenticationService;
		}

		[HttpGet, Route("account", Name = "Account_Index")]
		[DonutOutputCache]
		public ActionResult Index()
		{
			return Redirect(Url.Home().Index());
		}

		[HttpGet, Route("account/login", Name = "Account_Login")]
		[DonutOutputCache(VaryByParam = "returnUrl", VaryByCustom = VaryByCustom.UserIsAuthenticated)]
		public ActionResult Login(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			var model = new Login
				{
					ReturnUrl = returnUrl,
					RememberMe = true
				};
			return View(model);
		}

		[HttpPost, Route("account/login")]
		public ActionResult Login(Login model)
		{
			if (ModelState.IsValid)
			{
				var user = Db.Query<User>("select top 1 * from [{0}] where (Username=@Username OR Email=@Username) and Password=@Password and IsDeleted=0".Fmt(Db.GetTableName<User>()), new
					{
						model.Username, 
						Password = model.Password.ToSHAHash()
					}).SingleOrDefault();
				if (user != null)
				{
					_authenticationService.SetLoginCookie(user, model.RememberMe);
					Metrics.Increment(Metric.Users_SuccessfulLogin);

					if (Url.IsLocalUrl(model.ReturnUrl))
						return Redirect(model.ReturnUrl);
					return RedirectToAction("Index", "Home");
				}
				ModelState.AddModelErrorFor<Login>(x => x.Username, string.Format("The user name or password provided is incorrect. Did you <a href='{0}'>forget your password?</a>", Url.Account().ForgotPassword()));
			}
			Metrics.Increment(Metric.Users_FailedLogin);

			// If we got this far, something failed, redisplay form
			model.Password = null; //clear the password so they have to re-enter it
			return View(model);
		}

		[Restricted]
		[HttpGet, Route("account/logout", Name = "Account_Logout")]
		public ActionResult Logout(string returnUrl)
		{
			Metrics.Increment(Metric.Users_Logout);
			_authenticationService.Logout();
			if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		#region Register

		[HttpGet, Route("account/register", Name = "Account_Register")]
		[DonutOutputCache(VaryByParam = "returnUrl", VaryByCustom = VaryByCustom.UserIsAuthenticated)]
		public ActionResult Register(string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View(new Register { ReturnUrl = returnUrl });
		}

		[HttpPost, Route("account/register")]
		public ActionResult Register(Register model)
		{
			if (ModelState.IsValid)
			{
				if (_authenticationService.ReservedUsernames.Any(x => model.Username.Equals(x, StringComparison.OrdinalIgnoreCase)))
				{
					ModelState.AddModelErrorFor<Register>(x => x.Username, "Username is unavailable");
				}
				else
				{
					var user = Db.Query<User>("select top 1 * from [{0}] where IsDeleted=0 AND (Username=@Username OR Email=@Email)".Fmt(Db.GetTableName<User>()), new
						{
							model.Username,
							model.Email
						}
					).SingleOrDefault();

					if (user != null)
					{
						if (user.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase))
						{
							ModelState.AddModelErrorFor<Register>(x => x.Username, "Username is already in use");
						}
						if (user.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase))
						{
							ModelState.AddModelErrorFor<Register>(x => x.Email, "A user with that email exists");
						}
					}					
				}
			}

			if (ModelState.IsValid)
			{
				var newUser = Mapper.Map<User>(model);
				newUser.Password = model.Password.ToSHAHash();

				_userService.Save(newUser);
				Metrics.Increment(Metric.Users_Register);

				_mailController.Welcome(new ViewModels.Mail.Welcome
					{
						Username = newUser.Username,
						To = newUser.Email
					}).Deliver();

				// Auto login the user
				_authenticationService.SetLoginCookie(newUser, false);

				NotifySuccess("Your account has been created and you have been logged in. Enjoy!");
				return Redirect(Url.Home().Index());
			}

			// If we got this far, something failed, redisplay form
			model.Password = null; //clear the password so they have to re-enter it
			return View(model);
		}

		[HttpPost, Route("account/validate-username", Name = "Account_ValidateUsername")]
		[DonutOutputCache(VaryByParam = "username")]
		public JsonResult ValidateUsername(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				return Json(false);

			if (_authenticationService.ReservedUsernames.Any(x => username.Equals(x, StringComparison.OrdinalIgnoreCase)))
				return Json(false);

			var isValid = Db.Query<int>("select count(*) from [{0}] where Username=@Username".Fmt(Db.GetTableName<User>()), new
				{
					username
				}
			).Single() == 0;

			return Json(isValid);
		}
		#endregion

		#region Forgot Password
		[HttpGet, Route("account/forgot-password", Name = "Account_ForgotPassword")]
		[DonutOutputCache]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost, Route("account/forgot-password")]
		public ActionResult ForgotPassword(ForgotPassword model)
		{
			if (ModelState.IsValid)
			{
				//get user by email address
				var user = Db.SingleOrDefault<User>(new { model.Email, IsDeleted = false });
				
				//if no matching user, error
				if (user == null)
				{
					ModelState.AddModelErrorFor<ForgotPassword>(x => x.Email, "A user could not be found with that email address");
					return View(model);
				}

				// Create token and send email
				var token = new PasswordRetrieval(user, Guid.NewGuid());
				Db.Save(token);
				Metrics.Increment(Metric.Users_SendPasswordResetEmail);

				_mailController.ForgotPassword(new ViewModels.Mail.ForgotPassword
					{
						To = user.Email,
						Token = token.Token
					}).Deliver();
				
				return View("ForgotPasswordConfirmation");

			}
			return View(model);
		}

		[HttpGet, Route("account/reset-password/{token}", Name = "Account_ResetPassword")]
		public ActionResult ResetPassword(string token)
		{
			if (User.Identity.IsAuthenticated)
			{
				NotifyInfo("You are already logged in. Log out and try again.");
				return Redirect(Url.Home().Index());
			}
			Guid guidToken;
			if (!Guid.TryParse(token, out guidToken))
			{
				NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
				return Redirect(Url.Account().ForgotPassword());
			}

			var model = new ResetPassword
				{
					Token = guidToken, 
					Data = Db.SingleOrDefault<PasswordRetrieval>(new { Token = guidToken })
				};

			if (model.Data == null)
			{
				NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
				return Redirect(Url.Account().ForgotPassword());
			}

			return View(model);
		}

		[HttpPost, Route("account/reset-password/{token}")]
		public ActionResult ResetPassword(ResetPassword model)
		{
			if (User.Identity.IsAuthenticated)
			{
				NotifyInfo("You are already logged in. Log out and try again.");
				return Redirect(Url.Home().Index());
			}
			if (ModelState.IsValid)
			{
				model.Data = Db.SingleOrDefault<PasswordRetrieval>(new { model.Token });

				if (model.Data == null)
				{
					NotifyWarning("Sorry! We couldn't verify that this user requested a password reset. Please try resetting again.");
					return Redirect(Url.Account().ForgotPassword());
				}

				var user = Db.Query<User>("delete from [{0}] where Id=@resetId;update [{1}] set Password=@Password, ModifiedOn=GetUtcDate() where Id=@UserId;select * from [{1}] where Id=@UserId"
					.Fmt(
						Db.GetTableName<PasswordRetrieval>(),
						Db.GetTableName<User>()
					), new
						{
							ResetId = model.Data.Id,
							Password = model.Password.ToSHAHash(),
							model.Data.UserId
						}).SingleOrDefault();
				Cache.Trigger(TriggerFor.Id<User>(user.Id));
				_authenticationService.SetLoginCookie(user, true);

				Metrics.Increment(Metric.Users_ResetPassword);
				//show confirmation
				return View("ResetPasswordConfirmation");
			}
			return View(model);
		}
		#endregion
	}
}
