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
        public static List<CourseModel> selectedCourses;

        public IActionResult OnGet()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("termId")))
            {
               return RedirectToPage("SelectTerm");
            }
            try
            {
                var jsonstring = HttpContext.Session.GetString("selectedCourses");
                selectedCourses = JsonConvert.DeserializeObject<List<CourseModel>>(jsonstring);
                return Page();
            }
            catch
            {
                selectedCourses = new List<CourseModel>();
                return Page();
            }
        }

        [HttpPost]
        public IActionResult OnPost(List<CourseModel> selCourses)
        {
            if (selCourses.Count == 0)
            {
                return Page();
            }
            var selectedCourses = JsonConvert.SerializeObject(selCourses);
            var courseResults = JsonConvert.SerializeObject(BannerScraper.getCourseResults(selCourses, HttpContext.Session.GetString("termId")));
            HttpContext.Session.SetString("selectedCourses", selectedCourses);
            HttpContext.Session.SetString("courseResults", courseResults);
            return RedirectToPage("CourseResults");
        }

        public JsonResult OnGetCourseSelections()
        {
            return new JsonResult(options.GetCourseSelections(subject));
        }
    }
}