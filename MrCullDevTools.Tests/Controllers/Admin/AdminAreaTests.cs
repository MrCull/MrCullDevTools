using System;
using MrCullDevTools.Areas.Admin;
using NUnit.Framework;

namespace MrCullDevTools.Tests.Controllers.Admin
{
	[SetUpFixture]
	public class AdminAreaTests
	{
		[SetUp]
		public void Setup()
		{
			AdminAreaRegistration.CreateMappings();
		}
	}
}
