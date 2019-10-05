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
    public class CourseSelectionModel : PageModel
    {
        private ICourseSelection options;
        public CourseSelectionModel(ICourseSelection options) => this.options = options;

        [BindProperty(SupportsGet = true)]
        public string subject { get; set; }
        public string courseNum { get; set; }

        public void OnGet()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> OnPost(List<Course> selCourses, BannerService client)
        {
            var courseResults = JsonConvert.SerializeObject(await client.GetCourseResults(selCourses, HttpContext.Session.GetString("termId")));
            var courses = JsonConvert.SerializeObject(selCourses);
            HttpContext.Session.SetString("courseResults", courseResults);
            HttpContext.Session.SetString("courseSelections", courses);
            return RedirectToPage("CourseResults");
        }

        public JsonResult OnGetCourseSelections()
        {
            return new JsonResult(options.GetCourseSelections(subject));
        }
    }
}