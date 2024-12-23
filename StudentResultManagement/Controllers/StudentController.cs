using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Org.BouncyCastle.Tls;
using StudentResultManagement.Data;
using StudentResultManagement.Models;
using System.Dynamic;
using System.Linq;
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

      

        public async Task<IActionResult> AllStudents(int seriesId, string seriesName, string? searchString, int pg = 1)
        {

            int PageSize = 10;

            if (pg < 1)
                pg = 1;

            var all_students = new List<Students>();



            if (!String.IsNullOrEmpty(searchString))
            {
                all_students = await _db.Students.Include(c => c.Series).
                    Where(s => s.Roll.Contains(searchString) && s.SeriesId == seriesId).
                    ToListAsync();
                ViewBag.SearchString = searchString;
                PageSize = 5;
            }

            else
            {
                all_students = await _db.Students.Include(c => c.Series).
                Where(c => c.SeriesId == seriesId).OrderBy(c => c.Roll).ToListAsync();
            }


            int totalItems = all_students.Count();

            var pager = new Pager(totalItems, pg, PageSize);

            int recSkip = (pg - 1) * PageSize;

            var data = all_students.Skip(recSkip).Take(PageSize).ToList();

            ViewBag.Pager = pager;

            ViewBag.SeriesId = seriesId;
            ViewBag.SeriesName = seriesName;

            return View(data);
        }

        public IActionResult Create(string SeriesName, int SeriesId, string returnUrl)
        {
            ViewBag.SeriesName = SeriesName;
            ViewBag.SeriesId = SeriesId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Students student, string returnUrl)
        {

            var has_Id = await _db.Students.Where(c => c.Roll == student.Roll).FirstOrDefaultAsync();

            var series = await _db.Series.Where(c => c.Id == student.SeriesId).FirstOrDefaultAsync();
            ViewBag.SeriesName = series.SeriesName;
            ViewBag.SeriesId = student.SeriesId;
            ViewBag.ReturnUrl = returnUrl;

            if (ModelState.IsValid && has_Id == null)
            {
                await _db.Students.AddAsync(student);
                await _db.SaveChangesAsync();
                ViewBag.Success = true;

                ModelState.Clear();


                return View();
            }



            if (has_Id != null)
            {
                ViewBag.IdMessage = "This roll no. has been added for a student!";
            }








            return View(student);


        }


        [HttpPost]
        public async Task<IActionResult> ImportFromExcelStudentData(string returnUrl, IFormFile file, int SeriesId)
        {
            if (file != null && file.Length > 0)
            {
                //Create an Instance of ExcelFileHandling
                ExcelFileHandling excelFileHandling = new ExcelFileHandling();

                //Call the CreateExcelFile method by passing the stream object which contains the Excel file
                var students = excelFileHandling.ParseExcelFile(file.OpenReadStream());

                // Now save these students to the database

                var existing_Students = new List<Students>();

                foreach (var student in students)
                {
                    var has_Id = await _db.Students.Where(c => c.Roll == student.Roll).FirstOrDefaultAsync();

                    if (has_Id != null)
                    {
                        existing_Students.Add(student);
                    }

                    else
                    {
                        student.SeriesId = SeriesId;
                        await _db.Students.AddAsync(student);
                        await _db.SaveChangesAsync();
                    }

                }



                ViewBag.Message = "Successful";
                return LocalRedirect(returnUrl);
            }



            return LocalRedirect(returnUrl);
        }
        public async Task<IActionResult> Edit(string SeriesName, int SeriesId, int id, string returnUrl)
        {
            ViewBag.SeriesName = SeriesName;
            ViewBag.SeriesId = SeriesId;

            var data = await _db.Students.Include(c => c.Series).FirstOrDefaultAsync(c => c.Id == id);

            ViewBag.ReturnURl = returnUrl;


            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,Students student, string returnURl)
        {
            if (ModelState.IsValid)
            {
                var data = await _db.Students.FindAsync(id);

                data.Roll = student.Roll;
                data.Name = student.Name;
                data.PhoneNo = student.PhoneNo;
                data.Email = student.Email;
                data.SeriesId = student.SeriesId;

                _db.Students.Update(data);
                await _db.SaveChangesAsync();
                return LocalRedirect(returnURl);
            }

            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string SeriesName, int SeriesId, int id, string returnUrl)
        {
            ViewBag.SeriesName = SeriesName;
            ViewBag.SeriesId = SeriesId;
            ViewBag.ReturnURl = returnUrl;

            var data = await _db.Students.Include(c => c.Series).FirstOrDefaultAsync(c => c.Id == id);

            return View(data);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id, Students student, string returnURl)
        {
            if (id == student.Id)
            {
                var data = await _db.Students.FindAsync(id);
                if (data != null)
                {
                    _db.Students.Remove(data);
                    await _db.SaveChangesAsync();
                    return LocalRedirect(returnURl);
                }


            }

            return View(student);
        }


        public IActionResult Details(int id)
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Semesters = _db.Semesters.ToList();
            var student_data = _db.Students.Include(c => c.Series).Where(c => c.Id == id).FirstOrDefault();
            mymodel.Student = student_data;


            mymodel.SeriesId = student_data.SeriesId;
            mymodel.SeriesName = student_data.Series.SeriesName;

            var total_courses = _db.Courses.ToList();
            
            double sum = 0.0;
            double credits = 0.0;
            
            foreach (var course in total_courses)
            {
                var data = _db.Results
                                    .Where(c => c.CourseId == course.Id && c.StudentId == id)
                                    .FirstOrDefault();
                if (data != null)
                {
                    double grade = (double) data.Grade;


                    if (grade > 0.0)
                    {
                        sum += (grade * course.Credit);
                        credits += course.Credit;

                    }

                }

            }

            mymodel.CGPA = "";

            if (credits > 0.0)
            {
                mymodel.CGPA = Math.Round(sum / credits, 2);

            }


            return View(mymodel);
        }


        public IActionResult DetailsOFaSemester(int semester_id, int student_id)
        {
            ViewBag.StudentId = student_id;
            ViewBag.SemesterId = semester_id;

            dynamic mymodel = new ExpandoObject();
            var total_courses = _db.Courses.Where(c => c.SemesterId == semester_id).ToList();
            mymodel.Courses = total_courses;
            mymodel.Student = _db.Students.Include(c => c.Series).Where(c => c.Id == student_id).FirstOrDefault();
            mymodel.Semester = _db.Semesters.Where(c => c.Id == semester_id).FirstOrDefault();

            mymodel.Results = new List<dynamic>();


            double sum = 0.0;
            double credits = 0.0;
            foreach (var course in total_courses)
            {
                var data = _db.Results
                                    .Where(c => c.CourseId == course.Id && c.StudentId == student_id)
                                    .FirstOrDefault();


                if (data == null)
                {
                    mymodel.Results.Add("Mark isn't added yet!");
                }

                else
                {
                   
                    

                    double grade = (double) data.Grade;

                    
                    if(grade > 0.0)
                    {
                        sum += (grade * course.Credit);
                        credits += course.Credit;

                    }



                    mymodel.Results.Add(data.Grade);

                }

            }

            mymodel.GPA = "";
            if (credits > 0.0)
            {
                mymodel.GPA = Math.Round(sum /credits, 2);

            }

            return View(mymodel);




        }

        public IActionResult AddMarkofACourse(int course_id, int student_id, string returnUrl)
        {

            ViewBag.CourseId = course_id;
            ViewBag.StudentId = student_id;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.StudentRoll = _db.Students.Where(c => c.Id == student_id).FirstOrDefault();
            ViewBag.CourseNo = _db.Courses.Where(c => c.Id == course_id).FirstOrDefault();

            return View();
        }

        [HttpPost]
        public IActionResult AddMarkofACourse(Result result, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                List<double> ct_marks = new List<double>();

                ct_marks.Add(result.CT1);
                ct_marks.Add(result.CT2);
                ct_marks.Add(result.CT3);
                ct_marks.Add(result.CT4);

                ct_marks = ct_marks.OrderByDescending(c => c).ToList();

                double total_marks = Math.Round(((ct_marks[0] + ct_marks[1] + ct_marks[2]) / 3.0) +
                    result.Attendence + result.Final, 2);

                double diff = Math.Round(total_marks - 40.0, 2);

                double grade = 0.0;

                if (diff >= 0.0)
                {
                    grade = Math.Round(2.0 + (diff * 0.05), 2);
                    if (grade > 4.0)
                    {
                        grade = 4.0;
                    }
                }

                result.Grade = grade;


                _db.Results.Add(result);
                _db.SaveChanges();
                return LocalRedirect(returnUrl);
            }

            return View(result);
        }

        public IActionResult UpdateMarkOfACourse(int course_id, int student_id, string returnUrl)
        {
            var data = _db.Results.Where(c => c.CourseId == course_id && c.StudentId == student_id).FirstOrDefault();
            
            ViewBag.CourseId = course_id;
            ViewBag.StudentId = student_id;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.StudentRoll = _db.Students.Where(c => c.Id == student_id).FirstOrDefault();
            ViewBag.CourseNo = _db.Courses.Where(c => c.Id == course_id).FirstOrDefault();


            return View(data);
        }

        [HttpPost]
        public IActionResult UpdateMarkOfACourse(Result result, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var data = _db.Results.Where(c=>c.CourseId==result.CourseId && 
                c.StudentId==result.StudentId).FirstOrDefault();

                
                List<double> ct_marks = new List<double>();

                ct_marks.Add(result.CT1);
                ct_marks.Add(result.CT2);
                ct_marks.Add(result.CT3);
                ct_marks.Add(result.CT4);

                ct_marks = ct_marks.OrderByDescending(c => c).ToList();

                double total_marks = Math.Round(((ct_marks[0] + ct_marks[1] + ct_marks[2]) / 3.0) +
                    result.Attendence + result.Final, 2);

                double diff = Math.Round(total_marks - 40.0, 2);

                double grade = 0.0;

                if (diff >= 0.0)
                {
                    grade = Math.Round(2.0 + (diff * 0.05), 2);
                    if (grade > 4.0)
                    {
                        grade = 4.0;
                    }
                }

                result.Grade = grade;

                data.CT1 = result.CT1;
                data.CT2 = result.CT2;
                data.CT3 = result.CT3;
                data.CT4 = result.CT4;
                data.Attendence = result.Attendence;
                data.Final = result.Final;
                data.Grade = result.Grade;

                _db.Results.Update(data);
                _db.SaveChanges();
                return LocalRedirect(ReturnUrl);
               
                
            }

            return View(result);
        }
    }
}
