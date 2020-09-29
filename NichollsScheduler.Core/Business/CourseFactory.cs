using AngleSharp.Dom;
using NichollsScheduler.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Business
{
    public class CourseFactory
    {
        public static CourseResultModel ParseCourseResultHtml(List<IElement> html, int i, CourseModel c)
        {
            var courseInfo = html.ElementAt(i).TextContent.Split('-').ToList();
            if (courseInfo.Count > 4)
            {
                for (int p = 0; p < courseInfo.Count - 3; p++)
                {
                    courseInfo[0] = courseInfo[0] + "-" + courseInfo[1];
                    courseInfo.RemoveAt(1);
                }
            }
            var creditHoursHtml = html.ElementAt(i + 1).TextContent.Split('\n').Where(s => s.Contains("Credits")).ElementAt(0).Trim();
            creditHoursHtml = creditHoursHtml.Remove(5);
            var courseDetails = html.ElementAt(i + 1).GetElementsByTagName("tr").ToList();
            List<string> times = new List<string>();
            List<string> inst = new List<string>();
            List<string> days = new List<string>();
            List<string> locations = new List<string>();
            for (int x = 1; x < courseDetails.Count(); x++)
            {
                string[] crseDetails = courseDetails.ElementAt(x).TextContent.Split('\n');
                times.Add(crseDetails[2]);
                if (crseDetails[3] == "&nbsp;")
                {
                    crseDetails[3] = "N/A";
                }
                days.Add(crseDetails[3]);
                locations.Add(crseDetails[4]);
                inst.Add(crseDetails[7].Replace("(P)", ""));
            }
            var result = new CourseResultModel();
            if (courseInfo[0].Contains(':'))
            {
                var fulltitle = courseInfo[0].Trim('\n', ' ').Split(":");
                result.Topic = fulltitle[1].Trim();
            }
            result.SearchModel = c;
            result.CourseRegistrationNum = courseInfo[1].Trim();
            result.Section = courseInfo[3].Trim('\n', ' ');
            result.Instructor = inst.Select(i => i.Trim()).FirstOrDefault();
            result.CreditHours = creditHoursHtml;
            result.Meetings = ParseCourseMeetingInformation(days, locations, times);
            return result;
        }

        public static CourseResultModel ParseSeatCapacitiesHtml(CourseResultModel CourseModel, IDocument htmlDoc)
        {
            var seatCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[1]']").TextContent;
            var seatActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[2]']").TextContent;
            var waitListCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[1]']").TextContent;
            var waitListActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[2]']").TextContent;
            CourseModel.RemainingSeats = Int32.Parse(seatCap) - Int32.Parse(seatActual);
            CourseModel.RemainingWaitlist = Int32.Parse(waitListCap) - Int32.Parse(waitListActual);
            return CourseModel;
        }
        public static Dictionary<char, List<Meeting>> ParseCourseMeetingInformation(List<string> days, List<string> locations, List<string> times) {
            var meetings = new Dictionary<char, List<Meeting>>();
            for(int i = 0; i < days.Count; i++) {
                foreach(char d in days[i]) {
                    if(!meetings.ContainsKey(d)) {
                        meetings[d] = new List<Meeting>();
                    }
                    var parsedTimes = Time.ParseTime(times[i]);
                    var meeting = new Meeting {
                        Location = locations[i],
                        StartTime = parsedTimes.Item1,
                        EndTime = parsedTimes.Item2
                    };
                    meetings[d].Add(meeting);
                }
            }
            return meetings;
        }

    }
}
