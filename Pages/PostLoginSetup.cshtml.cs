using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Projekt.Models;
using Projekt.Data;

namespace Projekt;

public class PostLoginSetupModel : PageModel
{
    private readonly IDB DB;
    public PostLoginSetupModel(IDB _DB)
    {
        DB = _DB;
    }

    public string secret = null;
    public string publicKey = null;

    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            SiteUser user = DB.GetUserByUsername(HttpContext.User.Identity.Name);
            if (user is not null)
            {
                secret = user.privateKeyEncrypted;
                publicKey = user.publicKey;
            }
        }
        return Page();
    }
}
