using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Web.Http;
using HtmlAgilityPack;

using PublicTransportSofia.Utils;

namespace PublicTransportSofia
{
    public class HttpRequester
    {
        private static HttpRequester instance;
        private HttpClient client;

        private HttpRequester()
        {
            this.client = new HttpClient();
        }

        public static HttpRequester Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new HttpRequester();
                }

                return instance;
            }
        }
        public async Task<string> GetVirtualTablesMainPage()
        {
            var uri = new Uri(StringResources.VirtualTablesBaseURL);
            var httpClient = new HttpClient();
            var virtualTablesMainPage = await httpClient.GetStringAsync(uri);
            //var virtualTablesMainPage = await this.client.GetStringAsync(uri);
            var captchaURL = "";
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

            // There are various options, set as needed
            htmlDoc.OptionFixNestedTags = true;

            // filePath is a path to a file containing the html
            htmlDoc.LoadHtml(virtualTablesMainPage);

            // Use:  htmlDoc.LoadHtml(xmlString);  to load from a string (was htmlDoc.LoadXML(xmlString)

            // ParseErrors is an ArrayList containing any errors from the Load statement
            if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required

            }
            else
            {

                if (htmlDoc.DocumentNode != null)
                {
                    var bodyNode = htmlDoc.DocumentNode.Descendants("img").Where(d =>
    d.Attributes.Contains("src") && d.Attributes["src"].Value.Contains("captcha")).FirstOrDefault();
                    captchaURL = bodyNode.Attributes.Where(a => a.Name == "src").Select(a => a.Value).FirstOrDefault();//Element("//body");//SelectSingleNode("//body");
                   // HtmlNode footer = htmlDoc.DocumentNode.Descendants().SingleOrDefault(x => x.c == "footer"); 
                    if (bodyNode != null)
                    {
                        // Do something with bodyNode
                    }
                }
            }
            var debug = 2;
            httpClient.Dispose();
            return  StringResources.SofiaTrafficBaseURL + captchaURL;
        }

        public async void GetMainPage()
        {
            //var body = String.Format("grant_type=client_credentials&client_id={0}&client_secret={1}&scope=notify.windows.com", MyCredentials, MyCredentials2);
            var body = String.Format("stopCode=2326&o=1&sec=5&0432c17d1f9285738465d347d4fc76f3=cf69fb71725ebf137b903a2da6af719f&go=1&submit=Провери");
            HttpStringContent theContent = new HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
            var headers = this.client.DefaultRequestHeaders;
            headers.Add("Host", "m.sofiatraffic.bg");
            headers.Add("Proxy-Connection", "keep-alive");
            //headers.Append("Content-Length", "144");
          //  headers.TryAppendWithoutValidation("Content-Length", "144");
           // headers.Add("Content-Length", "144");
            headers.CacheControl.ParseAdd("max-age=0");
            headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            headers.Add("Origin", "http://m.sofiatraffic.bg");
            headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.65 Safari/537.36");
         //   headers.Add("Content-Type", "application/x-www-form-urlencoded");
            headers.TryAppendWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
            headers.Add("Referer", "http://m.sofiatraffic.bg/vt");
            headers.AcceptEncoding.ParseAdd("gzip, deflate");
            headers.AcceptLanguage.ParseAdd("bg-BG,bg;q=0.8,en;q=0.6,sq;q=0.4");
            headers.Cookie.ParseAdd("PHPSESSID=je4ekdi3ak5842peto4rv3pn62; vjfmrii=89ea942e251ce5928a442d2c8f053d5c; alpocjengi=d98c07b1c96658a22074ec291e3163628c716843");
        
            HttpResponseMessage aResponse = await client.PostAsync(new Uri("http://m.sofiatraffic.bg/vt"), theContent);
            var responseHeadersKeys =  aResponse.Headers.Keys;
            var responseHeadersValues = aResponse.Headers.Values;
            var cookies = responseHeadersKeys.Zip(responseHeadersValues, (k, v) => new { k, v })
                .Where(x => x.k == "Set-Cookie").Select(x => x.v);
            var ponormalno = string.Join("; ", cookies);
            //var content = aResponse.Content;
            //var con = await content.ReadAsStringAsync();
            //var headers = aResponse.Headers;
            //StringBuilder sb = new StringBuilder();
            //foreach (var header in responseHeaders)
            //{
            //    var h = String.Format("Header: {0} ", header);
            //    sb.Append(h + "\n");
            //}

            //var res = sb.ToString();
          //  var uri = new Uri("http://m.sofiatraffic.bg/vt");
          //  var httpClient = new HttpClient();
          ////  httpClient.DefaultRequestHeaders.
          //  var userAgnet = httpClient.DefaultRequestHeaders.UserAgent;
          //  string result;
          //  // Always catch network exceptions for async methods
          //  try
          //  {
          //      result = await httpClient.GetStringAsync(uri);
          //  }
          //  catch
          //  {
          //      // Details in ex.Message and ex.HResult.       
          //  }

          //  // Once your app is done using the HttpClient object call dispose to 
          //  // free up system resources (the underlying socket and memory used for the object)
          //  httpClient.Dispose();
            var debug = 1;
        }
    }
}
