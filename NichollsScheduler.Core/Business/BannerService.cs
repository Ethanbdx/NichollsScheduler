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
        private CourseResultModelFactory CourseResultModelFactory = new CourseResultModelFactory();

        public BannerService(HttpClient client, ILogger logger, string connectionString)
        {
            this.ConnectionString = connectionString;
            this.Client = client;
            this.Logger = logger;
        }

        public async Task<List<CourseModel>> GetCoursesInfo(string subject)
        {
            var coursesInfo = new List<CourseModel>();

            try {
                using (var connection = new SqlConnection(this.ConnectionString)) {
                    var task = await connection.QueryAsync<CourseModel>("dbo.sp_GetCourseInfo", new { subject = subject }, commandType: CommandType.StoredProcedure);
                    coursesInfo = task.ToList();
                }
            } catch(Exception e) {
                this.Logger.LogError(e.Message);
            }
            
            return coursesInfo;
        }
        public async Task<List<SubjectModel>> GetCourseSubjects()
        {
            var courseSubjects = new List<SubjectModel>();

            try {
                using (var connection = new SqlConnection(this.ConnectionString)) {
                    var task = await connection.QueryAsync<SubjectModel>("dbo.sp_GetCourseSubjects", commandType: CommandType.StoredProcedure);
                    courseSubjects = task.ToList();
                }
            } catch (Exception e) {

                this.Logger.LogError(e.Message);
            }

            return courseSubjects;
        }
        public async Task<List<TermModel>> GetTerms()
        {
            //Caching for terms
            string cacheId = DateTime.Now.ToShortDateString();
            var termResult = new List<TermModel>();
            if (this.CachedTerms.TryGetValue<List<TermModel>>(cacheId, out _))
            {
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
            }
            catch (Exception e)
            {
                this.Logger.LogError( e.Message);
            }

            return termResult;
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
        private async Task<CourseResultList> GetCourses(CourseModel courseModel, string termId)
        {
            CourseResultList courseRes = new CourseResultList(courseModel);

            //Check for cache, it if exists...just get current seat count.
            string cacheId = $"{termId}.{courseModel.SubjectCode}{courseModel.CourseNumber}";
            if (this.CachedCourses.TryGetValue<CourseResultList>(cacheId, out _))
            {
                courseRes = this.CachedCourses.Get<CourseResultList>(cacheId);
                for (int i = 0; i < courseRes.Results.Count; i++)
                {
                    (int remainingSeats, int remainingWaitlist) seatData = await GetSeatCapacities(courseRes.Results[i].CourseRegistrationNum, termId);
                    courseRes.Results[i].RemainingSeats = seatData.remainingSeats;
                    courseRes.Results[i].RemainingWaitlist = seatData.remainingWaitlist;
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
                    for (int i = 0; i < trows.Count(); i+= 2)
                    {
                        //Collecting and Parsing all the data required to make a CourseResult object
                        var courseRows = trows.Skip(i).Take(2);
                        CourseResultModel courseResult = this.CourseResultModelFactory.CreateCourseFromHtml(courseRows, courseModel);

                        (int remainingSeats, int remainingWaitlist) seatData = await GetSeatCapacities(courseResult.CourseRegistrationNum, termId);
                        courseResult.RemainingSeats = seatData.remainingSeats;
                        courseResult.RemainingWaitlist = seatData.remainingWaitlist;

                        courseRes.Results.Add(courseResult);
                    }
                    courseRes.Results.OrderByDescending(x => x.RemainingSeats).ThenBy(x => x.Section).ToList();
                    this.CachedCourses.Set(cacheId, courseRes, TimeSpan.FromDays(15));
                }
                catch
                {
                    Logger.LogInformation($"Match not found for {courseModel.SubjectCode} {courseModel.CourseNumber}.");
                    this.CachedCourses.Set(cacheId, courseRes, TimeSpan.FromDays(15));
                }
            }
            else
            {
                Logger.LogWarning($"Banner responded with a {result.StatusCode} while searching for {courseModel.SubjectCode} {courseModel.CourseNumber}.");
            }
            
            return courseRes;
        }
        private async Task<(int, int)> GetSeatCapacities(string courseRegistrationNum, string termId)
        {
            HttpResponseMessage httpmsg = await this.Client.GetAsync($"bwckschd.p_disp_detail_sched?term_in={termId}&crn_in={courseRegistrationNum}");
            string html = await httpmsg.Content.ReadAsStringAsync();
            var htmlDoc = await BrowsingContext.New(Configuration.Default.WithXPath()).OpenAsync(req => req.Content(html));
            var seatCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[1]']").TextContent;
            var seatActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[2]']").TextContent;
            var waitListCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[1]']").TextContent;
            var waitListActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[2]']").TextContent;

            var remainingSeats = Int32.Parse(seatCap) - Int32.Parse(seatActual);
            var remainingWaitlist = Int32.Parse(waitListCap) - Int32.Parse(waitListActual);
            return (remainingSeats, remainingWaitlist);
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
