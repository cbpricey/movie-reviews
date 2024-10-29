namespace Fall2024_Assignment3_cbprice.Models;

public interface IAzureAIService
{
    Task<List<AIReviewModel>> GetAIReviewsForMovie(string movieTitle, int releaseYear);
    Task<List<AITweetModel>> GetAITweetsForActor(string actorName);
    Task<string> GetOpenAIResponse(string userInput);
}
