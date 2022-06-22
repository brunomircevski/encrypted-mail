using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projekt.Models;
using Projekt.Data;

namespace Projekt.Pages;

public class MessageModel : PageModel
{
    private readonly IDB DB;
    public MessageModel(IDB _DB)
    {
        DB = _DB;
    }

    [BindProperty(SupportsGet = true)]
    public int id { get; set; }

    [BindProperty(SupportsGet = true)]
    public string action { get; set; }

    public IActionResult OnGet()
    {

        if (action == "delete")
        {
            DB.RemoveMessage(id, HttpContext.User.Identity.Name);
            return RedirectToPage("/App/Mailbox");
        }
        return Page();
    }
}
