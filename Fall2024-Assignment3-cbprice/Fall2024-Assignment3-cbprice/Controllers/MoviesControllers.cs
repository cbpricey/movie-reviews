using Fall2024_Assignment3_cbprice.Data;
using Fall2024_Assignment3_cbprice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Fall2024_Assignment3_cbprice.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAzureAIService _azureAIService;

        public MoviesController(ApplicationDbContext context, IAzureAIService azureAIService)
        {
            _context = context;
            _azureAIService = azureAIService;
        }

        // List all movies
        public async Task<IActionResult> Index()
        {
            var movies = await _context.Movies.ToListAsync();
            return View(movies);
        }

        // Show details of a movie
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            Console.WriteLine($"Fetching reviews for '{movie.Title}' released in {movie.ReleaseYear}");
            var reviews = await _azureAIService.GetAIReviewsForMovie(movie.Title, movie.ReleaseYear);

            string overallSentiment;
            if (reviews.Any())
            {
                Console.WriteLine($"Number of reviews fetched: {reviews.Count}");
                overallSentiment = reviews.Select(r => r.Sentiment)
                                          .GroupBy(s => s)
                                          .OrderByDescending(g => g.Count())
                                          .FirstOrDefault()?.Key ?? "Neutral";
            }
            else
            {
                overallSentiment = "No sentiment data available";
            }

            var viewModel = new MovieDetailsViewModel
            {
                Movie = movie,
                Reviews = reviews,
                OverallSentiment = overallSentiment
            };

            return View(viewModel);
        }


        // Create a new movie
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,IMDBLink,Genre,ReleaseYear,PosterUrl")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // Edit a movie
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,IMDBLink,Genre,ReleaseYear,PosterUrl")] Movie movie)
        {
            if (id != movie.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // Delete a movie
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
