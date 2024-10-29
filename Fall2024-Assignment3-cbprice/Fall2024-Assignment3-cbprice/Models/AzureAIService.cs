using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using VaderSharp2;

namespace Fall2024_Assignment3_cbprice.Models;

public class AzureAIService : IAzureAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _openAIEndpoint;
    private readonly string _apiKey;

    public AzureAIService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _openAIEndpoint = configuration["OpenAI:Endpoint"]!;
        _apiKey = configuration["OpenAI:ApiKey"]!;
    }

    public async Task<string> GetOpenAIResponse(string userInput)
    {
        try
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_openAIEndpoint}");
            requestMessage.Headers.Add("api-key", $"{_apiKey}");

            var payload = new
            {
                model = "gpt-35-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful AI assistant." },
                new { role = "user", content = userInput }
            }
            };

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var aiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);

            return aiResponse?.choices?.FirstOrDefault()?.message?.content ?? "No response from AI";
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            return $"Error: {httpEx.Message}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    public async Task<List<AIReviewModel>> GetAIReviewsForMovie(string movieTitle, int releaseYear)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_openAIEndpoint}");
        requestMessage.Headers.Add("api-key", $"{_apiKey}");

        var payload = new
        {
            model = "gpt-35-turbo",
            messages = new[]
            {
            new { role = "system", content = "You are a helpful AI that writes movie reviews for a website about movies. I will give you a movie title and year and you will write reviews for it. please mix up the reviews make some good, some bad, and some nuteral. mix up the format you write the review in, you are supposed to be representing different people writing different reviews from their perspective so dont make them all bland and similar. please do not number the reviews just give me a normal review and then i want to know who it is from by adding a -John Doe at the end of the review, exept please change the name to something creative. make sure that for at least 1 review out of the 5 that you will generate has the name May McClain. In order for me to split the reviews i will need a delimiter in between each review so please start each review with '&$%' and then begin the review context with no numbering" },
            new { role = "user", content = $"Write 10 different reviews for the {releaseYear} movie '{movieTitle}'." }
        }
        };

        requestMessage.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var aiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);

            if (aiResponse == null || aiResponse.choices == null)
            {
                Console.WriteLine("No AI response or choices available.");
                return new List<AIReviewModel>();
            }

            var sentimentAnalyzer = new SentimentIntensityAnalyzer();
            var reviews = new List<AIReviewModel>();


            var concatenatedReviews = aiResponse.choices.FirstOrDefault()?.message.content;

            if (concatenatedReviews != null)
            {
                var individualReviews = concatenatedReviews.Split(new[] { "&$%" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var reviewText in individualReviews)
                {
                    var reviewContent = reviewText.Trim();
                    if (!string.IsNullOrWhiteSpace(reviewContent))
                    {

                        Console.WriteLine($"Fetched review: {reviewContent}");

                        var sentimentScore = sentimentAnalyzer.PolarityScores(reviewContent).Compound;
                        var sentiment = sentimentScore > 0.05 ? "Positive" : sentimentScore < -0.05 ? "Negative" : "Neutral";

                        Console.WriteLine($"Sentiment Score: {sentimentScore}, Determined Sentiment: {sentiment}");

                        reviews.Add(new AIReviewModel { ReviewText = reviewContent, Sentiment = sentiment });

                    }
                }
            }

            return reviews.Take(10).ToList();
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            return new List<AIReviewModel>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            return new List<AIReviewModel>();
        }
    }




    public class OpenAIResponse
    {
        public List<Choice> choices { get; set; } = new List<Choice>();
    }

    public class Choice
    {
        public Message message { get; set; } = new Message();
    }

    public class Message
    {
        public string role { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }


    public async Task<List<AITweetModel>> GetAITweetsForActor(string actorName)
    {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_openAIEndpoint}");
            requestMessage.Headers.Add("api-key", $"{_apiKey}");

            var payload = new
            {
                model = "gpt-35-turbo",
                messages = new[]
                {
                new { role = "system", content = "You are a helpful AI that generates tweets about actors.I will give you a actor name and you will write tweets for it. please mix up the tweets make some good, some bad, and some nuteral. mix up the format you write the tweet in, you are supposed to be representing different people writing different tweets from their perspective so dont make them all bland and similar. please do not number the tweets just give me a normal tweet and then i want to know who it is from by adding a -John Doe at the end of the tweet, exept please change the name to something creative. make sure that for at least 1 tweet out of the 20 that you will generate has the name May McClain. In order for me to split the tweets i will need a delimiter in between each tweet so please start each tweet with '&$%' and then begin the tweet context with no numbering" },
                new { role = "user", content = $"Write 20 different Tweets about the actor '{actorName}'." }
            }
            };

            requestMessage.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();

                var aiResponse = JsonConvert.DeserializeObject<OpenAIResponse>(responseContent);

                if (aiResponse == null || aiResponse.choices == null)
                {
                    Console.WriteLine("No AI response or choices available.");
                    return new List<AITweetModel>();
                }

                var sentimentAnalyzer = new SentimentIntensityAnalyzer();
                var tweets = new List<AITweetModel>();


                var concatenatedReviews = aiResponse.choices.FirstOrDefault()?.message.content;

                if (concatenatedReviews != null)
                {
                    var individualReviews = concatenatedReviews.Split(new[] { "&$%" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var reviewText in individualReviews)
                    {
                        var reviewContent = reviewText.Trim();
                        if (!string.IsNullOrWhiteSpace(reviewContent))
                        {

                            Console.WriteLine($"Fetched review: {reviewContent}");

                            var sentimentScore = sentimentAnalyzer.PolarityScores(reviewContent).Compound;
                            var sentiment = sentimentScore > 0.05 ? "Positive" : sentimentScore < -0.05 ? "Negative" : "Neutral";

                            Console.WriteLine($"Sentiment Score: {sentimentScore}, Determined Sentiment: {sentiment}");

                            tweets.Add(new AITweetModel { TweetText = reviewContent, Sentiment = sentiment });

                        }
                    }
                }

                return tweets.Take(20).ToList();
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return new List<AITweetModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return new List<AITweetModel>();
            }
        }


    }




//            if (aiResponse?.choices != null)
//            {
//                foreach (var message in aiResponse.choices)
//                {
//                    var sentimentScore = sentimentAnalyzer.PolarityScores(message.message.content).Compound;
//                    var sentiment = sentimentScore > 0.05 ? "Positive" : sentimentScore < -0.05 ? "Negative" : "Neutral";
//                    tweets.Add(new AITweetModel { ReviewText = message.message.content, Sentiment = sentiment });
//                }
//            }
//        }
//        catch (HttpRequestException httpEx)
//        {
//            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");

//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"General Error: {ex.Message}");

//        }

//        return tweets;
//    }



//}
