namespace Fall2024_Assignment3_cbprice.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string IMDBLink { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;

        // Related data (Movies)
        public List<Movie> Movies { get; set; } = new List<Movie>();

        // AI Tweets and Sentiment
        public List<AITweetModel> AITweets { get; set; } = new List<AITweetModel>();
    }
}
