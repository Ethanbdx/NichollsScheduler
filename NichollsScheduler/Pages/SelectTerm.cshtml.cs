using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NichollsScheduler.Pages
{
    public class SelectTermModel : PageModel
    {
        public string termId { get; set; }
        public static Dictionary<string,string> availableTerms;

        public async Task<IActionResult> OnGetAsync()
        {
            availableTerms = await Startup.webScraper.getTerms();
            return Page();
        }

        [HttpPost]
        public IActionResult OnPost(string termId)
        {
            if(termId.Length < 0)
            {
                return Page();
            }
            return RedirectToPage("CourseSelection", new { termId });
        }
    }
}