using ServiceStack.Text;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MrCullDevTools.JsonConfig), "PreStart")]

namespace MrCullDevTools 
{
	public static class JsonConfig
	{
		public static void PreStart() 
		{
			JsConfig.EmitCamelCaseNames = true;
			JsConfig.AlwaysUseUtc = true;
			JsConfig.DateHandler = JsonDateHandler.ISO8601;
			JsConfig.ExcludeTypeInfo = true;
		}
	}
}
