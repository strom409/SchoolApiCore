namespace Examination_Management.Services.OptionalMaxMarks
{
    public class OptionalMaxMarksDto
    {
        public string? OpMaxID { get; set; }
        public decimal? MaxMarks { get; set; }
        public decimal? MinMarks { get; set; }
        public string? ClassId { get; set; }
        public string? UnitID { get; set; }
        public string Current_Session { get; set; }
        public string? SessionID { get; set; }
        public string? OptionalSubjectid { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? UnitName { get; set; }
        public string? SectionName { get; set; }
    }
}
