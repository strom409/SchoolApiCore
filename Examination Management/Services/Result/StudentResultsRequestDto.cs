namespace Examination_Management.Services.Result
{
    public class StudentResultsRequestDto
    {
        public int? StudentID { get; set; }
        public string? AdmissionNo { get; set; }
        public int? ClassID { get; set; }
        public int? SectionID { get; set; }
        public int? RollNo { get; set; }
        public string? SubjectIDs { get; set; }   
        public string? UnitIDs { get; set; }       
        public string? Current_Session { get; set; }
        public bool? GetAllResults { get; set; }

    }
}
