namespace Student.Services.ClassMaster
{
    public class ClassModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Current_Session { get; set; }
        public int SessionID { get; set; }
        public int SubDepartmentID { get; set; }
        public string ClassIncharg { get; set; }
    }
}
