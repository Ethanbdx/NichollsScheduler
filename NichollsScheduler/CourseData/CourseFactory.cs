using AngleSharp.Dom;
using NichollsScheduler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NichollsScheduler.Banner_Data
{
    public static class CourseFactory
    {
        public static CourseResultModel parseCourseResultHtml(List<IElement> html, int i, CourseModel c)
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
            List<string> time = new List<string>();
            List<string> inst = new List<string>();
            List<string> days = new List<string>();
            List<string> location = new List<string>();
            List<string> schedType = new List<string>();
            for (int x = 1; x < courseDetails.Count(); x++)
            {
                string[] crseDetails = courseDetails.ElementAt(x).TextContent.Split('\n');
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
            if (c.subject == "ENGL")
            {
                if (c.courseNumber == "215" || c.courseNumber == "216" || c.courseNumber == "217")
                {
                    var fulltitle = courseInfo[0].Trim('\n', ' ').Split(":");
                    return new CourseResultModel
                    {
                        title = fulltitle[0],
                        topic = fulltitle[1],
                        courseRegistrationNum = courseInfo[1].Trim(),
                        subject = c.subject,
                        courseNumber = c.courseNumber,
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
            return new CourseResultModel
            {
                title = courseInfo[0].Trim('\n', ' '),
                courseRegistrationNum = courseInfo[1].Trim(),
                subject = c.subject,
                courseNumber = c.courseNumber,
                section = courseInfo[3].Trim('\n', ' '),
                time = time,
                days = days,
                location = location,
                instructor = inst,
                creditHours = creditHoursHtml,
                scheduleType = schedType
            };
        }
        public static CourseResultModel parseSeatCapacitiesHtml(CourseResultModel CourseModel, IDocument htmlDoc)
        {
            var seatCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[1]']").TextContent;
            var seatActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[2]']").TextContent;
            var waitListCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[1]']").TextContent;
            var waitListActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[2]']").TextContent;
            CourseModel.seatCap = Int32.Parse(seatCap);
            CourseModel.seatActual = Int32.Parse(seatActual);
            CourseModel.waitListCap = Int32.Parse(waitListCap);
            CourseModel.waitListActual = Int32.Parse(waitListActual);
            return CourseModel;
        }

    }
}
