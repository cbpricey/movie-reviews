namespace Fall2024_Assignment3_cbprice.Models
{
    public class ActorDetailsViewModel
    {
        public Actor? Actor { get; set; }
        public List<AITweetModel> Tweets { get; set; } = new List<AITweetModel>();
        public string? OverallSentiment { get; set; }
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }

}