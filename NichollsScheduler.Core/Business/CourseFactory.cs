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
            var result = new CourseResultModel();
            if (courseInfo[0].Contains(':'))
            {
                var fulltitle = courseInfo[0].Trim('\n', ' ').Split(":");
                result.CourseTitle = c.CourseTitle;
                result.Topic = fulltitle[1].Trim();
            } 
            else 
            {
                result.CourseTitle = c.CourseTitle;
            }
            
            result.CourseRegistrationNum = courseInfo[1].Trim();
            result.SubjectCode = courseInfo[2].Trim().Split(' ').GetValue(0).ToString();
            result.CourseNumber = courseInfo[2].Trim().Split(' ').GetValue(1).ToString();
            result.Section = courseInfo[3].Trim('\n', ' ');
            result.Time = time;
            result.Days = days.Select(d => d.Trim()).ToList();
            result.Location = location;
            result.Instructor = inst.Select(i => i.Trim()).ToList();
            result.CreditHours = double.Parse(creditHoursHtml);
            result.ScheduleType = schedType;
            return result;
        }

        public static CourseResultModel ParseSeatCapacitiesHtml(CourseResultModel CourseModel, IDocument htmlDoc)
        {
            var seatCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[1]']").TextContent;
            var seatActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[2]/td[2]']").TextContent;
            var waitListCap = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[1]']").TextContent;
            var waitListActual = htmlDoc.QuerySelector("*[xpath>'/body/div[3]/table[1]/tbody/tr[2]/td/table/tbody/tr[3]/td[2]']").TextContent;
            CourseModel.SeatCap = Int32.Parse(seatCap);
            CourseModel.SeatActual = Int32.Parse(seatActual);
            CourseModel.WaitListCap = Int32.Parse(waitListCap);
            CourseModel.WaitListActual = Int32.Parse(waitListActual);
            return CourseModel;
        }

    }
}
