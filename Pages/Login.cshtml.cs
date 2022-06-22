using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Projekt.Models;
using Projekt.Data;

namespace Projekt;

public class LoginModel : PageModel
{
    private readonly IDB DB;
    public LoginModel(IDB _DB)
    {
        DB = _DB;
    }

    [BindProperty]
    public SiteUser user { get; set; }

    [BindProperty(SupportsGet = true)]
    public int success { get; set; }

    public bool error = false;

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity.IsAuthenticated) return RedirectToPage("/App/Mailbox");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid == false)
        {
            error = true;
            return Page();
        }

        if (DB.ValidateUser(user))
        {
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, user.username)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuthentication");
            await HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(claimsIdentity));

            return RedirectToPage("/PostLoginSetup");
        }
        error = true;
        return Page();
    }
}
