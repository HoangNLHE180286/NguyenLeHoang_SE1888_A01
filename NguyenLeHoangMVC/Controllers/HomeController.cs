using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NguyenLeHoangMVC_DataLayer.Models;
using NguyenLeHoangMVC_BusinessLayer.Services;

namespace NguyenLeHoangMVC.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly INewsService _newsService;

        public HomeController(ILogger<HomeController> logger, INewsService newsService) {
            _logger = logger;
            _newsService = newsService;
        }

        public async Task<IActionResult> Index() {

            var activeNews = await _newsService.GetActiveNewsAsync();
            return View(activeNews);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}