namespace HR.Services.PayRoll
{
    public class LoanPayment
    {
        public decimal LoanInstallment { get; set; }
        public long LoanID { get; set; }
        public long EmployeeID { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string ActionType { get; set; }
    }
}
