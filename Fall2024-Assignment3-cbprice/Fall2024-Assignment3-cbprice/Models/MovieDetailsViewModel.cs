namespace Fall2024_Assignment3_cbprice.Models;

public class MovieDetailsViewModel
{
    public Movie? Movie { get; set; }
    public List<AIReviewModel> Reviews { get; set; } = new List<AIReviewModel>();
    public string? OverallSentiment { get; set; } = string.Empty;
    public List<Actor> Actors { get; set; } = new List<Actor>();
}