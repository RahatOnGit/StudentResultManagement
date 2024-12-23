using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
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

       

        public async Task<IActionResult> AllCourses(int semesterId, string semesterName)
        {
            var data = await _db.Courses.Include(c => c.Semester).
                Where(c=>c.SemesterId==semesterId).ToListAsync();


            ViewBag.SemesterId = semesterId;
            ViewBag.SemesterName = semesterName;

            return View(data);
        }

        public IActionResult Create(string SemesterName, int SemesterId, string returnUrl)
        {
            ViewBag.SemesterName = SemesterName;
            ViewBag.SemesterId = SemesterId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Course course ,string returnUrl)
        {
            var has_course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseNo == course.CourseNo);
            


            if (ModelState.IsValid && has_course==null)
            {
                await _db.Courses.AddAsync(course);
                await _db.SaveChangesAsync();
                
                return LocalRedirect(returnUrl);
            }

            if (has_course!=null)
            {
                ViewBag.Message = "Course No. Exists!";
            }

            var data = await _db.Semesters.FirstOrDefaultAsync(c => c.Id == course.SemesterId);

            ViewBag.SemesterName = data.Name;

            ViewBag.SemesterId = course.SemesterId;





            return View(course);
        }

        public async Task<IActionResult> Edit(string SemesterName, int SemesterId, int id, string returnUrl)
        {
            ViewBag.SemesterName = SemesterName;
            ViewBag.SemesterId = SemesterId;

            var data = await _db.Courses.Include(c => c.Semester).FirstOrDefaultAsync(c => c.Id == id);

            ViewBag.ReturnURl = returnUrl;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,Course course, string returnURl)
        {
            if (ModelState.IsValid)
            {
               var data = await _db.Courses.Where(c=>c.Id==id).FirstOrDefaultAsync();
                
                data.SemesterId = course.SemesterId;
                data.CourseNo = course.CourseNo;
                data.CourseTitle = course.CourseTitle;
                data.Credit = course.Credit;

                _db.Courses.Update(data);
                await _db.SaveChangesAsync();
                return LocalRedirect(returnURl);
            }

            return View(course);
        }

        public async Task<IActionResult> Delete(string SemesterName, int SemesterId, int id, string returnUrl)
        {
            ViewBag.SemesterName = SemesterName;
            ViewBag.SemesterId = SemesterId;
            ViewBag.ReturnURl = returnUrl;

            var data = await _db.Courses.Include(c=>c.Semester).FirstOrDefaultAsync(c => c.Id == id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, Course course, string returnURl)
        {
            if (id == course.Id)
            {
                var data = await _db.Courses.FindAsync(id);
                if (data != null)
                {
                    _db.Courses.Remove(data);
                    await _db.SaveChangesAsync();
                    return LocalRedirect(returnURl);
                }


            }

            return View();
        }

    }
}
