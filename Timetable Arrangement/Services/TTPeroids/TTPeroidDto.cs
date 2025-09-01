namespace Timetable_Arrangement.Services.TTPeroids
{
    public class TTPeroidDto
    {
        public long PeroidID { get; set; }  
        public long PIDFK { get; set; }     
        public string PeroidFrom { get; set; }
        public string PeroidTo { get; set; }
        public long DIDFK { get; set; }
    }
}
