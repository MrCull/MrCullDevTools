using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Mvc;
using System.Xml.Linq;
using CacheStack.DonutCaching;
using MrCullDevTools.Infrastructure;
using MrCullDevTools.Infrastructure.Extensions;
using MrCullDevTools.ViewModels.Home;
using MrCullDevTools.ViewModels.Shared;
using MvcKickstart.Infrastructure;
using MvcKickstart.Infrastructure.Extensions;
using ServiceStack.CacheAccess;
using StackExchange.Profiling;
using System.Collections.Generic;
using System.Net.Http;
using System;
using System.Net.Http.Headers;
using System.IO.Compression;
using System.Collections;
using System.Net;
using MrCullDevTools.Controllers.Helper;
using Newtonsoft.Json;
using System.Web;

namespace MrCullDevTools.Controllers
{
	public class HomeController : BaseController
	{
        System.Random rnd;

		public HomeController(IDbConnection db, ICacheClient cache, IMetricTracker metrics) : base(db, cache, metrics)
		{
            rnd = new System.Random();
		}

		[HttpGet, Route("", Name = "Home_Index")]
		[DonutOutputCache]
		public ActionResult Index()
		{
			var model = new Index();
			return View(model);
		}

        [HttpGet, Route("home/ContIdGen", Name = "Home_ContIdGen")]
        [DonutOutputCache]
        public ActionResult ContIdGen()
        {
            var model = new ContIdGen();
            model.Prefix = "****";
            model.Quantity = "100";
            model.ContainerIDs = new System.Collections.ArrayList();
            return View(model);
        }

        [HttpPost, Route("home/ContIdGen")]
        public ActionResult ContIdGen(ContIdGen model)
        {
            string localPrefix;
            string prefixAndSerial;
            model.ContainerIDs = new System.Collections.ArrayList();

            for (int i=0;i<int.Parse(model.Quantity);i++)
            {
                if (model.Prefix == "****")
                {
                    switch (rnd.Next(1,6))
                    {
                        case 1: localPrefix = "EIMU"; break;
                        case 2: localPrefix = "MSKU"; break;
                        case 3: localPrefix = "TCLU"; break;
                        case 4: localPrefix = "CCLU"; break;
                        case 5: localPrefix = "HLCU"; break;
                        case 6: localPrefix = "EGHU"; break;
                        case 7: localPrefix = "MSCU"; break;
                        default: localPrefix =  "MACU"; break;
                    }
                }
                else
                {
                    localPrefix = model.Prefix;
                }

                prefixAndSerial = localPrefix + rnd.Next(1,999999).ToString().PadRight(6, '0');
                model.ContainerIDs.Add(AddCheckDigit(prefixAndSerial));
            }

            return View(model);
        }

        [HttpGet, Route("home/RandomNumbers", Name = "Home_RandomNumbers")]
        [DonutOutputCache]
        public ActionResult RandomNumbers()
        {
            var model = new RandomNumbers();
            model.Numbers = new System.Collections.ArrayList();
            model.Min = "1";
            model.Max = "99";
            model.Quantity = "10";

            return View(model);
        }

        [HttpPost, Route("home/RandomNumbers")]
        public async System.Threading.Tasks.Task<ActionResult> RandomNumbers(RandomNumbers model)
        {
            model.Numbers = new System.Collections.ArrayList();
            HttpClient client = new HttpClient();
            string baseApiAddress = ConfigurationManager.AppSettings["baseRandomApiAddress"];
            client.BaseAddress = new Uri(baseApiAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html,application/xhtml+xml,application/xml");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Charset", "ISO-8859-1");

            HttpResponseMessage response = await client.GetAsync(string.Format("/integers/?num={0}&min={1}&max={2}&col=1&base=10&format=plain&rnd=new", model.Quantity, model.Min, model.Max));

            using (var responseStream = await response.Content.ReadAsStreamAsync())
            using (var decompressedStream = new GZipStream(responseStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(decompressedStream))
            {
                if (response.IsSuccessStatusCode)
                {
                    var mydata = streamReader.ReadToEnd(); // ReadAsStringAsync<MyData>().Result;
                    foreach (var num in mydata.Split('\n'))
                    {
                        if (num != string.Empty)
                            model.Numbers.Add(num);
                    }
                }
                else
                {
                    model.ReasonPhrase = response.ReasonPhrase;
                }
            }

            return View(model);
        }


        [HttpGet, Route("home/SqlFormatter", Name = "Home_SqlFormatter")]
        [DonutOutputCache]
        public ActionResult SqlFormatter()
        {
            var model = new SqlFormatter();
            model.inputSQL = "select fieldA, fieldB from example where fieldA < fieldB";
            return View(model);
        }

        [HttpPost, Route("home/SqlFormatter")]
        public ActionResult SqlFormatter(SqlFormatter model)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.gudusoft.com/format.php");
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(string.Format("rqst_input_sql={0}", model.inputSQL));
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var json = streamReader.ReadToEnd();

                SqlFormatterDataContract deserializedSqlFormatter = JsonConvert.DeserializeObject<SqlFormatterDataContract>(json);
                ModelState.Clear();
                model.outputSQL = deserializedSqlFormatter.rspn_formatted_sql;
            }

