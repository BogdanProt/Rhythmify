using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Rhythmify.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Rhythmify.Data;
using Rhythmify.Models;
using Microsoft.EntityFrameworkCore;

namespace Rhythmify.Controllers
{
    public class SpotifyController : Controller
    {
        private readonly SpotifyService _spotifyService;
        private readonly ApplicationDbContext db;

        private readonly UserManager<User> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;
        public SpotifyController(SpotifyService spotifyService, ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _spotifyService = spotifyService;
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Playlists = GetAllPlaylists();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _spotifyService.SearchTracksAsync(query);
            TempData["Playlists"] = GetAllPlaylists();
            return View("Index", result);
        }
        [HttpPost]
        public async Task<ActionResult> Index(Song song, [FromForm] string playlistId)
        {
            var existingSong = await db.Songs.SingleOrDefaultAsync(s => s.PreviewUrl == song.PreviewUrl);
            var playlist = await db.Playlists.FindAsync(Int32.Parse(playlistId));

            if (playlist != null)
            {
                System.Diagnostics.Debug.WriteLine(song.Title);
                System.Diagnostics.Debug.WriteLine("12312312312321312");

                if (existingSong == null)
                {
                    db.Songs.Add(song);
                    existingSong = song;
                }
                if (playlist.PlaylistSongs == null)
                {
                    playlist.PlaylistSongs = new List<PlaylistSong>();
                    playlist.PlaylistSongs.Add(new PlaylistSong { PlaylistId = playlist.Id, SongId = existingSong.Id, TimeAdded=DateTime.Now });
                    await db.SaveChangesAsync(); // Await the SaveChangesAsync call
                }

                else if (!playlist.PlaylistSongs.Any(ps => ps.SongId == existingSong.Id))
                {
                    playlist.PlaylistSongs.Add(new PlaylistSong { PlaylistId = playlist.Id, SongId = existingSong.Id, TimeAdded=DateTime.Now });
                    await db.SaveChangesAsync(); // Await the SaveChangesAsync call
                }
            }

            return View("Index");

        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllPlaylists()
        {
            var selectList = new List<SelectListItem>();

            var roles = db.Playlists.Where(p => p.User.Id == _userManager.GetUserId(User));

            foreach (var role in roles)
            {

                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Title.ToString()
                });
            }
            return selectList;
        }
    }
}
