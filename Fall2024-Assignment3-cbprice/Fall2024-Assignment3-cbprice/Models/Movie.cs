namespace Fall2024_Assignment3_cbprice.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string IMDBLink { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public string PosterUrl { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }

    // Related data (Actors)
    public List<Actor> Actors { get; set; } = new List<Actor>();

    // AI Reviews and Sentiment
    public List<AIReviewModel> AIReviews { get; set; } = new List<AIReviewModel>();
}


