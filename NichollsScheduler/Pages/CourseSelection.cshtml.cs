using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NichollsScheduler.Data;

namespace NichollsScheduler.Pages
{
    public class CourseSelectionModel : PageModel
    {
        private ICourseSelection options;
        public CourseSelectionModel(ICourseSelection options) => this.options = options;

        [BindProperty(SupportsGet =true)]
        public string subject { get; set; }
        public string courseNum { get; set; }

        public void OnGet()
        {
           
        }

        [HttpPost]
        public IActionResult OnPost(List<Course> selCourses)
        {
            var courses = JsonConvert.SerializeObject(selCourses);
            HttpContext.Session.SetString("selectedCourses", courses);
            return RedirectToPage("Availability");
        }

        public JsonResult OnGetCourseSelections()
        {
            return new JsonResult(options.GetCourseSelections(subject));
        }
    }
}