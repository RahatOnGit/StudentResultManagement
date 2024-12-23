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

        public double CT1{  get; set; }

        public double CT2{ get; set; }

        public double CT3{ get; set; }

        public double CT4{ get; set; }

        public double Attendence{ get; set; }

        public double Final { get; set; }

        public double? Grade { get; set; }
    }
}
