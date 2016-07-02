﻿using System.Web;

namespace MrCullDevTools.Tests.Utilities
{
	public static class RequestExtensions
	{
		public static void SetAsAjaxRequest(this HttpRequestBase request)
		{
			request.Headers.Add("X-Requested-With", "XMLHttpRequest");
		}
	}
}
