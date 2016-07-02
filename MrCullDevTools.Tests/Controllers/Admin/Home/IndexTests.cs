using System.Data;
using System.Web.Mvc;
using MrCullDevTools.Areas.Admin.ViewModels.Home;
using MrCullDevTools.Tests.Utilities;
using NUnit.Framework;
using Should.Fluent;

namespace MrCullDevTools.Tests.Controllers.Admin.Home
{
	public class IndexTests : ControllerTestBase
	{
		[Test]
		public void ActionRequiresAuthorization()
		{
			Controller.ActionShouldBeDecoratedWithRestricted(x => x.Index(), requireAdmin:true);
		}

		[Test]
		public void GivenGetRequest_ReturnsIndexView()
		{
			var result = Controller.Index() as ViewResult;
			result.Should().Not.Be.Null();
			var viewModel = result.Model as Index;
			viewModel.Should().Not.Be.Null();
		}
	}
}
