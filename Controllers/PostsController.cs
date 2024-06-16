
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Data;
using Rhythmify.Models;

namespace ArticlesApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(
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
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var connections = await db.Connections
                                      .Where(c => c.UserId == currentUser.Id)
                                      .Select(c => c.FriendId)
                                      .ToListAsync();

            // Include postarile utilizatorului curent
            connections.Add(currentUser.Id);

            var posts = await db.Posts
                                .Include(p => p.User)
                                .Where(p => connections.Contains(p.UserId))
                                .OrderByDescending(p => p.Timestamp)
                                .AsNoTracking()
                                .ToListAsync();

            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }



        [Authorize]
        public async Task<IActionResult> Show(int id)
        {
            var post = await db.Posts
                               .Include(p => p.User)
                               .Include(p => p.Comments)
                               .ThenInclude(c => c.User)
                               .AsNoTracking()
                               .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var likes = await db.Likes.Where(l => l.PostId == id).CountAsync();
            var userLiked = await db.Likes.AnyAsync(l => l.PostId == id && l.UserId == userId);

            ViewBag.Likes = likes;
            ViewBag.UserLiked = userLiked;

            return View(post);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeRequest request)
        {
            var userId = _userManager.GetUserId(User);
            var existingLike = await db.Likes.FirstOrDefaultAsync(l => l.PostId == request.PostId && l.UserId == userId);

            if (existingLike == null)
            {
                var like = new Like { PostId = request.PostId, UserId = userId };
                db.Likes.Add(like);
            }
            else
            {
                db.Likes.Remove(existingLike);
            }

            await db.SaveChangesAsync();

            var likesCount = await db.Likes.CountAsync(l => l.PostId == request.PostId);
            return Json(new { likes = likesCount, liked = existingLike == null });
        }

        public class ToggleLikeRequest
        {
            public int PostId { get; set; }
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            var userId = _userManager.GetUserId(User);

            var comment = new Comment
            {
                Content = content,
                Timestamp = DateTime.Now,
                PostId = postId,
                UserId = userId
            };

            db.Comments.Add(comment);
            await db.SaveChangesAsync();

            return RedirectToAction("Show", new { id = postId });
        }

        [Authorize]
        public IActionResult New()
        {

            Post post = new Post();

            return View(post);
        }

        // Se adauga articolul in baza de date
        [HttpPost]
        [Authorize]
        public IActionResult New(Post post)
        {
            post.Timestamp = DateTime.Now;
            post.User = db.Users.Find(_userManager.GetUserId(User));

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();

                TempData["message"] = "Postarea a fost adaugata!!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await db.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (post.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(post);
        }

        // Se adauga articolul modificat in baza de date
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Post rqpost)
        {
            var post = await db.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (post.UserId != currentUser.Id)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                post.Content = rqpost.Content;
                TempData["message"] = "Postarea a fost modificata!!";
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(rqpost);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            TempData["message"] = "Postarea a fost stearsa";
            return RedirectToAction("Index");
        }
    }
}
