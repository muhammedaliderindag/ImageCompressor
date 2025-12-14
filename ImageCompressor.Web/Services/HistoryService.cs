using Microsoft.EntityFrameworkCore;

namespace ImageCompressor.Web.Data;

public class HistoryService
{
    private readonly AppDbContext _context;

    public HistoryService(AppDbContext context)
    {
        _context = context;
    }

    // Geçmişe ekle
    public async Task AddHistoryAsync(int userId, string action, string fileName, string details)
    {
        var history = new UserHistory
        {
            UserId = userId,
            ActionType = action,
            FileName = fileName,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };
        _context.UserHistories.Add(history);
        await _context.SaveChangesAsync();
    }

    // Kullanıcının geçmişini getir (Ters tarih sırasıyla)
    public async Task<List<UserHistory>> GetUserHistoryAsync(int userId)
    {
        return await _context.UserHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync();
    }
}