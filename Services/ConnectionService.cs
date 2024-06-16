using Rhythmify.Data;
using Microsoft.EntityFrameworkCore;
using Rhythmify.Models;

public interface IConnectionService
{
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm); // Interfata pentru cautarea utilizatorilor
    Task AddConnectionAsync(string userId, string friendId); // Interfata pentru adaugarea unei conexiuni
    Task RemoveConnectionAsync(string userId, string friendId); // Interfata pentru eliminarea unei conexiuni
    Task<IEnumerable<User>> GetConnectionsAsync(string userId); // Interfata pentru obtinerea conexiunilor unui utilizator
}


public class ConnectionService : IConnectionService
{
    private readonly ApplicationDbContext _context;

    public ConnectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Metoda pentru cautarea utilizatorilor pe baza termenului de cautare

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        return await _context.Users
            .Where(u => u.UserName.Contains(searchTerm) || u.DisplayName.Contains(searchTerm))
            .ToListAsync();
    }

    // Metoda pentru adaugarea unei conexiuni

    public async Task AddConnectionAsync(string userId, string friendId)
    {
        // Verifica daca conexiunea exista deja

        var existingConnection = await _context.Connections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.FriendId == friendId);

        if (existingConnection == null)
        {
            // Creeaza o noua conexiune daca nu exista

            var connection = new Connection
            {
                UserId = userId,
                FriendId = friendId
            };
            _context.Connections.Add(connection);
            await _context.SaveChangesAsync();
        }
    }

    // Metoda pentru eliminarea unei conexiuni

    public async Task RemoveConnectionAsync(string userId, string friendId)
    {
        // Gaseste conexiunea dintre utilizator si prieten

        var connection = await _context.Connections
            .FirstOrDefaultAsync(c => c.UserId == userId && c.FriendId == friendId);

        if (connection != null)
        {
            _context.Connections.Remove(connection);
            await _context.SaveChangesAsync();
        }
    }

    // Metoda pentru obtinerea conexiunilor unui utilizator

    public async Task<IEnumerable<User>> GetConnectionsAsync(string userId)
    {
        var connections = await _context.Connections
            .Where(c => c.UserId == userId)
            .Include(c => c.Friend)
            .ToListAsync();

        return connections.Select(c => c.Friend);
    }
}




