namespace ComputerCourses.Models
{
    public class Description
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int CourseId { get; set; }
        public List<int> Marks { get; set; } = new List<int>();
        public DateTime StudyStart { get; set; } = DateTime.Now;

        public double? AverageMark() {
            if (Marks != null && Marks.Count != 0) { return Marks.Average(); } else { return null; }
        }
    }
}
