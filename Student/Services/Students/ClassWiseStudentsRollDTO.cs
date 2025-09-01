namespace Student.Services.Students
{
    public class ClassWiseStudentsRollDTO
    {
        public string ClassID { get; set; }
        public string ClassName { get; set; }
        public string TotalStudents { get; set; }
        public string MaleStudents { get; set; }
        public string FemaleStudents { get; set; }
        public string PresentTotal { get; set; }
        public string AbsentTotal { get; set; }
        public string Leave { get; set; }
    }
}
