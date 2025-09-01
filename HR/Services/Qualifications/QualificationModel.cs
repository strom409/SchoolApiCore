namespace HR.Services.Qualifications
{
    public class QualificationModel
    {
        public long QID { get; set; }
        public string Qualification { get; set; }
        public string UserName { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}
