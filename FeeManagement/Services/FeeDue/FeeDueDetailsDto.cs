namespace FeeManagement.Services.FeeDue
{
    public class FeeDueDetailsDto
    {
        public long FeeDueID { get; set; }
        public long SectionIDFK { get; set; }
        public string SectionName { get; set; }
        public long ClassIDFK { get; set; }
        public string ClassName { get; set; }
        public long StudentIDFK { get; set; }
        public string StudentName { get; set; }
        public long StudentInfoIDFK { get; set; }
        public int FHIDFK { get; set; }
        public string FeeHeadName { get; set; }
        public long FSIDFK { get; set; }
        public string FeeStructureName { get; set; }
        public int FCategoryID { get; set; }
        public int FeeHeadType { get; set; }
        public string Current_Session { get; set; }
        public string FeeMonth { get; set; }
        public string? FeeYear { get; set; }
        public string? SystemYear { get; set; }
        public string? SystemMonth { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? BillDate { get; set; }
        public decimal ToPay { get; set; }
        public int IsPaid { get; set; }
        public string Remarks { get; set; }
    }
}
