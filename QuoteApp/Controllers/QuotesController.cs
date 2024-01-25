using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuoteApp.Data;
using QuoteApp.Models;
using System.Security.Claims;

namespace QuoteApp.Controllers
{
    public class QuotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*public async Task<IActionResult> Index()
        {
            return View(await _context.Quote.ToListAsync());
        }*/


        // GET: ShowSearchForm
        public IActionResult ShowSearchForm()
        {
            return View();
        }

        // GET: Quotes
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewBag.SortParam = sortOrder;
            if (searchString == ViewBag.SortParam)
            {
                searchString += "_desc";
                ViewBag.SortParam = "";
            }
            else
            {
                ViewBag.SortParam = searchString;
            }
            var quotes = from s in _context.Quote
                           select s;
            switch (searchString)
            {
                case "quotation":
                    quotes = quotes.OrderBy(s => s.Quotation);
                    break;
                case "quotation_desc":
                    quotes = quotes.OrderByDescending(s => s.Quotation);
                    break;
                case "author":
                    quotes = quotes.OrderBy(s => s.Author);
                    break;
                case "author_desc":
                    quotes = quotes.OrderByDescending(s => s.Author);
                    break;
                case "genre":
                    quotes = quotes.OrderBy(s => s.Genre);
                    break;
                case "genre_desc":
                    quotes = quotes.OrderByDescending(s => s.Genre);
                    break;
                default:
                    break;
            }
            return View(quotes.ToList());
        }

        public async Task<IActionResult> ShowSearchResults(string SearchTermQuotation, string SearchTermAuthor, string SearchTermGenre)
        {
            return View("Index", await _context.Quote
                .Where(q => q.Quotation.Contains(SearchTermQuotation) || string.IsNullOrEmpty(SearchTermQuotation))
                .Where(q => q.Author.Contains(SearchTermAuthor) || string.IsNullOrEmpty(SearchTermAuthor))
                .Where(q => q.Genre.Contains(SearchTermGenre) || string.IsNullOrEmpty(SearchTermGenre))
                .ToListAsync());
        }

        // GET: Quotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null)
            {
                return NotFound();
            }

            return View(quote);
        }

        // GET: Quotes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Quotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Quote quote)
        {
            if (ModelState.IsValid)
            {
                // Dodajemy identyfikator użytkownika do cytatu
                quote.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(quote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quote);
        }

        // GET: Quotes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote.FindAsync(id);
            if (quote == null || quote.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }
            return View(quote);
        }

        // POST: Quotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Quote quote)
        {
            if (id != quote.Id || quote.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuoteExists(quote.Id))
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
            return View(quote);
        }

        // GET: Quotes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quote = await _context.Quote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (quote == null || quote.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier) || string.IsNullOrEmpty(quote.UserId))
            {
                return NotFound();
            }

            return View(quote);
        }

        // POST: Quotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quote = await _context.Quote.FindAsync(id);
            if (quote != null && quote.UserId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                _context.Quote.Remove(quote);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool QuoteExists(int id)
        {
            return _context.Quote.Any(e => e.Id == id);
        }
    }
}

// EOF