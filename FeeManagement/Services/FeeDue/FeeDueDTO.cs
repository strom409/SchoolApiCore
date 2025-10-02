namespace FeeManagement.Services.FeeDue
{
    public class FeeDueDTO
    {
        public long FeeDueID { get; set; }
        public long ClassIDFK { get; set; }
        public long SectionIDFK { get; set; }
        public long StudentIdFk { get; set; }
        public long StudentInfoIdFk { get; set; }
        public int? FhIdFk { get; set; }
        public int? DidFk { get; set; }
        public long? FsIdFk { get; set; }
        public int? FCategoryId { get; set; }
        public int? FeeHeadType { get; set; }
        public string? FeeHeadName { get; set; }
        public string? CurrentSession { get; set; }
        public string? FeeMonth { get; set; }
        public int? FeelMonthIdFk { get; set; }
        public string? FeeYear { get; set; }
        public string? SystemYear { get; set; }
        public string? SystemMonth { get; set; }
        public int? TransactionType { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? BillDate { get; set; }
        public string? UserName { get; set; }
        public int? IsPaid { get; set; }
        public long? RebateIdFk { get; set; }
        public long? DiscountIdFk { get; set; }
        public long? BulkAdvancedDfk { get; set; }
        public long? FeeAdvancedDfk { get; set; }
        public decimal? FeeRate { get; set; }
        public decimal? Rebate { get; set; }
        public decimal? Balance { get; set; }
        public decimal? BulkAdvance { get; set; }
        public decimal? Advance { get; set; }
        public decimal? Discount { get; set; }
        public decimal? LateFee { get; set; }
        public int? AdvanceType { get; set; }
        public decimal? ToPay { get; set; }
        public int? IsDeleted { get; set; }
        public int? IsApplicable { get; set; }
        public string? Remarks { get; set; }
        public decimal? Adjustment { get; set; }
        public long? AdjId { get; set; }
        public int? IsFlat { get; set; }
        public decimal? Percentage { get; set; }
    }
}
