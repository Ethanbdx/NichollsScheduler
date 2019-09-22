using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NichollsScheduler.Logic
{
    public class WebScraper
    {
        private static Uri baseAddress = new Uri("https://banner.nicholls.edu/PROD/");
        private static CookieContainer cookie = new CookieContainer();
        private static HttpClientHandler handler = new HttpClientHandler()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls,
            CookieContainer = cookie,
            UseCookies = true
        };
        private static readonly HttpClient client = new HttpClient(handler)
        {
            BaseAddress = baseAddress
        };

        public async Task<Dictionary<string,string>> getTerms()
        {
            HttpResponseMessage terms = await client.GetAsync("bwckschd.p_disp_dyn_sched");
            string htmlDocument = await terms.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlDocument);
            var selection = htmlDoc.DocumentNode.Descendants("option").ToDictionary(t => t.InnerText, t => t.GetAttributeValue("value", "0"));
            List<string> toDelete = selection.Keys.Where(k => k.Contains("View only") || k.Contains("None")).ToList();
            //toDelete.ForEach(k => selection.Remove(k));
            return selection;
 
        }
    }
}
