using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_cbprice.Models;

namespace Fall2024_Assignment3_cbprice.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<AIReviewModel> AIReviews { get; set; }
    public DbSet<AITweetModel> AITweets { get; set; } // Add this line
}
