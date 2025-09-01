namespace HR.Services.SalaryPayment
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    namespace EasioCore
    {
        public class ExpensesData
        {
            public string EPID { get; set; }
            public string EID { get; set; }
            public string ExpensesDate { get; set; }
            public string Rate { get; set; }
            public string Qnty { get; set; }
            public string Amount { get; set; }
            public string CashOut { get; set; }
            public string Balance { get; set; }
            public string CashLID { get; set; }  // Cash Bank Ledeger
            public string ExpenseLID { get; set; } // Expenses Ledeger ID
            public string CreditorLID { get; set; } = "0";// Creditor Ledeger ID
            public string FYear { get; set; }
            public string SYear { get; set; }
            public string SMonth { get; set; }
            public string IsDeleted { get; set; }
            public string DeletedBy { get; set; }
            public string DeletedOn { get; set; }
            public string UserName { get; set; }
            public string UpdatedBy { get; set; }
            public string UpdatedOn { get; set; }
            public string IsCancelled { get; set; }
            public string CancelledBy { get; set; }
            public string CancelledOn { get; set; }
            public string PaymentNo { get; set; }
            public string Remarks { get; set; }
            public string PaymentMode { get; set; }
            public string CreditorLName { get; set; }
            public string CashLName { get; set; }
            public string ExpensesLName { get; set; }
            public string ExpenseType { get; set; } = "0";
            public string PaymentType { get; set; } = "0";
            public int ActionType { get; set; } = 0;
        }
    }
}
