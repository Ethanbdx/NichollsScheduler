using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;
using Microsoft.AspNetCore.Http;
using NichollsScheduler.Banner_Data;
using NichollsScheduler.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NichollsScheduler.Logic
{
    public static class BannerScraper
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls,
        };
        private static readonly HttpClient client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://banner.nicholls.edu/PROD/")
        };

        public static async Task<Dictionary<string, string>> getTerms()
        {
            try
            {
                HttpResponseMessage terms = await client.GetAsync("bwckschd.p_disp_dyn_sched");
                string htmlDocument = await terms.Content.ReadAsStringAsync();
                var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(htmlDocument));
                var termSelect = document.QuerySelectorAll("option").ToDictionary(t => t.TextContent, t => t.GetAttribute("value")).ToList();

                //Removing the "Select Term Option"
                termSelect.RemoveAt(0);
                //Removing all other options except the 3 most recent.
                termSelect.RemoveRange(3, termSelect.Count - 3);

                var termResult = termSelect.ToDictionary(x => x.Key, x => x.Value);
                return termResult;
            }
            catch
            {
                Console.WriteLine("No Terms Found");
                return null;
            }

        }
        public static List<List<CourseResultModel>> getCourseResults(List<CourseModel> courses, string termId)
        {
            List<List<CourseResultModel>> courseResults = new List<List<CourseResultModel>>();
            Parallel.ForEach(courses, CourseModel =>
            {
                var result = getCourses(CourseModel, termId);
                courseResults.Add(result.Result);
            });
            return courseResults;
        }
        private static async Task<List<CourseResultModel>> getCourses(CourseModel CourseModel, string termId)
        {
            List<CourseResultModel> courseRes = new List<CourseResultModel>();

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
                new KeyValuePair<string, string>("sel_subj", CourseModel.subject),
                new KeyValuePair<string, string>("sel_crse", CourseModel.courseNumber),
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
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            try
            {
                var trows = htmlDoc.QuerySelectorAll("*[xpath>'/body/div[3]/table[1]/tbody/tr']").ToList();
                for (int i = 0; i < trows.Count(); i++)
                {
                    //Collecting and Parsing all the data required to make a CourseResult object
                    CourseResultModel courseResult = CourseFactory.parseCourseResultHtml(trows, i, CourseModel);
                    courseResult = getSeatCapacities(courseResult, termId).Result;
                    courseRes.Add(courseResult);
                    i++;
                }
            }
            catch
            {
                //Adding a blank case in the event of a fail.
                courseRes.Add(new CourseResultModel
                {
                    subject = CourseModel.subject,
                    courseNumber = CourseModel.courseNumber
                });
            }
            return courseRes;
        }
        private static async Task<CourseResultModel> getSeatCapacities(CourseResultModel CourseModel, string termId)
        {
            HttpResponseMessage httpmsg = await client.GetAsync($"bwckschd.p_disp_detail_sched?term_in={termId}&crn_in={CourseModel.courseRegistrationNum}");
            string html = await httpmsg.Content.ReadAsStringAsync();
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            return CourseFactory.parseSeatCapacitiesHtml(CourseModel, htmlDoc);
        }
    }
}
