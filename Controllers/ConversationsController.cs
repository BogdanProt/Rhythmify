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
        [Authorize]
        public IActionResult Index()
        {
            String userId = _userManager.GetUserId(User);
            List<Conversation> lc = new List<Conversation>();
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

        
        [HttpPost]
        [Authorize]
        public IActionResult New(User user)
        {
            System.Diagnostics.Debug.WriteLine(user.UserName);
            System.Diagnostics.Debug.WriteLine("asdnjasdijsadfsad");
            if (user.UserName != null)
            {
                System.Diagnostics.Debug.WriteLine("intrat lol");
                User receiver = db.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
                String senderId = _userManager.GetUserId(User);
                User sender = null;
                if (senderId != null)
                {
                    System.Diagnostics.Debug.WriteLine("sender not null");
                    sender = db.Users.Find(senderId);
                    Conversation c = new Conversation();
                    c.User1 = sender;
                    c.User2 = receiver;
                    c.User1Id = senderId;

                    if (receiver != null && c.User1Id != c.User2Id)
                    {
                        c.User2Id = receiver.Id;
                        System.Diagnostics.Debug.WriteLine("model valid");
                        Conversation search = db.Conversations.Where(u => (u.User1Id == c.User1Id && u.User2Id == c.User2Id) || (u.User2Id == c.User1Id && u.User1Id == c.User2Id)).FirstOrDefault();
                        if (search == null)
                        {
                            db.Conversations.Add(c);
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                        else c=search; 
                        
                    }
                    if (user.UserName != senderId)
                    {
                        System.Diagnostics.Debug.WriteLine("redirect show");
                        return RedirectToAction("Show", new { id = c.Id });
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("redirect index");
                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

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