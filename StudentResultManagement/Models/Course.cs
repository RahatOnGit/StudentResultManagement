using System.ComponentModel.DataAnnotations.Schema;

namespace StudentResultManagement.Models
{
    public class Course
    {
        public int Id { get; set; } 

        public string CourseNo { get; set; }

        public string CourseTitle { get; set; }

        public int? SemesterId { get; set; }

        [ForeignKey("SemesterId")]
        public Semesters? Semester { get; set; }  




    }
}
