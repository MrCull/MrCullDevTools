using System.Web.Mvc;
using NUnit.Framework;
using Should.Fluent;

namespace MrCullDevTools.Tests.Controllers.Account
{
	public class IndexTests : ControllerTestBase
	{
		[Test]
		public void GivenGetRequest_ReturnsRedirectToHomepage()
		{
			var result = Controller.Index() as RedirectResult;
			result.Should().Not.Be.Null();
			result.Url.Should().Equal("/");
		}
	}
}
