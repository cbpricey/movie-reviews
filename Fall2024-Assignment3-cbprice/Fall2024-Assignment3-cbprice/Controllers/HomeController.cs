using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fall2024_Assignment3_cbprice.Models;
using System.Diagnostics;

namespace Fall2024_Assignment3_cbprice.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAzureAIService _azureAIService;

        public HomeController(ILogger<HomeController> logger, IAzureAIService azureAIService)
        {
            _logger = logger;
            _azureAIService = azureAIService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetOpenAIResponse(string userInput)
        {
            string openAIResponse = await _azureAIService.GetOpenAIResponse(userInput);

            ViewBag.OpenAIResponse = openAIResponse;
            return View("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
