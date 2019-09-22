using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NichollsScheduler.Pages
{
    public class IndexModel : PageModel
    {
        public string termId;
        public void OnGet(string _termId)
        {
            termId = _termId;
        }
    }
}
