using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentResultManagement.Data;
using StudentResultManagement.Models;
using System.Collections.Generic;
using System.Dynamic;


namespace StudentResultManagement.Controllers
{

    [Authorize]
    public class GenerateResultController : Controller
    {
        private readonly ApplicationDbContext _db;

        public GenerateResultController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.Series = new SelectList(_db.Series, "Id", "SeriesName");
            ViewBag.Semester = new SelectList(_db.Semesters, "Id", "Name");
            return View();
        }

        public IActionResult AllStudents(GenerateResultViewModel model)
        {
            ViewBag.Semester_Id = model.Semester;
            ViewBag.Series_Id = model.Series;

            var semester = _db.Semesters.Where(c => c.Id == model.Semester).FirstOrDefault();
            ViewBag.SemesterName = semester.Name;

            var series = _db.Series.Where(c=>c.Id == model.Series).FirstOrDefault();
            ViewBag.SeriesName = series.SeriesName;

            var data = _db.Students.Where(c=>c.SeriesId==model.Series).ToList();
            return View(data); 
        }

        public IActionResult GeneratePdf(int series, int semester)
        {
            var all_students = _db.Students.Where(c=>c.SeriesId == series).ToList();
            var all_courses = _db.Courses.Where(c => c.SemesterId == semester).ToList();

            ViewBag.Series = series;
            ViewBag.Semester = semester;

            var results = new List<dynamic>();
            foreach(var student in all_students)
            {
                dynamic one_student = new ExpandoObject();
                one_student.Id = student.Roll;
                one_student.Name = student.Name;
                var marks = new List<string>();
                var courses = new List<string>();


                foreach (var course in all_courses)
                {
                    

                    // Fetching the mark for the current student and course
                    var obtainedMark = _db.Results
                        .FirstOrDefault(m => m.StudentId == student.Id && m.CourseId == course.Id);
                    if (obtainedMark != null)
                    {
                        marks.Add(obtainedMark.Mark);
                    }
                    else
                    {
                        marks.Add("0");

                    }

                    courses.Add(course.CourseNo);
                    
                }
                one_student.Marks = marks;
                one_student.courses = courses;

                results.Add(one_student);

            }
           

             return View(results);
            


        }

       

        public IActionResult GenerateResultSheetPDF(int series, int semester)
        {

            var all_students = _db.Students.Where(c => c.SeriesId == series).ToList();
            var seriesData = _db.Series.Find(series);
            var semesterData = _db.Semesters.Find(semester);
            var all_courses = _db.Courses.Where(c=>c.SemesterId==semester).ToList();

            Dictionary<int, List<string>> data = new Dictionary<int, List<string>>();
            List<string> coursesName
                = new List<string>();

            foreach(var i in all_courses)
            {
                coursesName.Add(i.CourseNo);
            }

            foreach (var student in all_students)
            {
                var student_id = student.Id;

                List<string> temp_data = new List<string>();
                foreach (var course in all_courses)
                {
                    var res = _db.Results.Where(c=>c.CourseId==course.Id && c.StudentId
                    ==student_id).FirstOrDefault();

                    if(res!=null)
                    {
                        temp_data.Add(res.Mark);
                    }

                    else
                    {
                        temp_data.Add("0");
                    }
                }

                data.Add(student_id, temp_data);
            }
           

            //Create an Instance of PDFService
            PDFService pdfService = new PDFService();

            string seriesName = seriesData.SeriesName;
            string semesterName = semesterData.Name;

           
            var pdfFile = pdfService.GeneratePDF(all_students , data, coursesName, seriesName, semesterName);

            //Return the PDF File
            return File(pdfFile, "application/pdf",seriesData.SeriesName+" Series , "+ semesterData.Name+" Semester Result" + ".pdf");
        }


    }
}
