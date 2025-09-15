namespace Timetable_Arrangement.Services.TimeTableArrangements
{
    public class TimeTableArrangementsDTO
    {
        public long ID { get; set; }
        public long TeacherID { get; set; }
        public long AbscentTeacherID { get; set; }
        public long DayID { get; set; }
        public long PeriodID { get; set; }
        public long ClassID { get; set; }
        public long SectionID { get; set; }
        public long SubjectID { get; set; }
        public DateTime OnDate { get; set; }
        public DateTime InsertedOn { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
