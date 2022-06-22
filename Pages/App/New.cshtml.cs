using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Projekt.Models;
using Projekt.Data;

namespace Projekt.Pages;

public class NewMessageModel : PageModel
{
    private readonly IDB DB;
    public NewMessageModel(IDB _DB)
    {
        DB = _DB;
    }

    [BindProperty]
    public Message message { get; set; }

    [BindProperty]
    public bool addToContacts { get; set; }

    [BindProperty(SupportsGet = true)]
    public int replyTo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string username { get; set; }

    public bool error = false;

    public IActionResult OnPost()
    {
        if (message.senderKey is not null)
            message.sender = new SiteUser { username = HttpContext.User.Identity.Name };

        if (addToContacts)
            DB.AddContact(HttpContext.User.Identity.Name, message.recipient.username);

        if (DB.SendMessage(message))
            return RedirectToPage("/App/Mailbox");
        else
        {
            error = true;
            return Page();
        }
    }
}
