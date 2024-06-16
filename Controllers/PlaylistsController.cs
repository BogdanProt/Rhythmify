﻿
using Rhythmify.Models;
using Microsoft.AspNetCore.Mvc;
using Rhythmify.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SpotifyAPI.Web;

namespace Rhythmify.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public PlaylistsController(
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
        public ActionResult Index()
        {
            var playlists = db.Playlists.Include(p => p.User).ToList();
            return View(playlists);
        }

        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Playlist playlist)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                playlist.User = db.Users.Find(userId);
                db.Playlists.Add(playlist);
                db.SaveChanges();
                return RedirectToAction("Show","Profiles",new { id = playlist.User.UserName });
            }

            return View(playlist);
        }

        [Authorize]
        public ActionResult Show(int id)
        {
            Playlist playlist = db.Playlists
                .Include(p => p.User)
    .Include(p => p.PlaylistSongs)
        .ThenInclude(ps => ps.Song)
    .FirstOrDefault(p => p.Id == id);
            return View(playlist);
        }
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            Playlist playlist = db.Playlists.Find(id);
            db.Playlists.Remove(playlist);
            db.SaveChanges();
            TempData["message"] = "Playlist sters";
            String username = _userManager.GetUserName(User);
            return RedirectToAction("Show", "Profiles", new { id = username });
        }
    }
}