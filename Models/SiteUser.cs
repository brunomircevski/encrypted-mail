using System.ComponentModel.DataAnnotations;

namespace Projekt.Models;

public class SiteUser
{
    public int id { get; set; }

    [Display(Name = "Username")]
    [RegularExpression(@"^[a-zA-Z0-9_.-]*$")]
    [Required]
    public string username { get; set; }

    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    [Required]
    public string passwordHash { get; set; }

    public string passwordHashWithSalt { get; set; }

    public byte[] salt { get; set; }
    
    public string publicKey { get; set; }
    
    public string privateKeyEncrypted { get; set; }

    public List<SiteUser> myContacts { get; set; }

    public List<SiteUser> meInOthersContacts { get; set; }

    public List<Message> sendMessages { get; set; }

    public List<Message> receivedMessages { get; set; }
}
