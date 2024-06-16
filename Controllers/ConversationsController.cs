using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Data;
using Rhythmify.Models;

namespace Rhythmify.Controllers
{
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

        // Index afiseaza conversatiile utilizatorului curent

        [Authorize]
        public IActionResult Index()
        {
            String userId = _userManager.GetUserId(User);
            List<Conversation> lc = new List<Conversation>();

            // Daca utilizatorul curent este autentificat, obtine conversatiile acestuia
            if (userId != null)
                lc = db.Conversations.Where(u => u.User1Id == userId || u.User2Id == userId).ToList();
            ViewBag.Conversations = lc;
            ViewBag.UserId = userId;
            List<User> receiverUsers = new List<User>();
            foreach (Conversation c in lc)
            {
                if (c.User1Id != userId) receiverUsers.Add(db.Users.Find(c.User1Id));
                else receiverUsers.Add(db.Users.Find(c.User2Id));
            }
            ViewBag.ReceiverUsers = receiverUsers;
            return View();
        }


        // New cu metoda POST adauga o noua conversatie

        [HttpPost]
        [Authorize]
        public IActionResult New(User user)
        {
            System.Diagnostics.Debug.WriteLine(user.UserName);
            if (user.UserName != null)
            {
                User receiver = db.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
                String senderId = _userManager.GetUserId(User);
                User sender = null;
                if (senderId != null)
                {
                    sender = db.Users.Find(senderId);
                    Conversation c = new Conversation();
                    c.User1 = sender;
                    c.User2 = receiver;
                    c.User1Id = senderId;

                    // Adauga conversatia doar daca utilizatorul destinatar este valid si nu exista deja o conversatie

                    if (receiver != null && c.User1Id != c.User2Id)
                    {
                        c.User2Id = receiver.Id;
                        Conversation search = db.Conversations.Where(u => (u.User1Id == c.User1Id && u.User2Id == c.User2Id) || (u.User2Id == c.User1Id && u.User1Id == c.User2Id)).FirstOrDefault();
                        if (search == null)
                        {
                            db.Conversations.Add(c);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else c=search; 
                        
                    }
                    // Redirectioneaza catre conversatia nou creata sau afiseaza un mesaj de eroare

                    if (user.UserName != senderId && receiver!=null)
                    {
                        return RedirectToAction("Show", new { id = c.Id });
                    }
                    else
                    {
                        TempData["message"] = "Nu exista user-ul " +user.UserName;
                        return View();
                    }
                }
            }
            return View();
        }

        // New cu metoda GET afiseaza formularul pentru adaugarea unei noi conversatii

        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

        // Show cu metoda GET afiseaza o conversatie specifica

        [HttpGet]
        [Authorize]
        public IActionResult Show(int id)
        {
            Conversation currentConversation = db.Conversations.Include("Messages").Include(c => c.User1)
    .Include(c => c.User2).Where(c => c.Id == id).FirstOrDefault();
            if (currentConversation != null)
            {
                return View(currentConversation);
            }
            return View(null);
        }

        // Show cu metoda POST adauga un mesaj nou in conversatie

        [HttpPost]
        [Authorize]
        public IActionResult Show([FromForm] Message msg)
        {
            msg.Timestamp = DateTime.Now;
            msg.Sender = db.Users.Find(_userManager.GetUserId(User));
            System.Diagnostics.Debug.WriteLine(msg.Content);
            System.Diagnostics.Debug.WriteLine(msg.ConversationID);

            if (ModelState.IsValid)
            {
                db.Messages.Add(msg);
                db.SaveChanges();
            }

            // Reincarca conversatia curenta pentru a include noul mesaj

            Conversation currentConversation = db.Conversations
                .Include(c => c.Messages)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .Where(c => c.Id == msg.ConversationID)
                .FirstOrDefault();

            if (currentConversation != null)
            {
                currentConversation.Messages = currentConversation.Messages
                    .OrderByDescending(m => m.Timestamp)
                    .ToList();
            }

            return View(currentConversation);
        }
    }
}