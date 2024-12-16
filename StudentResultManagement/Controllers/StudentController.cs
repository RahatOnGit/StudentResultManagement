using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudentResultManagement.Data;
using StudentResultManagement.Models;
using System.Dynamic;
using System.Net.Sockets;

namespace StudentResultManagement.Controllers
{

    [Authorize]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(string searchString)
        {
           
            var data = _db.Students.Include(c => c.Series).ToList();
            
            if (!String.IsNullOrEmpty(searchString))
            {
                data = _db.Students.Include(c => c.Series).Where(s => s.Roll.Contains(searchString)).ToList();
                ViewBag.SearchString = searchString;
            }



            return View(data);
        }

        public IActionResult Create()
        {
            ViewBag.Series = new SelectList(_db.Series, "Id", "SeriesName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Students student)
        {

            var has_Id = await _db.Students.Where(c=>c.Roll==student.Roll).FirstOrDefaultAsync();


            if (ModelState.IsValid && has_Id==null)
            {
                await _db.Students.AddAsync(student);
                await _db.SaveChangesAsync();
                ViewBag.Success = true;
                return View();
            }

           

            if (has_Id!=null)
            {
                ViewBag.Series = new SelectList(_db.Series, "Id", "SeriesName");
                ViewBag.IdMessage = "This roll no. has been added for a student!";
            }

           
                
     
           



            return View(student);


        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Series = new SelectList(_db.Series, "Id", "SeriesName");

            var data = await _db.Students.Include(c => c.Series).FirstOrDefaultAsync(c=>c.Id==id);

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Students student)
        {
            if (ModelState.IsValid)
            {
                _db.Students.Update(student);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(student);
        }

        public IActionResult Details(int id)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Semesters = _db.Semesters.ToList();
            mymodel.Student = _db.Students.Include(c=>c.Series).Where(c=>c.Id==id).FirstOrDefault();

            var total_courses = _db.Courses.ToList();
            double Total = 0.0;
            int count = 0;
            foreach (var course in total_courses)
            {
                var data = _db.Results
                                    .Where(c => c.CourseId==course.Id && c.StudentId == id)
                                    .FirstOrDefault();
                if (data != null)
                { 
                    Total += (Convert.ToDouble(data.Mark));
                    count += 1;
                   

                }

            }

            mymodel.AverageResult = "";
            if (count > 0)
            {
                mymodel.AverageResult = Math.Round(Total / (count * 1.0), 2);

            }


            return View(mymodel);
        }


        public  IActionResult Semester(int semester_id, int student_id)
        {
            ViewBag.StudentId = student_id;
            ViewBag.SemesterId = semester_id;

            dynamic mymodel = new ExpandoObject();
            var total_courses = _db.Courses.Where(c=>c.SemesterId== semester_id).ToList();
            mymodel.Courses = total_courses;
            mymodel.Student = _db.Students.Include(c => c.Series).Where(c => c.Id == student_id).FirstOrDefault();
            mymodel.Semester = _db.Semesters.Where(c => c.Id == semester_id).FirstOrDefault();
          
            mymodel.Results = new List<dynamic>();

            
            double Total = 0.0;
            int count = 0;
            foreach (var course in total_courses)
            {
                var data =  _db.Results
                                    .Where(c => c.CourseId == course.Id && c.StudentId == student_id)
                                    .FirstOrDefault();
                if (data == null)
                {
                    mymodel.Results.Add("Grade isn't added yet!");
                }

                else
                {
                    Total += (Convert.ToDouble(data.Mark));
                    count += 1;
                    mymodel.Results.Add(data);

                }

            }

            mymodel.AverageResult = "";
            if (count > 0)
            {
                mymodel.AverageResult = Math.Round( Total / (count * 1.0) , 2 );

            }

            return View(mymodel);



            
        }

        public IActionResult Course(int course_id, int student_id, string returnUrl)
        {
            
            ViewBag.CourseId = course_id;
            ViewBag.StudentId = student_id;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.StudentRoll = _db.Students.Where(c=>c.Id== student_id).FirstOrDefault(); 
            ViewBag.CourseNo = _db.Courses.Where(c=>c.Id == course_id).FirstOrDefault();
           
            return View();
        }

        [HttpPost]
        public IActionResult Course(Result result, string returnUrl)
        {
            if(ModelState.IsValid)
            {
                _db.Results.Add(result);
                _db.SaveChanges();
                return LocalRedirect(returnUrl);
            }

            return View(result);
        }

        public IActionResult UpdateCourse(int course_id, int student_id, string returnUrl)
        {
            var data = _db.Results.Where(c=>c.CourseId==course_id && c.StudentId==student_id).FirstOrDefault();
            ViewBag.CourseId = course_id;
            ViewBag.StudentId = student_id;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.StudentRoll = _db.Students.Where(c => c.Id == student_id).FirstOrDefault();
            ViewBag.CourseNo = _db.Courses.Where(c => c.Id == course_id).FirstOrDefault();


            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateCourse(int id,Result result, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Results.FirstOrDefault(result=>result.Id==id);
                data.Mark = result.Mark;
                _db.Results.Update(data);
                _db.SaveChanges();



                return LocalRedirect(ReturnUrl);
            }

            return View(result);
        }
    }
}
