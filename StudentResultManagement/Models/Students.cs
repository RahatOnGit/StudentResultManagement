using System.ComponentModel.DataAnnotations.Schema;

namespace StudentResultManagement.Models
{
    public class Students
    {
        public int Id { get; set; }

        public string Roll { get; set; }

        public string Name { get; set; }

        public string PhoneNo { get; set; }

        public string? Email { get; set; }

        public int? SeriesId { get; set; }

        [ForeignKey("SeriesId")]
        public Series? Series { get; set; }
    }
}
