using System.Text.Json.Serialization;

namespace HR.Services
{
    public  class Employee
    {
        public Nullable<long> EmployeeID { get; set; }
        public string? EmployeeName { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public string? DOBString { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PinNo { get; set; }
        public string? PhoneNo { get; set; }
        public string? Qualification { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public string? SessionOfJoin { get; set; }
        ///  public Nullable<byte[]> Photo { get; set; }
        public string? PhotoPath { get; set; }
        public string? FatherName { get; set; }
        public Nullable<long> EmployeeCode { get; set; }
        public Nullable<System.DateTime> DateOfPermanent { get; set; }
        public Nullable<decimal> OpeningBalance { get; set; }
        public Nullable<bool> Withdrawn { get; set; }
        public Nullable<System.DateTime> DOW { get; set; }
        public string? LIDFK { get; set; }
        public string? SpouseName { get; set; }
        public string? OtherQual { get; set; }
        public string? E_Mail { get; set; }
        public string? AdhaarNo { get; set; }
        public Nullable<int> m_status { get; set; }
        public string? PANCard { get; set; }
        public string? NPSNo { get; set; }
        //EmployeeDetail ED = new EmployeeDetail();


    }
}
