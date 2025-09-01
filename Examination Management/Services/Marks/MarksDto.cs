namespace Examination_Management.Services.Marks
{
    public class MarksDto
    {
        public string? MarksID { get; set; }
        public string? StudentID { get; set; }
        public string? ClassID { get; set; }
        public string? SectionID { get; set; }
        public string? Rollno { get; set; }
        public string? MaxID { get; set; }
        public string? UnitID { get; set; }
        public string? SubjectID { get; set; }
        public string? OptionalID { get; set; }
        public string? Current_Session { get; set; }
        public string? SessionID { get; set; }
        public int? Status { get; set; }
        public decimal? Marks { get; set; }
        public DateTime? Date { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? SectionName { get; set; }
        public string? UnitName { get; set; }
        public string? StudentName { get; set; }

    }
}
