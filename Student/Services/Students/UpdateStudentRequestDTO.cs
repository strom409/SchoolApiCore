using Student.Services.Students;
using System;

namespace Student.Services.Students
{
    public class UpdateStudentRequestDTO
    {
        public string? StudentID { get; set; }
        public string? StudentInfoID { get; set; }
        public string? AdmissionNo { get; set; }
        public string? StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string? DOB { get; set; }
        public string? FatherName { get; set; }
        public string? DOA { get; set; }
        public string? Session { get; set; }
        public string? SessionOfAdmission { get; set; }
        public string? PhotoPath { get; set; }
        public string? StudentCatID { get; set; }
        public string? StudentCatName { get; set; }
        public string? ClassID { get; set; }
        public string? Aadhaar { get; set; }
        public string? Gender { get; set; }
        public string? SectionID { get; set; }
        public string? ClassName { get; set; }
        public string? SectionName { get; set; }
        public string? RollNo { get; set; }

        public string? MontherName { get; set; }
        public string? MobileFather { get; set; }
        public string? MobileMother { get; set; }
        public string? LandLineNo { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? PinCode { get; set; }
        public string? DistrictName { get; set; }
        public string? DistrictID { get; set; }
        public string? FatherQualification { get; set; }
        public string? MotherQualification { get; set; }
        public string? FatherOccupation { get; set; }
        public string? MotherOccupation { get; set; }
        public string? FatherIcome { get; set; }
        public string? MotherIcome { get; set; }
        //public string? FatherPhoto { get; set; }
       // public string? MotherPhoto { get; set; }
        public string? Remarks { get; set; }
        public string? SEmail { get; set; }

        // Info Part

        public string? IsDischarged { get; set; }
        public string? Discharged { get; set; }
        public string? DSession { get; set; }
        public string? DDate { get; set; }
        public string? DRemarks { get; set; }
        public string? DBy { get; set; }

        public string? RouteID { get; set; }
        public string? busstopid { get; set; }
        public string? RouteName { get; set; }
        public string? BusStopName { get; set; }
        public string? BusFee { get; set; }


        //public string UserName { get; set; }
        public string? UpdatedBy { get; set; }
        public string? PrimaryBoardNo { get; set; }
        public string? HighBoardNo { get; set; }
        public string? MiddleBoardNo { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhoneNo { get; set; }
        public string? GuardianQualification { get; set; }
        public string? GuardialAccupation { get; set; }
        public int? ActionType { get; set; }
        public int? UpdateType { get; set; }
        public string? AcademicNo { get; set; }
        public string? HID { get; set; }
        public string? HouseName { get; set; }


        //UDISEDATA
        public string? PEN { get; set; }
        public string? WEIGHT { get; set; }
        public string? Height { get; set; }
        public string? NAMEASPERADHAAR { get; set; }
        public string? DOBAsPerAadhaar { get; set; }
        //public string ExamRollNo { get; set; }


        //Update BoardNo and DateSS
        public string? PrePrimaryBoardNo { get; set; }
        public string? PrimaryDate { get; set; }
        public string? PrePrimaryDate { get; set; }
        public string? MiddleDate { get; set; }
        public string? HighDate { get; set; }
        public string? HigherBoardNo { get; set; }
        public string? HigherDate { get; set; }
        public string? MAdhaar { get; set; }
        public string? FAdhaar { get; set; }
        public string? BloodGroup { get; set; }
        public string? PrPincode { get; set; }
        public string? PrDistrictID { get; set; }
        public string? PrDistrict { get; set; }
        public string? StateName { get; set; }

        public string? StateID { get; set; }         

        public string? PrStateID { get; set; }        

        public string? PrStateName { get; set; }
        public IFormFile? StudentPhoto { get; set; }
        public IFormFile? FatherPhoto { get; set; }
        public IFormFile? MotherPhoto { get; set; }
    
    }

    public class StudentRollNoUpdate
    {
        public int? StudentInfoID { get; set; }
        public string RollNo { get; set; }
        public string UpdatedBy { get; set; }
    }
    public class BulkRollNoUpdateRequest
    {
        public List<StudentRollNoUpdate> BulkUpdates { get; set; } = new List<StudentRollNoUpdate>();
        public bool? IsWholeClassUpdate { get; set; } = false;

    }
    public class StudentSessionData
    {
        public string? FromSession { get; set; }
        public string? ToSession { get; set; }

        public long StudentID { get; set; }
        public long StudentInfoID { get; set; }
        public long ClassID { get; set; }
        public long SectionID { get; set; }
        public long RollNo { get; set; }

        public long? RouteID { get; set; }
        public long? BuStopID { get; set; }

        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

      
    }
    public class StudentSessionUpdateRequest
    {
        public List<StudentSessionData>? Students { get; set; }
    }

}