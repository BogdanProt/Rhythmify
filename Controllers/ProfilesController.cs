using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Rhythmify.Data;
using Rhythmify.Models;

namespace Rhythmify.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfilesController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        public IActionResult Show(String id)
        {
            try
            {
                User u = db.Users.Where(u => u.DisplayName == id).FirstOrDefault();
                ViewBag.Followers = db.Connections.Where(us => us.FriendId == u.Id).Count();
                List<Post> posts=db.Posts.Where(p=>p.UserId==u.Id).ToList();
                ViewBag.Posts = posts;
                ViewBag.PostCount = 0;
                ViewBag.PlaylistCount = 0;
                if(posts!=null) ViewBag.PostCount=posts.Count();
                List<Playlist> playlists = db.Playlists.Where(p => p.User.Id == u.Id).ToList();
                if(playlists!=null)
                    ViewBag.PlaylistCount=playlists.Count();
                ViewBag.Playlists = playlists;
                return View(u);
            }
            catch (Exception ex)
            {
                return View(null);
            }
        }
    }
}