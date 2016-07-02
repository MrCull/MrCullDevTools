using MrCullDevTools.Tests.Extensions;
using MrCullDevTools.Areas.Admin.Controllers;
using NUnit.Framework;

namespace MrCullDevTools.Tests.Routing.Admin
{
	public class HomeTests : RouteTestBase
	{
		[Test]
		public void DefaultRoute()
		{
			"~/admin".ShouldMapTo<HomeController>(x => x.Index());
		}
	}
}
