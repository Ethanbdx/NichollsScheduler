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
    public class ManualRegisterModel : PageModel
    {
        public static string termName;

        public static List<CourseResult> selectedCourses;

        public IActionResult OnGet()
        {
            if (!HttpContext.Session.Keys.Contains("selectedCourseResults"))
            {
                return RedirectToPage("CourseResults");
            }
            termName = HttpContext.Session.GetString("termName");
            selectedCourses = JsonConvert.DeserializeObject<List<CourseResult>>(HttpContext.Session.GetString("selectedCourseResults"));
            return Page();
        }
    }
}