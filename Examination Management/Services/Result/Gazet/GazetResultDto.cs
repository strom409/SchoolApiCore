namespace Examination_Management.Services.Result.Gazet
{
    public class GazetResultDto
    {
        public int StudentID { get; set; }
        public string AdmissionNo { get; set; }
        public int RollNo { get; set; }
        public string Current_Session { get; set; }
        public int SessionID { get; set; }
        public string StudentName { get; set; }
        public string FathersName { get; set; }
        public string MothersName { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string BloodGroup { get; set; }
        public string StudentAadhaar { get; set; }
        public string FatherAadhaar { get; set; }
        public string MotherAadhaar { get; set; }
        public string PresentAddress { get; set; }
        public string PerminantAddress { get; set; }
        public string PhoneNo { get; set; }
        public string PhoneNo2 { get; set; }
        public string SEmail { get; set; }
        public string Category { get; set; }
        public string Remarks { get; set; }
        public string RegistrationNo { get; set; }
        public string StudentCode { get; set; }
        public string FeeBookNo { get; set; }
        public string BusFeeBookNo { get; set; }
        public string PhotoPath { get; set; }
        public string BusRoute { get; set; }
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal Marks { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; }
        public decimal GradePoint { get; set; }
        public string TeacherRemarks { get; set; }
        public string PrincipalRemarks { get; set; }
        public int Rank { get; set; }
    }

}
