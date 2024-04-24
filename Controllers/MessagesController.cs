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

            [HttpPost]
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
            public IActionResult Delete(int id)
            {
                Message mess = db.Messages.Find(id);
                db.Messages.Remove(mess);
                db.SaveChanges();
                return Redirect("/Conversations/Show/" + mess.ConversationID);
            }

            // In acest moment vom implementa editarea intr-o pagina View separata
            // Se editeaza un mesaj existent

            public IActionResult Edit(int id)
            {
                Message mess = db.Messages.Find(id);
                ViewBag.Message = mess;
                return View();
            }

            [HttpPost]
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

