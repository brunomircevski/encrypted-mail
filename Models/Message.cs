using System.ComponentModel.DataAnnotations;

namespace Projekt.Models;

public class Message
{
    public int id { get; set; }

    [Required]
    public string title { get; set; }

    public string text { get; set; }

    [DataType(DataType.Date)]
    public DateTime date { get; set; }

    public SiteUser sender { get; set; }

    public SiteUser recipient { get; set; }

    public string senderKey { get; set; }

    [Required]
    public string recipientKey { get; set; }

    public Message replyTo { get; set; }

    public Nullable<int> replyToId { get; set; }
}

