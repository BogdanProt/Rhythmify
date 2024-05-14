using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
                User u = db.Users.Find(id);
                return View(u);
            }
            catch (Exception ex)
            {
                return View(null);
            }
        }
    }
}
