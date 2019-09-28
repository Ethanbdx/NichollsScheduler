using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NichollsScheduler.Logic;

namespace NichollsScheduler.Pages
{
    public class SelectTermModel : PageModel
    {
        public string termId { get; set; }
        public static Dictionary<string,string> availableTerms;

        public async Task<IActionResult> OnGetAsync(BannerService client)
        { 
            availableTerms = await client.getTerms();
            return Page();
        }

        [HttpPost]
        public IActionResult OnPost(string termId)
        {
            if (termId == null)
            {
                return Page();
            }
            if(termId.Length < 0)
            {
                return Page();
            }
            HttpContext.Session.SetString("TermId", termId);
            return RedirectToPage("Availability");
        }
    }
}