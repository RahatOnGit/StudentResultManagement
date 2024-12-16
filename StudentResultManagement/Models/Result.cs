using System.ComponentModel.DataAnnotations.Schema;

namespace StudentResultManagement.Models
{
    public class Result
    {
        public int Id { get; set; }

        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]

        public Students? Student { get; set; }

        public int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public string Mark {  get; set; }
    }
}
