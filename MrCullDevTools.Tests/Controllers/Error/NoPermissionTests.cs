using System.Web.Mvc;
using MrCullDevTools.Controllers;
using MrCullDevTools.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace MrCullDevTools.Tests.Controllers.Error
{
	public class NoPermissionTests : TestBase
	{
		[Test]
		public void GivenRequest_ReturnsNoPermissionPageView()
		{
			var controller = new ErrorController(Db, Cache, Metrics);
			ControllerUtilities.SetupControllerContext(controller);

			var result = controller.NoPermission() as ViewResult;
			result.Should().Not.Be.Null();
			result.ViewName.Should().Equal("");
		}
	}
}
