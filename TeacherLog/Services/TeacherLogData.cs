namespace TeacherLog.Services
{
    public class TeacherLogData
    {
        public string LogId { get; set; }
        public string EmployeeCode { get; set; }
        public string Period { get; set; }
        public string ClassID { get; set; }
        public string SectionID { get; set; } = "0";
        public string SubjectID { get; set; } = "0";
        public string SubSubjectID { get; set; } = "0";
        public string TotalStrength { get; set; }
        public string TotalPresent { get; set; }
        public string PresentNow { get; set; }
        public string Day { get; set; }
        public string Date { get; set; }
        public string AbsenceReason { get; set; }
        public string Topic { get; set; }
        public string HomeWork { get; set; }
        public string Aids { get; set; }
        public string Remarks { get; set; }
        public string SubjectName { get; set; }
        public string SubSubjectName { get; set; }
        public string EmployeeName { get; set; }
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public int ActionType { get; set; }
        public string classwork { get; set; }
        public string studentid { get; set; }
        public string current_session { get; set; }
        public string Performance { get; set; }
        public string EDI { get; set; }
        public List<string> FilePaths { get; set; }

    }
}
