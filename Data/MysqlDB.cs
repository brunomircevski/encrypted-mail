using Microsoft.EntityFrameworkCore;
using Projekt.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Projekt.Data;

public class MysqlDB : DbContext, IDB
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(connectionString: @"Data Source=localhost;port=3306;Initial Catalog=Projekt;User Id=net;password=root",
            new MySqlServerVersion(new Version(10, 6, 7)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var users = modelBuilder.Entity<SiteUser>();
        users.HasMany(x => x.myContacts).WithMany(x => x.meInOthersContacts);
        users.Ignore(x => x.passwordHash);
        users.Property(x => x.passwordHashWithSalt).IsRequired();
        users.Property(x => x.salt).IsRequired();
        users.Property(x => x.salt).HasColumnType("blob");
        users.Property(x => x.publicKey).IsRequired();
        users.Property(x => x.privateKeyEncrypted).IsRequired();

        var messages = modelBuilder.Entity<Message>();
        messages.HasOne(x => x.sender).WithMany(x => x.sendMessages);
        messages.HasOne(x => x.recipient).WithMany(x => x.receivedMessages);
        messages.Ignore(x => x.replyToId);

    }

    public DbSet<SiteUser> Users { get; set; }
    public DbSet<Message> Messages { get; set; }

    public bool ValidateUser(SiteUser user)
    {
        SiteUser DBuser = Users.Where(x => x.username == user.username).FirstOrDefault();

        if (DBuser is not null)
        {
            user.passwordHashWithSalt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.passwordHash,
                salt: DBuser.salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (user.passwordHashWithSalt == DBuser.passwordHashWithSalt) return true;
        }

        return false;
    }

    public bool RegisterNewUser(SiteUser user)
    {
        if (Users.Where(x => x.username == user.username).FirstOrDefault() is not null)
        {
            return false;
        }

        user.salt = new byte[128 / 8];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(user.salt);
        }

        user.passwordHashWithSalt = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: user.passwordHash,
            salt: user.salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        Users.Add(user);
        SaveChanges();

        return true;
    }

    public SiteUser GetUserByUsername(string username)
    {
        SiteUser DBuser = Users.Where(x => x.username == username).FirstOrDefault();
        if (DBuser is not null) return DBuser;

        return null;
    }

    public bool SendMessage(Message m)
    {
        m.sender = Users.Where(x => x.username == m.sender.username).FirstOrDefault();
        m.recipient = Users.Where(x => x.username == m.recipient.username).FirstOrDefault();
        m.date = DateTime.Now;

        if (m.replyToId != 0) m.replyTo = Messages.Where(x => x.id == m.replyToId).FirstOrDefault();

        if (m.recipient is null) return false;

        Messages.Add(m);
        SaveChanges();

        return true;
    }

    public List<Message> GetMessagesByUsername(string username)
    {
        SiteUser user = GetUserByUsername(username);
        if (user is null) return null;

        List<Message> messages = Messages
        .Include(x => x.recipient).Include(x => x.sender)
        .Where(x => x.recipient.id == user.id)
        .OrderByDescending(x => x.date).Take(20)
        .Select(x => new Message
        {
            id = x.id,
            title = x.title,
            text = x.text,
            date = x.date,
            recipientKey = x.recipientKey,
            sender = new SiteUser
            {
                username = x.sender.username,
                publicKey = x.sender.publicKey
            },
            replyToId = x.replyTo.id
        }).ToList();

        return messages;
    }

    public Message GetMessage(int id, string username)
    {
        Message m = Messages.Include(x => x.recipient).Include(x => x.sender)
        .Where(x => x.id == id)
        .Select(x => new Message
        {
            id = x.id,
            title = x.title,
            text = x.text,
            date = x.date,
            recipientKey = x.recipientKey,
            senderKey = x.senderKey,
            sender = new SiteUser
            {
                username = x.sender.username,
                publicKey = x.sender.publicKey
            },
            recipient = new SiteUser
            {
                username = x.recipient.username,
            },
            replyToId = x.replyTo.id
        }).FirstOrDefault();

        if (m is null) return null;

        if (m.sender.username != username && m.recipient.username != username) return null;

        if (m.sender.username == username)
        {
            m.recipientKey = null;
        }
        else
        {
            m.senderKey = null;
        }

        return m;
    }

    public void RemoveMessage(int id, string username)
    {
        while(true) {
            Message reply = Messages.Include(x => x.replyTo).Where(x => x.replyTo.id == id).FirstOrDefault();
            if(reply is not null) {
                reply.replyTo = null;
                SaveChanges();
            }
            else break;
        }

        Remove(Messages.Single(x => x.id == id && (x.recipient.username == username || x.sender.username == username)));
        SaveChanges();
    }

    public bool AddContact(string username, string username2)
    {
        SiteUser u1 = Users.Include(x => x.myContacts).Single(x => x.username == username);
        SiteUser u2 = u1.myContacts.FirstOrDefault(x => x.username == username2);

        if (u2 is null)
        {
            Users.Single(x => x.username == username).myContacts.Add(Users.Single(x => x.username == username2));
            SaveChanges();
            return true;
        }

        return false;
    }

    public List<SiteUser> GetContacts(string username)
    {
        return Users.Include(x => x.myContacts).Single(x => x.username == username).myContacts;
    }

    public void RemoveContact(string username, string username2)
    {
        //Remove(Users.Include(x => x.myContacts).Single(x => x.username == username).myContacts.Single(x => x.username == username2));
        //SaveChanges();
        int id1 = GetUserByUsername(username).id;
        int id2 = GetUserByUsername(username2).id;
        Database.ExecuteSqlRaw("DELETE FROM SiteUserSiteUser WHERE meInOthersContactsid={0} AND myContactsid={1}", id1, id2);
    }

    public int GetNewestMessageIdByUsername(string username)
    {
        SiteUser user = GetUserByUsername(username);
        if (user is null) return 0;

        return Messages
        .Include(x => x.recipient).Include(x => x.sender)
        .Where(x => x.recipient.id == user.id)
        .OrderByDescending(x => x.date).Take(1)
        .Select(x => x.id).FirstOrDefault();
    }

    public List<Message> GetSentMessagesByUsername(string username)
    {
        SiteUser user = GetUserByUsername(username);
        if (user is null) return null;

        List<Message> messages = Messages
        .Include(x => x.recipient).Include(x => x.sender)
        .Where(x => x.sender.id == user.id)
        .OrderByDescending(x => x.date).Take(20)
        .Select(x => new Message
        {
            id = x.id,
            title = x.title,
            text = x.text,
            date = x.date,
            recipient = new SiteUser {
                username = x.recipient.username
            },
            senderKey = x.senderKey,
            replyToId = x.replyTo.id
        }).ToList();

        return messages;
    }

}
