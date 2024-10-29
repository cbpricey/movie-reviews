using Fall2024_Assignment3_cbprice.Data;
using Fall2024_Assignment3_cbprice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fall2024_Assignment3_cbprice.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureAIService _azureAIService;
        private readonly ILogger<ActorsController> _logger;

        public ActorsController(ApplicationDbContext context, IAzureAIService azureAIService, ILogger<ActorsController> logger)
        {
            _context = context;
            _azureAIService = azureAIService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _context.Actors.ToListAsync();
            return View(actors);
        }

        //Show details of actor
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();

            Console.WriteLine($"Fetching tweets about '{actor.Name}'");
            var tweets = await _azureAIService.GetAITweetsForActor(actor.Name);

            string overallSentiment;
            if (tweets.Any())
            {
                Console.WriteLine($"Number of tweets fetched: {tweets.Count}");

                overallSentiment = tweets.Select(r => r.Sentiment)
                                            .GroupBy(s => s)
                                            .OrderByDescending(g => g.Count())
                                            .FirstOrDefault()?.Key ?? "Neutral";
            }
            else
            {
                overallSentiment = " No sentiment data available";

            }
            var viewModel = new ActorDetailsViewModel
            {
                Actor = actor,
                Tweets = tweets,
                OverallSentiment = overallSentiment
            };

            return View(viewModel);
        }

        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null) return NotFound();

        //    // Fetch the actor including their associated tweets
        //    var actor = await _context.Actors.Include(a => a.AITweets).FirstOrDefaultAsync(a => a.Id == id);
        //    if (actor == null) return NotFound();

        //    // If tweets are not saved in the database, call the AI service
        //    if (!actor.AITweets.Any())
        //    {
        //        var tweets = await _azureAIService.GetAITweetsForActor(actor.Name);
        //        await SaveTweetsToDatabase(actor.Id, tweets); // Save tweets to database
        //        actor.AITweets = tweets; // Assign fetched tweets to the actor's property
        //    }

        //    // Calculate overall sentiment based on saved tweets
        //    string overallSentiment = "No sentiment data available";
        //    if (actor.AITweets.Any())
        //    {
        //        overallSentiment = actor.AITweets
        //            .Select(t => t.Sentiment)
        //            .GroupBy(s => s)
        //            .OrderByDescending(g => g.Count())
        //            .FirstOrDefault()?.Key ?? "Neutral";
        //    }

        //    // Create a view model to pass to the view
        //    var viewModel = new ActorDetailsViewModel
        //    {
        //        Actor = actor,
        //        Tweets = actor.AITweets, // Use the tweets loaded from the database
        //        OverallSentiment = overallSentiment
        //    };

        //    return View(viewModel);
        //}

        //// Method to save tweets to the database
        //private async Task SaveTweetsToDatabase(int actorId, List<AITweetModel> tweets)
        //{
        //    foreach (var tweet in tweets)
        //    {
        //        tweet.ActorId = actorId; // Associate the tweet with the actor
        //        _context.AITweets.Add(tweet); // Add each tweet to the context
        //    }
        //    await _context.SaveChangesAsync(); // Save all changes to the database
        //}


        // Create a new actor
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Gender,Age,IMDBLink,PhotoUrl")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // Edit an actor
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();

            return View(actor);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,IMDBLink,PhotoUrl")] Actor actor)
        {
            if (id != actor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // Delete an actor
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }

    }
}
