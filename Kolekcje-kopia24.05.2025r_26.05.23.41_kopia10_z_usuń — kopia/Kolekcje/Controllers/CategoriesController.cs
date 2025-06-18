using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kolekcje.Data;
using Kolekcje.Models;

namespace Kolekcje.Controllers
{
    public class CategoriesController(ApplicationDbContext context) : Controller()
    {
        private readonly ApplicationDbContext _context = context;

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.CollectionCategories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionCategory = await _context.CollectionCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectionCategory == null)
            {
                return NotFound();
            }

            return View(collectionCategory);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CollectionCategory collectionCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(collectionCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(collectionCategory);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionCategory = await _context.CollectionCategories.FindAsync(id);
            if (collectionCategory == null)
            {
                return NotFound();
            }
            return View(collectionCategory);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CollectionCategory collectionCategory)
        {
            if (id != collectionCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collectionCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectionCategoryExists(collectionCategory.Id))
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
            return View(collectionCategory);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var collectionCategory = await _context.CollectionCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collectionCategory == null)
            {
                return NotFound();
            }

            return View(collectionCategory);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var collectionCategory = await _context.CollectionCategories.FindAsync(id);
            if (collectionCategory != null)
            {
                _context.CollectionCategories.Remove(collectionCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        

        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var category = await _context.CollectionCategories.FindAsync(id);
            if (category == null)
                return NotFound();

            // Sprawdź, czy kategoria jest używana
            bool isUsed = _context.Collections.Any(c => c.CollectionCategoryId == id);
            if (isUsed)
                return BadRequest("Kategoria jest przypisana do kolekcji.");

            _context.CollectionCategories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok();
        }
               
        private bool CollectionCategoryExists(int id)
        {
            return _context.CollectionCategories.Any(e => e.Id == id);
        }

    }
}