            return View(model);
        }

        [HttpGet, Route("home/JsonFormatter", Name = "Home_JsonFormatter")]
        [DonutOutputCache]
        public ActionResult JsonFormatter()
        {
            var model = new JsonFormatter();
            model.inputJson = "{\"blah\":\"v\", \"blah2\":\"v2\"}";
            return View(model);
        }

        [HttpPost, Route("home/JsonFormatter")]
        public ActionResult JsonFormatter(JsonFormatter model)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(model.inputJson);
            model.outputJson = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            return View(model);
        }

        [HttpGet, Route("home/ClientDetails", Name = "Home_ClientDetails")]
        [DonutOutputCache]
        public ActionResult ClientDetails()
        {
            var model = new ClientDetails();

            model.userHostAddress = Request.UserHostAddress;
            model.os = GetUserPlatform(System.Web.HttpContext.Current.Request);
            model.browser = GetUserEnvironment(System.Web.HttpContext.Current.Request);
            //model.osVersion = GetMobileVersion(Request.UserAgent, Request.de);

            return View(model);
        }

        [HttpGet, Route("sitemap.xml")]
		public FileResult SiteMap()
		{
			var doc = new XDocument();
			XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
			var urlset = new XElement(ns + "urlset");

			urlset.Add(new XElement(ns + "url",
						new XElement(ns + "loc", Url.Absolute(Url.Home().Index())),
						new XElement(ns + "priority", "1.0")));

			// TODO: Add in additional urls as needed

			doc.Add(urlset);

			using (var ms = new MemoryStream())
			{
				doc.Save(ms);

				return File(ms.ToArray(), "text/xml");
			}
		}

		#region Partials

		[Route("__partial__Home_Profiler")]
		public ActionResult Profiler()
		{
			if (User.IsAdmin)
			{
				return Content(MiniProfiler.RenderIncludes().ToHtmlString());
			}
			return new EmptyResult();
		}

		[Route("__partial__Home_Notification")]
		public ActionResult Notification()
		{
			return PartialView("_Notification");
		}

		[Route("__partial__Home_Menu")]
		[DonutOutputCache(VaryByParam = "nav", VaryByCustom = VaryByCustom.User)]
		public ActionResult Menu(Navigation nav)
		{
			var model = new Menu
				{
					Nav = nav
				};
			return PartialView("_Menu", model);
		}

		[Route("__partial__Home_Analytics")]
		[DonutOutputCache]
		public ActionResult Analytics()
		{
			var model = new Analytics
				{
					Id = ConfigurationManager.AppSettings["Analytics:Id"],
					Domain = ConfigurationManager.AppSettings["Analytics:Domain"]
				};

			if (string.IsNullOrEmpty(model.Id))
				return new EmptyResult();

			if (string.IsNullOrEmpty(model.Domain))
				model.Domain = "auto";

			return PartialView("_Analytics", model);
		}

		[Route("__partial__Home_TagManager")]
		[DonutOutputCache]
		public ActionResult TagManager()
		{
			var model = new TagManager
				{
					Id = ConfigurationManager.AppSettings["Analytics:TagManagerId"],
				};

			if (string.IsNullOrEmpty(model.Id))
				return new EmptyResult();

			return PartialView("_TagManager", model);
		}


        private string AddCheckDigit(string equipmentNumber)
        {
            Dictionary<char, int> AlphabetCodes = new Dictionary<char, int>();
            List<int> PowerOfMultipliers = new List<int>();
            var step = 10;
            var total = 0;

            //populate dictionary...
            //...create a dictionary entry for all letters of the alphabet using their Ascii value to identify them.
            //if you subtract their ascii value by the value of the first alpha ascii character (in this case 65 for
            //uppercase 'A'), it will give you it's position in the alphabet, Add 10 to this and skip over all multiples
            //of 11 to give you ISO Owner Code numbers for each letter.
            for (int i = 65; i < 91; i++)
            {
                char c = (char)i;
                int pos = i - 65 + step;

                if (c == 'A' || c == 'K' || c == 'U')  //omit multiples of 11.
                    step += 1;

                AlphabetCodes.Add(c, pos);  //add to dictionary
            }

            //populate list...
            //create a list of 10, 2^x numbers for calculation.  List should contain 1, 2, 4, 8, 16, 32, 64 etc..
            for (int i = 0; i < 10; i++)
            {
                int result = (int)System.Math.Pow(2, i);  //power of 2 calculation.
                PowerOfMultipliers.Add(result);  //add to list.
            }

            for (int i = 0; i < 10; i++)  //loop through the first 10 characters (the 11th is the check digit!).
            {
                if (AlphabetCodes.ContainsKey(equipmentNumber[i]))  //if the current character is in the dictionary.
                    total += (AlphabetCodes[equipmentNumber[i]] * PowerOfMultipliers[i]);  //add it's value to the total.
                else
                {
                    int serialNumber = (int)equipmentNumber[i] - 48;  //it must be a number, so get the number from the char ascii value.
                    total += (serialNumber * PowerOfMultipliers[i]);  //and add it to the total.
                }
            }

            int checkDigit = (int)total % 11;  //this should give you the check digit

            //The check digit shouldn't equal 10 according to ISO best practice - BUT there are containers out there that do, so we'll
            //double check and set the check digit to 0...again according to ISO best practice.
            if (checkDigit == 10)
                checkDigit = 0;

            return equipmentNumber + checkDigit.ToString();
        }

        private string getWindowsOS(string userAgent)
        {

            if (userAgent.IndexOf("Windows NT 6.3") > 0)
            {
                return ("Windows 8.1");
            }
            else if (userAgent.IndexOf("Windows NT 6.2") > 0)
            {
                return ("Windows 8");
            }
            else if (userAgent.IndexOf("Windows NT 6.1") > 0)
            {
                return ("Windows 7");
            }
            else if (userAgent.IndexOf("Windows NT 6.0") > 0)
            {
                return ("Windows 6");
            }
            else if (userAgent.IndexOf("Windows NT 5.2") > 0)
            {
                return ("Windows Server 2003; Windows XP x64 Edition");
            }
            else if (userAgent.IndexOf("Windows NT 5.1") > 0)
            {
                return ("Windows XP");
            }
            else if (userAgent.IndexOf("Windows NT 5.01") > 0)
            {
                return ("Windows 2000, Service Pack 1 (SP1)");
            }
            else if (userAgent.IndexOf("Windows NT 5.0") > 0)
            {
                return ("Windows 2000");
            }
            else if (userAgent.IndexOf("Windows NT 4.0") > 0)
            {
                return ("Microsoft Windows NT 4.0");
            }
            else if (userAgent.IndexOf("Win 9x 4.90") > 0)
            {
                return ("Windows Millennium Edition (Windows Me)");
            }
            else if (userAgent.IndexOf("Windows 98") > 0)
            {
                return ("Windows 98");
            }
            else if (userAgent.IndexOf("Windows 95") > 0)
            {
                return ("Windows 95");
            }
            else if (userAgent.IndexOf("Windows CE") > 0)
            {
                return ("Windows CE");
            }
            else if (Request.UserAgent.IndexOf("Intel Mac OS X") > 0)
            {
                return ("Intel Mac OS X");
            }
            else
            {
                return ("Other");
            }
        }

        public String GetUserEnvironment(HttpRequest request)
        {
            var browser = request.Browser;
            var platform = GetUserPlatform(request);
            return string.Format("{0} {1} / {2}", browser.Browser, browser.Version, platform);
        }

        public String GetUserPlatform(HttpRequest request)
        {
            var ua = request.UserAgent;

            if (ua.Contains("Android"))
                return string.Format("Android {0}", GetMobileVersion(ua, "Android"));

            if (ua.Contains("iPad"))
                return string.Format("iPad OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("iPhone"))
                return string.Format("iPhone OS {0}", GetMobileVersion(ua, "OS"));

            if (ua.Contains("Linux") && ua.Contains("KFAPWI"))
                return "Kindle Fire";

            if (ua.Contains("RIM Tablet") || (ua.Contains("BB") && ua.Contains("Mobile")))
                return "Black Berry";

            if (ua.Contains("Windows Phone"))
                return string.Format("Windows Phone {0}", GetMobileVersion(ua, "Windows Phone"));

            if (ua.Contains("Mac OS"))
                return "Mac OS";

            if (ua.Contains("Windows NT 5.1") || ua.Contains("Windows NT 5.2"))
                return "Windows XP";

            if (ua.Contains("Windows NT 6.0"))
                return "Windows Vista";

            if (ua.Contains("Windows NT 6.1"))
                return "Windows 7";

            if (ua.Contains("Windows NT 6.2"))
                return "Windows 8";

            if (ua.Contains("Windows NT 6.3"))
                return "Windows 8.1";

            if (ua.Contains("Windows NT 10"))
                return "Windows 10";

            //fallback to basic platform:
            return request.Browser.Platform + (ua.Contains("Mobile") ? " Mobile " : "");
        }

        public String GetMobileVersion(string userAgent, string device)
        {
            var temp = userAgent.Substring(userAgent.IndexOf(device) + device.Length).TrimStart();
            var version = string.Empty;

            foreach (var character in temp)
            {
                var validCharacter = false;
                int test = 0;

                if (Int32.TryParse(character.ToString(), out test))
                {
                    version += character;
                    validCharacter = true;
                }

                if (character == '.' || character == '_')
                {
                    version += '.';
                    validCharacter = true;
                }

                if (validCharacter == false)
                    break;
            }

            return version;
        }
        #endregion
    }
}