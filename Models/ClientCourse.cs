using System.Text.Json.Serialization;

namespace ComputerCourses.Models
{
    public class ClientCourse
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        [JsonIgnore]
        public Client? Client { get; set; }
        public int CourseId { get; set; }
        [JsonIgnore]
        public Course? Course { get; set; }
        public List<int> Marks { get; set; } = new List<int>();

        public double? AverageMark()
        {
            if (Marks != null && Marks.Count != 0) { return Marks.Average(); } else { return null; }
        }
    }
}
