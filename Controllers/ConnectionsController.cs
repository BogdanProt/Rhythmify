using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rhythmify.Models;

public class ConnectionsController : Controller
{
    private readonly IConnectionService _connectionService;
    private readonly UserManager<User> _userManager;

    public ConnectionsController(IConnectionService connectionService, UserManager<User> userManager)
    {
        _connectionService = connectionService;
        _userManager = userManager;
    }

    [Authorize]
    public IActionResult Search()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Search(string searchTerm)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var users = await _connectionService.SearchUsersAsync(searchTerm);
        var connections = await _connectionService.GetConnectionsAsync(currentUser.Id);

        var model = users.Select(u => new UserSearchResult(u, connections.Any(c => c.Id == u.Id))).ToList();

        return View(model);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddConnection(string friendId)
    {
        var user = await _userManager.GetUserAsync(User);
        await _connectionService.AddConnectionAsync(user.Id, friendId);
        return RedirectToAction("Index", "Connections");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveConnection(string friendId)
    {
        var user = await _userManager.GetUserAsync(User);
        await _connectionService.RemoveConnectionAsync(user.Id, friendId);
        return RedirectToAction("Index", "Connections");
    }


    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var connections = await _connectionService.GetConnectionsAsync(user.Id);
        return View(connections);
    }
}

public class UserSearchResult
{
    public User User { get; set; }
    public bool IsConnected { get; set; }

    public UserSearchResult(User user, bool isConnected)
    {
        User = user;
        IsConnected = isConnected;
    }
}
