namespace HR.Services.Salary
{

    public class SalaryPayment
    {
        public string SPID { get; set; }
        public string PIDFK { get; set; }
        public string EmployeeCodeFK { get; set; }
        public string EmployeeName { get; set; }
        public string SAmount { get; set; }
        public string NetPay { get; set; }
        public string Balance { get; set; }
        public string SDate { get; set; }
        public string SMonth { get; set; }
        public string SYear { get; set; }
        public string MonthNo { get; set; }
        public string UpdatedBy { get; set; }
        public string SalaryIDFK { get; set; }
        public string IsDeleted { get; set; }
        public string DeletedOn { get; set; }
        public string SRemarks { get; set; }
        public string SalaryMethod { get; set; }

    }

    public class SalaryPaymentReport : SalaryPayment
    {
        public string TempBasicPay { get; set; }
        public string TotalAllownce { get; set; }
        public string GrossPay { get; set; }
        public string CPFDeduction { get; set; }
        public string LeavesTaken { get; set; }
        public string TotelLeavDedAmt { get; set; }
        public string TotalDeduction { get; set; }

    }
}
