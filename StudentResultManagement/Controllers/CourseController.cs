using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{

    [Authorize]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CourseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var data = _db.Courses.Include(c=>c.Semester).ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Semesters = new SelectList(_db.Semesters, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course)
        {


            if (ModelState.IsValid)
            {
                await _db.Courses.AddAsync(course);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            

            
            return View(course);


        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Semesters = new SelectList(_db.Semesters, "Id", "Name");

            var data = await _db.Courses.Include(c => c.Semester).FirstOrDefaultAsync(c => c.Id == id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _db.Courses.Update(course);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data = await _db.Courses.Include(c=>c.Semester).FirstOrDefaultAsync(c => c.Id == id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, Course course)
        {
            if (id == course.Id)
            {
                var data = await _db.Courses.FindAsync(id);
                if (data != null)
                {
                    _db.Courses.Remove(data);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


            }

            return View();
        }

    }
}
