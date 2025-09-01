namespace HR.Services.Salary
{
    public class GrossPaySalary
    {
        public decimal GrossPay { get; set; } = 0;
        public decimal GP63 { get; set; } = 0;
        public decimal GP37 { get; set; } = 0;
        public decimal BP { get; set; } = 0;
        public decimal DA { get; set; } = 0;
        public decimal Pay { get; set; } = 0;
        public decimal HRA { get; set; } = 0;
        public decimal Medical { get; set; } = 0;
        public decimal OtherAllow { get; set; } = 0;
        public decimal EPF { get; set; } = 0;
        public decimal EPFRate { get; set; } = 0;
        public decimal LI { get; set; } = 0;
        public decimal NetPay { get; set; } = 0;
        public decimal ELeave { get; set; } = 0;
        public decimal GrossPayFinal { get; set; } = 0;
        /// <summary>
        /// / Deduction 
        /// </summary>
        public decimal EPFDeduction { get; set; } = 0;
        public decimal LIDeduction { get; set; } = 0;
        public decimal OtherDeduction { get; set; } = 0;

        /// <summary>
        /// / Employee Details
        /// </summary>
        public string EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EDID { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentID { get; set; }
        public string Year { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedOn { get; set; }
        public string EPFStatus { get; set; }
    }
}
