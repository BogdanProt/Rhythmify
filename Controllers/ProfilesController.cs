using Microsoft.AspNetCore.Authorization;
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

        // Show afiseaza profilul unui utilizator specific

        [Authorize]
        public IActionResult Show(String id)
        {
            try
            {
                User u = db.Users.Where(u => u.UserName == id).FirstOrDefault();

                // Seteaza ViewBag.Followers cu numarul de conexiuni (followers) ale utilizatorului
                ViewBag.Followers = db.Connections.Where(us => us.FriendId == u.Id).Count();

                // Obtine postarile utilizatorului si seteaza ViewBag.Posts si ViewBag.
                List<Post> posts=db.Posts.Where(p=>p.UserId==u.Id).ToList();
                ViewBag.Posts = posts;
                ViewBag.PostCount = 0;
                ViewBag.PlaylistCount = 0;
                if(posts!=null) ViewBag.PostCount=posts.Count();

                // Obtine playlist-urile utilizatorului si seteaza ViewBag.Playlists si ViewBag.PlaylistCount
                List<Playlist> playlists = db.Playlists.Where(p => p.User.Id == u.Id).ToList();
                if(playlists!=null)
                    ViewBag.PlaylistCount=playlists.Count();
                ViewBag.Playlists = playlists;

                // Verifica daca utilizatorul curent este conectat cu utilizatorul al carui profil este afisat
                Connection c = db.Connections.Where(c => c.FriendId == u.Id && c.UserId == _userManager.GetUserId(User)).FirstOrDefault();
                if (c != null)
                    ViewBag.Connected = true;
                else ViewBag.Connected = false;
                return View(u);
            }
            catch (Exception ex)
            {
                return View(null);
            }
        }
    }
}