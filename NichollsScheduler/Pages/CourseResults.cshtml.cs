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

        public static List<List<CourseResult>> courseResults;

        public void OnGet()
        {
            string courseRes = HttpContext.Session.GetString("courseResults");
            var results = JsonConvert.DeserializeObject<List<List<CourseResult>>>(courseRes);
            courseResults = results;

        }
        public void OnPost()
        {
            //TODO - Checking for overlapping schedules...done here or maybe within view?
        }
    }
}
