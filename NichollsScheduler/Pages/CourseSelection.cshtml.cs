using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NichollsScheduler.Pages
{
    public class CourseSelectionModel : PageModel
    {
        public static string term { get; set; }

        public void OnGet(string termId)
        {
            term = termId;
        }
    }
}