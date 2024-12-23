using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

      

        public IActionResult GeneratePdf(GenerateResultViewModel model)
        {
            int series_id = model.Series;
            int semester_id = model.Semester;

            var all_students = _db.Students.Where(c => c.SeriesId == series_id).OrderBy
                (c => c.Roll).ToList();
            var all_courses = _db.Courses.Where(c => c.SemesterId == semester_id).ToList();

            ViewBag.Series = new SelectList(_db.Series, "Id", "SeriesName");
            ViewBag.Semester = new SelectList(_db.Semesters, "Id", "Name");

            ViewBag.SeriesId = series_id;
            ViewBag.SemesterId = semester_id;

            if (all_students.Count == 0)
            {
                ViewBag.Message = true;
                return View("Index");
            }

            var results = new List<dynamic>();
            foreach (var student in all_students)
            {
                dynamic one_student = new ExpandoObject();
                one_student.Id = student.Roll;
                one_student.Name = student.Name;
                var marks = new List<double>();
                var courses = new List<string>();


                foreach (var course in all_courses)
                {


                    // Fetching the mark for the current student and course
                    var obtainedMark = _db.Results
                        .Where(m => m.StudentId == student.Id && m.CourseId == course.Id).FirstOrDefault();

                    if (obtainedMark != null)
                    {
                        double grade = (double)obtainedMark.Grade;
                        marks.Add(grade);
                    }

                    else
                    {
                        marks.Add(0.0);

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

            var all_students = _db.Students.Where(c => c.SeriesId == series).OrderBy
                (c => c.Roll).ToList();
            var seriesData = _db.Series.Find(series);
            var semesterData = _db.Semesters.Find(semester);
            var all_courses = _db.Courses.Where(c => c.SemesterId == semester).ToList();

            Dictionary<int, List<double>> data = new Dictionary<int, List<double>>();
            List<string> coursesName
                = new List<string>();

            foreach (var i in all_courses)
            {
                coursesName.Add(i.CourseNo);
            }

            foreach (var student in all_students)
            {
                var student_id = student.Id;

                List<double> temp_data = new List<double>();
                foreach (var course in all_courses)
                {
                    var res = _db.Results.Where(c => c.CourseId == course.Id && c.StudentId
                    == student_id).FirstOrDefault();

                    if (res != null)
                    {
                        double grade = (double)res.Grade;

                        temp_data.Add(grade);
                    }

                    else
                    {
                        temp_data.Add(0.0);
                    }
                }

                data.Add(student_id, temp_data);
            }


            //Create an Instance of PDFService
            PDFService pdfService = new PDFService();

            string seriesName = seriesData.SeriesName;
            string semesterName = semesterData.Name;


            var pdfFile = pdfService.GeneratePDF(all_students, data, coursesName, seriesName, semesterName);

            //Return the PDF File
            return File(pdfFile, "application/pdf", seriesData.SeriesName + " Series , " + semesterData.Name + " Semester Result" + ".pdf");
        }

        public IActionResult ImportMarksFromExcel()
        {
            ViewBag.Courses = new SelectList(_db.Courses, "Id", "CourseNo");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ImportMarksFromExcel(int CourseId, IFormFile file)
        {

            ViewBag.Courses = new SelectList(_db.Courses, "Id", "CourseNo");

            if (file != null && file.Length > 0)
            {
                //Create an Instance of ExcelFileHandling
                ExcelFileHandling excelFileHandling = new ExcelFileHandling();

                //Call the CreateExcelFile method by passing the stream object which contains the Excel file
                var marks = excelFileHandling.ParseMarksExcelFile(file.OpenReadStream());

                foreach (var mark in marks)
                {
                    var student = await _db.Students.Where(c=>c.Roll==mark.Roll).FirstOrDefaultAsync();

                    if (student != null)
                    {
                        var has_result = await _db.Results.Where(c=>c.StudentId==student.Id && c.CourseId==CourseId).
                            FirstOrDefaultAsync();

                        if(has_result==null)
                        {
                            //Need to create
                            List<double> ct_marks = new List<double>();

                            ct_marks.Add(mark.CT1);
                            ct_marks.Add(mark.CT2);
                            ct_marks.Add(mark.CT3);
                            ct_marks.Add(mark.CT4);

                            ct_marks = ct_marks.OrderByDescending(c => c).ToList();

                            double total_marks = Math.Round(((ct_marks[0] + ct_marks[1] + ct_marks[2]) / 3.0) +
                                mark.Attendance + mark.Final, 2);

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

                            mark.Grade = grade;

                            var result = new Result
                            {
                                StudentId = student.Id,
                                CourseId = CourseId,
                                CT1 = mark.CT1,
                                CT2 = mark.CT2,
                                CT3 = mark.CT3,
                                CT4 = mark.CT4,
                                Attendence = mark.Attendance,
                                Final = mark.Final,
                                Grade = mark.Grade
                            };


                            _db.Results.Add(result);
                            _db.SaveChanges();
                        }

                        else
                        {
                            //Need to update
                            


                            List<double> ct_marks = new List<double>();

                            ct_marks.Add(mark.CT1);
                            ct_marks.Add(mark.CT2);
                            ct_marks.Add(mark.CT3);
                            ct_marks.Add(mark.CT4);

                            ct_marks = ct_marks.OrderByDescending(c => c).ToList();

                            double total_marks = Math.Round(((ct_marks[0] + ct_marks[1] + ct_marks[2]) / 3.0) +
                                mark.Attendance + mark.Final, 2);

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

                            mark.Grade = grade;

                            has_result.CT1 = mark.CT1;
                            has_result.CT2 = mark.CT2;
                            has_result.CT3 = mark.CT3;
                            has_result.CT4 = mark.CT4;
                            has_result.Attendence = mark.Attendance;
                            has_result.Final = mark.Final;
                            has_result.Grade = mark.Grade;

                            _db.Results.Update(has_result);
                            _db.SaveChanges();
                        }
                    }
                }
                
                   
                
                TempData["Message"] = "ok";
                return View(); // Redirect to a view showing success or list of products
            }

            return View();

        }

    }
}
