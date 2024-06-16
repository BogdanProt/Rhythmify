using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rhythmify.Data;
using Rhythmify.Models;

namespace Rhythmify.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext db;

        public MessagesController(ApplicationDbContext context)
        {
            db = context;
        }

        // New cu metoda POST adauga un mesaj nou

        [HttpPost]
        [Authorize]
        public IActionResult New(Message mess)
        {
            mess.Timestamp = DateTime.Now;

            try
            {
                db.Messages.Add(mess);
                db.SaveChanges();
                return Redirect("/Conversations/Show/" + mess.ConversationID);
            }

            catch (Exception)
            {
                return Redirect("/Conversations/Show/" + mess.ConversationID);
            }

        }

        // Stergerea unui mesaj asociat unui canal din baza de date
        [HttpPost]
        [Authorize]
        public IActionResult Delete(int id)
        {
            Message mess = db.Messages.Find(id);
            db.Messages.Remove(mess);
            db.SaveChanges();
            return Redirect("/Conversations/Show/" + mess.ConversationID);
        }


        // Afisarea formularului de editare pentru un mesaj

        [Authorize]
        public IActionResult Edit(int id)
        {
            Message mess = db.Messages.Find(id);
            ViewBag.Message = mess;
            return View();
        }

        // Edit cu metoda POST actualizeaza un mesaj existent

        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, Message requestMessage)
        {
            Message mess = db.Messages.Find(id);
            try
            {

                mess.Content = requestMessage.Content;

                db.SaveChanges();

                return Redirect("/Conversations/Show/" + mess.ConversationID);
            }
            catch (Exception e)
            {
                return Redirect("/Conversation/Show/" + mess.ConversationID);
            }

        }
    }
}
