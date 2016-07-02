using MrCullDevTools.Tests.Extensions;
using MrCullDevTools.Controllers;
using NUnit.Framework;

namespace MrCullDevTools.Tests.Routing
{
	public class HomeTests : RouteTestBase
	{
		[Test]
		public void DefaultRoute()
		{
			"~/".ShouldMapTo<HomeController>(x => x.Index());
		}
	}
}
