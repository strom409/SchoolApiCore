namespace Student.Services.Students
{
    public class SessionDto
    {
        public string? SessionID { get; set; }
        public string? Session { get; set; }
        public DateTime? SessionFrom { get; set; }
        public DateTime? SessionTo { get; set; }
        public string? Current_Year { get; set; }
    }
}
