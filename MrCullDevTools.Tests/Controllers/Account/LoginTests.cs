﻿using System.Web.Mvc;
using MrCullDevTools.Infrastructure.Extensions;
using FizzWare.NBuilder.Generators;
using Moq;
using MrCullDevTools.Infrastructure;
using MrCullDevTools.Models.Users;
using MrCullDevTools.Tests.Utilities;
using MrCullDevTools.ViewModels.Account;
using NUnit.Framework;
using Should.Fluent;

namespace MrCullDevTools.Tests.Controllers.Account
{
	public class LoginTests : ControllerTestBase
	{
		[Test]
		public void GivenUnauthenticatedRequest_ReturnsLoginView()
		{
			var result = Controller.Login("") as ViewResult;
			result.Should().Not.Be.Null();
		}
		[Test]
		public void GivenAuthenticatedRequest_ReturnsHomePage()
		{
			ControllerUtilities.SetupControllerContext(Controller, User);

			var result = Controller.Login("") as RedirectToRouteResult;
			result.Should().Not.Be.Null();
		}

		[Test]
		public void GivenValidUsernameAndPassword_ReturnsHomePage()
		{
			var model = new Login
							{
								Username = User.Username,
								Password = "password"
							};

			var result = Controller.Login(model) as RedirectToRouteResult;
			result.Should().Not.Be.Null();
			result.RouteValues["controller"].Should().Equal("Home");
			result.RouteValues["action"].Should().Equal("Index");
			AuthenticationService.Verify(x => x.SetLoginCookie(It.Is<User>(u => u.Username == User.Username), model.RememberMe), Times.Once());
		}

		[Test]
		public void GivenInvalidUsernameAndPassword_ReturnsLogin()
		{
			var model = new Login
			{
				Username = User.Username,
				Password = GetRandom.String(20)
			};

			var result = Controller.Login(model) as ViewResult;
			result.Should().Not.Be.Null();
			AuthenticationService.Verify(x => x.SetLoginCookie(It.Is<User>(u => u.Username == User.Username), model.RememberMe), Times.Never());

			result.Model.Should().Be.OfType<Login>();
			var typedModel = result.Model as Login;
			typedModel.Username.Should().Equal(model.Username);
			typedModel.Password.Should().Be.NullOrEmpty();
			result.ViewName.Should().Equal("");

			var modelState = result.ViewData.ModelState;
			modelState.ContainsKey("Username").Should().Be.True();
		}

		[Test]
		public void GivenLocalReturnUrl_ReturnsRedirectToReturnUrl()
		{
			var model = new Login
			{
				Username = User.Username,
				Password = "password",
				ReturnUrl = "/home/index"
			};

			var result = Controller.Login(model) as RedirectResult;
			result.Should().Not.Be.Null();
			AuthenticationService.Verify(x => x.SetLoginCookie(It.Is<User>(u => u.Username == User.Username), model.RememberMe), Times.Once());
			result.Url.Should().Equal(model.ReturnUrl);
		}

		[Test]
		public void GivenExternalReturnUrl_ReturnsHomePage()
		{
			var model = new Login
			{
				Username = User.Username,
				Password = "password",
				ReturnUrl = "http://google.com"
			};

			var result = Controller.Login(model) as RedirectToRouteResult;
			result.Should().Not.Be.Null();
			result.RouteValues["controller"].Should().Equal("Home");
			result.RouteValues["action"].Should().Equal("Index");
			AuthenticationService.Verify(x => x.SetLoginCookie(It.Is<User>(u => u.Username == User.Username), model.RememberMe), Times.Once());
		}
	}
}
