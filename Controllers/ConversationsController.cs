
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Data;
using Rhythmify.Models;
using System;
using System.Threading.Channels;

namespace Rhythmify.Controllers
{
    [Authorize]
    public class ConversationsController : Controller
    {

        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ConversationsController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        // Se afiseaza lista tuturor canalelor impreuna cu categoria 
        // din care fac parte
        // Pentru fiecare canal se afiseaza si userul care a postat canalul respectiv
        // HttpGet implicit
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            var conversations = db.Conversations.Include("User1").Include("User2")
            .OrderBy(a => a.User2.DisplayName + a.User1.DisplayName);

            ViewBag.Conversations = conversations;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
                ViewBag.Alert = TempData["messageType"];
            }

            return View();
        }

        // Se afiseaza un singur canal in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate mesajele asociate unui canal
        // Se afiseaza si userul care a postat canalul respectiv
        // HttpGet implicit

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Conversation conversation = db.Conversations.Include("User1").Include("User2")
                                         .Include("Messages")
                                         .Where(ch => ch.Id == id && (ch.User1.Id== _userManager.GetUserId(User) || ch.User2.Id== _userManager.GetUserId(User)))
                                         .First();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(conversation);
        }


        // Adaugarea unui mesaj asociat unui canal in baza de date
        // Toate rolurile pot adauga mesaje in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show([FromForm] Message message)
        {
            message.Timestamp = DateTime.Now;
            message.Sender = db.Users.Where(u => u.Id == _userManager.GetUserId(User)).FirstOrDefault();

            if (ModelState.IsValid)
            {
                db.Messages.Add(message);
                db.SaveChanges();
                return Redirect("/Conversations/Show/" + message.ConversationID);
            }

            else
            {
                Conversation conversation = db.Conversations.Include("User1").Include("User2")
                                         .Include("Messages")
                                         .Where(ch => ch.Id == message.ConversationID && (ch.User1.Id == _userManager.GetUserId(User) || ch.User2.Id == _userManager.GetUserId(User)))
                                         .First();

                return View(conversation);
            }
        }

        // Se creeaza canalul
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New()
        {
            Conversation conversation = new Conversation();

            return View(conversation);
        }

        // Se adauga canalul in baza de date
        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public async Task<ActionResult> New(Conversation conversation)
        {
            // preluam id-ul utilizatorului care creeaza canalul
            conversation.User1Id = _userManager.GetUserId(User);
            System.Diagnostics.Debug.WriteLine(conversation.UserName);
            conversation.User2 = db.Users.Where(u=>u.UserName==conversation.UserName).FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(conversation.User2.UserName);
            System.Diagnostics.Debug.WriteLine("fail");
            conversation.User2Id = db.Users.Where(u => u.UserName == conversation.UserName).FirstOrDefault().Id;

            if (ModelState.IsValid)
            {
                db.SaveChanges();
                TempData["message"] = "Conversatia a fost creata!";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                return View(conversation);
            }
        }

    }
}
