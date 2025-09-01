namespace Attendence.Services
{
    public class Attendance
    {
        public string AttendanceID { get; set; }
        public string ClassID { get; set; }
        public string SectionID { get; set; }
        public string StudentID { get; set; }
        public string Current_Session { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public string Narration { get; set; }
        public int ActionType { get; set; } = 0;
        public string ClassName { get; set; }
        public string SectionName { get; set; }
        public string Prescent { get; set; }
        public string Abscent { get; set; }
        public string Leave { get; set; }
        public string Halfleave { get; set; }
        public string StudentName { get; set; }
        public string Rollno { get; set; }
        public string FatherPhone { get; set; }

        public string MotherPhone { get; set; }

        public string Address { get; set; }

        public string PresentAddress { get; set; }

    }
}
