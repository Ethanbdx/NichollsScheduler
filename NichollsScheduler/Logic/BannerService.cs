﻿using HtmlAgilityPack;
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
            try
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
            catch
            {
                return null;
            }

        }
        public async Task<List<List<CourseResult>>> GetCourseResults(List<Course> courses, string termId)
        {
            List<List<CourseResult>> courseResults = new List<List<CourseResult>>();
            foreach (Course c in courses)
            {
                List<CourseResult> courseRes = new List<CourseResult>();
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
                        CourseResult course = parseCourseResultHtml(trows, i, c);
                        course = getSeatCapacities(course, termId).Result;
                        courseRes.Add(course);
                        i++;
                    }
                    courseResults.Add(courseRes);
                }
                catch
                {
                    courseRes.Add(new CourseResult
                    {
                        subject = c.subject,
                        courseNumber = c.courseNum
                    });
                    courseResults.Add(courseRes);
                }
            }
            return courseResults;
        }
        private CourseResult parseCourseResultHtml(IEnumerable<HtmlNode> html, int i, Course c)
        {
            var courseInfo = html.ElementAt(i).InnerText.Split('-').ToList();
            if (courseInfo.Count > 4)
            {
                for (int p = 0; p < courseInfo.Count - 3; p++)
                {
                    courseInfo[0] = courseInfo[0] + "-" + courseInfo[1];
                    courseInfo.RemoveAt(1);
                }
            }
            var creditHoursHtml = html.ElementAt(i + 1).Element("td").InnerText.Split('\n').Where(s => s.Contains("Credits")).ElementAt(0).Trim();
            creditHoursHtml = creditHoursHtml.Remove(5);
            var courseDetails = html.ElementAt(i + 1).Element("td").Element("table").Elements("tr");
            List<string> time = new List<string>();
            List<string> inst = new List<string>();
            List<string> days = new List<string>();
            List<string> location = new List<string>();
            List<string> schedType = new List<string>();
            for (int x = 1; x < courseDetails.Count(); x++)
            {
                string[] crseDetails = courseDetails.ElementAt(x).InnerText.Split('\n');
                time.Add(crseDetails[2]);
                if (crseDetails[3] == "&nbsp;")
                {
                    crseDetails[3] = "N/A";
                }
                days.Add(crseDetails[3]);
                location.Add(crseDetails[4]);
                inst.Add(crseDetails[7].Replace("(P)", ""));
                schedType.Add(crseDetails[6].Trim());
            }
            if(c.subject == "ENGL")
            {
                if(c.courseNum == "215" || c.courseNum == "216" || c.courseNum == "217")
                {
                    var fulltitle = courseInfo[0].Trim('\n', ' ').Split(":");
                    return new CourseResult
                    {
                        title = fulltitle[0],
                        topic = fulltitle[1],
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
                    };
                }
            }
            return new CourseResult
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
            };
        }
        private async Task<CourseResult> getSeatCapacities(CourseResult course, string termId)
        {
            HttpResponseMessage httpmsg = await client.GetAsync($"bwckschd.p_disp_detail_sched?term_in={termId}&crn_in={course.courseRegistrationNum}");
            string html = await httpmsg.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var seatCap = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/table[1]/tr[2]/td/table/tr[2]/td[1]").InnerText;
            var seatActual = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/table[1]/tr[2]/td/table/tr[2]/td[2]").InnerText;
            var waitListCap = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/table[1]/tr[2]/td/table/tr[3]/td[1]").InnerText;
            var waitListActual = htmlDoc.DocumentNode.SelectSingleNode("/html/body/div[3]/table[1]/tr[2]/td/table/tr[3]/td[2]").InnerText;
            course.seatCap = Int32.Parse(seatCap);
            course.seatActual = Int32.Parse(seatActual);
            course.waitListCap = Int32.Parse(waitListCap);
            course.waitListActual = Int32.Parse(waitListActual);
            return course;
        }
    }
}
