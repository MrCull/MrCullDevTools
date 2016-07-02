using ServiceStack.Logging;
using ServiceStack.Logging.Log4Net;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MrCullDevTools.LoggingConfig), "PreStart")]

namespace MrCullDevTools 
{
	public static class LoggingConfig
	{
		public static void PreStart() 
		{
			LogManager.LogFactory = new Log4NetFactory(true);
		}
	}
}
