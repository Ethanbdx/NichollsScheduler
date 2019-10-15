using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NichollsScheduler.Data;
using NichollsScheduler.Logic;

namespace NichollsScheduler.Pages
{
    public class CourseResultsModel : PageModel
    {
        [BindProperty]
        public List<string> selectedCourse { get; set; }

        public static List<string> prevSelectedCourses;
        public static List<List<CourseResult>> courseResults;

        public IActionResult OnGet()
        {
            
            string courseRes = HttpContext.Session.GetString("courseResults");
            if (String.IsNullOrEmpty(courseRes))
            {
                return RedirectToPage("CourseSelection");
            }

            var results = JsonConvert.DeserializeObject<List<List<CourseResult>>>(courseRes);
            courseResults = results;
            try
            {
                var selectCourses = JsonConvert.DeserializeObject<List<CourseResult>>(HttpContext.Session.GetString("selectedCourseResults"));
                var courseRegsNums = from c in selectCourses
                                     select c.courseRegistrationNum;
                prevSelectedCourses = courseRegsNums.ToList();
                return Page();
            }
            catch
            {
                prevSelectedCourses = new List<string>();
                return Page();
            }
        }

        public IActionResult OnPost()
        {
            if(selectedCourse.Count == 0)
            {
                return Page();
            }
            var selectedCourses = (from c in courseResults.SelectMany(l => l.Distinct())
                                  where selectedCourse.Contains(c.courseRegistrationNum)
                                  select c).ToList();
            HttpContext.Session.SetString("selectedCourseResults", JsonConvert.SerializeObject(selectedCourses));
            return RedirectToPage("ConfirmSchedule");
        }
    }
}
