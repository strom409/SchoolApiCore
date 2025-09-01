namespace Examination_Management.Services.Result
{
    public class StudentResultDto
    {
        public string StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public string StudentName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public string DepartmentName { get; set; }
        public int RollNo { get; set; }
        public int ResultID { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal Marks { get; set; }
        public decimal? MaxMarks { get; set; }
        public string Grade { get; set; }
        public decimal? Percentage { get; set; }
        public string TeacherRemarks { get; set; }
        public string PrincipalRemarks { get; set; }
    }

}
