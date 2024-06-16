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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Playlists = GetAllPlaylists();
            return View();
        }

        // Search cu metoda POST cauta melodii pe Spotify

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _spotifyService.SearchTracksAsync(query);
            TempData["Playlists"] = GetAllPlaylists();
            return View("Index", result);
        }

        // Index cu metoda POST adauga o melodie intr-un playlist

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Index(Song song, [FromForm] string playlistId)
        {

            // Cauta melodia existenta in baza de date dupa PreviewUrl
            var existingSong = await db.Songs.SingleOrDefaultAsync(s => s.PreviewUrl == song.PreviewUrl);

            // Cauta playlist-ul specificat si include relatiile necesare
            var playlist = await db.Playlists
                .Include(p => p.PlaylistSongs)
                    .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == Int32.Parse(playlistId));

            if (playlist != null)
            {
                System.Diagnostics.Debug.WriteLine(song.Title);
                System.Diagnostics.Debug.WriteLine("12312312312321312");

                if (existingSong == null)
                {
                    db.Songs.Add(song);
                    await db.SaveChangesAsync();
                    existingSong = song;
                }
                else
                {
                    // Ataseaza melodia existenta la context daca nu este deja atasata
                    if (!db.Entry(existingSong).IsKeySet)
                    {
                        db.Songs.Attach(existingSong);
                    }
                }

                // Initializeaza colectia PlaylistSongs daca este necesar
                if (playlist.PlaylistSongs == null)
                {
                    playlist.PlaylistSongs = new List<PlaylistSong>();
                }

                // Adauga melodia in playlist daca nu este deja adaugata
                if (!playlist.PlaylistSongs.Any(ps => ps.SongId == existingSong.Id))
                {
                    playlist.PlaylistSongs.Add(new PlaylistSong { PlaylistId = playlist.Id, SongId = existingSong.Id, TimeAdded=DateTime.Now });
                    await db.SaveChangesAsync(); // Await the SaveChangesAsync call
                }
            }

            return View("Index");

        }

        // Selecteaza toate playlisturile utilizatorului
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
