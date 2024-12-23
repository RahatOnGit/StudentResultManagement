using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentResultManagement.Data;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class SemesterController : Controller
    {
        private ApplicationDbContext _db;

        public SemesterController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.Semesters.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Semesters semester)
        {


            if (ModelState.IsValid)
            {
                await _db.Semesters.AddAsync(semester);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            return View(semester);

        }







        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _db.Semesters.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int? id, Semesters semesters)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid && id == semesters.Id)
            {
                _db.Semesters.Update(semesters);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(semesters);

        }

        



        public async Task<IActionResult> Delete(int id)
        {
            var data = await _db.Semesters.FindAsync(id);

            if (data == null)
            {
                return NotFound();
            }



            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id, Semesters semesters)
        {
            if (ModelState.IsValid && id == semesters.Id)
            {
                var data = await _db.Semesters.FindAsync(id);

                if (data == null)
                {
                    return NotFound();
                }

                _db.Semesters.Remove(data);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(semesters);
        }





    }

}



