using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_cbprice.Models;

public class AITweetModel
{
    [Key] //defines property as the primary key
    public int Id { get; set; } // Primary key
    public string Text { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string TweetText { get; set; } = string.Empty;

    public int ActorId { get; set; } // Foreign key to Actor
    public Actor? Actor { get; set; } // Navigation property
}
