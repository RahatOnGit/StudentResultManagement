using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class SeriesController : Controller
    {
        private ApplicationDbContext _db;

        public SeriesController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.Series.OrderBy(c=>c.SeriesName).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Series series)
        {


            if (ModelState.IsValid)
            {
                await _db.Series.AddAsync(series);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(series);

        }

       







     





    }
}


