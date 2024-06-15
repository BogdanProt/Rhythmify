using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using Rhythmify.Services;
using Microsoft.AspNetCore.Mvc;


namespace Rhythmify.Controllers
{
    public class SpotifyController : Controller
    {
        private readonly SpotifyService _spotifyService;

        public SpotifyController(SpotifyService spotifyService)
        {
            _spotifyService = spotifyService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            var result = await _spotifyService.SearchTracksAsync(query);
            return View("Index", result);
        }
    }
}
