namespace HR.Services.Salary
{
    public class SalaryModel
    {
        public long? SalaryID { get; set; }
        public Nullable<long> EmployeeID { get; set; }
        public Nullable<long> EmployeeCode { get; set; }
        public Nullable<long> EDID { get; set; }
        public string? EmployeeName { get; set; }
        public string? SubDepartmentName { get; set; }
        public string? DepartmentID { get; set; }
        public string? DepartmentIDS { get; set; }
        public string? Designation { get; set; }
        public string? DesignationID { get; set; }
        public string? Status { get; set; }

        public Nullable<decimal> BasicPay { get; set; }
        public Nullable<decimal> LoanBalance { get; set; }
        public Nullable<decimal> DarenessAllownce { get; set; }
        public Nullable<decimal> SACAllownce { get; set; }
        public Nullable<decimal> HouseRentAllownce { get; set; }
        public Nullable<decimal> MedicalAllownce { get; set; }
        public Nullable<decimal> AdditionslAllownce { get; set; }
        public Nullable<decimal> Increment { get; set; }
        public Nullable<int> LeavesAvailable { get; set; }
        public Nullable<decimal> LeavesTaken { get; set; }
        public Nullable<decimal> CPFundCollection { get; set; }
        public Nullable<decimal> SecurityFundcollection { get; set; }
        public Nullable<decimal> InsuranceAmount { get; set; }
        public Nullable<decimal> DARate { get; set; }
        public Nullable<decimal> EmployeeCPShare { get; set; }
        public Nullable<decimal> EmployerCPShare { get; set; }
        public Nullable<decimal> LoanDeduction { get; set; }
        public Nullable<decimal> InsuranceInstallment { get; set; }
        public Nullable<decimal> CPFundEmprRate { get; set; }
        public Nullable<decimal> Insurance1PercentRate { get; set; }
        public Nullable<decimal> Insurance1PercentAmt { get; set; }
        public Nullable<decimal> CPFDeduction { get; set; }
        public Nullable<decimal> CPFRecoveryDedAmt { get; set; }
        public Nullable<decimal> CPFLoanCollection { get; set; }
        public Nullable<decimal> SecurityDeduction { get; set; }
        public Nullable<decimal> PenaltyDeduction { get; set; }
        public Nullable<decimal> TotalAllownce { get; set; }
        public Nullable<decimal> TotalLeavAddAmt { get; set; }
        public Nullable<decimal> TotelLeavDedAmt { get; set; }
        public Nullable<decimal> TotalDeduction { get; set; }
        public Nullable<decimal> Pay { get; set; }
        public Nullable<decimal> GrossPay { get; set; }
        public Nullable<decimal> NetPay { get; set; }
        public string Year { get; set; }
        public Nullable<long> MonthID { get; set; }
        public string Month { get; set; }
        public Nullable<decimal> SpAllownceA { get; set; }
        public Nullable<decimal> SpAllownceB { get; set; }
        public Nullable<decimal> LoanAmountRefund { get; set; }
        public Nullable<decimal> LoanRecovery { get; set; }
        public Nullable<decimal> LoanTaken { get; set; }
        public string Scale { get; set; }
        public Nullable<bool> LeavesApplied { get; set; }



        public Nullable<bool> BankAccount { get; set; }
        public string BankAccountNo { get; set; }
        public string CPFundAccountNo { get; set; }
        public string InsurancePolicyNo { get; set; }
        public string Grade { get; set; }
        public Nullable<bool> SalaryStoped { get; set; }

        public Nullable<decimal> TravelAllownce { get; set; }
        public Nullable<decimal> RationAllownce { get; set; }



        public Nullable<System.DateTime> SalaryDate { get; set; }
        public Nullable<decimal> CPFLBalanceRefund { get; set; }
        public Nullable<decimal> CPFLoanTaken { get; set; }
        public Nullable<decimal> CPFLoanBalance { get; set; }
        public Nullable<decimal> TempBasicPay { get; set; }
        public Nullable<int> ExcessLeaves { get; set; }
        public Nullable<int> WorkingDays { get; set; }
        public Nullable<decimal> ExcessLeaveDeduction { get; set; }
        public Nullable<decimal> TransportDedAmt { get; set; }
        public Nullable<decimal> WelFund { get; set; }
        public Nullable<bool> CPFundStatus { get; set; }
        public string FYear { get; set; }
        public int? Days { get; set; }
        public Nullable<decimal> ExtraWorkFK { get; set; }
        public string UserName { get; set; }
    }
}
