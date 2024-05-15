
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
        public IActionResult Index()
        {
            var posts = db.Posts;

            // ViewBag.OriceDenumireSugestiva
            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public IActionResult Show(int id)
        {
            Post post = db.Posts
                              .Where(p => p.Id == id)
                              .First();
            //adauga comentariile de la postare

            return View(post);
        }

        /*[HttpPost]
        public IActionResult Show([FromForm] Comment comm)
        {
            comm.Date = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Articles/Show/" + comm.ArticleId);

            }

            else
            {
                Article art = db.Articles.Include("Category").Include("Comments")
                              .Where(art => art.Id == comm.ArticleId).First();

                return View(art);
            }
        }*/

        // Se afiseaza formularul in care se vor completa datele unui articol
        // impreuna cu selectarea categoriei din care face parte
        // HttpGet implicit

        public IActionResult New()
        {

            Post post = new Post();

            return View(post);
        }

        // Se adauga articolul in baza de date
        [HttpPost]
        public IActionResult New(Post post)
        {
            post.Timestamp= DateTime.Now;
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

        public IActionResult Edit(int id)
        {
            
                Post post = db.Posts
                                             .Where(p => p.Id == id)
                                             .First();

                return View(post);

        }

        // Se adauga articolul modificat in baza de date
        [HttpPost]
        public IActionResult Edit(int id, Post rqpost)
        {
            Post post = db.Posts.Find(id);
            if (ModelState.IsValid)
            {
                post.Content = rqpost.Content;
                TempData["message"] = "Postarea a fost modificata!!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View(rqpost);
            }
        }

        [HttpPost]
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

