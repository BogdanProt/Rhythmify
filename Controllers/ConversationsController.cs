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
        /*[HttpPost]
        public IActionResult New(string id)
        {
            User receiver= db.Users.Where(u=>u.DisplayName==id).FirstOrDefault();
            String senderId = _userManager.GetUserId(User);
            User sender = null;
            if (senderId!=null)
            {
                sender = db.Users.Find(senderId);
                Conversation c=new Conversation();
                c.User1 = sender;
                c.User2 = receiver;
                c.User1Id = senderId;
                c.User2Id = receiver.Id;
                if(ModelState.IsValid)
                {
                    db.SaveChanges();
                    RedirectToAction("Show", id);
                }
                if (id != senderId)
                {
                    RedirectToAction("Show", id);
                }
                else RedirectToAction("Index");
            }

            return View();
        }*/

        [HttpPost]
        public IActionResult New(User user)
        {
            System.Diagnostics.Debug.WriteLine(user.DisplayName);
            System.Diagnostics.Debug.WriteLine("asdnjasdijsadfsad");
            if (user.DisplayName != null)
            {
                System.Diagnostics.Debug.WriteLine("intrat lol");
                User receiver = db.Users.Where(u => u.DisplayName == user.DisplayName).FirstOrDefault();
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
                    if (user.DisplayName != senderId)
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
        public IActionResult New()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Conversation currentConversation = db.Conversations.Include("Messages").Include(c => c.User1)
    .Include(c => c.User2).Where(c => c.Id == id).FirstOrDefault();
            if (currentConversation != null)
            {
                //if (currentConversation.User1Id == _userManager.GetUserId(User))
                //{
                //    ViewBag.Sender = db.Users.Find(currentConversation.User1Id);
                //    ViewBag.Receiver = db.Users.Find(currentConversation.User2Id);
                //}
                //else
                //{
                //    ViewBag.Sender = db.Users.Find(currentConversation.User2Id);
                //    ViewBag.Receiver = db.Users.Find(currentConversation.User1Id);
                //}
                //ViewBag.Conversation = currentConversation;
                return View(currentConversation);
            }
            return View(null);
        }
        [HttpPost]
        public IActionResult Show([FromForm] Message msg)
        {
            msg.Timestamp = DateTime.Now;
            msg.Sender = db.Users.Find(_userManager.GetUserId(User));
            System.Diagnostics.Debug.WriteLine(msg.Content);
            System.Diagnostics.Debug.WriteLine(msg.ConversationID);
            Conversation currentConversation = db.Conversations.Include("Messages").Include(c => c.User1)
    .Include(c => c.User2).Where(c => c.Id == msg.ConversationID).First();

            if (ModelState.IsValid)
            {
                db.Messages.Add(msg);
                db.SaveChanges();
            }
            return View(currentConversation);
        }
    }
}