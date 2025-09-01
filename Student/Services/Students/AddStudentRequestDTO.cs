using Azure.Core;
using Microsoft.Data.SqlClient;

namespace Student.Services.Students
{
    public class AddStudentRequestDTO
    {
        public string? AdmissionNo { get; set; }
        public string? StudentName { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? DOA { get; set; }
        public string? Gender { get; set; }
        public string? FatherName { get; set; }
        public string? PermanentPincode { get; set; }
        public string? PermanentDistrict { get; set; }
        public string? FatherQualification { get; set;}
        public string? FatherOccupation { get; set; }
        public string? MontherName { get; set; }
        public string? MotherQualification { get; set; }
        public string? MotherOccupation { get; set; }
        public string? PresentAddress { get; set; }
        public string? UID { get; set; }
        public string? PermanentAddress { get; set; }
        public string? Session { get; set; }
        public string? MobileFather { get; set; }
        public string? MobileMother { get; set; }
        public string? LandLineNo { get; set; }
        public string? SEmail { get; set; }
        public string? Aadhaar { get; set; }
        public string? DistrictID { get; set; }
        public string? StudentCatID { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianPhoneNo { get; set; }
        public string? GuardialAccupation { get; set; }
        public string? GuardianQualification { get; set; }
        public string? PinNo { get; set; }
        public string? StudentCatName { get; set; }
        public string? DistrictName { get; set; }

        // StudentInfo related
        public int? deptmentid { get; set; }
        public string? classid { get; set; }
        public string? sectionid { get; set; }
        public string? rollno { get; set; }
        public string? remarks { get; set; }
        public long? BusStopID { get; set; }
        public long? RouteID { get; set; }
        public string? PhotoPath { get; set; }
        public string? AcademicNo { get; set; }
        public string? BloodGroup { get; set; }

        // Property for AddGPS
        public string? GPSLocation { get; set; }

        // ------ Additional Properties from StudentCreateRequest ------

        public string? StudentID { get; set; }
        public string? StudentInfoID { get; set; }
        public string? StudentCode { get; set; }
        public string? SessionOfAdmission { get; set; }
        public string? PrePrimaryBoardNo { get; set; }
        public string? PrimaryBoardNo { get; set; }
        public string? MiddleBoardNo { get; set; }
        public string? HighBoardNo { get; set; }
        public string? HigherBoardNo { get; set; }
        // public string? IsDischarged { get; set; }
        public bool? IsDischarged { get; set; }
        public bool? Discharged { get; set; }

        public string? DSession { get; set; }
        public string? DDate { get; set; }
        public string? DRemarks { get; set; }
        public string? DBy { get; set; }
        public string? UserName { get; set; }
        public string? UpdatedOn { get; set; }
       // public string? Discharged { get; set; }
        public int? ActionType { get; set; } = 1;
        public int? UpdateType { get; set; } = 0;
        public string? RouteName { get; set; }
        public string? BusStopName { get; set; }
        public string? BusFee { get; set; }
        public string? HID { get; set; }
        public string? HouseName { get; set; }
        public string? Pen { get; set; }
        public string? Weight { get; set; }
        public string? Height { get; set; }
        public string? NameAsPerAadhaar { get; set; }
        public string? DOBAsPerAadhaar { get; set; }
        public string? PrePrimaryDate { get; set; }
        public string? PrimaryDate { get; set; }
        public string? MiddleDate { get; set; }
        public string? HighDate { get; set; }
        public string? HigherDate { get; set; }
        public string? FAdhaar { get; set; }
        public string? MAdhaar { get; set; }
        public string? FatherIncome { get; set; }
        //public string? MotherPhoto { get; set; }
        //public string? FatherPhoto { get; set; }
        public string? MotherIncome { get; set; }
        public string? PrDistrictID { get; set; }
        public string? Apaarid { get; set; }
        public string? StateName { get; set; }

        public string? StateID { get; set; }

        public string? PrStateID { get; set; }

        public string? PrStateName { get; set; }

        public IFormFile? StudentPhoto { get; set; }
        public IFormFile? FatherPhoto { get; set; }
        public IFormFile? MotherPhoto { get; set; }

    }

}
