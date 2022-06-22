using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Projekt.Models;
using Projekt.Data;

namespace Projekt;

public class RegisterModel : PageModel
{
    private readonly IDB DB;
    public RegisterModel(IDB _DB)
    {
        DB = _DB;
    }

    [BindProperty]
    public SiteUser user { get; set; }

    public bool error = false;

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("/App/Mailbox");
        return Page();
    }

    public IActionResult OnPost()
    {
        if (ModelState.IsValid == false)
        {
            error = true;
            return Page();
        }

        if (!DB.RegisterNewUser(user))
        {
            error = true;
            return Page();
        }

        return RedirectToPage("/Login", new { success = 1 });
    }
}
