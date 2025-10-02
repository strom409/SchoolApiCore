namespace FeeManagement.Services.FeeDueRebate
{
    public class FeeDueRebateDTO
    {
        public long RebateID { get; set; }
        public long FeeDueIDFK { get; set; }
        public long FeeHeadIDFK { get; set; }
        public int FeeCatgIDFK { get; set; }
        public long ClassIDFK { get; set; }
        public long SectionIDFK { get; set; }
        public long StudentIDFK { get; set; }
        public long AdmissionNoIDFK { get; set; }
        public int IsYearly { get; set; }
        public int IsFlat { get; set; }
        public decimal? FlatAmount { get; set; }
        public decimal? RebatePercent { get; set; }
        public decimal? RebatePercentAmount { get; set; }
        public string OnMonth { get; set; }
        public int MonthIDFK { get; set; }
        public bool IsDeleted { get; set; }
        public bool Status { get; set; }
        public string Current_Session { get; set; }
        public string UserName { get; set; }
        public DateTime OnDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Remarks { get; set; }
    }

}
