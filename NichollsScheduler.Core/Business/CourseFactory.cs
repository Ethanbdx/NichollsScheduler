using AngleSharp.Dom;
using NichollsScheduler.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Core.Business
{
    public class CourseResultModelFactory
    {
        public CourseResultModel CreateCourseFromHtml(IEnumerable<IElement> html, CourseModel c) {
            string[] courseInfo = html.ElementAt(0).TextContent.Split(" - ");
            string creditHours = html.ElementAt(1).TextContent.Split('\n').Where(s => s.Contains("Credits")).ElementAt(0).Trim().Remove(5);
            IElement[] courseDetailsElements = html.ElementAt(1).GetElementsByTagName("tr").Skip(1).ToArray();

            List<string> times = new List<string>();
            List<string> inst = new List<string>();
            List<string> days = new List<string>();
            List<string> locations = new List<string>();
            for (int i = 0; i < courseDetailsElements.Length; i++) {
                string[] crseDetails = courseDetailsElements.ElementAt(i).TextContent.Split('\n');
                times.Add(crseDetails[2]);
                if (crseDetails[2] == "&nbsp;") {
                    crseDetails[2] = "N/A";
                }
                days.Add(crseDetails[3]);
                locations.Add(crseDetails[4]);
                inst.Add(crseDetails[7].Replace("(P)", ""));
            }

            var courseResult = new CourseResultModel(c);
            if(courseInfo[0].Contains(':')) {
                var fullTitle = courseInfo[0].Trim('\n', ' ').Split(":");
                courseResult.Topic = fullTitle[1].Trim();
            }

            courseResult.CourseRegistrationNum = courseInfo[1].Trim();
            courseResult.Section = courseInfo[3].Trim();
            courseResult.Instructor = inst.Select(x => x.Trim()).FirstOrDefault();
            courseResult.CreditHours = creditHours;
            courseResult.Meetings = ParseCourseMeetingInformation(days, locations, times);
            return courseResult;
        }

        private List<Meeting> ParseCourseMeetingInformation(List<string> days, List<string> locations, List<string> times) {
            var meetings = new List<Meeting>();
            for(int i = 0; i < days.Count; i++) {
                    if(times[0] == "TBA") {
                        return null;
                    }
                    var meetingDays = days[i].ToCharArray();
                    var parsedTimes = Time.ParseTimes(times[i]);
                    var meeting = new Meeting {
                        Location = locations[i],
                        StartTime = parsedTimes.Item1,
                        EndTime = parsedTimes.Item2,
                        Days = meetingDays
                    };
                    meetings.Add(meeting);
                }
            return meetings.OrderBy(x => x.StartTime.Value).ToList();
        }

    }
}
