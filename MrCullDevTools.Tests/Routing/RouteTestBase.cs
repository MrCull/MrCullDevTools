using System;
using System.Web.Routing;

namespace MrCullDevTools.Tests.Routing
{
	public abstract class RouteTestBase : IDisposable
	{
		protected RouteTestBase()
		{
			if (RouteTable.Routes == null || RouteTable.Routes.Count == 0)
				MvcAttributeRoutesHack.MapAttributeRoutes();
		}

		public void Dispose()
		{
			RouteTable.Routes.Clear();
		}
	}
}
