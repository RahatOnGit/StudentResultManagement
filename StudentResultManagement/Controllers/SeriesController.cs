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
            return View(_db.Series.ToList());
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







        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _db.Series.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int? id, Series series)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid && id == series.Id)
            {
                 _db.Series.Update(series);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(series);

        }



        public async Task<IActionResult> Delete(int id)
        {
            var data = await _db.Series.FirstOrDefaultAsync(x => x.Id == id);

            if (data == null)
            {
                return NotFound();
            }



            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id, Series series)
        {
            if (ModelState.IsValid && id == series.Id)
            {
                var data = await _db.Series.FindAsync(id);

                if (data == null)
                {
                    return NotFound();
                }

                _db.Series.Remove(data);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(series);
        }





    }
}


