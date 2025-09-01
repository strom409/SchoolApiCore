namespace HR.Services
{
    public class SubDepartment
    {
        public long SubDepartmentID { get; set; }
        public string SubDepartmentName { get; set; }
        public Nullable<long> DepartmentID { get; set; }
        public string Current_Session { get; set; }
        public Nullable<long> SessionID { get; set; }
    }
}
