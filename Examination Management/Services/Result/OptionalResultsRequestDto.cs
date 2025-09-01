namespace Examination_Management.Services.Result
{
    public class OptionalResultsRequestDto
    {
        public int? StudentID { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? RollNo { get; set; }
        public string? SubjectIDs { get; set; }   // comma-separated optional subjects
        public string? UnitIDs { get; set; }      // comma-separated unit IDs
        public string? Current_Session { get; set; }
        public bool GetAllResults { get; set; } = false;
    }
}
