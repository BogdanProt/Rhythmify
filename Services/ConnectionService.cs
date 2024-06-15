using Rhythmify.Data;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Models;

public interface IConnectionService
{
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    Task AddConnectionAsync(string userId, string friendId);
    Task RemoveConnectionAsync(string userId, string friendId); // Adaugă această linie
    Task<IEnumerable<User>> GetConnectionsAsync(string userId);
}


public class ConnectionService : IConnectionService
{
    private readonly ApplicationDbContext _context;

    public ConnectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        return await _context.Users
            .Where(u => u.UserName.Contains(searchTerm) || u.DisplayName.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task AddConnectionAsync(string userId, string friendId)
    {
        var existingConnection = await _context.Connections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.FriendId == friendId);

        if (existingConnection == null)
        {
            var connection = new Connection
            {
                UserId = userId,
                FriendId = friendId
            };
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveConnectionAsync(string userId, string friendId)
    {
        var connection = await _context.Connections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.FriendId == friendId);

        if (connection != null)
        {
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<User>> GetConnectionsAsync(string userId)
    {
        var connections = await _context.Connections
            .Where(c => c.UserId == userId)
            .Include(c => c.Friend)
            .ToListAsync();

        return connections.Select(c => c.Friend);
    }
}




