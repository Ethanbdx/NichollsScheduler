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

        public static List<CourseResult> courseResults;
        public static List<Course> courseSelections;
        public void OnGet()
        {
            string courseSel = HttpContext.Session.GetString("courseSelections");
            string courseRes = HttpContext.Session.GetString("courseResults");
            var results = JsonConvert.DeserializeObject<List<CourseResult>>(courseRes);
            var selections = JsonConvert.DeserializeObject<List<Course>>(courseSel);
            courseResults = results;
            courseSelections = selections;
        }
    }
}