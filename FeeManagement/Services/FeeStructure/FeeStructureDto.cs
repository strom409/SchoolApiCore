namespace FeeManagement.Services.FeeStructure
{
    public class FeeStructureDto
    {
        public long FSID { get; set; }
        public long? FHIDFK { get; set; }
        public long? CIDFK { get; set; }
        public string CSession { get; set; }
        public decimal? Amount { get; set; }
        public string UserName { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }
        public string Title { get; set; }
        public string Remarks { get; set; }
        public long? SecIDFK { get; set; }
        public long? FeeCatID { get; set; }
        public long? FeeSID { get; set; }
        public decimal? Rate { get; set; }
    }
}
