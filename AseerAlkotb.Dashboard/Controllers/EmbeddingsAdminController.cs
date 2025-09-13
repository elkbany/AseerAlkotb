using AseerAlkotb.Application.BackgroundJobs;
using AseerAlkotb.Application.Contracts;
using AseerAlkotb.Dashboard.Models.Embeddings;
using AseerAlkotb.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AseerAlkotb.Dashboard.Controllers
{
    public class EmbeddingsAdminController : Controller
    {
            private readonly ApplicationDbContext _db;
            private readonly IEmbeddingService _embedding;          
            private readonly IEmbeddingRefreshJob? _job;                 

            public EmbeddingsAdminController(
                ApplicationDbContext db,
                IEmbeddingService embedding,
                IEmbeddingRefreshJob? job = null) 
            {
                _db = db;
                _embedding = embedding;
                _job = job;
            }

            // GET: /EmbeddingsAdmin
            public async Task<IActionResult> Index(string? q = null, int page = 1, int pageSize = 20)
            {
                var baseQuery = _db.Books.AsNoTracking()
                    .Include(b => b.Author)
                    .Include(b => b.Categories)
                    .Select(b => new EmbeddingBookRowVM
                    {
                        BookId = b.Id,
                        Title = b.Title ?? "",
                        Author = b.Author != null ? b.Author.Name : "",
                        Categories = string.Join(", ", b.Categories.Select(c => c.Name)),
                        LastUpdated = _db.BookEmbeddings
                            .Where(e => e.BookId == b.Id)
                            .OrderByDescending(e => e.LastUpdated)
                            .Select(e => (DateTime?)e.LastUpdated)
                            .FirstOrDefault()
                    });

                if (!string.IsNullOrWhiteSpace(q))
                    baseQuery = baseQuery.Where(x => x.Title.Contains(q) || x.Author.Contains(q));

                var total = await baseQuery.CountAsync();
                var rows = await baseQuery
                    .OrderByDescending(x => x.LastUpdated)
                    .ThenBy(x => x.Title)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var vm = new EmbeddingsIndexVM
                {
                    Query = q,
                    Page = page,
                    PageSize = pageSize,
                    Total = total,
                    Rows = rows
                };
                return View(vm);
            }

            // POST: /EmbeddingsAdmin/RebuildOne/5
            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> RebuildOne(int id)
            {
                // Background way (preferred for UX if عندك Job Queue):
                if (_job != null)
                {
                    var enq = _job.TriggerBookUpdate(id);
                    TempData["ok"] = enq ? "Book queued for embedding rebuild." : "Nothing enqueued.";
                    return RedirectToAction(nameof(Index));
                }

                // Direct way (sync in request):
                await _embedding.UpdateBookEmbeddingsAsync(id);
                TempData["ok"] = "Embeddings rebuilt for this book.";
                return RedirectToAction(nameof(Index));
            }

            // POST: /EmbeddingsAdmin/RebuildAll
            [HttpPost, ValidateAntiForgeryToken]
            public IActionResult RebuildAll()
            {
                if (_job == null)
                {
                    TempData["err"] = "Background job service is not available.";
                    return RedirectToAction(nameof(Index));
                }
                var enq = _job.TriggerFullRebuild();
                TempData["ok"] = enq ? "Full rebuild enqueued." : "Nothing enqueued.";
                return RedirectToAction(nameof(Index));
            }

            // POST: /EmbeddingsAdmin/DeleteOne/5
            [HttpPost, ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteOne(int id)
            {
                var rows = await _db.BookEmbeddings.Where(e => e.BookId == id).ToListAsync();
                _db.BookEmbeddings.RemoveRange(rows);
                await _db.SaveChangesAsync();
                TempData["ok"] = "Embeddings deleted for this book.";
                return RedirectToAction(nameof(Index));
            }

            // GET: /EmbeddingsAdmin/ViewEmbeddings/5
            public async Task<IActionResult> ViewEmbeddings(int id)
            {
                var items = await _db.BookEmbeddings
                    .AsNoTracking()
                    .Where(e => e.BookId == id)
                    .OrderByDescending(e => e.LastUpdated)
                    .ToListAsync();
                return View(items);
            }

            // GET: /EmbeddingsAdmin/Status
            public IActionResult Status()
            {
                if (_job == null) return Content("Background job service not available.");
                var s = _job.GetStatus();
                return Json(s);
            }
        }
    }
