using System.ComponentModel;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.XPath;
using NichollsScheduler.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using NichollsScheduler.Core.Data;
using System.Data.SQLite;

namespace NichollsScheduler.Core.Business
{
    public class BannerService
    {
        private static HttpClientHandler handler = new HttpClientHandler()
        {
            SslProtocols = System.Security.Authentication.SslProtocols.Tls12,
        };
        private static HttpClient client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://banner.nicholls.edu/prod/")
        };
        private MemoryCache CachedTerms = new MemoryCache(new MemoryCacheOptions());
        private MemoryCache CachedCourses = new MemoryCache(new MemoryCacheOptions());

        public async Task<List<object>> GetCoursesInfo(string subject) {

            using var connection = new SQLiteConnection(SQLiteDriver.DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT CourseNumber, CourseTitle FROM Courses WHERE Subject = $subject";
            command.Parameters.AddWithValue("$subject", subject);

            using var resultReader = await command.ExecuteReaderAsync();
            var coursesInfo = new List<object>();
            while(resultReader.Read()) {
                coursesInfo.Add(new { courseNumber = resultReader.GetString(0), courseTitle = resultReader.GetString(1) });
            }
            await connection.CloseAsync();
            return coursesInfo;
        }
        public async Task<List<object>> GetCourseSubjects() {
            using var connection = new SQLiteConnection(SQLiteDriver.DB_PATH);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT FullSubject, SubjectCode FROM Subjects";

            using var resultReader = await command.ExecuteReaderAsync();
            var courseSubjects = new List<object>();
            while (resultReader.Read()) {
                //anonymous object for subjects
                var subject = new { fullSubject = resultReader.GetString(0), subjectCode = resultReader.GetString(1) };
                courseSubjects.Add(subject);
            }
            await connection.CloseAsync();
            return courseSubjects;
        }
        public async Task<List<TermModel>> GetTerms()
        {
            //Caching for terms
            string cacheId = DateTime.Now.ToShortDateString();
            var termResult = new List<TermModel>();
            if(this.CachedTerms.TryGetValue<List<TermModel>>(cacheId, out _)) {
                return this.CachedTerms.Get<List<TermModel>>(cacheId);
            }

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

                termResult.AddRange(termSelect.Select(kvp => new TermModel { TermName = kvp.Key, TermId = int.Parse(kvp.Value)}));
                
                this.CachedTerms.Set<List<TermModel>>(cacheId, termResult, TimeSpan.FromDays(1));

                return termResult;
            }
            catch
            { 

                throw new Exception("Error. There was an issue getting the available terms.");
            }
        }
        public List<List<CourseResultModel>> GetCourseResults(List<CourseModel> courses, string termId)
        {
            List<List<CourseResultModel>> courseResults = new List<List<CourseResultModel>>();
            foreach(var courseModel in courses) {
                var result = GetCourses(courseModel, termId);
                courseResults.Add(result.Result.OrderByDescending(x => x.RemainingSeats).ThenBy(x => x.Section).ToList());
            }
            return courseResults.OrderBy(x => x.Count).ToList();
        }
        private async Task<List<CourseResultModel>> GetCourses(CourseModel CourseModel, string termId)
        {

            List<CourseResultModel> courseRes = new List<CourseResultModel>();

            //Check for cache, it if exists...just get current seat count.
            string cacheId = $"{termId}.{CourseModel.SubjectCode}{CourseModel.CourseNumber}";
            if(this.CachedCourses.TryGetValue<List<CourseResultModel>>(cacheId, out _)) {
                courseRes = this.CachedCourses.Get<List<CourseResultModel>>(cacheId);
                for(int i = 0; i < courseRes.Count; i++) {
                    courseRes[i] = await GetSeatCapacities(courseRes[i], termId);
                }
                return courseRes;
            }

            List<KeyValuePair<string, string>> values = BannerQueryValues.GetKeyValues(termId, CourseModel.SubjectCode, CourseModel.CourseNumber);

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
                    CourseResultModel courseResult = CourseFactory.ParseCourseResultHtml(trows, i, CourseModel);
                    courseResult = await GetSeatCapacities(courseResult, termId);
                    courseRes.Add(courseResult);
                    i++;
                }
                this.CachedCourses.Set<List<CourseResultModel>>(cacheId, courseRes, TimeSpan.FromDays(3));
            }
            catch(Exception e) {
                Console.WriteLine(e.Message); ;
                //Add default case if fail.
                courseRes.Add(new CourseResultModel { SearchModel = CourseModel });
            }

            return courseRes;
        }
        private async Task<CourseResultModel> GetSeatCapacities(CourseResultModel CourseModel, string termId)
        {
            HttpResponseMessage httpmsg = await client.GetAsync($"bwckschd.p_disp_detail_sched?term_in={termId}&crn_in={CourseModel.CourseRegistrationNum}");
            string html = await httpmsg.Content.ReadAsStringAsync();
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            return CourseFactory.ParseSeatCapacitiesHtml(CourseModel, htmlDoc);
        }
    }
    internal static class BannerQueryValues {
        public static List<KeyValuePair<string, string>> GetKeyValues(string termId, string subject, string courseNumber) {
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
                new KeyValuePair<string, string>("sel_subj", subject),
                new KeyValuePair<string, string>("sel_crse", courseNumber),
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
            return values;
        }
    }
}
