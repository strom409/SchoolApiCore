namespace Timetable_Arrangement.Services.TTAssignPeroids
{
    public class TimeTable
    {
        public string? DID { get; set; }
        public string? DayName { get; set; }
        public string? PID { get; set; }
        public string? peroid { get; set; }
        public string? PeroidName { get; set; }
        public string? PeroidID { get; set; }
        public string? PIDFK { get; set; }
        public string? PeroidFrom { get; set; }
        public string? PeroidTo { get; set; }
        public string? ATTID { get; set; }
        public string? ClassID { get; set; }
        public string? SecID { get; set; }
        public string? SubSubjectID { get; set; }
        public string? TeacherID { get; set; }
        public string? AbscentTeacherID { get; set; }
        public string? AbscentTeacherName { get; set; }
        public string? PeroidFK { get; set; }
        public string? TeacherName { get; set; }
        public string? DIDFK { get; set; }

        public string? SubSubjectName { get; set; }
        public string? EmployeeID { get; set; }
        public string? employeename { get; set; }
        public string? Designation { get; set; }
        public string? DesignationID { get; set; }
        public string? Gender { get; set; }
        public string? PeroidFKduration { get; set; }

        public string? ClassName { get; set; }

        public string? SectionName { get; set; }
        public string? DayIDS { get; set; }
        public string? IsHalfDay { get; set; } = "0";
        public string? Alldata { get; set; }
        public string? IsFree { get; set; } = "0";

        public string? OnDate { get; set; }


        public string? UserName { get; set; }
    }
}
