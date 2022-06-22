using Projekt.Models;
using Microsoft.EntityFrameworkCore;

namespace Projekt.Data;

public interface IDB
{
    public bool ValidateUser(SiteUser user);
    public bool RegisterNewUser(SiteUser user);
    public SiteUser GetUserByUsername(string username);
    public bool SendMessage(Message m);
    public List<Message> GetMessagesByUsername(string username);
    public Message GetMessage(int id, string username);
    public void RemoveMessage(int id, string username);
    public bool AddContact(string username, string username2);
    public List<SiteUser> GetContacts(string username);
    public void RemoveContact(string username, string username2);
    public int GetNewestMessageIdByUsername(string username);
    public List<Message> GetSentMessagesByUsername(string username);
}
