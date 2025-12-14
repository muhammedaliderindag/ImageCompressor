using ImageCompressor.Web.Auth;
using Microsoft.EntityFrameworkCore;

namespace ImageCompressor.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> Users { get; set; }
    public DbSet<UserHistory> UserHistories { get; set; }
}