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
using System.Data.SQLite;
using Microsoft.Extensions.Logging;

using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace NichollsScheduler.Core.Business
{
    public class BannerService
    {
        private readonly HttpClient Client;
        private readonly ILogger Logger;
        private readonly string ConnectionString;
        private MemoryCache CachedTerms = new MemoryCache(new MemoryCacheOptions());
        private MemoryCache CachedCourses = new MemoryCache(new MemoryCacheOptions());

        public BannerService(HttpClient client, ILogger logger, string connectionString)
        {
            this.ConnectionString = connectionString;
            this.Client = client;
            this.Logger = logger;
        }

        public async Task<List<CourseModel>> GetCoursesInfo(string subject)
        {
            var coursesInfo = new List<CourseModel>();

            using (var connection = new SqlConnection(this.ConnectionString)) {
                var task = await connection.QueryAsync<CourseModel>("dbo.sp_GetCourseInfo", new { subject = subject }, commandType: CommandType.StoredProcedure);
                coursesInfo = task.ToList();
            }
            return coursesInfo;
        }
        public async Task<List<SubjectModel>> GetCourseSubjects()
        {
            var courseSubjects = new List<SubjectModel>();

            using (var connection = new SqlConnection(this.ConnectionString)) {
                var task = await connection.QueryAsync<SubjectModel>("dbo.sp_GetCourseSubjects", commandType: CommandType.StoredProcedure);
                courseSubjects = task.ToList();
            }
            
            return courseSubjects;
        }
        public async Task<List<TermModel>> GetTerms()
        {
            Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Term list requested.");
            //Caching for terms
            string cacheId = DateTime.Now.ToShortDateString();
            var termResult = new List<TermModel>();
            if (this.CachedTerms.TryGetValue<List<TermModel>>(cacheId, out _))
            {
                Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Term list retrived from cache.");
                return this.CachedTerms.Get<List<TermModel>>(cacheId);
            }

            try
            {

                HttpResponseMessage terms = await this.Client.GetAsync("bwckschd.p_disp_dyn_sched");
                string htmlDocument = await terms.Content.ReadAsStringAsync();
                var document = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(htmlDocument));
                var termSelect = document.QuerySelectorAll("option").ToDictionary(t => t.TextContent, t => t.GetAttribute("value")).ToList();

                //Removing the "Select Term Option"
                termSelect.RemoveAt(0);
                //Removing all other options except the 3 most recent.
                termSelect.RemoveRange(3, termSelect.Count - 3);

                termResult.AddRange(termSelect.Select(kvp => new TermModel { TermName = kvp.Key, TermId = int.Parse(kvp.Value) }));

                this.CachedTerms.Set(cacheId, termResult, TimeSpan.FromDays(1));

                Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Term list retrived from banner.");
                return termResult;
            }
            catch
            {

                throw new Exception("Error. There was an issue getting the available terms.");
            }
        }
        public List<CourseResultList> GetCourseResults(List<CourseModel> courses, string termId)
        {
            List<CourseResultList> courseResults = new List<CourseResultList>();

            //Runs a search for each course on different threads.
            Parallel.ForEach(courses, courseModel =>
            {
                var resultList = GetCourses(courseModel, termId);
                courseResults.Add(resultList.Result);
            });
            return courseResults.OrderBy(x => x.Results.Count).ToList();
        }
        public void SaveSchedule(List<int> selectedCourseCRNs) {

        }
        public void RetrieveSchedule(string scheduleId) {

        }
        private SaveCourseToDatabase() {
            
        }
        private async Task<CourseResultList> GetCourses(CourseModel courseModel, string termId)
        {
            Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Searching for {courseModel.SubjectCode} {courseModel.CourseNumber}.");
            CourseResultList courseRes = new CourseResultList(courseModel);

            //Check for cache, it if exists...just get current seat count.
            string cacheId = $"{termId}.{courseModel.SubjectCode}{courseModel.CourseNumber}";
            if (this.CachedCourses.TryGetValue<CourseResultList>(cacheId, out _))
            {
                Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Match found in cache for {courseModel.SubjectCode} {courseModel.CourseNumber}.");
                courseRes = this.CachedCourses.Get<CourseResultList>(cacheId);
                for (int i = 0; i < courseRes.Results.Count; i++)
                {
                    courseRes.Results[i] = await GetSeatCapacities(courseRes.Results[i], termId);
                }
                return courseRes;
            }

            List<KeyValuePair<string, string>> values = BannerQueryValues.GetKeyValues(termId, courseModel.SubjectCode, courseModel.CourseNumber);
            HttpContent content = new FormUrlEncodedContent(values);
            HttpResponseMessage result = await this.Client.PostAsync("bwckschd.p_get_crse_unsec", content);
            var html = await result.Content.ReadAsStringAsync();
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    var trows = htmlDoc.QuerySelectorAll("*[xpath>'/body/div[3]/table[1]/tbody/tr']").ToList();
                    for (int i = 0; i < trows.Count(); i++)
                    {
                        //Collecting and Parsing all the data required to make a CourseResult object
                        CourseResultModel courseResult = CourseFactory.ParseCourseResultHtml(trows, i, courseModel);
                        courseResult = await GetSeatCapacities(courseResult, termId);
                        courseRes.Results.Add(courseResult);
                        i++;
                    }
                    Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Went to banner and found {courseModel.SubjectCode} {courseModel.CourseNumber}.");
                    courseRes.Results.OrderByDescending(x => x.RemainingSeats).ThenBy(x => x.Section).ToList();
                    this.CachedCourses.Set(cacheId, courseRes, TimeSpan.FromDays(15));
                }
                catch
                {
                    if (result.IsSuccessStatusCode)
                    {
                        Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Match not found for {courseModel.SubjectCode} {courseModel.CourseNumber} from banner.");
                        this.CachedCourses.Set(cacheId, courseRes, TimeSpan.FromDays(15));
                    }
                }
            }
            else
            {
                Logger.Log(LogLevel.Information, $"[{DateTime.Now}] Banner responded with a {result.StatusCode} while searching for {courseModel.SubjectCode} {courseModel.CourseNumber}.");
            }
            
            return courseRes;
        }
        private async Task<CourseResultModel> GetSeatCapacities(CourseResultModel CourseModel, string termId)
        {
            HttpResponseMessage httpmsg = await this.Client.GetAsync($"bwckschd.p_disp_detail_sched?term_in={termId}&crn_in={CourseModel.CourseRegistrationNum}");
            string html = await httpmsg.Content.ReadAsStringAsync();
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            return CourseFactory.ParseSeatCapacitiesHtml(CourseModel, htmlDoc);
        }
    }
    internal static class BannerQueryValues
    {
        public static List<KeyValuePair<string, string>> GetKeyValues(string termId, string subject, string courseNumber)
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
