using System.Text.Json.Serialization;

namespace Student.Services.ClassMaster
{
    public class SectionDto
    {
        public int? SectionID { get; set; }
        public string? SectionName { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SessionId { get; set; }
        public string? Current_Session { get; set; }
        public string? EmpCode { get; set; }
        public string? EmployeName { get; set; }
    }
}
