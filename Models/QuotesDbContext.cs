using Microsoft.EntityFrameworkCore;

namespace quotes_app.Models;

public class QuotesDbContext : DbContext
{
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<User> Users { get; set; }

    public QuotesDbContext(DbContextOptions<QuotesDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}

