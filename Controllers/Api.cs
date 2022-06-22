using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Projekt.Data;
using Projekt.Models;

namespace Projekt.Controllers
{
    [Authorize]
    public class Api : Controller
    {
        private readonly IDB DB;
        public Api(IDB _DB)
        {
            DB = _DB;
        }

        JsonSerializerOptions JsonOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public string GetPublicKey(string username)
        {
            if (username is null) return null;

            SiteUser user = DB.GetUserByUsername(username);
            if (user is not null) return user.publicKey;

            return null;
        }

        public JsonResult GetMyMessages()
        {
            List<Message> messages = DB.GetMessagesByUsername(HttpContext.User.Identity.Name);

            return Json(messages, JsonOptions);
        }

        public JsonResult GetMessage(int id)
        {
            Message m = DB.GetMessage(id, HttpContext.User.Identity.Name);

            return Json(m, JsonOptions);
        }

        public int GetNewestMessageId() {
            return DB.GetNewestMessageIdByUsername(HttpContext.User.Identity.Name);
        }

        public JsonResult GetSentMessages()
        {
            List<Message> messages = DB.GetSentMessagesByUsername(HttpContext.User.Identity.Name);

            return Json(messages, JsonOptions);
        }
    }
}