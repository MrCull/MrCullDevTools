using Moq;
using MrCullDevTools.Areas.Admin.Controllers;
using MrCullDevTools.Models.Users;
using MrCullDevTools.Tests.Utilities;

namespace MrCullDevTools.Tests.Controllers.Admin.Home
{
	public abstract class ControllerTestBase : TestBase
	{
		protected User User { get; private set; }
		protected HomeController Controller { get; set; }

		public override void Setup()
		{
			base.Setup();

			User = Generator.SetupUser(x =>
				{
					x.Username = "admin";
					x.IsAdmin = true;
				});

			Controller = new HomeController(Db, Cache, Metrics);
			ControllerUtilities.SetupControllerContext(Controller, User);
		}
	}
}
