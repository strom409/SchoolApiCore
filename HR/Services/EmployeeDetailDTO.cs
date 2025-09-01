namespace HR.Services
{
    public class EmployeeDetailDTO
    {
        public long? EDID { get; set; }
        public long? EmployeeID { get; set; }
        public string Current_Session { get; set; }
        public long? SessionID { get; set; }
        public string Status { get; set; }
        public long? DesignationID { get; set; }
        public bool? BankAccount { get; set; }
        public string BankAccountNo { get; set; }
        public long? CPFundAccountNo { get; set; }
        public string InsurancePolicyNo { get; set; }
        public string Grade { get; set; }
        public decimal? BasicPay { get; set; }
        public decimal? CPFundCollection { get; set; }
        public decimal? SecurityFundcollection { get; set; }
        public decimal? LoanBalance { get; set; }
        public decimal? DarenessAllownce { get; set; }
        public decimal? SACAllownce { get; set; }
        public decimal? HouseRentAllownce { get; set; }
        public decimal? MedicalAllownce { get; set; }
        public decimal? AdditionslAllownce { get; set; }
        public decimal? TravelAllownce { get; set; }
        public decimal? Increment { get; set; }
        public decimal? LeavesAvailable { get; set; }
        public decimal? LeavesTaken { get; set; }
        public System.DateTime? DateofIncrement { get; set; }
        public decimal? InsuranceAmount { get; set; }
        public decimal? RationAllownce { get; set; }
        public decimal? DARate { get; set; }
        public decimal? EmployeeCPShare { get; set; }
        public decimal? EmployerCPShare { get; set; }
        public decimal? LoanDeduction { get; set; }
        public bool? CPFundStatus { get; set; }
        public string Remarks { get; set; }
        public long? RouteID { get; set; }
        public long? SubDepartmentID { get; set; }
        public decimal? InsuranceInstallment { get; set; }
        public string Scale { get; set; }
        public decimal? Insurance1PercentRate { get; set; }
        public decimal? Insurance1PercentAmt { get; set; }
        public decimal? CPFDeduction { get; set; }
        public decimal? CPFRecoveryDedAmt { get; set; }
        public decimal? CPFLoanTaken { get; set; }
        public decimal CPFLoanCollection { get; set; }
        public decimal? SecurityDeduction { get; set; }
        public decimal? PenaltyDeduction { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public decimal? SpAllownceA { get; set; }
        public decimal? SpAllownceB { get; set; }
        public bool? LeavesApplied { get; set; }
        public decimal? CPFundIntrest { get; set; }
        public decimal? CPFPensionRate { get; set; } // 


        public decimal? NPSRate { get; set; }
        public bool? SalaryStoped { get; set; }
        public decimal? ExcessLeaves { get; set; }
        public int? WorkingDays { get; set; }
        public decimal? TransportDedAmt { get; set; }
        public decimal? WelFund { get; set; }
        public string FYear { get; set; }
        public bool? WithdrawnEmp { get; set; }
        public int? QidFk { get; set; }
        public int? IsBEd { get; set; }
        public DateTime? DateOfWithdraw { get; set; }
        public string WithdrawRemarks { get; set; }
        public decimal? ExtraWork { get; set; }
        public int? IsTeacher { get; set; }
        public long? ESIDFK { get; set; }
        public string UserName { get; set; }
        public string BankBranch { get; set; }
        public string IFSCCode { get; set; }
        public string BankName { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }

        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
    }
}
