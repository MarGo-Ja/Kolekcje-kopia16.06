using Kolekcje.Data;
using Kolekcje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Kolekcje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        private ApplicationDbContext? context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var liczbaPozycji = _context.Collections.Count();
            ViewBag.LiczbaPozycji = liczbaPozycji;

            // Przyk³ad: ostatnie 3 kolekcje
            var ostatnieKolekcje = _context.Collections
                .Include(c => c.Category) // <-- dodane: do³¹cz kategoriê
                .OrderByDescending(c => c.Id)
                .Take(3)
                .ToList();
            ViewBag.OstatnieKolekcje = ostatnieKolekcje;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
