using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projekt.Models;
using Projekt.Data;

namespace Projekt.Pages;

public class ContactsModel : PageModel
{
    private readonly IDB DB;
    public ContactsModel(IDB _DB)
    {
        DB = _DB;
    }

    [BindProperty(SupportsGet = true)]
    public string username { get; set; }

    public List<SiteUser> contacts = null;

    public IActionResult OnGet()
    {
        if (username is not null)
        {
            DB.RemoveContact(HttpContext.User.Identity.Name, username);
            return RedirectToPage("/App/Contacts");
        }

        contacts = DB.GetContacts(HttpContext.User.Identity.Name);
        return Page();
    }
}
