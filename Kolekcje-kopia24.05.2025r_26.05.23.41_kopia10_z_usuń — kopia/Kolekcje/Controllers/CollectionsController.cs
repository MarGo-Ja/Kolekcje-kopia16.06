using Kolekcje.Data;
using Kolekcje.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Kolekcje.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Autoryzacja


        // GET: /Collections/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name");
            return View();
        }

        //Edit
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var collection = await _context.Collections.FindAsync(id);
            if (collection == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name", collection.CollectionCategoryId);
            return View(collection);
        }

        //Delete
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var collection = await _context.Collections
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (collection == null) return NotFound();

            return View(collection);
        }



        // POST: /Collections/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Collection collection, string? NewCategoryName, IFormFile? ImageFile)
        {
            if (!string.IsNullOrEmpty(NewCategoryName))
            {
                // Dodaj nową kategorię do bazy
                var newCategory = new CollectionCategory { Name = NewCategoryName };
                _context.CollectionCategories.Add(newCategory);
                await _context.SaveChangesAsync();

                // Ustaw nową kategorię dla kolekcji
                collection.CollectionCategoryId = newCategory.Id;
            }
            // 2. Obsługa przesyłania obrazu
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                collection.ImagePath = "/images/" + fileName;
            }

            // Sprawdź poprawność modelu
            if (!ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name");
            return View(collection);
        }

        [HttpPost]
        public IActionResult PokazZaznaczone(List<int> selectedIds)
        {
            if (selectedIds == null || selectedIds.Count == 0)
            {
                TempData["Message"] = "Nie zaznaczono żadnej kolekcji.";
                return RedirectToAction("Index");
            }

            var zaznaczoneKolekcje = _context.Collections
                .Include(c => c.Category)
                .Where(c => selectedIds.Contains(c.Id))
                .ToList();

            ViewBag.TotalCount = zaznaczoneKolekcje.Count;
            ViewBag.Categories = _context.CollectionCategories
                .Select(c => c.Name)
                .ToList();

            return View("Index", zaznaczoneKolekcje);
        }


        /*
        [HttpPost]
        public IActionResult PokazZaznaczone(List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return RedirectToAction("Index");

            var kolekcje = _context.Collections
                .Include(c => c.Category)
                .Where(c => selectedIds.Contains(c.Id))
                .ToList();

            return View("Index", kolekcje); // zwraca ten sam widok z przefiltrowanymi danymi
        }
        */

        /*
        //26.05.2025r.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Collection collection, IFormFile ImageFile)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                collection.ImagePath = "/images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name");
            return View(collection);
        }
        */

        //27.05.2025r. Połączone metody Create tak aby dodawać kolekcję i obrazy.
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Collection collection, string? NewCategoryName, IFormFile? ImageFile)
        {
            // 1. Obsługa nowej kategorii
            if (!string.IsNullOrEmpty(NewCategoryName))
            {
                var newCategory = new CollectionCategory { Name = NewCategoryName };
                _context.CollectionCategories.Add(newCategory);
                await _context.SaveChangesAsync();

                collection.CollectionCategoryId = newCategory.Id;
            }

            // 2. Obsługa przesyłania obrazu
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                collection.ImagePath = "/images/" + fileName;
            }

            // 3. Sprawdzenie poprawności i zapis do bazy
            if (ModelState.IsValid)
            {
                _context.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Jeśli model nie jest poprawny, ponownie załaduj kategorie i zwróć widok
            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name");
            return View(collection);
        }
        */
        /*
        [HttpPost]
        public IActionResult Create(Collection model)
        {
            if (model.ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ObrazekPlik.FileName);
                var path = Path.Combine(_env.WebRootPath, "obrazy", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                model.ImageFile.CopyTo(stream);
                model.ObrazekNazwa = fileName;
            }

            model.Id = _kolekcje.Count + 1;
            _kolekcje.Add(model);
            return RedirectToAction("Index");
        }
        */

        //Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Collection collection, string? NewCategoryName, IFormFile? ImageFile)
        {
            //27.05.2025r.
            if (id != collection.Id) return NotFound();


            if (!string.IsNullOrEmpty(NewCategoryName))
            {
                // Dodaj nową kategorię do bazy
                var newCategory = new CollectionCategory { Name = NewCategoryName };
                _context.CollectionCategories.Add(newCategory);
                await _context.SaveChangesAsync();

                // Ustaw nową kategorię dla kolekcji
                collection.CollectionCategoryId = newCategory.Id;
            }
            /*
            // Jeśli przesłano nowy obraz
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                collection.ImagePath = "/images/" + fileName;
            }
            */
            // 2. Obsługa przesyłania obrazu
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                collection.ImagePath = "/images/" + fileName;
            }


            if (!ModelState.IsValid)
            {
                _context.Update(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name", collection.CollectionCategoryId);
            return View(collection);
        }

        //Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        /*
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collection = await _context.Collections.FindAsync(id);
            if (collection != null)
            {
                _context.Collections.Remove(collection);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        */

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Collection collection)
        {
            if (ModelState.IsValid)
            {
                _context.Collections.Add(collection);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // lub gdziekolwiek chcesz
            }

            ViewBag.Categories = new SelectList(_context.CollectionCategories, "Id", "Name", collection.CollectionCategoryId);
            return View(collection);
        }
        */
        // Dodałem 17.05.2025r.

        /* Zakomentowałem 19.05.2025r.
         public async Task<IActionResult> Index()
         {
             var collections = await _context.Collections
                 .Include(c => c.Category) // pobierz kategorię razem z kolekcją
                 .ToListAsync();
             ViewBag.TotalCount = collections.Count;
             return View(collections);
         }
        */
        //Dodałem 19.05.2025r.
        /*
        public async Task<IActionResult> Index(string searchString)
        {
            var collections = from c in _context.Collections
                              select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                collections = collections.Where(c => c.Title.Contains(searchString));
                
            }

            ViewBag.TotalCount = await collections.CountAsync();
            return View(await collections.ToListAsync());
        }
        */
        //Wyłączyłem 20.05.2025r.
        // 20.05.2025r. Dodałem wyszukiwanie także po kategoriach.


        /*
        public async Task<IActionResult> Index(string searchString, string categoryFilter)
        {
            var query = _context.Collections.Include(c => c.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(c => c.Category.Name == categoryFilter);
            }

            // Corrected the issue by casting _context.Categories to the appropriate type
            ViewBag.Categories = await _context.CollectionCategories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            return View(await query.ToListAsync());

        }
        */
        //Metoda Index z liczeniem pozycji w kolekcji
        
        public async Task<IActionResult> Index(string searchString, string categoryFilter)
        {
            var query = _context.Collections.Include(c => c.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Title.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(c => c.Category.Name == categoryFilter);
            }

            // Ustawienie listy kategorii do selecta
            ViewBag.Categories = await _context.CollectionCategories
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();

            // Pobranie wyników i zliczenie pozycji
            var collections = await query.ToListAsync();
            ViewBag.TotalCount = collections.Count;

            return View(collections);
        }


    }

}
