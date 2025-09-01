namespace Examination_Management.Services.Result
{
    public class OptionalStudentResultDto
    {
        public long OpMarksID { get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; } = null!;
        public int ClassID { get; set; }
        public string ClassName { get; set; } = null!;
        public int SectionID { get; set; }
        public string SectionName { get; set; } = null!;
        public int RollNo { get; set; }
        public int OptionalSubjectID { get; set; }
        public string OptionalSubjectName { get; set; } = null!;
        public int UnitID { get; set; }
        public string UnitName { get; set; } = null!;
        public decimal Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public string? Grade { get; set; }
        public decimal? Percentage { get; set; }
        public string? TeacherRemarks { get; set; }
        public string? PrincipalRemarks { get; set; }
    }
}
