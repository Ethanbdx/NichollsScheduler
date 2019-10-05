using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using NichollsScheduler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NichollsScheduler.Logic
{
    public class BannerService
    {
        public BannerService() { }
       
        private static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls,
        };
        private static readonly HttpClient client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://banner.nicholls.edu/PROD/")
        };
        public async Task<Dictionary<string, string>> getTerms()
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
        public async Task<List<CourseResult>> GetCourseResults(List<Course> courses, string termId)
        {
            List<CourseResult> courseResults = new List<CourseResult>();
            foreach(Course c in courses)
            {
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("term_in", termId),
                new KeyValuePair<string, string>("sel_subj", "dummy"),
                new KeyValuePair<string, string>("sel_day", "dummy"),
                new KeyValuePair<string, string>("sel_schd", "dummy"),
                new KeyValuePair<string, string>("sel_insm", "dummy"),
                new KeyValuePair<string, string>("sel_camp", "dummy"),
                new KeyValuePair<string, string>("sel_levl", "dummy"),
                new KeyValuePair<string, string>("sel_sess", "dummy"),
                new KeyValuePair<string, string>("sel_instr", "dummy"),
                new KeyValuePair<string, string>("sel_ptrm", "dummy"),
                new KeyValuePair<string, string>("sel_attr", "dummy"),
                new KeyValuePair<string, string>("sel_subj", c.subject),
                new KeyValuePair<string, string>("sel_crse", c.courseNum),
                new KeyValuePair<string, string>("sel_title", ""),
                new KeyValuePair<string, string>("sel_schd", "%"),
                new KeyValuePair<string, string>("sel_from_cred", ""),
                new KeyValuePair<string, string>("sel_to_cred", ""),
                new KeyValuePair<string, string>("sel_levl", "%"),
                new KeyValuePair<string, string>("sel_ptrm", "%"),
                new KeyValuePair<string, string>("sel_instr", "%"),
                new KeyValuePair<string, string>("begin_hh", "0"),
                new KeyValuePair<string, string>("begin_mi", "0"),
                new KeyValuePair<string, string>("begin_ap", "a"),
                new KeyValuePair<string, string>("end_hh", "0"),
                new KeyValuePair<string, string>("end_mi", "0"),
                new KeyValuePair<string, string>("end_ap", "a")
            };
                HttpContent content = new FormUrlEncodedContent(values);
                HttpResponseMessage result = await client.PostAsync("bwckschd.p_get_crse_unsec", content);
                var html = await result.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                try
                {
                    var tbody = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/table[1]");
                    var trows = tbody.Elements("tr");
                    for (int i = 0; i < trows.Count(); i++)
                    {
                        var courseInfo = trows.ElementAt(i).InnerText.Split('-');
                        var creditHoursHtml = trows.ElementAt(i + 1).Element("td").InnerText.Split('\n').Where(s => s.Contains("Credits")).ElementAt(0).Trim();
                        var courseDetails = trows.ElementAt(i + 1).Element("td").Element("table").Elements("tr");
                        List<string> time = new List<string>();
                        List<string> inst = new List<string>();
                        List<string> days = new List<string>();
                        List<string> location = new List<string>();
                        List<string> schedType = new List<string>();
                        for (int x = 1; x < courseDetails.Count(); x++)
                        {
                            string[] crseDetails = courseDetails.ElementAt(x).InnerText.Split('\n');
                            time.Add(crseDetails[2]);
                            days.Add(crseDetails[3]);
                            location.Add(crseDetails[4]);
                            inst.Add(crseDetails[7].Replace("(P)", ""));
                            schedType.Add(crseDetails[6].Trim());
                        }
                        courseResults.Add(new CourseResult
                        {
                            title = courseInfo[0].Trim('\n', ' '),
                            courseRegistrationNum = courseInfo[1].Trim(),
                            subject = c.subject,
                            courseNumber = c.courseNum,
                            section = courseInfo[3].Trim('\n', ' '),
                            time = time,
                            days = days,
                            location = location,
                            instructor = inst,
                            creditHours = creditHoursHtml,
                            scheduleType = schedType
                        });
                        i++;
                    }
                }
                catch
                {
                    
                }
                
            }
            return courseResults;
        }
    }
}
