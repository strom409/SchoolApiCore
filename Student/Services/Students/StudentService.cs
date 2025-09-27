
using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Student.Repository;
using Student.Repository.SQL;
using Student.Services.ClassMaster;
using Student.Services.Students;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Student.Services.Students
{
    public class StudentService : IStudentService
    {

        private readonly IConfiguration _configuration;

        public StudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        /// 
        public async Task<ResponseModel> AddStudent(AddStudentRequestDTO request, string clientId)
        {
            #region Initialize

            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Added!" };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                #endregion

                #region Validation

                if (string.IsNullOrWhiteSpace(request.StudentName) ||
                    string.IsNullOrWhiteSpace(request.AdmissionNo) ||
                    request.DOB == default || request.DOA == default)
                {
                    response.Message = "Required fields are missing (StudentName, AdmissionNo, DOB, BOA).";
                    return response;
                }

                #endregion

                #region Duplicate Check 1 - Full Entry

                var check1Params = new[]
                {
            new SqlParameter("@StudentName", request.StudentName ?? string.Empty),
            new SqlParameter("@FatherName", request.FatherName ?? string.Empty),
            new SqlParameter("@MontherName", request.MontherName ?? string.Empty),
            new SqlParameter("@PresentAddress", request.PresentAddress ?? string.Empty),
            new SqlParameter("@MobileFather", request.MobileFather ?? string.Empty),
        };

                string sqlDuplicate1 = @"
            SELECT COUNT(*) 
            FROM Students 
            WHERE StudentName = @StudentName 
              AND FathersName = @FatherName 
              AND MothersName = @MontherName 
              AND PresentAddress = @PresentAddress 
              AND PhoneNo = @MobileFather";

                DataSet ds1 = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlDuplicate1, check1Params);
                int exists1 = 0;
                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    exists1 = Convert.ToInt32(ds1.Tables[0].Rows[0][0]);
                }

                if (exists1 > 0)
                {
                    response.Status = 0;
                    response.Message = "Duplicate Entry Not Allowed!";
                    return response;
                }

                #endregion

                #region Duplicate Check 2 - AdmissionNo

                var check2Params = new[]
                {
            new SqlParameter("@AdmissionNo", request.AdmissionNo ?? string.Empty),
        };

                string sqlDuplicate2 = "SELECT COUNT(*) FROM Students WHERE AdmissionNo = @AdmissionNo";

                DataSet ds2 = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlDuplicate2, check2Params);
                int exists2 = 0;
                if (ds2 != null && ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                {
                    exists2 = Convert.ToInt32(ds2.Tables[0].Rows[0][0]);
                }

                if (exists2 > 0)
                {
                    response.Status = 0;
                    response.Message = "AdmissionNo/UID already allotted to another student!";
                    return response;
                }

                #endregion

                #region Prepare Parameters for Insert

                string proc = "AddstudentAPINew";

                var insertParams = new List<SqlParameter>
{
    new SqlParameter("@StudentName", request.StudentName ?? string.Empty),
    new SqlParameter("@DOB", request.DOB ?? (object)DBNull.Value),
    new SqlParameter("@DOA", request.DOA ?? (object)DBNull.Value),
    new SqlParameter("@AdmissionNo", request.AdmissionNo ?? string.Empty),
    new SqlParameter("@Gender", request.Gender ?? string.Empty),
    new SqlParameter("@DistrictID", request.DistrictID ?? string.Empty),
    new SqlParameter("@DistrictName", request.DistrictName ?? string.Empty),
    new SqlParameter("@Aadhaar", request.Aadhaar ?? string.Empty),
    new SqlParameter("@PrPincode", request.PermanentPincode ?? string.Empty),
    new SqlParameter("@PrDistrict", request.PermanentDistrict ?? string.Empty),
    new SqlParameter("@StudentCatID", request.StudentCatID ?? string.Empty),
    new SqlParameter("@StudentCatName", request.StudentCatName ?? string.Empty),
    new SqlParameter("@PinNo", request.PinNo ?? string.Empty),
    new SqlParameter("@PhotoPath", request.PhotoPath ?? string.Empty),
    new SqlParameter("@ClassID", request.classid ?? string.Empty),
    new SqlParameter("@SectionID", request.sectionid ?? string.Empty),
    new SqlParameter("@Session", request.Session ?? string.Empty),
    new SqlParameter("@RollNo", request.rollno ?? string.Empty),
    new SqlParameter("@PresentAddress", request.PresentAddress ?? string.Empty),
    new SqlParameter("@PermanentAddress", request.PermanentAddress ?? string.Empty),
    new SqlParameter("@FatherName", request.FatherName ?? string.Empty),
    new SqlParameter("@MotherName", request.MontherName ?? string.Empty), // note DTO still has typo, fix DTO or map here
    new SqlParameter("@MobileFather", request.MobileFather ?? string.Empty),
    new SqlParameter("@MobileMother", request.MobileMother ?? string.Empty),
    new SqlParameter("@LandLineNo", request.LandLineNo ?? string.Empty),
    new SqlParameter("@FatherQualification", request.FatherQualification ?? string.Empty),
    new SqlParameter("@MotherQualification", request.MotherQualification ?? string.Empty),
    new SqlParameter("@FatherIncome", request.FatherIncome ?? string.Empty),
    new SqlParameter("@MotherIncome", request.MotherIncome ?? string.Empty),
    new SqlParameter("@FatherOccupation", request.FatherOccupation ?? string.Empty),
    new SqlParameter("@MotherOccupation", request.MotherOccupation ?? string.Empty),
     new SqlParameter("@FatherPhoto", string.Empty),     // no DB save
    new SqlParameter("@MotherPhoto", string.Empty),
    new SqlParameter("@Remarks", request.remarks ?? string.Empty),
    new SqlParameter("@SEmail", request.SEmail ?? string.Empty),
    new SqlParameter("@AcademicNo", request.AcademicNo ?? string.Empty),
    new SqlParameter("@GuardianName", request.GuardianName ?? string.Empty),
    new SqlParameter("@GuardianPhoneNo", request.GuardianPhoneNo ?? string.Empty),
    new SqlParameter("@GuardianQualification", request.GuardianQualification ?? string.Empty),
    new SqlParameter("@BusStopID", request.BusStopID ?? (object)DBNull.Value),
    new SqlParameter("@RouteID", request.RouteID ?? (object)DBNull.Value),
    new SqlParameter("@GuardialOccupation", request.GuardialAccupation ?? string.Empty), // DTO typo; consider fix
    new SqlParameter("@StudentCode", request.StudentCode ?? (object)DBNull.Value),
    new SqlParameter("@StudentInfoID", request.StudentInfoID ?? (object)DBNull.Value),
    new SqlParameter("@SessionOfAdmission", request.SessionOfAdmission ?? string.Empty),
    new SqlParameter("@IsDischarged", request.IsDischarged ?? (object)DBNull.Value),
    new SqlParameter("@DSession", request.DSession ?? string.Empty),
    new SqlParameter("@DDate", DateTime.TryParse(request.DDate, out var dDateVal) ? dDateVal : (object)DBNull.Value),
    new SqlParameter("@DRemarks", request.DRemarks ?? string.Empty),
    new SqlParameter("@DBy", request.DBy ?? string.Empty),
    new SqlParameter("@UserName", request.UserName ?? string.Empty),
    new SqlParameter("@UpdatedOn", DateTime.TryParse(request.UpdatedOn, out var updatedOnVal) ? updatedOnVal : (object)DBNull.Value),
    new SqlParameter("@Discharged", request.Discharged ?? (object)DBNull.Value),
    new SqlParameter("@UpdateType", request.UpdateType ?? 0),
    new SqlParameter("@RouteName", request.RouteName ?? string.Empty),
    new SqlParameter("@BusStopName", request.BusStopName ?? string.Empty),
    new SqlParameter("@BusFee", decimal.TryParse(request.BusFee, out var busFeeVal) ? busFeeVal : (object)DBNull.Value),
    new SqlParameter("@HID", request.HID ?? string.Empty),
    new SqlParameter("@HouseName", request.HouseName ?? string.Empty),
    new SqlParameter("@Pen", request.Pen ?? string.Empty),
    new SqlParameter("@Weight", request.Weight ?? string.Empty),
    new SqlParameter("@Height", request.Height ?? string.Empty),
    new SqlParameter("@NameAsPerAadhaar", request.NameAsPerAadhaar ?? string.Empty),
    new SqlParameter("@DOBAsPerAadhaar", DateTime.TryParse(request.DOBAsPerAadhaar, out var dobAadhaarVal) ? dobAadhaarVal : (object)DBNull.Value),
    new SqlParameter("@BloodGroup", request.BloodGroup ?? string.Empty),
    new SqlParameter("@PrePrimaryDate", DateTime.TryParse(request.PrePrimaryDate, out var prePrimaryVal) ? prePrimaryVal : (object)DBNull.Value),
    new SqlParameter("@PrimaryDate", DateTime.TryParse(request.PrimaryDate, out var primaryVal) ? primaryVal : (object)DBNull.Value),
    new SqlParameter("@MiddleDate", DateTime.TryParse(request.MiddleDate, out var middleVal) ? middleVal : (object)DBNull.Value),
    new SqlParameter("@HighDate", DateTime.TryParse(request.HighDate, out var highVal) ? highVal : (object)DBNull.Value),
    new SqlParameter("@HigherDate", DateTime.TryParse(request.HigherDate, out var higherVal) ? higherVal : (object)DBNull.Value),
    new SqlParameter("@FAdhaar", request.FAdhaar ?? string.Empty),
    new SqlParameter("@MAdhaar", request.MAdhaar ?? string.Empty),
    new SqlParameter("@PrDistrictID", request.PrDistrictID ?? string.Empty),
    new SqlParameter("@Apaarid", request.Apaarid ?? string.Empty),
    new SqlParameter("@StateID", request.StateID ?? string.Empty),
    new SqlParameter("@StateName", request.StateName ?? string.Empty),
    new SqlParameter("@PrStateID", request.PrStateID ?? string.Empty),
    new SqlParameter("@PrStateName", request.PrStateName ?? string.Empty),
    new SqlParameter("@Religion", request.Religion ?? string.Empty),
    new SqlParameter("@MotherTounge", request.MotherTounge ?? string.Empty),
    new SqlParameter("@BankName", request.BankName ?? string.Empty),
    new SqlParameter("@AccountNo", request.AccountNo ?? string.Empty),
    new SqlParameter("@AccountType", request.AccountType ?? string.Empty),
    new SqlParameter("@IFCCode", request.IFCCode ?? string.Empty),
  //  new SqlParameter("@Scategory", request.Scategory ?? string.Empty),
  //  new SqlParameter("@ScategoryID", request.ScategoryID ?? (object)DBNull.Value),
    new SqlParameter("@BPLStatus", request.BPLStatus ?? (object)DBNull.Value),
    new SqlParameter("@SDisability", request.SDisability ?? string.Empty),
    new SqlParameter("@Tehsil", request.Tehsil ?? string.Empty),
    new SqlParameter("@TehsilPer", request.TehsilPer ?? string.Empty),
    new SqlParameter("@BPLCategory", request.BPLCategory ?? string.Empty),
    new SqlParameter("@CWSNStatus", request.CWSNStatus ?? (object)DBNull.Value),
    new SqlParameter("@BPLCategoryID", request.BPLCategoryID ?? (object)DBNull.Value),
    new SqlParameter("@Category", request.Category ?? string.Empty),
    new SqlParameter("@CategoryID", request.CategoryID ?? (object)DBNull.Value)
};



                #endregion

                #region Insert Student

                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    proc,
                    insertParams.ToArray());

                // 6️⃣ Get StudentID by AdmissionNo
                string studentIdQuery = "SELECT MAX(StudentID) FROM Students WHERE AdmissionNo = @AdmissionNo";
                var studentIdParam = new SqlParameter("@AdmissionNo", request.AdmissionNo ?? string.Empty);
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, studentIdQuery, new[] { studentIdParam });

                string studentId = ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0][0].ToString() : null;


                if (!string.IsNullOrEmpty(studentId))
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Added Successfully";
                    response.ResponseData = new { StudentID = studentId }; // ✅ Return to gateway
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to retrieve StudentID.";
                }

                return response;


                #endregion
            }
            catch (Exception ex)
            {
                #region Exception

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while adding student.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "AddStudent",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");

                return response;

                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddNewStudentWithRegNo(AddStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Added!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Duplicate Check 1 - Name/Father/Mother/Address/Phone
                SqlParameter[] checkParams1 =
                {
            new SqlParameter("@StudentName", request.StudentName),
            new SqlParameter("@FatherName", request.FatherName),
            new SqlParameter("@MontherName", request.MontherName),
            new SqlParameter("@PresentAddress", request.PresentAddress),
            new SqlParameter("@MobileFather", request.MobileFather),
        };

                string checkDuplicateSql1 = @"SELECT COUNT(*) AS Cnt FROM Students 
            WHERE StudentName = @StudentName AND FathersName = @FatherName 
            AND MothersName = @MontherName AND PresentAddress = @PresentAddress 
            AND PhoneNo = @MobileFather";

                var ds1 = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateSql1, checkParams1);
                if (ds1 != null && ds1.Tables.Count > 0 && Convert.ToInt32(ds1.Tables[0].Rows[0]["Cnt"]) > 0)
                {
                    response.Message = "Duplicate Entry Not Allowed!";
                    return response;
                }
                #endregion

                #region Duplicate Check 2 - AdmissionNo
                SqlParameter[] checkParams2 =
                {
            new SqlParameter("@AdmissionNo", request.AdmissionNo)
        };

                string checkDuplicateSql2 = "SELECT COUNT(*) AS Cnt FROM Students WHERE AdmissionNo = @AdmissionNo";
                var ds2 = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateSql2, checkParams2);
                if (ds2 != null && ds2.Tables.Count > 0 && Convert.ToInt32(ds2.Tables[0].Rows[0]["Cnt"]) > 0)
                {
                    response.Message = "AdmissionNo/UID already aloted to other student!";
                    return response;
                }
                #endregion

                #region Insert Student via Stored Procedure
                SqlParameter[] insertParams =
                {
            new SqlParameter("@StudentCode", request.StudentCode ?? string.Empty),
    new SqlParameter("@StudentInfoID", request.StudentInfoID ?? string.Empty),
    new SqlParameter("@StudentID", request.StudentID ?? string.Empty),
    new SqlParameter("@PinNo", request.PinNo ?? string.Empty),
    new SqlParameter("@FatherIcome", request.FatherIncome ?? string.Empty),
    new SqlParameter("@MotherIcome", request.MotherIncome ?? string.Empty),
    new SqlParameter("@FatherPhoto", request.FatherPhoto),
    new SqlParameter("@MotherPhoto", request.MotherPhoto),
    new SqlParameter("@SessionOfAdmission", request.SessionOfAdmission ?? string.Empty),
    new SqlParameter("@PrePrimaryBoardNo", request.PrePrimaryBoardNo ?? string.Empty),
    new SqlParameter("@PrimaryBoardNo", request.PrimaryBoardNo ?? string.Empty),
    new SqlParameter("@MiddleBoardNo", request.MiddleBoardNo ?? string.Empty),
    new SqlParameter("@HighBoardNo", request.HighBoardNo ?? string.Empty),
    new SqlParameter("@HigherBoardNo", request.HigherBoardNo ?? string.Empty),
   // new SqlParameter("@IsDischarged", request.IsDischarged ?? string.Empty),
    new SqlParameter("@DSession", request.DSession ?? string.Empty),
    new SqlParameter("@DDate", request.DDate ?? string.Empty),
    new SqlParameter("@DRemarks", request.DRemarks ?? string.Empty),
    new SqlParameter("@DBy", request.DBy ?? string.Empty),
    new SqlParameter("@UserName", request.UserName ?? string.Empty),
    new SqlParameter("@UpdatedOn", request.UpdatedOn ?? string.Empty),
    //new SqlParameter("@Discharged", request.Discharged ?? string.Empty),
    new SqlParameter("@ActionType", request.ActionType ?? 1),
    new SqlParameter("@UpdateType", request.UpdateType ?? 0),
    new SqlParameter("@RouteName", request.RouteName ?? string.Empty),
    new SqlParameter("@BusStopName", request.BusStopName ?? string.Empty),
    new SqlParameter("@BusFee", request.BusFee ?? string.Empty),
    new SqlParameter("@HID", request.HID ?? string.Empty),
    new SqlParameter("@HouseName", request.HouseName ?? string.Empty),
    new SqlParameter("@Pen", request.Pen ?? string.Empty),
    new SqlParameter("@Weight", request.Weight ?? string.Empty),
    new SqlParameter("@Height", request.Height ?? string.Empty),
    new SqlParameter("@NameAsPerAadhaar", request.NameAsPerAadhaar ?? string.Empty),
    new SqlParameter("@DOBAsPerAadhaar", request.DOBAsPerAadhaar ?? string.Empty),
    new SqlParameter("@BloodGroup", request.BloodGroup ?? string.Empty),
    new SqlParameter("@PrePrimaryDate", request.PrePrimaryDate ?? string.Empty),
    new SqlParameter("@PrimaryDate", request.PrimaryDate ?? string.Empty),
    new SqlParameter("@MiddleDate", request.MiddleDate ?? string.Empty),
    new SqlParameter("@HighDate", request.HighDate ?? string.Empty),
    new SqlParameter("@HigherDate", request.HigherDate ?? string.Empty),
    new SqlParameter("@FAdhaar", request.FAdhaar ?? string.Empty),
    new SqlParameter("@MAdhaar", request.MAdhaar ?? string.Empty)
        };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "AddstudentWithRegNoAPI", insertParams);
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Added Successfully";
                }
                else
                {
                    response.Message = "Student Code Already Exists!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddNewGPS(AddStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Added!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@GPSLocation", request.GPSLocation ?? "")
        };
                #endregion

                #region SQL Insert
                string insertQuery = "INSERT INTO GPSLive (GPSLocation) VALUES (@GPSLocation)";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, sqlParams);
                #endregion

                #region Result Handling
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Added Successfully";
                }
                else
                {
                    response.Message = "Code Already Exists!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentsByClass(long classId, string clientId)
        {
            #region Initialize Response and Connection
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };

            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
                return response;
            }
            #endregion

            #region Define SQL Query and Parameters
            List<StudentDTO> students = new List<StudentDTO>();

            string query = @"
SELECT 
    s.StudentID,
    s.AdmissionNo,
    dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) AS AcademicNo,
    s.StudentName,
    s.DOB,
    s.BOA,
    s.FathersName,
    s.FathersQualification,
    s.FathersJob,
    s.MothersName,
    s.MothersQualification,
    s.MothersJob,
    s.PresentAddress,
    s.PerminantAddress,
    s.SessionOfAdmission,
    s.PhoneNo,
    s.Gender,
    s.APAARSTUDENTID,
    s.HouseName,
    s.PrPincode, 
    s.PrDistrict, 
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,s.PrDistrictID,
    s.Pincode,
s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.UID, s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,
    si.StudentInfoID,
    si.RollNo,
    si.PhotoPath,
    si.ClassID,
    c.ClassName,              
    si.Current_Session,
    si.RouteID,
    si.BusStopID,
    si.SessionID,
    si.Remarks,
    si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
    si.PrimaryBoardNo, si.PrimaryDate,
    si.MiddleBoardNo, si.MiddleDate,
    si.HighBoardNo, si.HighDate,
    si.HigherBoardNo, si.HigherDate,
    si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
    bs.BusStop,
    bs.BusRate,
    se.SectionName,
    t.RouteName
FROM Students s
INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
INNER JOIN Classes c ON si.ClassID = c.ClassID
LEFT JOIN Sections se ON si.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = si.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
WHERE si.ClassID = @ClassID";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@ClassID", classId)
    };
            #endregion

            #region Execute Query and Map Data
            try
            {

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var student = new StudentDTO
                        {
                            StudentID = row["StudentID"]?.ToString(),
                            StudentInfoID = row["StudentInfoID"]?.ToString(),
                            //  StudentCode = row["StudentCode"]?.ToString(),

                            AdmissionNo = row["AdmissionNo"]?.ToString(),
                            StudentName = row["StudentName"]?.ToString(),
                            DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
                            DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
                            FatherName = row["FathersName"]?.ToString(),
                            FatherQualification = row["FathersQualification"]?.ToString(),
                            FatherOccupation = row["FathersJob"]?.ToString(),
                            MotherQualification = row["MothersQualification"]?.ToString(),
                            MotherOccupation = row["MothersJob"]?.ToString(),
                            MontherName = row["MothersName"]?.ToString(),
                            MobileFather = row["Fphn"]?.ToString(),
                            MobileMother = row["Mphn"]?.ToString(),
                            LandLineNo = row["landlineno"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PermanentAddress = row["PerminantAddress"]?.ToString(),
                            SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            Discharged = row["Discharged"]?.ToString(),
                            DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
                            DSession = row["DSession"]?.ToString(),
                            DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
                            ClassName = row["ClassName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString(),
                            DBy = row["DBy"]?.ToString(),
                            IsDischarged = row["IsDischarged"]?.ToString(),
                            SEmail = row["SEmail"]?.ToString(),
                            BloodGroup = row["BloodGroup"]?.ToString(),
                            PinCode = row["Pincode"]?.ToString(),
                            Aadhaar = row["Saadhaarcard"]?.ToString(),
                            FAdhaar = row["Faadhaarcard"]?.ToString(),
                            MAdhaar = row["Maadhaarcard"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SessionID"]?.ToString(),
                            RollNo = row["RollNo"]?.ToString(),
                            PhotoPath = row["PhotoPath"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            RouteID = row["RouteID"]?.ToString(),
                            AcademicNo = row["AcademicNo"]?.ToString(),
                            busstopid = row["BusStopID"]?.ToString(),
                            RouteName = row["RouteName"]?.ToString(),
                            BusStopName = row["BusStop"]?.ToString(),
                            BusFee = row["BusRate"]?.ToString(),
                            GuardianName = row["GuardianName"]?.ToString(),
                            GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                            GuardianQualification = row["GuardianQualification"]?.ToString(),
                            GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                            DistrictID = row["DistrictID"]?.ToString(),
                            DistrictName = row["DistrictName"]?.ToString(),
                            StudentCatID = row["StudentCatID"]?.ToString(),
                            StudentCatName = row["StudentCatName"]?.ToString(),
                            PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                            HighBoardNo = row["HighBoardNo"]?.ToString(),
                            MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                            PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                            HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                            PrimaryDate = row["PrimaryDate"]?.ToString(),
                            HighDate = row["HighDate"]?.ToString(),
                            MiddleDate = row["MiddleDate"]?.ToString(),
                            PrePrimaryDate = row["PrePrimaryDate"]?.ToString(),
                            HigherDate = row["HigherDate"]?.ToString(),
                            Session = row["Current_Session"]?.ToString(),
                            HID = row["HID"]?.ToString(),
                            PEN = row["PEN"]?.ToString(),
                            WEIGHT = row["WEIGHT"]?.ToString(),
                            Height = row["Height"]?.ToString(),
                            NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                            DOBASPERADHAAR = row["DOBASPERADHAAR"]?.ToString(),
                            Apaarid = row["APAARSTUDENTID"]?.ToString(),
                            HouseName = row["HouseName"]?.ToString(),
                            PrPincode = row["PrPincode"]?.ToString(),
                            PrDistrict = row["PrDistrict"]?.ToString(),
                            PrDistrictID = row["PrDistrictID"]?.ToString(),
                        };
                        students.Add(student);
                    }

                    response.ResponseData = students;
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Success";
                }
                else
                {
                    response.ResponseData = new List<StudentDTO>();

                }
                #endregion

                #region Error Handling
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentsByClass", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while retrieving data.";
                response.Error = ex.Message;
            }
            #endregion

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="admissionNo"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentByAdmissionNo(string admissionNo, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region SQL Query
                string query = @"
SELECT 
    s.StudentID,
    s.AdmissionNo,
    dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) AS AcademicNo,
    s.StudentName,
    s.DOB,
    s.BOA,
    s.FathersName,
    s.FathersQualification,
    s.FathersJob,
    s.MothersName,
    s.MothersQualification,
    s.MothersJob,
    s.PresentAddress,
    s.PerminantAddress,
    s.SessionOfAdmission,
    s.PhoneNo,
    s.HouseName,
    s.APAARSTUDENTID,
    s.PrPincode,
    s.PrDistrict,
    s.PrDistrictID,
    s.Gender,
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.UID, s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR, s.Religion,
    s.MotherTounge,
    s.BankName,
    s.AccountNo,
    s.AccountType,
    s.IFCCode,
    s.BPLStatus,
    s.SDisability,
    s.Tehsil,
    s.TehsilPer,
    si.StudentInfoID,
    si.RollNo,
    si.PhotoPath,
    si.ClassID,
    si.Current_Session,
    si.RouteID,
    si.BusStopID,
    si.SessionID,
    si.Remarks,
    si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
    si.PrimaryBoardNo, si.PrimaryDate,
    si.MiddleBoardNo, si.MiddleDate,
    si.HighBoardNo, si.HighDate,
    si.HigherBoardNo, si.HigherDate,
    si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
    c.ClassName, se.SectionName,
    bs.BusStop,
    bs.BusRate,
    t.RouteName
FROM Students s
INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
INNER JOIN Classes c ON si.ClassID = c.ClassID
LEFT JOIN Sections se ON si.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = si.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
WHERE s.AdmissionNo = @AdmissionNo";
                #endregion

                #region Execute Query
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@AdmissionNo", admissionNo)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());
                #endregion

                #region Map Data to DTO
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    var student = new StudentDTO
                    {
                        StudentID = row["StudentID"]?.ToString(),
                        StudentInfoID = row["StudentInfoID"]?.ToString(),
                        AdmissionNo = row["AdmissionNo"]?.ToString(),
                        StudentName = row["StudentName"]?.ToString(),
                        Apaarid = row["APAARSTUDENTID"]?.ToString(),
                        HouseName = row["HouseName"]?.ToString(),
                        PrPincode = row["PrPincode"]?.ToString(),
                        PrDistrict = row["PrDistrict"]?.ToString(),
                        DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
                        DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
                        FatherName = row["FathersName"]?.ToString(),
                        FatherQualification = row["FathersQualification"]?.ToString(),
                        FatherOccupation = row["FathersJob"]?.ToString(),
                        MotherQualification = row["MothersQualification"]?.ToString(),
                        MotherOccupation = row["MothersJob"]?.ToString(),
                        MontherName = row["MothersName"]?.ToString(),
                        MobileFather = row["Fphn"]?.ToString(),
                        MobileMother = row["Mphn"]?.ToString(),
                        LandLineNo = row["landlineno"]?.ToString(),
                        ClassName = row["ClassName"]?.ToString(),
                        SectionName = row["SectionName"]?.ToString(),
                        PresentAddress = row["PresentAddress"]?.ToString(),
                        PermanentAddress = row["PerminantAddress"]?.ToString(),
                        SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        Discharged = row["Discharged"]?.ToString(),
                        DSession = row["DSession"]?.ToString(),
                        DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
                        DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
                        DBy = row["DBy"]?.ToString(),
                        IsDischarged = row["IsDischarged"]?.ToString(),
                        SEmail = row["SEmail"]?.ToString(),
                        BloodGroup = row["BloodGroup"]?.ToString(),
                        PinCode = row["Pincode"]?.ToString(),
                        Aadhaar = row["Saadhaarcard"]?.ToString(),
                        FAdhaar = row["Faadhaarcard"]?.ToString(),
                        MAdhaar = row["Maadhaarcard"]?.ToString(),
                        ClassID = row["ClassID"]?.ToString(),
                        SectionID = row["SessionID"]?.ToString(),
                        RollNo = row["RollNo"]?.ToString(),
                        PhotoPath = row["PhotoPath"]?.ToString(),
                        Remarks = row["Remarks"]?.ToString(),
                        RouteID = row["RouteID"]?.ToString(),
                        AcademicNo = row["AcademicNo"]?.ToString(),
                        busstopid = row["BusStopID"]?.ToString(),
                        RouteName = row["RouteName"]?.ToString(),
                        BusStopName = row["BusStop"]?.ToString(),
                        BusFee = row["BusRate"]?.ToString(),
                        GuardianName = row["GuardianName"]?.ToString(),
                        GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                        GuardianQualification = row["GuardianQualification"]?.ToString(),
                        GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                        DistrictID = row["DistrictID"]?.ToString(),
                        DistrictName = row["DistrictName"]?.ToString(),
                        StudentCatID = row["StudentCatID"]?.ToString(),
                        StudentCatName = row["StudentCatName"]?.ToString(),
                        PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                        HighBoardNo = row["HighBoardNo"]?.ToString(),
                        MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                        PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                        HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                        PrimaryDate = row["PrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrimaryDate"]).ToString("yyyy-MM-dd") : null,
                        HighDate = row["HighDate"] != DBNull.Value ? Convert.ToDateTime(row["HighDate"]).ToString("yyyy-MM-dd") : null,
                        MiddleDate = row["MiddleDate"] != DBNull.Value ? Convert.ToDateTime(row["MiddleDate"]).ToString("yyyy-MM-dd") : null,
                        PrePrimaryDate = row["PrePrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrePrimaryDate"]).ToString("yyyy-MM-dd") : null,
                        HigherDate = row["HigherDate"] != DBNull.Value ? Convert.ToDateTime(row["HigherDate"]).ToString("yyyy-MM-dd") : null,
                        Session = row["Current_Session"]?.ToString(),
                        HID = row["HID"]?.ToString(),
                        PEN = row["PEN"]?.ToString(),
                        WEIGHT = row["WEIGHT"]?.ToString(),
                        Height = row["Height"]?.ToString(),
                        NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                        DOBASPERADHAAR = row["DOBASPERADHAAR"]?.ToString(),
                        Religion = row["Religion"]?.ToString(),
                        MotherTounge = row["MotherTounge"]?.ToString(), // Note: DB column "MotherTounge"
                        BankName = row["BankName"]?.ToString(),
                        AccountNo = row["AccountNo"]?.ToString(),
                        AccountType = row["AccountType"]?.ToString(),
                        IFCCode = row["IFCCode"]?.ToString(),
                        BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                        SDisability = row["SDisability"]?.ToString(),
                        Tehsil = row["Tehsil"]?.ToString(),
                        TehsilPer = row["TehsilPer"]?.ToString(),
                        PrDistrictID = row["PrDistrictID"]?.ToString(),
                    };

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student found.";
                    response.ResponseData = student;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentByAdmissionNo", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Message = "An error occurred while processing.";
                response.Error = ex.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentName"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentsByName(string studentName, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };
            #endregion

            try
            {
                #region Get Connection String

                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region SQL Query

                string query = @"
            SELECT 
                s.StudentID,
                s.AdmissionNo,
                dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) AS AcademicNo,
                s.StudentName,
                s.DOB,
                s.BOA,
                s.APAARSTUDENTID,
                s.HouseName,
                s.PrPincode, 
                s.PrDistrict, 
                s.FathersName,
                s.FathersQualification,
                s.FathersJob,
                s.MothersName,
                s.MothersQualification,
                s.MothersJob,
                s.PresentAddress,
                s.PerminantAddress,
                s.SessionOfAdmission,
                s.PhoneNo,
                s.Gender,
                s.Discharged,
                s.TransportFee,
                s.StudentFeeRebate,
                s.WithdrawDate,
                s.Withdrawnarration,
                s.Detnewadmission,
                s.PhoneNo2,
                s.Ledgerid, s.landlineno,
                s.FeeRemarks, s.BloodGroup,
                s.Pincode, s.SEmail,s.PrDistrictID,
                s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
                s.UID, s.Fphn, s.Mphn,
                s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
                s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
                s.Scategory, s.ScategoryID, s.categoryID, s.category,
                s.HID, 
                s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR, s.Religion,
                s.MotherTounge,
                s.BankName,
                s.AccountNo,
                s.AccountType,
                s.IFCCode,
                s.BPLStatus,
                s.SDisability,
                s.Tehsil,
                s.TehsilPer,
                si.StudentInfoID,
                si.RollNo,
                si.PhotoPath,
                si.ClassID,
                si.Current_Session,
                si.RouteID,
                si.BusStopID,
                si.SessionID,
                si.Remarks,
                si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
                si.PrimaryBoardNo, si.PrimaryDate,
                si.MiddleBoardNo, si.MiddleDate,
                si.HighBoardNo, si.HighDate,
                si.HigherBoardNo, si.HigherDate,
                si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
                c.ClassName,
                se.SectionName,
                bs.BusStop,
                bs.BusRate,
                t.RouteName
            FROM Students s
            INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
            INNER JOIN Classes c ON si.ClassID = c.ClassID
            LEFT JOIN Sections se ON si.SectionID = se.SectionID
            LEFT JOIN Transport t ON t.RouteID = si.RouteID
            LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
            WHERE s.StudentName LIKE '%' + @StudentName + '%'";
                #endregion

                #region Execute Query
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@StudentName", studentName)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());
                #endregion

                #region Map Data to DTO List

                List<StudentDTO> students = new List<StudentDTO>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var student = new StudentDTO
                        {
                            StudentID = row["StudentID"]?.ToString(),
                            StudentInfoID = row["StudentInfoID"]?.ToString(),
                            AdmissionNo = row["AdmissionNo"]?.ToString(),
                            StudentName = row["StudentName"]?.ToString(),
                            Apaarid = row["APAARSTUDENTID"]?.ToString(),
                            HouseName = row["HouseName"]?.ToString(),
                            PrPincode = row["PrPincode"]?.ToString(),
                            PrDistrict = row["PrDistrict"]?.ToString(),
                            DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
                            DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
                            FatherName = row["FathersName"]?.ToString(),
                            FatherQualification = row["FathersQualification"]?.ToString(),
                            FatherOccupation = row["FathersJob"]?.ToString(),
                            MotherQualification = row["MothersQualification"]?.ToString(),
                            MotherOccupation = row["MothersJob"]?.ToString(),
                            MontherName = row["MothersName"]?.ToString(),
                            MobileFather = row["Fphn"]?.ToString(),
                            MobileMother = row["Mphn"]?.ToString(),
                            LandLineNo = row["landlineno"]?.ToString(),
                            ClassName = row["ClassName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PermanentAddress = row["PerminantAddress"]?.ToString(),
                            SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            Discharged = row["Discharged"]?.ToString(),
                            DSession = row["DSession"]?.ToString(),
                            DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
                            DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
                            DBy = row["DBy"]?.ToString(),
                            IsDischarged = row["IsDischarged"]?.ToString(),
                            SEmail = row["SEmail"]?.ToString(),
                            BloodGroup = row["BloodGroup"]?.ToString(),
                            PinCode = row["Pincode"]?.ToString(),
                            Aadhaar = row["Saadhaarcard"]?.ToString(),
                            FAdhaar = row["Faadhaarcard"]?.ToString(),
                            MAdhaar = row["Maadhaarcard"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SessionID"]?.ToString(),
                            RollNo = row["RollNo"]?.ToString(),
                            PhotoPath = row["PhotoPath"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            RouteID = row["RouteID"]?.ToString(),
                            AcademicNo = row["AcademicNo"]?.ToString(),
                            busstopid = row["BusStopID"]?.ToString(),
                            RouteName = row["RouteName"]?.ToString(),
                            BusStopName = row["BusStop"]?.ToString(),
                            BusFee = row["BusRate"]?.ToString(),
                            GuardianName = row["GuardianName"]?.ToString(),
                            GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                            GuardianQualification = row["GuardianQualification"]?.ToString(),
                            GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                            DistrictID = row["DistrictID"]?.ToString(),
                            DistrictName = row["DistrictName"]?.ToString(),
                            StudentCatID = row["StudentCatID"]?.ToString(),
                            StudentCatName = row["StudentCatName"]?.ToString(),
                            PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                            HighBoardNo = row["HighBoardNo"]?.ToString(),
                            MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                            PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                            HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                            PrimaryDate = row["PrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrimaryDate"]).ToString("yyyy-MM-dd") : null,
                            HighDate = row["HighDate"] != DBNull.Value ? Convert.ToDateTime(row["HighDate"]).ToString("yyyy-MM-dd") : null,
                            MiddleDate = row["MiddleDate"] != DBNull.Value ? Convert.ToDateTime(row["MiddleDate"]).ToString("yyyy-MM-dd") : null,
                            PrePrimaryDate = row["PrePrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrePrimaryDate"]).ToString("yyyy-MM-dd") : null,
                            HigherDate = row["HigherDate"] != DBNull.Value ? Convert.ToDateTime(row["HigherDate"]).ToString("yyyy-MM-dd") : null,
                            Session = row["Current_Session"]?.ToString(),
                            HID = row["HID"]?.ToString(),
                            PEN = row["PEN"]?.ToString(),
                            WEIGHT = row["WEIGHT"]?.ToString(),
                            Height = row["Height"]?.ToString(),
                            NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                            DOBASPERADHAAR = row["DOBASPERADHAAR"]?.ToString(),
                            Religion = row["Religion"]?.ToString(),
                            MotherTounge = row["MotherTounge"]?.ToString(), // Note: DB column "MotherTounge"
                            BankName = row["BankName"]?.ToString(),
                            AccountNo = row["AccountNo"]?.ToString(),
                            AccountType = row["AccountType"]?.ToString(),
                            IFCCode = row["IFCCode"]?.ToString(),
                            BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                            SDisability = row["SDisability"]?.ToString(),
                            Tehsil = row["Tehsil"]?.ToString(),
                            TehsilPer = row["TehsilPer"]?.ToString(),
                            PrDistrictID = row["PrDistrictID"]?.ToString(),
                        };
                        students.Add(student);
                    }
                }

                response.IsSuccess = true;
                response.Status = 1;
                response.Message = "Students found.";
                response.ResponseData = students;
            }
            #endregion

            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentsByName", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while fetching students.";
                response.Error = ex.Message;
                response.ResponseData = null;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentInfoId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentByStudentInfoId(long studentInfoId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };
            #endregion

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
                response.ResponseData = null;
                return response;
            }
            #endregion

            #region SQL Query
            string query = @"
        SELECT 
            s.StudentID,
            s.AdmissionNo,
            dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) AS AcademicNo,
            s.StudentName,
            s.APAARSTUDENTID,
            s.HouseName,
            s.DOB,
            s.BOA,
            s.PrPincode, 
            s.PrDistrict,
            s.FathersName,
            s.FathersQualification,
            s.FathersJob,
            s.MothersName,
            s.MothersQualification,
            s.MothersJob,
            s.PresentAddress,
            s.PerminantAddress,
            s.SessionOfAdmission,
            s.PhoneNo,
            s.Gender,
            s.Discharged,
            s.TransportFee,
            s.StudentFeeRebate,
            s.WithdrawDate,
            s.Withdrawnarration,
            s.Detnewadmission,
            s.PhoneNo2,
            s.PrDistrictID,
            s.Ledgerid, s.landlineno,
            s.FeeRemarks, s.BloodGroup,
            s.Pincode, s.SEmail,
            s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
            s.UID, s.Fphn, s.Mphn,
            s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
            s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
            s.Scategory, s.ScategoryID, s.categoryID, s.category,s.StateID ,s.StateName ,s.PrStateID,s.PrStateName,
            s.HID, 
            s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,s.Religion,
            s.MotherTounge,
            s.BankName,
            s.AccountNo,
            s.AccountType,
            s.IFCCode,
            s.BPLStatus,
            s.SDisability,
            s.Tehsil,
            s.TehsilPer,
            s.CWSNStatus,      
            s.Category,        
            s.BPLCategory,     
            s.OldSchoolName,   
            s.OldYear,
            s.OldLastDay,
            s.OldGrade,
            s.OldMarks,
            s.OldAcademicNo,
            si.StudentInfoID,
            si.RollNo,
            si.PhotoPath,
            si.ClassID,
            si.Current_Session,
            si.RouteID,
            si.BusStopID,
            si.SessionID,
            si.Remarks,
            si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
            si.PrimaryBoardNo, si.PrimaryDate,
            si.MiddleBoardNo, si.MiddleDate,
            si.HighBoardNo, si.HighDate,
            si.HigherBoardNo, si.HigherDate,
            si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
            c.ClassName,
            se.SectionName,
            bs.BusStop,
            bs.BusRate,
            t.RouteName
        FROM Students s
        INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
        INNER JOIN Classes c ON si.ClassID = c.ClassID
        LEFT JOIN Sections se ON si.SectionID = se.SectionID
        LEFT JOIN Transport t ON t.RouteID = si.RouteID
        LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
        WHERE si.StudentInfoID = @StudentInfoID";
            #endregion


            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@StudentInfoID", studentInfoId)
    };

            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());

                #region Helper for Safe Date
                string SafeDate(object dbValue)
                {
                    if (dbValue == DBNull.Value || string.IsNullOrWhiteSpace(dbValue?.ToString()))
                        return null;

                    return DateTime.TryParse(dbValue.ToString(), out var dt) ? dt.ToString("yyyy-MM-dd") : null;
                }
                #endregion

                #region Map Data to DTO
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];

                    var student = new StudentDTO
                    {
                        StudentID = row["StudentID"]?.ToString(),
                        StudentInfoID = row["StudentInfoID"]?.ToString(),
                        AdmissionNo = row["AdmissionNo"]?.ToString(),
                        StudentName = row["StudentName"]?.ToString(),
                        DOB = SafeDate(row["DOB"]),
                        DOA = SafeDate(row["BOA"]),
                        FatherName = row["FathersName"]?.ToString(),
                        FatherQualification = row["FathersQualification"]?.ToString(),
                        FatherOccupation = row["FathersJob"]?.ToString(),
                        MotherQualification = row["MothersQualification"]?.ToString(),
                        MotherOccupation = row["MothersJob"]?.ToString(),
                        MontherName = row["MothersName"]?.ToString(),
                        MobileFather = row["Fphn"]?.ToString(),
                        MobileMother = row["Mphn"]?.ToString(),
                        LandLineNo = row["landlineno"]?.ToString(),
                        ClassName = row["ClassName"]?.ToString(),
                        SectionName = row["SectionName"]?.ToString(),
                        PresentAddress = row["PresentAddress"]?.ToString(),
                        PermanentAddress = row["PerminantAddress"]?.ToString(),
                        SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        Discharged = row["Discharged"]?.ToString(),
                        DSession = row["DSession"]?.ToString(),
                        DDate = SafeDate(row["DDate"]),
                        DRemarks = DischargeStatus(row["IsDischarged"]?.ToString()),
                        DBy = row["DBy"]?.ToString(),
                        IsDischarged = row["IsDischarged"]?.ToString(),
                        SEmail = row["SEmail"]?.ToString(),
                        BloodGroup = row["BloodGroup"]?.ToString(),
                        PinCode = row["Pincode"]?.ToString(),
                        Aadhaar = row["Saadhaarcard"]?.ToString(),
                        FAdhaar = row["Faadhaarcard"]?.ToString(),
                        MAdhaar = row["Maadhaarcard"]?.ToString(),
                        ClassID = row["ClassID"]?.ToString(),
                        SectionID = row["SessionID"]?.ToString(),
                        RollNo = row["RollNo"]?.ToString(),
                        Remarks = row["Remarks"]?.ToString(),
                        RouteID = row["RouteID"]?.ToString(),
                        AcademicNo = row["AcademicNo"]?.ToString(),
                        busstopid = row["BusStopID"]?.ToString(),
                        RouteName = row["RouteName"]?.ToString(),
                        BusStopName = row["BusStop"]?.ToString(),
                        BusFee = row["BusRate"]?.ToString(),
                        GuardianName = row["GuardianName"]?.ToString(),
                        GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                        GuardianQualification = row["GuardianQualification"]?.ToString(),
                        GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                        DistrictID = row["DistrictID"]?.ToString(),
                        DistrictName = row["DistrictName"]?.ToString(),
                        StudentCatID = row["ScategoryID"]?.ToString(),
                        StudentCatName = row["Scategory"]?.ToString(),
                        PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                        HighBoardNo = row["HighBoardNo"]?.ToString(),
                        MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                        PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                        HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                        PrimaryDate = SafeDate(row["PrimaryDate"]),
                        HighDate = SafeDate(row["HighDate"]),
                        MiddleDate = SafeDate(row["MiddleDate"]),
                        PrePrimaryDate = SafeDate(row["PrePrimaryDate"]),
                        HigherDate = SafeDate(row["HigherDate"]),
                        Session = row["Current_Session"]?.ToString(),
                        HID = row["HID"]?.ToString(),
                        PEN = row["PEN"]?.ToString(),
                        WEIGHT = row["WEIGHT"]?.ToString(),
                        Height = row["Height"]?.ToString(),
                        NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                        DOBASPERADHAAR = SafeDate(row["DOBASPERADHAAR"]),
                        Religion = row["Religion"]?.ToString(),
                        MotherTounge = row["MotherTounge"]?.ToString(),
                        BankName = row["BankName"]?.ToString(),
                        AccountNo = row["AccountNo"]?.ToString(),
                        AccountType = row["AccountType"]?.ToString(),
                        IFCCode = row["IFCCode"]?.ToString(),
                        BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                        SDisability = row["SDisability"]?.ToString(),
                        Tehsil = row["Tehsil"]?.ToString(),
                        TehsilPer = row["TehsilPer"]?.ToString(),
                        Apaarid = row["APAARSTUDENTID"]?.ToString(),
                        HouseName = row["HouseName"]?.ToString(),
                        PrPincode = row["PrPincode"]?.ToString(),
                        PrDistrict = row["PrDistrict"]?.ToString(),
                        PrDistrictID = row["PrDistrictID"]?.ToString(),
                        StateID = row["StateID"]?.ToString(),
                        StateName = row["StateName"]?.ToString(),
                        PrStateID = row["PrStateID"]?.ToString(),
                        PrStateName = row["PrStateName"]?.ToString(),
                        BPLCategory = row["BPLCategory"]?.ToString(),
                        CWSNStatus = row["CWSNStatus"] != DBNull.Value ? Convert.ToBoolean(row["CWSNStatus"]) : (bool?)null,
                        Category = row["Category"]?.ToString(),
                        OldSchoolName = row["OldSchoolName"]?.ToString(),
                        OldYear = row["OldYear"]?.ToString(),
                        OldLastDay = row["OldLastDay"]?.ToString(),
                        OldGrade = row["OldGrade"]?.ToString(),
                        OldMarks = row["OldMarks"]?.ToString(),
                        OldAcademicNo = row["OldAcademicNo"]?.ToString()
                    };

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student details fetched successfully.";
                    response.ResponseData = student;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentByStudentInfoId", ex.Message + " | " + ex.StackTrace);

                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while fetching student details.";
                response.ResponseData = null;
            }

            return response;
        }

        //    public async Task<ResponseModel> GetStudentByStudentInfoId(long studentInfoId, string clientId)
        //    {
        //        #region Initialize Response
        //        var response = new ResponseModel
        //        {
        //            IsSuccess = true,
        //            Message = "No Records Found!",
        //            Status = 0
        //        };
        //        #endregion

        //        #region Get Connection String

        //        var connectionStringHelper = new ConnectionStringHelper(_configuration);
        //        string connectionString = connectionStringHelper.GetConnectionString(clientId);

        //        if (string.IsNullOrEmpty(connectionString))
        //        {
        //            response.IsSuccess = false;
        //            response.Status = 0;
        //            response.Message = "Invalid client ID.";
        //            response.ResponseData = null; // Assuming you have a Data property of type object
        //            return response;
        //        }
        //        #endregion

        //        #region SQL Query
        //        string query = @"
        //    SELECT 
        //        s.StudentID,
        //        s.AdmissionNo,
        //        dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) AS AcademicNo,
        //        s.StudentName,
        //        s.APAARSTUDENTID,
        //        s.HouseName,
        //        s.DOB,
        //        s.BOA,
        //        s.PrPincode, 
        //        s.PrDistrict,
        //        s.FathersName,
        //        s.FathersQualification,
        //        s.FathersJob,
        //        s.MothersName,
        //        s.MothersQualification,
        //        s.MothersJob,
        //        s.PresentAddress,
        //        s.PerminantAddress,
        //        s.SessionOfAdmission,
        //        s.PhoneNo,
        //        s.Gender,
        //        s.Discharged,
        //        s.TransportFee,
        //        s.StudentFeeRebate,
        //        s.WithdrawDate,
        //        s.Withdrawnarration,
        //        s.Detnewadmission,
        //        s.PhoneNo2,
        //        s.PrDistrictID,
        //        s.Ledgerid, s.landlineno,
        //        s.FeeRemarks, s.BloodGroup,
        //        s.Pincode, s.SEmail,
        //        s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
        //        s.UID, s.Fphn, s.Mphn,
        //        s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
        //        s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
        //        s.Scategory, s.ScategoryID, s.categoryID, s.category,s.StateID ,s.StateName ,s.PrStateID,s.PrStateName,
        //        s.HID, 
        //        s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,
        //        si.StudentInfoID,
        //        si.RollNo,
        //        si.PhotoPath,
        //        si.ClassID,
        //        si.Current_Session,
        //        si.RouteID,
        //        si.BusStopID,
        //        si.SessionID,
        //        si.Remarks,
        //        si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
        //        si.PrimaryBoardNo, si.PrimaryDate,
        //        si.MiddleBoardNo, si.MiddleDate,
        //        si.HighBoardNo, si.HighDate,
        //        si.HigherBoardNo, si.HigherDate,
        //        si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
        //        c.ClassName,
        //        se.SectionName,
        //        bs.BusStop,
        //        bs.BusRate,
        //        t.RouteName
        //    FROM Students s
        //    INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
        //    INNER JOIN Classes c ON si.ClassID = c.ClassID
        //    LEFT JOIN Sections se ON si.SectionID = se.SectionID
        //    LEFT JOIN Transport t ON t.RouteID = si.RouteID
        //    LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
        //    WHERE si.StudentInfoID = @StudentInfoID";
        //        #endregion

        //        #region Execute Query
        //        var parameters = new List<SqlParameter>
        //{
        //    new SqlParameter("@StudentInfoID", studentInfoId)
        //};

        //        try
        //        {
        //            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());
        //            #endregion

        //            #region Map Data to DTO

        //            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            {
        //                DataRow row = ds.Tables[0].Rows[0];
        //                var student = new StudentDTO
        //                {
        //                    StudentID = row["StudentID"]?.ToString(),
        //                    StudentInfoID = row["StudentInfoID"]?.ToString(),
        //                    AdmissionNo = row["AdmissionNo"]?.ToString(),
        //                    StudentName = row["StudentName"]?.ToString(),
        //                    DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
        //                    DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
        //                    FatherName = row["FathersName"]?.ToString(),
        //                    FatherQualification = row["FathersQualification"]?.ToString(),
        //                    FatherOccupation = row["FathersJob"]?.ToString(),
        //                    MotherQualification = row["MothersQualification"]?.ToString(),
        //                    MotherOccupation = row["MothersJob"]?.ToString(),
        //                    MontherName = row["MothersName"]?.ToString(),
        //                    MobileFather = row["Fphn"]?.ToString(),
        //                    MobileMother = row["Mphn"]?.ToString(),
        //                    LandLineNo = row["landlineno"]?.ToString(),
        //                    ClassName = row["ClassName"]?.ToString(),
        //                    SectionName = row["SectionName"]?.ToString(),
        //                    PresentAddress = row["PresentAddress"]?.ToString(),
        //                    PermanentAddress = row["PerminantAddress"]?.ToString(),
        //                    SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
        //                    Gender = row["Gender"]?.ToString(),
        //                    Discharged = row["Discharged"]?.ToString(),
        //                    DSession = row["DSession"]?.ToString(),
        //                    DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
        //                    DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
        //                    DBy = row["DBy"]?.ToString(),
        //                    IsDischarged = row["IsDischarged"]?.ToString(),
        //                    SEmail = row["SEmail"]?.ToString(),
        //                    BloodGroup = row["BloodGroup"]?.ToString(),
        //                    PinCode = row["Pincode"]?.ToString(),
        //                    Aadhaar = row["Saadhaarcard"]?.ToString(),
        //                    FAdhaar = row["Faadhaarcard"]?.ToString(),
        //                    MAdhaar = row["Maadhaarcard"]?.ToString(),
        //                    ClassID = row["ClassID"]?.ToString(),
        //                    SectionID = row["SessionID"]?.ToString(),
        //                    RollNo = row["RollNo"]?.ToString(),
        //                    PhotoPath = row["PhotoPath"]?.ToString(),
        //                    Remarks = row["Remarks"]?.ToString(),
        //                    RouteID = row["RouteID"]?.ToString(),
        //                    AcademicNo = row["AcademicNo"]?.ToString(),
        //                    busstopid = row["BusStopID"]?.ToString(),
        //                    RouteName = row["RouteName"]?.ToString(),
        //                    BusStopName = row["BusStop"]?.ToString(),
        //                    BusFee = row["BusRate"]?.ToString(),
        //                    GuardianName = row["GuardianName"]?.ToString(),
        //                    GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
        //                    GuardianQualification = row["GuardianQualification"]?.ToString(),
        //                    GuardialAccupation = row["GuardialAccupation"]?.ToString(),
        //                    DistrictID = row["DistrictID"]?.ToString(),
        //                    DistrictName = row["DistrictName"]?.ToString(),
        //                    StudentCatID = row["StudentCatID"]?.ToString(),
        //                    StudentCatName = row["StudentCatName"]?.ToString(),
        //                    PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
        //                    HighBoardNo = row["HighBoardNo"]?.ToString(),
        //                    MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
        //                    PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
        //                    HigherBoardNo = row["HigherBoardNo"]?.ToString(),
        //                    PrimaryDate = row["PrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrimaryDate"]).ToString("yyyy-MM-dd") : null,
        //                    HighDate = row["HighDate"] != DBNull.Value ? Convert.ToDateTime(row["HighDate"]).ToString("yyyy-MM-dd") : null,
        //                    MiddleDate = row["MiddleDate"] != DBNull.Value ? Convert.ToDateTime(row["MiddleDate"]).ToString("yyyy-MM-dd") : null,
        //                    PrePrimaryDate = row["PrePrimaryDate"] != DBNull.Value ? Convert.ToDateTime(row["PrePrimaryDate"]).ToString("yyyy-MM-dd") : null,
        //                    HigherDate = row["HigherDate"] != DBNull.Value ? Convert.ToDateTime(row["HigherDate"]).ToString("yyyy-MM-dd") : null,
        //                    Session = row["Current_Session"]?.ToString(),
        //                    HID = row["HID"]?.ToString(),
        //                    PEN = row["PEN"]?.ToString(),
        //                    WEIGHT = row["WEIGHT"]?.ToString(),
        //                    Height = row["Height"]?.ToString(),
        //                    NAMEASPERADHAAR = row["NAMEASPERADHAAR"]?.ToString(),
        //                    DOBASPERADHAAR = row["DOBASPERADHAAR"] != DBNull.Value
        //                     ? Convert.ToDateTime(row["DOBASPERADHAAR"]).ToString("yyyy-MM-dd") : null,
        //                    Apaarid = row["APAARSTUDENTID"]?.ToString(),
        //                    HouseName = row["HouseName"]?.ToString(),
        //                    PrPincode = row["PrPincode"]?.ToString(),
        //                    PrDistrict = row["PrDistrict"]?.ToString(),
        //                    PrDistrictID = row["PrDistrictID"]?.ToString(),
        //                    StateID = row["StateID"]?.ToString(),
        //                    StateName = row["StateName"]?.ToString(),
        //                    PrStateID = row["PrStateID"]?.ToString(),
        //                    PrStateName = row["PrStateName"]?.ToString()

        //                };


        //                response.IsSuccess = true;
        //                response.Status = 1;
        //                response.Message = "Student details fetched successfully.";
        //                response.ResponseData = student; // Set Data property here
        //            }
        //            #endregion

        //        }
        //        catch (Exception ex)
        //        {
        //            Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentByStudentInfoId", ex.Message + " | " + ex.StackTrace);

        //            response.IsSuccess = false;
        //            response.Status = 0;
        //            response.Message = "An error occurred while fetching student details.";
        //            response.ResponseData = null;
        //        }

        //        return response;
        //    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentByPhone(string phoneNo, string clientId)
        {
            #region Initialize Response and Validate Connection String
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };


            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
                response.ResponseData = null;
                return response;
            }
            #endregion

            #region SQL Query and Parameters

            StudentDTO? student = null;

            string query = @"
SELECT 
    s.StudentID,
    s.AdmissionNo,
dbo.AcademicNo(si.Current_Session,           
                   s.StudentID,
                   c.SubDepartmentID) AS AcademicNo,
    s.StudentName,
    s.DOB,
    s.BOA,
    s.HouseName,
    s.APAARSTUDENTID,
    s.PrPincode, 
    s.PrDistrict,
    s.FathersName,
    s.FathersQualification,
    s.FathersJob,
    s.MothersName,
    s.MothersQualification,
    s.MothersJob,
    s.PresentAddress,
    s.PerminantAddress,
    s.SessionOfAdmission,
    s.PhoneNo,
    s.Gender,
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.UID, s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, s.PrDistrictID,
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,s.Religion, s.MotherTounge, s.BankName, s.AccountNo,
    s.AccountType, s.IFCCode, s.BPLStatus, s.SDisability, s.Tehsil, s.TehsilPer,
    si.StudentInfoID,
    si.RollNo,
    si.PhotoPath,
    si.ClassID,
    si.Current_Session,
    si.RouteID,
    si.BusStopID,
    si.SessionID,
    si.Remarks,
    si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
    si.PrimaryBoardNo, si.PrimaryDate,
    si.MiddleBoardNo, si.MiddleDate,
    si.HighBoardNo, si.HighDate,
    si.HigherBoardNo, si.HigherDate,
    si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
    c.ClassName,se.SectionName,
    bs.BusStop,
    bs.BusRate,
    t.RouteName
FROM Students s
INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
INNER JOIN Classes c ON si.ClassID = c.ClassID
LEFT JOIN Sections se ON si.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = si.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
WHERE s.PhoneNo = @PhoneNo";


            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@PhoneNo", phoneNo)
    };

            #endregion

            #region Execute and Map Data
            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    student = new StudentDTO
                    {
                        StudentID = row["StudentID"]?.ToString(),
                        StudentInfoID = row["StudentInfoID"]?.ToString(),
                        AdmissionNo = row["AdmissionNo"]?.ToString(),
                        StudentName = row["StudentName"]?.ToString(),
                        Apaarid = row["APAARSTUDENTID"]?.ToString(),
                        HouseName = row["HouseName"]?.ToString(),
                        PrPincode = row["PrPincode"]?.ToString(),
                        PrDistrict = row["PrDistrict"]?.ToString(),
                        DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
                        DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
                        FatherName = row["FathersName"]?.ToString(),
                        FatherQualification = row["FathersQualification"]?.ToString(),
                        FatherOccupation = row["FathersJob"]?.ToString(),
                        MotherQualification = row["MothersQualification"]?.ToString(),
                        MotherOccupation = row["MothersJob"]?.ToString(),
                        MontherName = row["MothersName"]?.ToString(),
                        MobileFather = row["Fphn"]?.ToString(),
                        MobileMother = row["Mphn"]?.ToString(),
                        LandLineNo = row["landlineno"]?.ToString(),
                        ClassName = row["ClassName"]?.ToString(),
                        SectionName = row["SectionName"]?.ToString(),
                        PresentAddress = row["PresentAddress"]?.ToString(),
                        PermanentAddress = row["PerminantAddress"]?.ToString(),
                        SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                        Gender = row["Gender"]?.ToString(),
                        Discharged = row["Discharged"]?.ToString(),
                        DSession = row["DSession"]?.ToString(),
                        DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
                        DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
                        DBy = row["DBy"]?.ToString(),
                        IsDischarged = row["IsDischarged"]?.ToString(),
                        SEmail = row["SEmail"]?.ToString(),
                        BloodGroup = row["BloodGroup"]?.ToString(),
                        PinCode = row["Pincode"]?.ToString(),
                        Aadhaar = row["Saadhaarcard"]?.ToString(),
                        FAdhaar = row["Faadhaarcard"]?.ToString(),
                        MAdhaar = row["Maadhaarcard"]?.ToString(),
                        ClassID = row["ClassID"]?.ToString(),
                        SectionID = row["SessionID"]?.ToString(),
                        RollNo = row["RollNo"]?.ToString(),
                        PhotoPath = row["PhotoPath"]?.ToString(),
                        Remarks = row["Remarks"]?.ToString(),
                        RouteID = row["RouteID"]?.ToString(),
                        AcademicNo = row["AcademicNo"]?.ToString(),
                        busstopid = row["BusStopID"]?.ToString(),
                        RouteName = row["RouteName"]?.ToString(),
                        BusStopName = row["BusStop"]?.ToString(),
                        BusFee = row["BusRate"]?.ToString(),
                        GuardianName = row["GuardianName"]?.ToString(),
                        GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                        GuardianQualification = row["GuardianQualification"]?.ToString(),
                        GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                        DistrictID = row["DistrictID"]?.ToString(),
                        DistrictName = row["DistrictName"]?.ToString(),
                        StudentCatID = row["StudentCatID"]?.ToString(),
                        StudentCatName = row["StudentCatName"]?.ToString(),
                        PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                        HighBoardNo = row["HighBoardNo"]?.ToString(),
                        MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                        PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                        HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                        PrimaryDate = row["PrimaryDate"]?.ToString(),
                        HighDate = row["HighDate"]?.ToString(),
                        MiddleDate = row["MiddleDate"]?.ToString(),
                        PrePrimaryDate = row["PrePrimaryDate"]?.ToString(),
                        HigherDate = row["HigherDate"]?.ToString(),
                        Session = row["Current_Session"]?.ToString(),
                        HID = row["HID"]?.ToString(),
                        PEN = row["PEN"]?.ToString(),
                        WEIGHT = row["WEIGHT"]?.ToString(),
                        Height = row["Height"]?.ToString(),
                        NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                        DOBASPERADHAAR = row["DOBASPERADHAAR"]?.ToString(),
                        Religion = row["Religion"]?.ToString(),
                        MotherTounge = row["MotherTounge"]?.ToString(), // Note: DB column "MotherTounge"
                        BankName = row["BankName"]?.ToString(),
                        AccountNo = row["AccountNo"]?.ToString(),
                        AccountType = row["AccountType"]?.ToString(),
                        IFCCode = row["IFCCode"]?.ToString(),
                        BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                        SDisability = row["SDisability"]?.ToString(),
                        Tehsil = row["Tehsil"]?.ToString(),
                        TehsilPer = row["TehsilPer"]?.ToString(),
                        PrDistrictID = row["PrDistrictID"]?.ToString(),
                    };

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student found.";
                    response.ResponseData = student;
                }

            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentByPhone", ex.Message + " | " + ex.StackTrace);

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while fetching student data.";
                response.ResponseData = null;
            }
            #endregion
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSession"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentsByCurrentSession(string currentSession, string clientId)
        {
            #region Initialize Response and Validate Connection String
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };

            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
                response.ResponseData = null;
                return response;
            }

            #endregion

            #region SQL Query and Parameters
            List<StudentDTO> students = new List<StudentDTO>();

            string query = @"
SELECT 
    s.StudentID,
    s.AdmissionNo,
    dbo.AcademicNo(@CurrentSession, s.StudentID, c.SubDepartmentID) AS AcademicNo,
    s.StudentName,
    s.DOB,
    s.BOA,
    s.HouseName,
    s.APAARSTUDENTID,
    s.PrPincode, 
    s.PrDistrict,
    s.FathersName,
    s.FathersQualification,
    s.FathersJob,
    s.MothersName,
    s.MothersQualification,
    s.MothersJob,
    s.PresentAddress,
    s.PerminantAddress,
    s.SessionOfAdmission,
    s.PhoneNo,
    s.Gender,
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.PrDistrictID,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.UID, s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,s.Religion, s.MotherTounge,
     s.BankName, s.AccountNo, s.AccountType, s.IFCCode,
    s.BPLStatus, s.SDisability, s.Tehsil, s.TehsilPer,
    si.StudentInfoID,
    si.RollNo,
    si.PhotoPath,
    si.ClassID,                                       
    si.Current_Session,
    si.RouteID,
    si.BusStopID,
    si.SessionID,
    si.Remarks,
    si.BoardNo, si.PrePrimaryBoardNo, si.PrePrimaryDate,
    si.PrimaryBoardNo, si.PrimaryDate,
    si.MiddleBoardNo, si.MiddleDate,
    si.HighBoardNo, si.HighDate,
    si.HigherBoardNo, si.HigherDate,
    si.IsDischarged, si.DSession, si.DDate, si.DRemarks, si.DBy,
    c.ClassName,se.SectionName,
    bs.BusStop,
    bs.BusRate,
    t.RouteName
FROM Students s
INNER JOIN StudentInfo si ON s.StudentID = si.StudentId
INNER JOIN Classes c ON si.ClassID = c.ClassID
LEFT JOIN Sections se ON si.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = si.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = si.BusStopID
WHERE si.Current_Session = @CurrentSession";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@CurrentSession", currentSession)
    };
            #endregion

            #region Execute and Map Data
            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var student = new StudentDTO
                        {
                            StudentID = row["StudentID"]?.ToString(),
                            StudentInfoID = row["StudentInfoID"]?.ToString(),
                            AdmissionNo = row["AdmissionNo"]?.ToString(),
                            AcademicNo = row["AcademicNo"]?.ToString(),
                            StudentName = row["StudentName"]?.ToString(),
                            Apaarid = row["APAARSTUDENTID"]?.ToString(),
                            PrPincode = row["PrPincode"]?.ToString(),
                            PrDistrict = row["PrDistrict"]?.ToString(),
                            HouseName = row["HouseName"]?.ToString(),
                            DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]).ToString("yyyy-MM-dd") : null,
                            DOA = row["BOA"] != DBNull.Value ? Convert.ToDateTime(row["BOA"]).ToString("yyyy-MM-dd") : null,
                            FatherName = row["FathersName"]?.ToString(),
                            FatherQualification = row["FathersQualification"]?.ToString(),
                            FatherOccupation = row["FathersJob"]?.ToString(),
                            MotherQualification = row["MothersQualification"]?.ToString(),
                            MotherOccupation = row["MothersJob"]?.ToString(),
                            MontherName = row["MothersName"]?.ToString(),
                            MobileFather = row["Fphn"]?.ToString(),
                            MobileMother = row["Mphn"]?.ToString(),
                            LandLineNo = row["landlineno"]?.ToString(),
                            ClassName = row["ClassName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PermanentAddress = row["PerminantAddress"]?.ToString(),
                            SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            Discharged = row["Discharged"]?.ToString(),
                            DSession = row["DSession"]?.ToString(),
                            DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
                            DRemarks = DischargeStatus(row["IsDischarged"].ToString()),
                            DBy = row["DBy"]?.ToString(),
                            IsDischarged = row["IsDischarged"]?.ToString(),
                            SEmail = row["SEmail"]?.ToString(),
                            BloodGroup = row["BloodGroup"]?.ToString(),
                            PinCode = row["Pincode"]?.ToString(),
                            Aadhaar = row["Saadhaarcard"]?.ToString(),
                            FAdhaar = row["Faadhaarcard"]?.ToString(),
                            MAdhaar = row["Maadhaarcard"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SessionID"]?.ToString(),
                            RollNo = row["RollNo"]?.ToString(),
                            PhotoPath = row["PhotoPath"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            RouteID = row["RouteID"]?.ToString(),
                            busstopid = row["BusStopID"]?.ToString(),
                            RouteName = row["RouteName"]?.ToString(),
                            BusStopName = row["BusStop"]?.ToString(),
                            BusFee = row["BusRate"]?.ToString(),
                            GuardianName = row["GuardianName"]?.ToString(),
                            GuardianPhoneNo = row["GuardianPhoneNo"]?.ToString(),
                            GuardianQualification = row["GuardianQualification"]?.ToString(),
                            GuardialAccupation = row["GuardialAccupation"]?.ToString(),
                            DistrictID = row["DistrictID"]?.ToString(),
                            DistrictName = row["DistrictName"]?.ToString(),
                            StudentCatID = row["StudentCatID"]?.ToString(),
                            StudentCatName = row["StudentCatName"]?.ToString(),
                            PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                            HighBoardNo = row["HighBoardNo"]?.ToString(),
                            MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                            PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                            HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                            PrimaryDate = row["PrimaryDate"]?.ToString(),
                            HighDate = row["HighDate"]?.ToString(),
                            MiddleDate = row["MiddleDate"]?.ToString(),
                            PrePrimaryDate = row["PrePrimaryDate"]?.ToString(),
                            HigherDate = row["HigherDate"]?.ToString(),
                            Session = row["Current_Session"]?.ToString(),
                            HID = row["HID"]?.ToString(),
                            PEN = row["PEN"]?.ToString(),
                            WEIGHT = row["WEIGHT"]?.ToString(),
                            Height = row["Height"]?.ToString(),
                            NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                            DOBASPERADHAAR = row["DOBASPERADHAAR"]?.ToString(),
                            Religion = row["Religion"]?.ToString(),
                            MotherTounge = row["MotherTounge"]?.ToString(), // Note: DB column "MotherTounge"
                            BankName = row["BankName"]?.ToString(),
                            AccountNo = row["AccountNo"]?.ToString(),
                            AccountType = row["AccountType"]?.ToString(),
                            IFCCode = row["IFCCode"]?.ToString(),
                            BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                            SDisability = row["SDisability"]?.ToString(),
                            Tehsil = row["Tehsil"]?.ToString(),
                            TehsilPer = row["TehsilPer"]?.ToString(),
                            PrDistrictID = row["PrDistrictID"]?.ToString(),
                        };

                        students.Add(student);
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = $"Found {students.Count} students for session {currentSession}.";
                    response.ResponseData = students;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "No students found for the specified session.";
                    response.ResponseData = null;
                }
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentsByCurrentSession", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while fetching students.";
                response.ResponseData = null;
            }
            #endregion
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllStudentsOnSectionID(string sectionId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    response.ResponseData = null;
                    return response;
                }
                #endregion

                #region Get Current Session
                var sessionParam = new SqlParameter("@SecID", sectionId);
                string sessionQuery = "SELECT MAX(Current_Session) FROM Sections WHERE SectionID = @SecID";
                var sessionDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, sessionParam);

                if (sessionDs.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(sessionDs.Tables[0].Rows[0][0].ToString()))
                {
                    response.Message = "Invalid Section ID";
                    return response;
                }

                string cs = sessionDs.Tables[0].Rows[0][0].ToString();
                #endregion

                #region Define Query
                string sql = @"
SELECT 
    Students.StudentID,
    StudentInfoID,
    Students.AdmissionNo,
    dbo.AcademicNo(@cs, Students.StudentID, SubDepartmentID) AS AcademicNo,
    StudentName,
    DOB,
    BOA,
    FathersName,
    FathersQualification,
    FathersJob,
    MothersName,
    HouseName,
    APAARSTUDENTID,
    MothersQualification,
    MothersJob,
    PresentAddress,
    PerminantAddress,
    PrPincode, 
    PrDistrict,
    PrDistrictID,
    SessionOfAdmission,
    PhoneNo,
    Gender,
    Discharged,
    Phoneno2,
    landlineno,
    WithdrawDate,
    Saadhaarcard,
    Faadhaarcard,
    Maadhaarcard,
    SEmail,
    BloodGroup,
    Religion,
    MotherTounge,
    BankName,
    AccountNo,
    AccountType,
    IFCCode,
    BPLStatus,
    SDisability,
    Tehsil,
    TehsilPer,
    Pincode,
    StudentInfo.ClassID,
    StudentInfo.SectionID,
    StudentInfo.Current_Session,
    Rollno,
    PhotoPath,
    StudentInfo.RouteID,
    StreamID,
    StudentInfo.Remarks,
    BoardNo,
    PrePrimaryBoardNo,
    PrePrimaryDate,
    PrimaryBoardNo,
    PrimaryDate,
    MiddleBoardNo,
    MiddleDate,
    HighBoardNo,
    HighDate,
    HigherBoardNo,
    HigherDate,
    StudentInfo.BusStopID,
    IsDischarged,
    DSession,
    DDate,
    DRemarks,
    DBy,
    BusFee,
    GuardianName,
    GuardialAccupation,
    GuardianPhoneNo,
    GuardianQualification,
    DistrictID,
    StudentCatID,
    StudentCatName,
    DistrictName,
    Classes.ClassName,
    se.SectionName,
    bs.BusStop,
    bs.BusRate,
    t.RouteName
FROM Students
INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
LEFT JOIN Sections se ON StudentInfo.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
WHERE StudentInfo.SectionID = @SecID
ORDER BY StudentInfo.ClassID, StudentInfo.SectionID, Rollno;
";
                #endregion

                #region Execute Query and Map Result
                var parameters = new[]
                {
            new SqlParameter("@cs", cs),
            new SqlParameter("@SecID", sectionId)
        };

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);
                var students = new List<StudentDTO>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var student = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        AcademicNo = dr["AcademicNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        Apaarid = dr["APAARSTUDENTID"]?.ToString(),
                        HouseName = dr["HouseName"]?.ToString(),
                        PrPincode = dr["PrPincode"]?.ToString(),
                        PrDistrict = dr["PrDistrict"]?.ToString(),
                        DOB = string.IsNullOrEmpty(dr["DOB"].ToString()) ? null : Convert.ToDateTime(dr["DOB"]).ToString("dd-MM-yyyy"),
                        DOA = string.IsNullOrEmpty(dr["BOA"].ToString()) ? null : Convert.ToDateTime(dr["BOA"]).ToString("dd-MM-yyyy"),
                        PhotoPath = dr["PhotoPath"].ToString(),
                        Aadhaar = dr["Saadhaarcard"].ToString(),
                        SEmail = dr["SEmail"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        Discharged = dr["Discharged"].ToString(),

                        ClassID = dr["ClassID"].ToString(),
                        SectionID = dr["SectionID"].ToString(),
                        RollNo = dr["Rollno"].ToString(),
                        Session = dr["Current_Session"].ToString(),
                        SessionOfAdmission = dr["SessionOfAdmission"].ToString(),

                        FatherName = dr["FathersName"].ToString(),
                        FatherQualification = dr["FathersQualification"].ToString(),
                        FatherOccupation = dr["FathersJob"].ToString(),
                        MobileFather = dr["PhoneNo"].ToString(),

                        MontherName = dr["MothersName"].ToString(),
                        MotherQualification = dr["MothersQualification"].ToString(),
                        MotherOccupation = dr["MothersJob"].ToString(),
                        MobileMother = dr["Phoneno2"].ToString(),

                        PresentAddress = dr["PresentAddress"].ToString(),
                        PermanentAddress = dr["PerminantAddress"].ToString(),
                        PinCode = dr["Pincode"].ToString(),

                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"].ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"].ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"].ToString(),
                        HighBoardNo = dr["HighBoardNo"].ToString(),
                        HigherBoardNo = dr["HigherBoardNo"].ToString(),

                        IsDischarged = dr["IsDischarged"].ToString(),

                        DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = string.IsNullOrEmpty(dr["DDate"].ToString()) ? null : Convert.ToDateTime(dr["DDate"]).ToString("dd-MM-yyyy"),

                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        RouteID = dr["RouteID"]?.ToString(),
                        busstopid = dr["BusStopID"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        BusStopName = dr["BusStop"]?.ToString(),
                        BusFee = dr["BusRate"]?.ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString(),
                        ClassName = dr["ClassName"]?.ToString(),
                        SectionName = dr["SectionName"]?.ToString(),
                        PrDistrictID = dr["PrDistrictID"]?.ToString(),
                        DRemarks = DischargeStatus(dr["IsDischarged"].ToString()),
                        DistrictName = dr["DistrictName"].ToString(),
                        Religion = dr["Religion"]?.ToString(),
                        MotherTounge = dr["MotherTounge"]?.ToString(), // Note: DB column "MotherTounge"
                        BankName = dr["BankName"]?.ToString(),
                        AccountNo = dr["AccountNo"]?.ToString(),
                        AccountType = dr["AccountType"]?.ToString(),
                        IFCCode = dr["IFCCode"]?.ToString(),
                        BPLStatus = dr["BPLStatus"] != DBNull.Value ? Convert.ToInt32(dr["BPLStatus"]) : (int?)null,
                        SDisability = dr["SDisability"]?.ToString(),
                        Tehsil = dr["Tehsil"]?.ToString(),
                        TehsilPer = dr["TehsilPer"]?.ToString()
                    };

                    students.Add(student);
                }

                if (students.Count > 0)
                {
                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = students;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = $"Error: {ex.Message}";
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetOnlyActiveStudentsOnClassID(long classId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                List<StudentDTO> students = new();

                #region Get Current Session
                string sessionQuery = "SELECT MAX(Current_Session) FROM Classes WHERE ClassId = @ClassID";
                var paramForSession = new SqlParameter("@ClassID", classId);
                var sessionDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, paramForSession);

                if (sessionDs.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(sessionDs.Tables[0].Rows[0][0]?.ToString()))
                {
                    response.Message = "Invalid Class ID.";
                    return response;
                }

                string cs = sessionDs.Tables[0].Rows[0][0].ToString();
                #endregion

                #region SQL Query - Get Active Students
                string sql = $@"
SELECT 
    Students.StudentID, 
    StudentInfoID, 
    Students.AdmissionNo,
    dbo.AcademicNo('{cs}', Students.StudentID, SubDepartmentID) AS AcademicNo,
    StudentName, 
    DOB, 
    BOA, 
    FathersName, 
    FathersQualification, 
    HouseName,
    APAARSTUDENTID,
    PrPincode, 
    PrDistrict,
    FathersJob,
    MothersName, 
    MothersQualification, 
    MothersJob, 
    PresentAddress, 
    PerminantAddress,
    SessionOfAdmission, 
    PhoneNo, 
    Gender, 
    Discharged, 
    Phoneno2, 
    landlineno, 
    WithdrawDate,
    Saadhaarcard, 
    Faadhaarcard, 
    Maadhaarcard, 
    SEmail, 
    PrDistrictID,
    BloodGroup, 
    Pincode,
    StudentInfo.ClassID, 
    StudentInfo.SectionID, 
    StudentInfo.Current_Session, 
    Rollno, 
    PhotoPath,
    StudentInfo.RouteID, 
    StreamID, 
    StudentInfo.Remarks,
    BoardNo, 
    PrePrimaryBoardNo, 
    PrePrimaryDate,
    PrimaryBoardNo, 
    PrimaryDate, 
    MiddleBoardNo, 
    MiddleDate, 
    HighBoardNo, 
    HighDate,
    HigherBoardNo, 
    HigherDate,
    StudentInfo.busstopid, 
    IsDischarged, 
    DSession, 
    DDate, 
    DRemarks,
    DBy, 
    BusFee, 
    GuardianName, 
    GuardialAccupation, 
    GuardianPhoneNo,
    GuardianQualification, 
    DistrictID, 
    StudentCatID, 
    StudentCatName, 
    DistrictName,
    Pen, 
    Height, 
    WEIGHT, 
    NAMEASPERADHAAR, 
    DOBASPERADHAAR,
    Classes.ClassName, 
    se.SectionName,
    bs.BusStop,
    bs.BusRate,
    t.RouteName
FROM Students
INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
LEFT JOIN Sections se ON StudentInfo.SectionID = se.SectionID
LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
WHERE StudentInfo.ClassID = @ClassID AND IsDischarged = 0
ORDER BY StudentInfo.ClassID, StudentInfo.SectionID, Rollno
";
                #endregion

                #region Execute Query & Map Data
                var paramForStudents = new SqlParameter("@ClassID", classId);
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, paramForStudents);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StudentDTO sd = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        AcademicNo = dr["AcademicNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        DOB = string.IsNullOrEmpty(dr["DOB"].ToString()) ? null : Convert.ToDateTime(dr["DOB"]).ToString("dd-MM-yyyy"),
                        DOA = string.IsNullOrEmpty(dr["BOA"].ToString()) ? null : Convert.ToDateTime(dr["BOA"]).ToString("dd-MM-yyyy"),
                        PhotoPath = dr["PhotoPath"].ToString(),
                        Aadhaar = dr["Saadhaarcard"].ToString(),
                        SEmail = dr["SEmail"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        Discharged = dr["Discharged"].ToString(),
                        Apaarid = dr["APAARSTUDENTID"]?.ToString(),
                        HouseName = dr["HouseName"]?.ToString(),
                        PrPincode = dr["PrPincode"]?.ToString(),
                        PrDistrict = dr["PrDistrict"]?.ToString(),
                        ClassID = dr["ClassID"].ToString(),
                        SectionID = dr["SectionID"].ToString(),
                        RollNo = dr["Rollno"].ToString(),
                        Session = dr["Current_Session"].ToString(),
                        SessionOfAdmission = dr["SessionOfAdmission"].ToString(),

                        FatherName = dr["FathersName"].ToString(),
                        FatherQualification = dr["FathersQualification"].ToString(),
                        FatherOccupation = dr["FathersJob"].ToString(),
                        MobileFather = dr["PhoneNo"].ToString(),

                        MontherName = dr["MothersName"].ToString(),
                        MotherQualification = dr["MothersQualification"].ToString(),
                        MotherOccupation = dr["MothersJob"].ToString(),
                        MobileMother = dr["Phoneno2"].ToString(),

                        RouteID = dr["RouteID"]?.ToString(),
                        busstopid = dr["BusStopID"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        BusStopName = dr["BusStop"]?.ToString(),
                        BusFee = dr["BusRate"]?.ToString(),

                        PresentAddress = dr["PresentAddress"].ToString(),
                        PermanentAddress = dr["PerminantAddress"].ToString(),
                        PinCode = dr["Pincode"].ToString(),

                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"].ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"].ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"].ToString(),
                        HighBoardNo = dr["HighBoardNo"].ToString(),
                        HigherBoardNo = dr["HigherBoardNo"].ToString(),

                        IsDischarged = dr["IsDischarged"].ToString(),
                        DRemarks = dr["DRemarks"]?.ToString(),
                        DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = string.IsNullOrEmpty(dr["DDate"].ToString()) ? null : Convert.ToDateTime(dr["DDate"]).ToString("dd-MM-yyyy"),

                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        ClassName = dr["ClassName"]?.ToString(),
                        SectionName = dr["SectionName"]?.ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),

                        PEN = dr["Pen"].ToString(),
                        Height = dr["Height"].ToString(),
                        WEIGHT = dr["WEIGHT"].ToString(),
                        NameAsPerAadhaar = dr["NAMEASPERADHAAR"].ToString(),
                        DOBASPERADHAAR = string.IsNullOrEmpty(dr["DOBASPERADHAAR"].ToString()) ? null : Convert.ToDateTime(dr["DOBASPERADHAAR"]).ToString("dd-MM-yyyy"),
                        BloodGroup = dr["BloodGroup"].ToString(),
                        PrDistrictID = dr["PrDistrictID"]?.ToString(),
                    };

                    students.Add(sd);
                }

                if (students.Count > 0)
                {
                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = students;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetOnlyActiveStudentsOnSectionID(long sectionId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Records Found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region SQL Query and Parameters
                string sql = @"
SELECT 
    Students.StudentID,
    StudentInfoID,
    Students.AdmissionNo,
    dbo.AcademicNo((SELECT Current_Session FROM Sections WHERE SectionID = @SectionID), Students.StudentID, SubDepartmentID) AS AcademicNo,
    StudentName,
    DOB,
    BOA,
    HouseName,
    APAARSTUDENTID,
    PrPincode, 
    PrDistrict,
    FathersName,
    FathersQualification,
    FathersJob,
    MothersName,
    MothersQualification,
    MothersJob,
    PresentAddress,
    PerminantAddress,
    SessionOfAdmission,
    PhoneNo,
    Gender,
    Discharged,
    Phoneno2,
    landlineno,
    WithdrawDate,  
    Saadhaarcard,
    Faadhaarcard,
    Maadhaarcard,
    SEmail,
    BloodGroup,
    PrDistrictID,
    Pincode,
    StudentInfo.Remarks,
    StudentInfo.ClassID,
    StudentInfo.SectionID,
    StudentInfo.Current_Session,
    Rollno,
    PhotoPath,
    StudentInfo.RouteID,
    StreamID,
    BoardNo,
    PrePrimaryBoardNo,
    PrePrimaryDate,
    PrimaryBoardNo,
    PrimaryDate,
    MiddleBoardNo,
    MiddleDate,
    HighBoardNo,
    HighDate,
    HigherBoardNo,
    HigherDate,
    StudentInfo.RouteID,
    StudentInfo.BusStopID,
    IsDischarged,
    DSession,
    DDate,
    DRemarks,
    DBy,
    BusFee,
    GuardianName,
    GuardialAccupation,
    GuardianPhoneNo,
    GuardianQualification,
    DistrictID,
    StudentCatID,
    StudentCatName,
    DistrictName,
    Pen,
    Height,
    WEIGHT,
    NAMEASPERADHAAR,
    DOBASPERADHAAR,
    Classes.ClassName, 
    t.RouteName,
    bs.BusStop,
    se.SectionName
FROM 
    Students
INNER JOIN 
    StudentInfo ON Students.StudentID = StudentInfo.StudentId
INNER JOIN 
    Classes ON StudentInfo.ClassID = Classes.ClassId
LEFT JOIN 
    Sections se ON StudentInfo.SectionID = se.SectionID
LEFT JOIN 
    Transport t ON t.RouteID = StudentInfo.RouteID
LEFT JOIN 
    BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
WHERE 
    StudentInfo.SectionID = @SectionID AND IsDischarged = 0
ORDER BY 
    StudentInfo.ClassID, StudentInfo.SectionID, Rollno;";

                SqlParameter[] paramForStudents =
                {
            new SqlParameter("@SectionID", sectionId)
        };
                #endregion

                #region Execute Query and Map Result
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, paramForStudents);
                var studentsList = new List<StudentDTO>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow reader in ds.Tables[0].Rows)
                    {
                        var student = new StudentDTO
                        {
                            StudentID = reader["StudentID"].ToString(),
                            StudentInfoID = reader["StudentInfoID"].ToString(),
                            AdmissionNo = reader["AdmissionNo"].ToString(),
                            AcademicNo = reader["AcademicNo"].ToString(),
                            StudentName = reader["StudentName"].ToString(),
                            Apaarid = reader["APAARSTUDENTID"]?.ToString(),
                            HouseName = reader["HouseName"]?.ToString(),
                            PrPincode = reader["PrPincode"]?.ToString(),
                            PrDistrict = reader["PrDistrict"]?.ToString(),
                            DOB = (reader["DOB"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["DOB"].ToString()))
        ? Convert.ToDateTime(reader["DOB"]).ToString("dd-MM-yyyy")
        : string.Empty,

                            DOA = (reader["BOA"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["BOA"].ToString()))
        ? Convert.ToDateTime(reader["BOA"]).ToString("dd-MM-yyyy")
        : string.Empty,
                            PhotoPath = reader["PhotoPath"]?.ToString() ?? "",
                            Aadhaar = reader["Saadhaarcard"]?.ToString() ?? "",
                            SEmail = reader["SEmail"]?.ToString() ?? "",
                            Gender = reader["Gender"]?.ToString() ?? "",
                            Remarks = reader["Remarks"]?.ToString() ?? "",
                            Discharged = reader["Discharged"]?.ToString() ?? "",

                            ClassID = reader["ClassID"].ToString(),
                            SectionID = reader["SectionID"].ToString(),
                            RollNo = reader["Rollno"].ToString(),
                            Session = reader["Current_Session"].ToString(),
                            SessionOfAdmission = reader["SessionOfAdmission"].ToString(),
                            ClassName = reader["ClassName"]?.ToString() ?? "",
                            SectionName = reader["SectionName"]?.ToString() ?? "",

                            FatherName = reader["FathersName"].ToString(),
                            FatherQualification = reader["FathersQualification"].ToString(),
                            FatherOccupation = reader["FathersJob"].ToString(),
                            MobileFather = reader["PhoneNo"].ToString(),

                            MontherName = reader["MothersName"].ToString(),
                            MotherQualification = reader["MothersQualification"].ToString(),
                            MotherOccupation = reader["MothersJob"].ToString(),
                            MobileMother = reader["Phoneno2"].ToString(),

                            PresentAddress = reader["PresentAddress"].ToString(),
                            PermanentAddress = reader["PerminantAddress"].ToString(),
                            PinCode = reader["Pincode"].ToString(),

                            PrePrimaryBoardNo = reader["PrePrimaryBoardNo"].ToString(),
                            PrimaryBoardNo = reader["PrimaryBoardNo"].ToString(),
                            MiddleBoardNo = reader["MiddleBoardNo"].ToString(),
                            HighBoardNo = reader["HighBoardNo"].ToString(),
                            HigherBoardNo = reader["HigherBoardNo"].ToString(),

                            IsDischarged = reader["IsDischarged"].ToString(),
                            DRemarks = reader["DRemarks"]?.ToString() ?? "",
                            DBy = reader["DBy"].ToString(),
                            DSession = reader["DSession"].ToString(),
                            DDate = (reader["DDate"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["DDate"].ToString()))
        ? Convert.ToDateTime(reader["DDate"]).ToString("dd-MM-yyyy")
        : string.Empty,

                            GuardianName = reader["GuardianName"].ToString(),
                            GuardialAccupation = reader["GuardialAccupation"].ToString(),
                            GuardianPhoneNo = reader["GuardianPhoneNo"].ToString(),
                            GuardianQualification = reader["GuardianQualification"].ToString(),

                            DistrictID = reader["DistrictID"].ToString(),
                            StudentCatID = reader["StudentCatID"].ToString(),
                            DistrictName = reader["DistrictName"].ToString(),
                            StudentCatName = reader["StudentCatName"].ToString(),

                            PEN = reader["Pen"].ToString(),
                            Height = reader["Height"].ToString(),
                            WEIGHT = reader["WEIGHT"].ToString(),
                            NameAsPerAadhaar = reader["NAMEASPERADHAAR"].ToString(),

                            DOBASPERADHAAR = (reader["DOBASPERADHAAR"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["DOBASPERADHAAR"].ToString()))
        ? Convert.ToDateTime(reader["DOBASPERADHAAR"]).ToString("dd-MM-yyyy")
        : string.Empty,

                            BloodGroup = reader["BloodGroup"].ToString(),
                            BusStopName = reader["BusStop"].ToString(),
                            RouteID = reader["RouteID"]?.ToString(),
                            busstopid = reader["BusStopID"]?.ToString(),
                            RouteName = reader["RouteName"]?.ToString(),
                            PrDistrictID = reader["PrDistrictID"]?.ToString(),
                            BusFee = reader["BusFee"]?.ToString()
                        };

                        studentsList.Add(student);
                    }
                }
                #endregion

                #region Final Response
                if (studentsList.Any())
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentsList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMaxRollno(string sectionId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Something went wrong!",
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region SQL Execution
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var command = new SqlCommand(@"SELECT ISNULL(MAX(rollno), 0) AS rollno 
                                           FROM StudentInfo 
                                           WHERE SectionID = @sectionid", connection);
                    command.Parameters.AddWithValue("@sectionid", sectionId);

                    var result = await command.ExecuteScalarAsync();
                    long roll = result != DBNull.Value ? Convert.ToInt64(result) : 0;
                    roll++;

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = roll.ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllDischargedStudentsOnSectionID(string sectionId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Get Current Session
                SqlParameter param1 = new SqlParameter("@SectionID", sectionId);
                string sessionQuery = "SELECT MAX(Current_Session) FROM Sections WHERE SectionID=@SectionID";

                DataSet sessionDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, param1);

                if (sessionDs.Tables.Count == 0 || sessionDs.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(sessionDs.Tables[0].Rows[0][0].ToString()))
                {
                    response.Message = "Invalid Section ID";
                    return response;
                }

                string cs = sessionDs.Tables[0].Rows[0][0].ToString();
                #endregion

                #region SQL Query for Discharged Students
                string sql = $@"
SELECT 
    Students.StudentID,
    StudentInfoID,
    Students.AdmissionNo,
    StudentName,
    DOB,
    BOA,
    HouseName,
    APAARSTUDENTID,
    PrPincode, 
    PrDistrict,
    dbo.AcademicNo('{cs}', Students.StudentID, SubDepartmentID) AS AcademicNo,
    FathersName,
    FathersQualification,
    FathersJob,
    MothersName,
    MothersQualification,
    MothersJob,
    PresentAddress,
    PerminantAddress,
    SessionOfAdmission,
    PhoneNo,
    Gender,
    Discharged,
    Phoneno2,
    landlineno,
    WithdrawDate,
    Saadhaarcard,
    Faadhaarcard,
    Maadhaarcard,
    SEmail,
    BloodGroup,
    PrDistrictID,
    Pincode,
    StudentInfo.ClassID,
    StudentInfo.SectionID,
    StudentInfo.Current_Session,
    Rollno,
    PhotoPath,
    StudentInfo.RouteID,
    StreamID,
    StudentInfo.Remarks,
    BoardNo,
    PrePrimaryBoardNo,
    PrePrimaryDate,
    PrimaryBoardNo,
    PrimaryDate,
    MiddleBoardNo,
    MiddleDate,
    HighBoardNo,
    HighDate,
    HigherBoardNo,
    HigherDate,
    StudentInfo.BusStopID,
    IsDischarged,
    DSession,
    DDate,
    DRemarks,
    DBy,
    BusFee,
    GuardianName,
    GuardialAccupation,
    GuardianPhoneNo,
    GuardianQualification,
    DistrictID,
    StudentCatID,
    StudentCatName,
    DistrictName,
    Classes.ClassName,
    se.SectionName,
    t.RouteName,
    bs.BusStop
FROM 
    Students 
    INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentID
    INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
    LEFT JOIN Sections se ON StudentInfo.SectionID = se.SectionID
    LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
    LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
WHERE 
    StudentInfo.SectionID = @SectionID 
    AND Discharged = 'True' 
    AND IsDischarged = 1
ORDER BY 
    StudentInfo.ClassID, SectionID, Rollno";
                #endregion

                #region Execute Query and Map Results
                SqlParameter param2 = new SqlParameter("@SectionID", sectionId);
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param2);

                List<StudentDTO> studentList = new List<StudentDTO>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        AcademicNo = dr["AcademicNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        PrPincode = dr["PrPincode"]?.ToString(),
                        PrDistrict = dr["PrDistrict"]?.ToString(),
                        DOB = Convert.ToDateTime(string.IsNullOrEmpty(dr["DOB"].ToString()) ? DateTime.Now.ToString() : dr["DOB"].ToString()).ToString("dd-MM-yyyy"),
                        DOA = Convert.ToDateTime(string.IsNullOrEmpty(dr["BOA"].ToString()) ? DateTime.Now.ToString() : dr["BOA"].ToString()).ToString("dd-MM-yyyy"),
                        PhotoPath = dr["PhotoPath"].ToString(),
                        Aadhaar = dr["Saadhaarcard"].ToString(),
                        SEmail = dr["SEmail"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Apaarid = dr["APAARSTUDENTID"]?.ToString(),
                        HouseName = dr["HouseName"]?.ToString(),

                        DBy = dr["DBy"].ToString(),
                        DRemarks = dr["DRemarks"].ToString(),
                        Remarks = dr["DRemarks"].ToString(),
                        Discharged = dr["Discharged"].ToString(),
                        ClassID = dr["ClassID"].ToString(),
                        SectionID = dr["SectionID"].ToString(),
                        RollNo = dr["Rollno"].ToString(),
                        Session = dr["Current_Session"].ToString(),
                        SessionOfAdmission = dr["SessionOfAdmission"].ToString(),
                        ClassName = dr["ClassName"]?.ToString() ?? "",
                        SectionName = dr["SectionName"].ToString(),
                        FatherName = dr["FathersName"].ToString(),
                        FatherQualification = dr["FathersQualification"].ToString(),
                        FatherOccupation = dr["FathersJob"].ToString(),
                        MobileFather = dr["PhoneNo"].ToString(),
                        MontherName = dr["MothersName"].ToString(),
                        MotherQualification = dr["MothersQualification"].ToString(),
                        MotherOccupation = dr["MothersJob"].ToString(),
                        MobileMother = dr["Phoneno2"].ToString(),
                        PresentAddress = dr["PresentAddress"].ToString(),
                        PermanentAddress = dr["PerminantAddress"].ToString(),
                        PinCode = dr["Pincode"].ToString(),
                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"].ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"].ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"].ToString(),
                        HighBoardNo = dr["HighBoardNo"].ToString(),
                        HigherBoardNo = dr["HigherBoardNo"].ToString(),
                        IsDischarged = dr["IsDischarged"].ToString(),
                        // DRemarks = DischargeStatus(dr["IsDischarged"].ToString()),
                        //  DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = !string.IsNullOrEmpty(dr["DDate"].ToString()) ? Convert.ToDateTime(dr["DDate"]).ToString("dd-MM-yyyy") : null,
                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString(),
                        RouteName = dr["RouteName"].ToString(),
                        BusStopName = dr["BusStop"].ToString(),
                        RouteID = dr["RouteID"].ToString(),
                        busstopid = dr["BusStopID"].ToString(),
                        BusFee = dr["BusFee"].ToString(),
                        PrDistrictID = dr["PrDistrictID"]?.ToString(),
                    };

                    studentList.Add(sd);
                }
                #endregion

                #region Finalize Response
                if (studentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> TotalStudentsRollForDashBoard(string session, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter param = new SqlParameter("@LiveSession", session);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "SDb_TotalStudentsRollForAPI", param);
                #endregion

                #region Process Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    response.ResponseData = $"{dr["TS"]}|{dr["TMS"]}|{dr["TFS"]}|{dr["TotalEmployees"]}|{dr["ActiveEmployees"]}|{dr["InactiveEmployees"]}|{dr["TME"]}|{dr["TFE"]}|{dr["TeachingEmployees"]}|{dr["NonTeachingEmployees"]}|{dr["TotalRoutes"]}";
                    // Format: TotalStudents|TotalMaleStudents|TotalFemaleStudents|TotalEmployees|ActiveEmployees|InactiveEmployees|TotalMaleEmployees|TotalFemaleEmployees|TeachingEmployees|NonTeachingEmployees|TotalRoutes

                    response.Message = "ok";
                    response.Status = 1;
                }
                else
                {
                    response.ResponseData = "0|0|0|0|0|0|0|0|0|0|0";
                    // Matching number of placeholders (11 values now)
                }

                return response;
            }
            #endregion


            catch (Exception ex)
            {
                #region Exception Handling
                response.Status = -1;
                response.IsSuccess = false;
                response.ResponseData = "0|0|0|0|0|0|0|0|0|0|0";
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ClassWisStudentsRollForDashBoard(string session, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter param = new SqlParameter("@LiveSession", session);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "CWS_RollForDashBoardAPI", param);
                #endregion

                #region Map Data to DTO
                List<ClassWiseStudentsRollDTO> studentList = new List<ClassWiseStudentsRollDTO>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var studentRoll = new ClassWiseStudentsRollDTO
                        {
                            ClassID = dr["ClassID"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            TotalStudents = dr["TotalStudents"].ToString(),
                            MaleStudents = dr["MaleStudents"].ToString(),
                            FemaleStudents = dr["FemaleStudents"].ToString(),
                            PresentTotal = dr["PresentTotal"].ToString(),
                            AbsentTotal = dr["AbsentTotal"].ToString(),
                            Leave = dr["LeaveTotal"].ToString()
                        };

                        studentList.Add(studentRoll);
                    }
                }
                #endregion

                #region Set Response
                if (studentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> TotalStudentsRollForDashBoardOnDate(string session, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Parse Input
                string[] getdat = session.Split(',');

                if (getdat.Length != 2)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid session format. Expected format: 'session,date'";
                    return response;
                }

                // Trim quotes and slashes from input values
                string liveSession = getdat[0].Trim().Trim('\"', '\\');
                string dateString = getdat[1].Trim().Trim('\"', '\\');

                // Parse the date using expected format
                if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid date format. Expected format: yyyy-MM-dd";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@LiveSession", liveSession),
            new SqlParameter("@date", parsedDate)
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "TotalStudentsRollForDashBoardAPIdat", param);
                #endregion

                #region Process Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var table = ds.Tables[0];
                    var list = new List<Dictionary<string, object>>();

                    foreach (DataRow row in table.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            dict[col.ColumnName] = row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }
                else
                {
                    response.ResponseData = "0|0|0"; // Optional default when no records found
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = "0|0|0";
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SectionWisStudentsRollWithAttendanceForDashBoard(string classID, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameter
                SqlParameter param = new SqlParameter("@CIDIn", classID);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "SWS_RollAndAttendanceForDashBoardAPI", param);
                #endregion

                #region Map Data to DTO
                List<ClassWiseStudentsRollDTO> sdL = new List<ClassWiseStudentsRollDTO>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        ClassWiseStudentsRollDTO Cr = new ClassWiseStudentsRollDTO
                        {
                            ClassID = dr["SecID"].ToString(),        // Section ID
                            SectionName = dr["ClassName"].ToString(),  // Section Name
                            TotalStudents = dr["TotalStudents"].ToString(),
                            MaleStudents = dr["MaleStudents"].ToString(),
                            FemaleStudents = dr["FemaleStudents"].ToString(),
                            PresentTotal = dr["PresentTotal"].ToString(),
                            AbsentTotal = dr["AbsentTotal"].ToString(),
                            Leave = dr["Leave"].ToString()
                        };

                        sdL.Add(Cr);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = sdL;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.ToString();
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classSectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetBoardNoWithDate(string classSectionId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Parse Parameters
                string[] data = classSectionId.Split(',');
                if (data.Length != 2)
                {
                    response.Message = "Invalid parameter. Required: ClassID,SectionID";
                    return response;
                }

                string classId = data[0];
                string sectionId = data[1];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Define SQL and Parameters
                SqlParameter[] parameters = {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", sectionId)
        };

                string sql = @"
SELECT 
    SI.StudentInfoID,
    SI.StudentId,
    S.StudentName,
    SI.RollNo,
    SI.PrePrimaryBoardNo,
    SI.PrePrimaryDate,
    SI.PrimaryBoardNo,
    SI.PrimaryDate,
    SI.MiddleBoardNo,
    SI.MiddleDate,
    SI.HighBoardNo,
    SI.HighDate,
    SI.HigherBoardNo,
    SI.HigherDate,
    SI.Current_Session
FROM
    StudentInfo SI
INNER JOIN
    Students S ON SI.StudentId = S.StudentID
WHERE
    SI.ClassID = @ClassID AND SI.SectionID = @SectionID
ORDER BY RollNo";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);
                #endregion

                #region Map Data to DTO
                List<StudentDTO> studentList = new List<StudentDTO>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO
                    {
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        StudentID = dr["StudentId"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        RollNo = dr["RollNo"].ToString(),
                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"].ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"].ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"].ToString(),
                        HighBoardNo = dr["HighBoardNo"].ToString(),
                        HigherBoardNo = dr["HigherBoardNo"].ToString(),
                        PrePrimaryDate = string.IsNullOrEmpty(dr["PrePrimaryDate"].ToString()) ? "" : Convert.ToDateTime(dr["PrePrimaryDate"]).ToString("yyyy-MM-dd"),
                        PrimaryDate = string.IsNullOrEmpty(dr["PrimaryDate"].ToString()) ? "" : Convert.ToDateTime(dr["PrimaryDate"]).ToString("yyyy-MM-dd"),
                        MiddleDate = string.IsNullOrEmpty(dr["MiddleDate"].ToString()) ? "" : Convert.ToDateTime(dr["MiddleDate"]).ToString("yyyy-MM-dd"),
                        HighDate = string.IsNullOrEmpty(dr["HighDate"].ToString()) ? "" : Convert.ToDateTime(dr["HighDate"]).ToString("yyyy-MM-dd"),
                        HigherDate = string.IsNullOrEmpty(dr["HigherDate"].ToString()) ? "" : Convert.ToDateTime(dr["HigherDate"]).ToString("yyyy-MM-dd"),
                    };

                    studentList.Add(sd);
                }

                if (studentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = studentList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel> GetNextAdmissionNoAsync(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "No Admission Number Found!",
                ResponseData = null
            };
            #endregion

            #region SQL Query
            string query = "SELECT TOP 1 AdmissionNo FROM Students ORDER BY StudentID DESC";
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Process Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string lastAdmissionNo = ds.Tables[0].Rows[0]["AdmissionNo"]?.ToString();
                    string nextAdmissionNo = null;

                    if (!string.IsNullOrEmpty(lastAdmissionNo))
                    {
                        var match = Regex.Match(lastAdmissionNo, @"(\d+)$");
                        if (match.Success)
                        {
                            string numberPart = match.Groups[1].Value;
                            long nextNumber = long.Parse(numberPart) + 1;
                            string prefix = lastAdmissionNo.Substring(0, lastAdmissionNo.Length - numberPart.Length);

                            nextAdmissionNo = prefix + nextNumber.ToString(new string('0', numberPart.Length));
                        }
                        else
                        {
                            nextAdmissionNo = lastAdmissionNo + "1"; // fallback
                        }

                        response.IsSuccess = true;
                        response.Status = 1;
                        response.Message = "Next Admission Number Generated Successfully!";
                        response.ResponseData = nextAdmissionNo;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetNextAdmissionNoAsync", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error Occurred: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AttendanceDashboardForDate(string session, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                SqlParameter param = new SqlParameter("@LiveSession", session);

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "AttendanceDashboardAPI",
                    param
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    response.ResponseData = $"{dr["PresentToday"]}|{dr["AbsentToday"]}|{dr["LeaveToday"]}|{dr["HalfDayToday"]}";
                    response.Message = "ok";
                    response.Status = 1;
                }
                else
                {
                    response.ResponseData = "0|0|0|0";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.IsSuccess = false;
                response.ResponseData = "0|0|0|0";
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>

        public async Task<ResponseModel> UpdateStudentAsync(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel();
            #endregion

            #region Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
                return response;
            }
            #endregion

            #region Input Validation
            if (string.IsNullOrEmpty(request.StudentID) || string.IsNullOrEmpty(request.StudentInfoID))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "StudentID and StudentInfoID are required fields.";
                return response;
            }

            bool isDuplicate = await IsAdmissionNoDuplicateAsync(request, clientId);
            if (isDuplicate)
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Duplicate admission number.";
                return response;
            }
            #endregion

            #region SQL Parameters
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@StudentID", request.StudentID ?? string.Empty),
        new SqlParameter("@StudentInfoID", request.StudentInfoID ?? string.Empty),
        new SqlParameter("@StudentName", request.StudentName ?? string.Empty),
        new SqlParameter("@DOB", (object?)request.DOB ?? DBNull.Value),
        new SqlParameter("@DOA", (object?)request.DOA ?? DBNull.Value),
        new SqlParameter("@AdmissionNo", request.AdmissionNo ?? string.Empty),
        new SqlParameter("@Gender", request.Gender ?? string.Empty),
        new SqlParameter("@DistrictID", request.DistrictID ?? string.Empty),
        new SqlParameter("@DistrictName", request.DistrictName ?? string.Empty),
        new SqlParameter("@Aadhaar", request.Aadhaar ?? string.Empty),
        new SqlParameter("@StudentCatID", request.StudentCatID ?? string.Empty),
        new SqlParameter("@StudentCatName", request.StudentCatName ?? string.Empty),
        new SqlParameter("@PhotoPath", string.Empty),
       // new SqlParameter("@PhotoPath", request.PhotoPath ?? string.Empty),
        new SqlParameter("@ClassID", request.ClassID ?? string.Empty),
        new SqlParameter("@SectionID", request.SectionID ?? string.Empty),
        new SqlParameter("@Session", request.Session ?? string.Empty),
        new SqlParameter("@SessionOfAdmission", request.SessionOfAdmission ?? string.Empty),
        new SqlParameter("@RollNo", request.RollNo ?? string.Empty),
        new SqlParameter("@PresentAddress", request.PresentAddress ?? string.Empty),
        new SqlParameter("@PermanentAddress", request.PermanentAddress ?? string.Empty),
        new SqlParameter("@FatherName", request.FatherName ?? string.Empty),
        new SqlParameter("@MontherName", request.MontherName ?? string.Empty),
        new SqlParameter("@MobileFather", request.MobileFather ?? string.Empty),
        new SqlParameter("@MobileMother", request.MobileMother ?? string.Empty),
        new SqlParameter("@LandLineNo", request.LandLineNo ?? string.Empty),
        new SqlParameter("@FatherQualification", request.FatherQualification ?? string.Empty),
        new SqlParameter("@MotherQualification", request.MotherQualification ?? string.Empty),
        new SqlParameter("@FatherIcome", request.FatherIcome ?? string.Empty),
        new SqlParameter("@MotherIcome", request.MotherIcome ?? string.Empty),
        new SqlParameter("@FatherOccupation", request.FatherOccupation ?? string.Empty),
        new SqlParameter("@MotherOccupation", request.MotherOccupation ?? string.Empty),
        new SqlParameter("@FatherPhoto", request.FatherPhoto),
        new SqlParameter("@MotherPhoto", request.MotherPhoto),
        new SqlParameter("@Remarks", request.Remarks ?? string.Empty),
        new SqlParameter("@SEmail", request.SEmail ?? string.Empty),
        new SqlParameter("@GuardianName", request.GuardianName ?? string.Empty),
        new SqlParameter("@GuardianPhoneNo", request.GuardianPhoneNo ?? string.Empty),
        new SqlParameter("@GuardianQualification", request.GuardianQualification ?? string.Empty),
        new SqlParameter("@GuardialAccupation", request.GuardialAccupation ?? string.Empty),
        new SqlParameter("@Religion", request.Religion ?? string.Empty),
        new SqlParameter("@MotherTounge", request.MotherTounge ?? string.Empty),
        new SqlParameter("@BankName", request.BankName ?? string.Empty),
        new SqlParameter("@AccountNo", request.AccountNo ?? string.Empty),
        new SqlParameter("@AccountType", request.AccountType ?? string.Empty),
        new SqlParameter("@IFCCode", request.IFCCode ?? string.Empty),
        new SqlParameter("@BPLStatus", (object?)request.BPLStatus ?? DBNull.Value),
        new SqlParameter("@SDisability", request.SDisability ?? string.Empty),
        new SqlParameter("@APAARStudentID ", request.Apaarid ?? string.Empty),
        new SqlParameter("@UpdatedType", (object?)request.UpdateType ?? DBNull.Value)
    };
            #endregion

            try
            {
                #region Execute Update
                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateStudentAPI",
                    parameters.ToArray());

                response.IsSuccess = result > 0;
                response.Status = response.IsSuccess ? 1 : 0;
                response.Message = response.IsSuccess ? "Student updated successfully." : "Failed to update student.";
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "UpdateStudentAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while updating the student.";
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> IsAdmissionNoDuplicateAsync(UpdateStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel();

            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);


            if (string.IsNullOrEmpty(connectionString))
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "Invalid client ID.";
            }
            string query = @"
SELECT 1 
FROM Students 
WHERE AdmissionNo = @AdmissionNo AND StudentID <> @StudentID
";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@AdmissionNo", request.AdmissionNo ?? string.Empty),
        new SqlParameter("@StudentID", request.StudentID ?? string.Empty)
    };

            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    SQLHelperCore.Connect,
                    CommandType.Text,
                    query,
                    parameters.ToArray());

                // If any rows are returned, it's a duplicate
                return ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0;
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "IsAdmissionNoDuplicateAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace}");
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateParentDetail(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Get StudentID by StudentInfoID
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                int studentId = Convert.ToInt32(ds.Tables[0].Rows[0]["StudentID"]);
                #endregion

                #region Default UpdateType
                if (request.UpdateType == null || request.UpdateType == 0)
                    request.UpdateType = 1;
                #endregion

                #region Prepare Update Parameters
                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID ?? string.Empty),
            new SqlParameter("@FatherName", request.FatherName ?? string.Empty),
            new SqlParameter("@FatherMobile", request.MobileFather ?? string.Empty),
            new SqlParameter("@FatherQualification", request.FatherQualification ?? string.Empty),
            new SqlParameter("@FatherOccupation", request.FatherOccupation ?? string.Empty),
            new SqlParameter("@FatherAadhaar", request.FAdhaar ?? string.Empty),
            new SqlParameter("@MotherName", request.MontherName ?? string.Empty),
            new SqlParameter("@MotherMobile", request.MobileMother ?? string.Empty),
            new SqlParameter("@MotherQualification", request.MotherQualification ?? string.Empty),
            new SqlParameter("@MotherOccupation", request.MotherOccupation ?? string.Empty),
            new SqlParameter("@MotherAadhaar", request.MAdhaar ?? string.Empty),
            new SqlParameter("@GuardianName", request.GuardianName ?? string.Empty),
            new SqlParameter("@GuardialAccupation", request.GuardialAccupation ?? string.Empty),
            new SqlParameter("@GuardianPhoneNo", request.GuardianPhoneNo ?? string.Empty),
            new SqlParameter("@GuardianQualification", request.GuardianQualification ?? string.Empty),
            new SqlParameter("@UpdatedBy", request.UpdatedBy ?? (object)DBNull.Value)
        };
                #endregion

                #region Execute Update
                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateStudentParentDetailsAPI",
                    sqlParam);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Parent Details Updated Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to Update Student Parent Details!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateAddressDetail(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Default UpdateType
                if (request.UpdateType == null || request.UpdateType == 0)
                    request.UpdateType = 1;
                #endregion

                #region Get StudentID by StudentInfoID
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                int studentId = Convert.ToInt32(ds.Tables[0].Rows[0]["StudentID"]);
                #endregion

                #region Prepare Update Parameters
                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID ?? string.Empty),
    new SqlParameter("@PresentAddress", request.PresentAddress ?? (object)DBNull.Value),
    new SqlParameter("@PermanentAddress", request.PermanentAddress ?? (object)DBNull.Value),
    new SqlParameter("@PinCode", request.PinCode ?? (object)DBNull.Value),
    new SqlParameter("@DistrictName", request.DistrictName ?? (object)DBNull.Value),
    new SqlParameter("@PrPincode", !string.IsNullOrWhiteSpace(request.PrPincode) ? (object)request.PrPincode : DBNull.Value),
    new SqlParameter("@PrDistrict", !string.IsNullOrWhiteSpace(request.PrDistrict) ? (object)request.PrDistrict : DBNull.Value),
    new SqlParameter("@PrDistrictID", !string.IsNullOrWhiteSpace(request.PrDistrictID) ? (object)request.PrDistrictID : DBNull.Value),
    new SqlParameter("@DistrictID", !string.IsNullOrWhiteSpace(request.DistrictID) ? (object)request.DistrictID : DBNull.Value),
    new SqlParameter("@StateName", !string.IsNullOrWhiteSpace(request.StateName) ? (object)request.StateName : DBNull.Value),
    new SqlParameter("@StateID", request.StateID != null ? (object)request.StateID : DBNull.Value),
    new SqlParameter("@PrStateID", request.PrStateID != null ? (object)request.PrStateID : DBNull.Value),
    new SqlParameter("@PrStateName", !string.IsNullOrWhiteSpace(request.PrStateName) ? (object)request.PrStateName : DBNull.Value),
    new SqlParameter("@Tehsil", !string.IsNullOrWhiteSpace(request.Tehsil) ? (object)request.Tehsil : DBNull.Value),
     new SqlParameter("@UpdatedBy", request.UpdatedBy ?? (object)DBNull.Value),
    new SqlParameter("@TehsilPer", !string.IsNullOrWhiteSpace(request.TehsilPer) ? (object)request.TehsilPer : DBNull.Value)
        };
                #endregion

                #region Execute Update
                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateStudentAddressAPINew",
                    sqlParam);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Address Details Updated Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to Update Student Address Details!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion

            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdatePersonalDetail(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Get StudentID from StudentInfo
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];
                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                #endregion

                #region Prepare SQL Parameters
                string updatedBy = request.UpdatedBy ?? "System";
                string endpoint = "update-personal-details";

                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID ?? string.Empty),
            new SqlParameter("@StudentName", request.StudentName ?? string.Empty),
            new SqlParameter("@Aadhaar", request.Aadhaar ?? string.Empty),
            new SqlParameter("@Gender", request.Gender ?? string.Empty),
            new SqlParameter("@PEN", request.PEN ?? string.Empty),
            new SqlParameter("@Weight", string.IsNullOrWhiteSpace(request.WEIGHT) ? (object)DBNull.Value : request.WEIGHT),
            new SqlParameter("@Height", string.IsNullOrWhiteSpace(request.Height) ? (object)DBNull.Value : request.Height),
            new SqlParameter("@StudentCatID",
                !string.IsNullOrWhiteSpace(request.StudentCatID) && int.TryParse(request.StudentCatID, out int catId)
                    ? (object)catId
                    : DBNull.Value),
            new SqlParameter("@StudentCatName", request.StudentCatName ?? string.Empty),
            new SqlParameter("@RollNo", request.RollNo ?? string.Empty),
            new SqlParameter("@PhoneNo", string.IsNullOrWhiteSpace(request.LandLineNo) ? (object)DBNull.Value : request.LandLineNo),
            new SqlParameter("@HID", request.HID ?? string.Empty),
            new SqlParameter("@HouseName", request.HouseName ?? string.Empty),
            new SqlParameter("@BloodGroup", request.BloodGroup ?? "Unknown"),
            new SqlParameter("@Religion", request.Religion ?? string.Empty),
            new SqlParameter("@MotherTounge", request.MotherTounge ?? string.Empty),
            new SqlParameter("@BankName", request.BankName ?? string.Empty),
            new SqlParameter("@AccountNo", request.AccountNo ?? string.Empty),
            new SqlParameter("@AccountType", request.AccountType ?? string.Empty),
            new SqlParameter("@IFCCode", request.IFCCode ?? string.Empty),
            new SqlParameter("@BPLStatus", (object?)request.BPLStatus ?? DBNull.Value),
            new SqlParameter("@SDisability", request.SDisability ?? string.Empty),
            new SqlParameter("@Apaarid", request.Apaarid ?? string.Empty),
            new SqlParameter("@NameAsPerAadhaar", request.NameAsPerAadhaar ?? string.Empty),
            new SqlParameter("@SEmail", request.SEmail ?? string.Empty),
            new SqlParameter("@BPLCategory", request.BPLCategory ?? string.Empty),
            new SqlParameter("@CWSNStatus", request.CWSNStatus ?? (object)DBNull.Value),
            new SqlParameter("@Category", request.Category ?? string.Empty),
             new SqlParameter("@Remarks", request.Remarks ?? string.Empty),
            //new SqlParameter("@PhotoPath", studentPhotoPath ?? string.Empty),
            new SqlParameter("@UpdatedBy", updatedBy),
            new SqlParameter("@Endpoint", endpoint)
        };
                #endregion

                #region Execute Stored Procedure
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateStudentPersonalDetailsAPINew",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Personal Details Updated Successfully";
                }
                else
                {
                    response.Message = "Failed to Update Student Personal Details!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateStudentRollNo(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Get StudentID from StudentInfo
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];

                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                request.StudentID = studentId.ToString(); // optional assignment back
                #endregion

                #region Prepare Parameters and Execute Update
                string updatedBy = request.UpdatedBy ?? "System";
                string endpoint = "update-rollno";

                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@RollNo", request.RollNo ?? string.Empty),
            new SqlParameter("@UpdatedBy", updatedBy),
            new SqlParameter("@Endpoint", endpoint)
        };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateRollNoAPINew",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student RollNo Updated Successfully";
                }
                else
                {
                    response.Message = "Failed to Update Student RollNo";
                }
                #endregion
            }
            catch (Exception e)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + e.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// 
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateBoardNo(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Get StudentID from StudentInfo
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];
                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                request.StudentID = studentId.ToString();
                #endregion

                #region Prepare Parameters and Execute Update
                SqlParameter[] sqlParam = { new SqlParameter("@StudentInfoID", request.StudentInfoID),
                    new SqlParameter("@PrePrimaryBoardNo", request.PrePrimaryBoardNo ?? (object)DBNull.Value),
                    new SqlParameter("@PrePrimaryDate", request.PrePrimaryDate ?? (object)DBNull.Value),
                    new SqlParameter("@PrimaryBoardNo", request.PrimaryBoardNo ?? (object)DBNull.Value),
                    new SqlParameter("@PrimaryDate", request.PrimaryDate ?? (object)DBNull.Value),
                    new SqlParameter("@MiddleBoardNo", request.MiddleBoardNo ?? (object)DBNull.Value),
                    new SqlParameter("@MiddleDate", request.MiddleDate ?? (object)DBNull.Value),
                    new SqlParameter("@HighBoardNo", request.HighBoardNo ?? (object)DBNull.Value),
                    new SqlParameter("@HighDate", request.HighDate ?? (object)DBNull.Value),
                    new SqlParameter("@HigherBoardNo", request.HigherBoardNo ?? (object)DBNull.Value),
                    new SqlParameter("@HigherDate", request.HigherDate ?? (object)DBNull.Value)
                };


                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateBoardNoAPI",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Board No Updated Successfully";
                }
                else
                {
                    response.Message = "Failed to Update Student Board No";
                }
                #endregion
            }
            catch (Exception e)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + e.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateDOB(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Retrieve StudentID
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];

                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                request.StudentID = studentId.ToString();
                #endregion

                #region Prepare Parameters and Execute Update
                string endpoint = "UpdateDOB"; // Optional: dynamically capture API path or set statically
                string updatedBy = request.UpdatedBy ?? "System";

                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@DOB", request.DOB ?? (object)DBNull.Value),
            new SqlParameter("@DOBASPERADHAAR", request.DOBAsPerAadhaar ?? (object)DBNull.Value),
            new SqlParameter("@UpdatedBy", updatedBy),
            new SqlParameter("@Endpoint", endpoint)
        };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateDOBAndDObAsperAdhaarNew",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student D-O-B Updated Successfully";
                }
                else
                {
                    response.Message = "Failed to Update Student DOB";
                }
                #endregion
            }
            catch (Exception e)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + e.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSection(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Section Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Retrieve StudentID
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];

                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                request.StudentID = studentId.ToString();
                #endregion

                #region Update Section
                string updatedBy = request.UpdatedBy ?? "System";
                string endpoint = "update-section";

                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@SectionID", request.SectionID),
            new SqlParameter("@UpdatedBy", updatedBy),
            new SqlParameter("@Endpoint", endpoint)
        };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateSectionIDInAlltablesAPINew",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Section Updated Successfully.";
                }
                else
                {
                    response.Message = "Failed to Update Student Section.";
                }
                #endregion
            }
            catch (Exception e)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + e.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateClass(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Retrieve StudentID
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];

                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                request.StudentID = studentId.ToString();
                #endregion

                #region Prepare Parameters and Execute Update
                string updatedBy = request.UpdatedBy ?? "System";
                string endpoint = "UpdateClassAndSectionAPI"; // Or get from context dynamically

                SqlParameter[] sqlParam = {
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@SectionID", request.SectionID),
            new SqlParameter("@NewClassID", request.ClassID),
            new SqlParameter("@RollNo", request.RollNo ?? string.Empty),
            new SqlParameter("@UpdatedBy", updatedBy),
            new SqlParameter("@Endpoint", endpoint)
        };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateClassAndSectionAPINew",
                    sqlParam
                );

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Class and Section Updated Successfully";
                }
                else
                {
                    response.Message = "Failed to Update Student Class and Section";
                }
                #endregion
            }
            catch (Exception e)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + e.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDischargedValue"></param>
        /// <returns></returns>
        public static string DischargeStatus(string isDischargedValue)
        {
            try
            {
                if (isDischargedValue == "0")
                {
                    return "Active";
                }
                else
                    if (isDischargedValue == "1")
                {
                    return "Discharged";
                }
                else
                {
                    return "undefined";
                }
            }
            catch (Exception er)
            {
                return "undefinedEr";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DischargeStudent(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Validate Parameters
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.StudentInfoID) ||
                    string.IsNullOrEmpty(request.DDate))
                {
                    response.Message = "Some Parameters missing (StudentID, StudentInfoID, Discharge Date)!";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = {
            new SqlParameter("@StudentID", request.StudentID),
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@StudentName", request.StudentName ?? (object)DBNull.Value),
            new SqlParameter("@DDate", request.DDate),
            new SqlParameter("@DBy", request.DBy ?? (object)DBNull.Value),
            new SqlParameter("@DRemarks", request.DRemarks ?? (object)DBNull.Value),
            new SqlParameter("@DSession", request.DSession ?? (object)DBNull.Value)
        };
                #endregion

                #region Get Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                #region Update Students Table
                string updateStudentsSql = "UPDATE Students SET Discharged = 'true' WHERE StudentID = @StudentID";
                int result = await Task.Run(() =>
                    SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateStudentsSql, sqlParams)
                );
                #endregion

                #region Update StudentInfo Table
                string updateStudentInfoSql = @"
            UPDATE StudentInfo 
            SET IsDischarged = 1,
                DBy = @DBy,
                DDate = @DDate,
                DSession = @DSession,
                DRemarks = @DRemarks
            WHERE StudentInfoID = @StudentInfoID";

                result += await Task.Run(() =>
                    SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateStudentInfoSql, sqlParams)
                );
                #endregion

                #region Finalize Response
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = (result == 1)
                        ? "Student Partially Discharged"
                        : "Student Discharged Successfully";
                }
                else
                {
                    response.Message = "Student Already Discharged!";
                }
                #endregion
            }
            #region Exception Handling
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }
            #endregion

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DischargeStudentForIntValue(UpdateStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Student Not Updated!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.StudentInfoID) ||
                    string.IsNullOrEmpty(request.DDate))
                {
                    response.Message = "Missing parameters: StudentID, StudentInfoID, or Discharge Date.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = {
                new SqlParameter("@StudentID", request.StudentID),
                new SqlParameter("@StudentInfoID", request.StudentInfoID),
                new SqlParameter("@StudentName", request.StudentName ?? (object)DBNull.Value),
                new SqlParameter("@DDate", request.DDate),
                new SqlParameter("@DBy", request.DBy ?? (object)DBNull.Value),
                new SqlParameter("@DRemarks", request.DRemarks ?? (object)DBNull.Value),
                new SqlParameter("@DSession", request.DSession ?? (object)DBNull.Value)
            };
                #endregion

                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                #region Update Students Table
                string updateStudentsSql = "UPDATE Students SET Discharged = 'true' WHERE StudentID = @StudentID";
                int result = await Task.Run(() =>
                    SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateStudentsSql, sqlParams)
                );
                #endregion

                #region Update StudentInfo Table
                string updateStudentInfoSql = @"
                UPDATE StudentInfo 
                SET IsDischarged = 1, 
                    DBy = @DBy, 
                    DDate = @DDate, 
                    DSession = @DSession, 
                    DRemarks = @DRemarks 
                WHERE StudentInfoID = @StudentInfoID";

                result += await Task.Run(() =>
                    SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateStudentInfoSql, sqlParams)
                );
                #endregion

                #region Finalize Response
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = (result == 1) ? "Student Partially Discharged" : "Student Discharged Successfully";
                }
                else
                {
                    response.Message = "Student Already Discharged!";
                }
                #endregion
            }
            #region Catch Block
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }
            #endregion

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> RejoinStudent(UpdateStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Updated!" };

            try
            {
                #region Validate Parameters
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.StudentInfoID) ||
                    string.IsNullOrEmpty(request.DDate))
                {
                    response.Message = "Some Parameters missing (StudentID, StudentInfoID, Discharge Date)!";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@StudentID", request.StudentID),
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@StudentName", request.StudentName ?? (object)DBNull.Value),
            new SqlParameter("@DDate", request.DDate),
            new SqlParameter("@DBy", request.DBy ?? (object)DBNull.Value),
            new SqlParameter("@DRemarks", request.DRemarks ?? (object)DBNull.Value),
            new SqlParameter("@DSession", request.DSession ?? (object)DBNull.Value),
        };
                #endregion

                #region Get Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                int result = 0;

                #region Rejoin Student in Students Table
                string sql1 = "UPDATE Students SET Discharged='false' WHERE StudentID=@StudentID";
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql1, sqlParams);
                #endregion

                #region Rejoin Student in StudentInfo Table
                string sql2 = @"UPDATE StudentInfo 
                        SET IsDischarged=0, DBy=@DBy, DDate=@DDate, DSession=@DSession, DRemarks=@DRemarks 
                        WHERE StudentInfoID=@StudentInfoID";
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql2, sqlParams);
                #endregion

                #region Final Result Check
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = result == 1 ? "Student Partially Rejoined" : "Student Rejoined Successfully";
                }
                else
                {
                    response.Message = "Student Already Rejoined!";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> RejoinStudentForIntValue(UpdateStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Updated!" };

            try
            {
                #region Validate Parameters
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.StudentInfoID) ||
                    string.IsNullOrEmpty(request.DDate))
                {
                    response.Message = "Some Parameters missing (StudentID, StudentInfoID, Discharge Date)!";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@StudentID", request.StudentID),
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@StudentName", request.StudentName ?? (object)DBNull.Value),
            new SqlParameter("@DDate", request.DDate),
            new SqlParameter("@DBy", request.DBy ?? (object)DBNull.Value),
            new SqlParameter("@DRemarks", request.DRemarks ?? (object)DBNull.Value),
            new SqlParameter("@DSession", request.DSession ?? (object)DBNull.Value),
        };
                #endregion

                #region Get Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                int result = 0;

                #region Rejoin Student in Students Table
                string sql1 = "UPDATE Students SET Discharged='false' WHERE StudentID=@StudentID";
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql1, sqlParams);
                #endregion

                #region Rejoin Student in StudentInfo Table (IsDischarged = 0 instead of false)
                string sql2 = @"UPDATE StudentInfo 
                        SET IsDischarged=0, DBy=@DBy, DDate=@DDate, DSession=@DSession, DRemarks=@DRemarks 
                        WHERE StudentInfoID=@StudentInfoID";
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql2, sqlParams);
                #endregion

                #region Final Result Check
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = result == 1 ? "Student Partially Discharged" : "Student Discharged Successfully";
                }
                else
                {
                    response.Message = "Student Already Discharged!!";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateStudentEducationAdmissionPrePrimaryEtc(UpdateStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Updated!" };

            try
            {
                #region Validate Parameters
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.StudentInfoID) ||
                    string.IsNullOrEmpty(request.SectionID) ||
                    string.IsNullOrEmpty(request.AdmissionNo))
                {
                    response.Message = "Some Parameters missing (StudentID, StudentInfoID, SectionID, AdmissionNo)!";
                    return response;
                }
                #endregion

                #region Get Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@StudentID", request.StudentID),
            new SqlParameter("@StudentInfoID", request.StudentInfoID),
            new SqlParameter("@StudentName", request.StudentName ?? (object)DBNull.Value),
            new SqlParameter("@DOB", request.DOB ?? (object)DBNull.Value),
            new SqlParameter("@DOA", request.DOA ?? (object)DBNull.Value),
            new SqlParameter("@AdmissionNo", request.AdmissionNo ?? (object)DBNull.Value),
            new SqlParameter("@Gender", request.Gender ?? (object)DBNull.Value),
            new SqlParameter("@DistrictID", request.DistrictID ?? (object)DBNull.Value),
            new SqlParameter("@Aadhaar", request.Aadhaar ?? (object)DBNull.Value),
            new SqlParameter("@StudentCatID", request.StudentCatID ?? (object)DBNull.Value),
            new SqlParameter("@PinNo", request.PinCode ?? (object)DBNull.Value),
            new SqlParameter("@PhotoPath", request.PhotoPath ?? (object)DBNull.Value),

            new SqlParameter("@ClassID", request.ClassID ?? (object)DBNull.Value),
            new SqlParameter("@SectionID", request.SectionID ?? (object)DBNull.Value),
            new SqlParameter("@Session", request.Session ?? (object)DBNull.Value),
            new SqlParameter("@SessionOfAdmission", request.SessionOfAdmission ?? (object)DBNull.Value),
            new SqlParameter("@RollNo", request.RollNo ?? (object)DBNull.Value),
            new SqlParameter("@PresentAddress", request.PresentAddress ?? (object)DBNull.Value),
            new SqlParameter("@PermanentAddress", request.PermanentAddress ?? (object)DBNull.Value),

            new SqlParameter("@FatherName", request.FatherName ?? (object)DBNull.Value),
            new SqlParameter("@MontherName", request.MontherName ?? (object)DBNull.Value),
            new SqlParameter("@MobileFather", request.MobileFather ?? (object)DBNull.Value),
            new SqlParameter("@MobileMother", request.MobileMother ?? (object)DBNull.Value),
            new SqlParameter("@LandLineNo", request.LandLineNo ?? (object)DBNull.Value),
            new SqlParameter("@FatherQualification", request.FatherQualification ?? (object)DBNull.Value),
            new SqlParameter("@MotherQualification", request.MotherQualification ?? (object)DBNull.Value),
            new SqlParameter("@FatherIcome", request.FatherIcome ?? (object)DBNull.Value),
            new SqlParameter("@MotherIcome", request.MotherIcome ?? (object)DBNull.Value),
            new SqlParameter("@FatherOccupation", request.FatherOccupation ?? (object)DBNull.Value),
            new SqlParameter("@MotherOccupation", request.MotherOccupation ?? (object)DBNull.Value),
            new SqlParameter("@FatherPhoto", request.FatherPhoto ?? (object)DBNull.Value),
            new SqlParameter("@MotherPhoto", request.MotherPhoto ?? (object)DBNull.Value),
            new SqlParameter("@Remarks", request.Remarks ?? (object)DBNull.Value),
            new SqlParameter("@SEmail", request.SEmail ?? (object)DBNull.Value),

            new SqlParameter("@UpdatedBy", "NA"),
            new SqlParameter("@UpdatedOn", "NA"),
            new SqlParameter("@UpdatedType", 4), // update board-related fields

            new SqlParameter("@GuardianName", request.GuardianName ?? (object)DBNull.Value),
            new SqlParameter("@GuardianPhoneNo", request.GuardianPhoneNo ?? (object)DBNull.Value),
            new SqlParameter("@GuardianQualification", request.GuardianQualification ?? (object)DBNull.Value),
            new SqlParameter("@GuardialAccupation", request.GuardialAccupation ?? (object)DBNull.Value),

            new SqlParameter("@DistrictName", request.DistrictName ?? (object)DBNull.Value),
            new SqlParameter("@StudentCatName", request.StudentCatName ?? (object)DBNull.Value),
        };
                #endregion



                #region Execute Update Procedure
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "UpdateStudentAPI", sqlParams);
                #endregion

                #region Result Handling
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Updated Successfully";
                }
                else
                {
                    response.Message = "Not Updated";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateStudentHeightWeightAdharNamePENEtcUDISE(UpdateStudentRequestDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Updated!" };

            try
            {

                #region Validate Parameters
                if (string.IsNullOrEmpty(request.StudentID) ||
                    string.IsNullOrEmpty(request.PEN) ||
                    string.IsNullOrEmpty(request.Height) ||
                    string.IsNullOrEmpty(request.WEIGHT) ||
                    string.IsNullOrEmpty(request.DOBAsPerAadhaar))
                {
                    response.Message = "Some Parameters missing (StudentID / PEN / HEIGHT / WEIGHT / DOBASPERADHAAR)!";
                    return response;
                }
                #endregion

                #region Get Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion
                #region Prepare SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@StudentID", request.StudentID),
            new SqlParameter("@PEN", request.PEN ?? (object)DBNull.Value),
            new SqlParameter("@Height", request.Height ?? (object)DBNull.Value),
            new SqlParameter("@WEIGHT", request.WEIGHT ?? (object)DBNull.Value),
            new SqlParameter("@NAMEASPERADHAAR", request.NameAsPerAadhaar ?? (object)DBNull.Value),
            new SqlParameter("@DOBASPERADHAAR", request.DOBAsPerAadhaar ?? (object)DBNull.Value),
            new SqlParameter("@BloodGroup", request.BloodGroup ?? (object)DBNull.Value),
        };
                #endregion

                #region Execute Update Query
                string sqlQuery = "UPDATE Students " +
                                  "SET PEN = @PEN, Height = @Height, WEIGHT = @WEIGHT, NAMEASPERADHAAR = @NAMEASPERADHAAR, " +
                                  "DOBASPERADHAAR = @DOBASPERADHAAR, BloodGroup = @BloodGroup " +
                                  "WHERE StudentID = @StudentID";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, sqlParams);
                #endregion

                #region Handle Result
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Updated Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateStudentSessionAsync(StudentSessionUpdateRequest request, string clientId)
        {
            var rp = new ResponseModel { IsSuccess = true, Message = "No Record updated!", Status = 0 };

            try
            {
                #region Get Connection String

                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                int tn = 0, invalid = 0, failed = 0;


                #region Null or Empty Check
                if (request?.Students == null || !request.Students.Any())
                {
                    return new ResponseModel
                    {
                        IsSuccess = false,
                        Status = 0,
                        Message = "The 'Students' list was null or empty in the request."
                    };
                }
                #endregion

                foreach (var nt in request.Students)
                {
                    #region Validate Input
                    if (string.IsNullOrEmpty(nt.FromSession) || string.IsNullOrEmpty(nt.ToSession) ||
                        nt.StudentID <= 0 || nt.StudentInfoID <= 0 ||
                        nt.ClassID <= 0 || nt.SectionID <= 0 || nt.RollNo <= 0)
                    {
                        invalid++;
                        continue;
                    }
                    #endregion

                    #region Prepare SQL Parameters
                    SqlParameter[] param = {
                new SqlParameter("@FromSession", nt.FromSession),
                new SqlParameter("@ToSession", nt.ToSession),
                new SqlParameter("@StudentID", nt.StudentID),
                new SqlParameter("@StudentInfoID", nt.StudentInfoID),
                new SqlParameter("@ClassID", nt.ClassID),
                new SqlParameter("@SectionID", nt.SectionID),
                new SqlParameter("@RollNo", nt.RollNo),
                new SqlParameter("@RouteID", nt.RouteID ?? 0),
                new SqlParameter("@BuStopID", nt.BuStopID ?? 0),
                new SqlParameter("@UpdatedBy", nt.UpdatedBy),
                new SqlParameter("@UpdatedOn", nt.UpdatedOn)
            };
                    #endregion

                    #region Execute Update
                    int result = await SQLHelperCore.ExecuteNonQueryAsync(
                        connectionString,
                        CommandType.StoredProcedure,
                        "UpgradeStudentSessionAPI",
                        param
                    );
                    #endregion

                    #region Tally Result
                    if (result > 0)
                        tn++;
                    else
                        failed++;
                    #endregion
                }

                #region Build Response Message
                if (tn > 0)
                {
                    rp.Status = 1;
                    if (invalid == 0)
                        rp.Message = $"{tn} students updated successfully. {failed} failed.";
                    else
                        rp.Message = $"Only {tn} students updated successfully. {invalid} had missing parameters.";
                }
                else if (invalid > 0)
                {
                    rp.Message = "Parameter values missing!";
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                return new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Message = "Error: " + ex.Message
                };
                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllSessions(string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No sessions found.",
                ResponseData = null
            };

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Define Query
                const string query = @"
            SELECT SessionID, [Session], SessionFrom, SessionTo, Current_Year
            FROM Sessions
            ORDER BY SessionID DESC";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                var dt = ds.Tables[0];
                #endregion

                #region Map Data to DTO List
                var list = new List<SessionDto>();

                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new SessionDto
                    {
                        SessionID = row["SessionID"]?.ToString(),
                        Session = row["Session"]?.ToString(),
                        SessionFrom = row["SessionFrom"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["SessionFrom"]),
                        SessionTo = row["SessionTo"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["SessionTo"]),
                        Current_Year = row["Current_Year"]?.ToString()
                    });
                }
                #endregion

                #region Prepare Response
                if (list.Count > 0)
                {
                    response.Message = "Session list fetched successfully.";
                    response.ResponseData = list;
                    response.Status = 1;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching sessions.";
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetAllSessions", ex.Message + " | " + ex.StackTrace);
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="updates"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateClassStudentRollNumbers(List<StudentRollNoUpdate> updates, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Students Not Updated!"
            };

            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }

                // Create DataTable for TVP (must match TVP order exactly)
                var updateBatch = new DataTable();
                updateBatch.Columns.Add("StudentInfoID", typeof(int));
                updateBatch.Columns.Add("RollNo", typeof(string));
                updateBatch.Columns.Add("ClassID", typeof(int));
                updateBatch.Columns.Add("SectionID", typeof(int));
                updateBatch.Columns.Add("Current_Session", typeof(string));

                // Get student details including Current_Session
                var studentIds = string.Join(",", updates.Select(u => u.StudentInfoID));
                var studentDetails = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    $"SELECT StudentInfoID, ClassID, SectionID, Current_Session FROM StudentInfo WHERE StudentInfoID IN ({studentIds})");

                if (studentDetails?.Tables.Count == 0)
                {
                    response.Message = "Failed to retrieve student details";
                    return response;
                }

                var detailsTable = studentDetails.Tables[0];

                // Map to DataTable matching TVP order
                foreach (var update in updates)
                {
                    var rows = detailsTable.Select($"StudentInfoID = {update.StudentInfoID}");
                    if (rows.Length > 0)
                    {
                        updateBatch.Rows.Add(
                            update.StudentInfoID,
                            update.RollNo ?? string.Empty,     // RollNo
                            rows[0]["ClassID"],                // ClassID
                            rows[0]["SectionID"],              // SectionID
                            rows[0]["Current_Session"]         // Current_Session
                        );
                    }
                }

                if (updateBatch.Rows.Count == 0)
                {
                    response.Message = "No matching student records found";
                    return response;
                }

                // Execute stored procedure
                var rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "UpdateStudentRollNumbersBulk",
                    new SqlParameter[]
                    {
                new SqlParameter("@Updates", updateBatch)
                    { SqlDbType = SqlDbType.Structured, TypeName = "dbo.RollNoUpdateType_v2" },
                new SqlParameter("@UpdatedBy", updates.FirstOrDefault()?.UpdatedBy ?? "System"),
                new SqlParameter("@Endpoint", "bulk-update-rollno")
                    });

                response.IsSuccess = rowsAffected > 0 || rowsAffected == -1;
                response.Status = response.IsSuccess ? 1 : 0;
                response.Message = response.IsSuccess
                    ? "Student roll numbers updated successfully"
                    : "No students were updated";
            }
            catch (SqlException sqlEx)
            {
                response.Status = -1;
                response.Message = sqlEx.Message.Contains("Duplicate RollNo")
                    ? "Duplicate roll number detected in class/section for current session"
                    : $"Database error: {sqlEx.Message}";
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.Message = $"Unexpected error: {ex.Message}";
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateOldSchoolBasicDetails(OldSchoolDetailsDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to update old school details"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Get StudentID from StudentInfo
                string getStudentIdQuery = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] getIdParam =
                {
            new SqlParameter("@StudentInfoID", request.StudentInfoID)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, getStudentIdQuery, getIdParam);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid StudentInfoID!";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];
                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID not found!";
                    return response;
                }

                int studentId = Convert.ToInt32(studentIdObj);
                #endregion

                #region Prepare SQL Parameters
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@StudentID", studentId),
            new SqlParameter("@OldSchoolName", (object?)request.OldSchoolName ?? DBNull.Value),
            new SqlParameter("@OldYear", (object?)request.OldYear ?? DBNull.Value),
            new SqlParameter("@OldLastDay", (object?)request.OldLastDay ?? DBNull.Value),
            new SqlParameter("@OldGrade", (object?)request.OldGrade ?? DBNull.Value),
            new SqlParameter("@OldMarks", (object?)request.OldMarks ?? DBNull.Value),
            new SqlParameter("@OldAcademicNo", (object?)request.OldAcademicNo ?? DBNull.Value),
        };
                #endregion

                #region SQL Query
                string query = @"
        UPDATE Students
        SET 
            OldSchoolName = @OldSchoolName,
            OldYear = @OldYear,
            OldLastDay = @OldLastDay,
            OldGrade = @OldGrade,
            OldMarks = @OldMarks,
            OldAcademicNo = @OldAcademicNo
        WHERE StudentID = @StudentID";
                #endregion

                #region Execute Query
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters.ToArray());

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Old school details updated successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "No student found to update";
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "UpdateOldSchoolBasicDetails", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
            }

            return response;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentInfoId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentIdAsync(string studentInfoId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Student ID not found",
                ResponseData = null
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid Client ID";
                    return response;
                }
                #endregion

                #region Validate input
                if (string.IsNullOrEmpty(studentInfoId))
                {
                    response.Message = "StudentInfoID is required";
                    return response;
                }
                #endregion

                #region Get StudentID from StudentInfo
                string query = "SELECT StudentID FROM StudentInfo WHERE StudentInfoID = @StudentInfoID";
                SqlParameter[] param = { new SqlParameter("@StudentInfoID", studentInfoId) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "No student found for the given StudentInfoID";
                    return response;
                }

                object studentIdObj = ds.Tables[0].Rows[0]["StudentID"];
                if (studentIdObj == null || studentIdObj == DBNull.Value)
                {
                    response.Message = "StudentID is null";
                    return response;
                }

                // ✅ Wrap into DTO
                var studentIdDto = new StudentIdDto
                {
                    StudentID = studentIdObj.ToString()
                };

                response.IsSuccess = true;
                response.Status = 1;
                response.Message = "Student ID retrieved successfully";
                response.ResponseData = studentIdDto;
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog(
                    "StudentRepository",
                    "GetStudentIdAsync",
                    ex.Message + " | " + ex.StackTrace
                );

                response.Message = "Internal server error";
                response.Error = ex.Message;
                response.Status = -1;
                return response;
            }
        }
        public async Task<ResponseModel> GetStudentAuditByDateAsync(string date, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No audit records found."
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid ClientId.";
                    return response;
                }
                #endregion

                #region Queries
                string sqlStudentsAudit = @"
SELECT 
    StudentID, AdmissionNo, StudentName, DOB, FathersName, MothersName, Gender,
    PhoneNo, PresentAddress, PerminantAddress, Discharged, PEN, Weight, Height,
    BloodGroup, SEmail, Pincode, Religion, MotherTounge, BPLStatus, SDisability, 
    BPLCategory, CWSNStatus, HouseName, HID, APAARSTUDENTID,
    NAMEASPERADHAAR, DOBASPERADHAAR, Faadhaarcard, Maadhaarcard, 
    OldSchoolName, OldYear, OldLastDay, OldGrade, OldMarks, OldAcademicNo,
    UpdatedOn, UpdatedBy, Endpoint, Category
FROM Students_Audit
WHERE CAST(UpdatedOn AS DATE) = @AuditDate
ORDER BY UpdatedOn DESC;
";

                string sqlStudentInfoAudit = @"
SELECT 
    StudentInfoID, StudentID, Current_Session, ClassID, SectionID, RollNo, RouteID,
    BusStopID, Remarks, IsDischarged, BusFee, WithdrawnOn AS DDate, DSession,
    UpdatedOn, UpdatedBy AS DBy, Endpoint AS DRemarks,
    PrePrimaryBoardNo, PrePrimaryDate,
    PrimaryBoardNo, PrimaryDate,
    MiddleBoardNo, MiddleDate,
    HighBoardNo, HighDate,
    HigherBoardNo, HigherDate
FROM StudentInfo_Audit
WHERE CAST(UpdatedOn AS DATE) = @AuditDate
ORDER BY UpdatedOn DESC;
";
                #endregion

                #region Execute Queries with separate parameters
                var studentsParameters = new SqlParameter[]
                {
            new SqlParameter("@AuditDate", SqlDbType.Date) { Value = Convert.ToDateTime(date) }
                };

                var studentInfoParameters = new SqlParameter[]
                {
            new SqlParameter("@AuditDate", SqlDbType.Date) { Value = Convert.ToDateTime(date) }
                };

                var studentsDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlStudentsAudit, studentsParameters);
                var studentInfoDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlStudentInfoAudit, studentInfoParameters);
                #endregion

                var auditList = new List<StudentDTO>();
                string SafeDate(object dbValue) => dbValue == DBNull.Value ? null : Convert.ToDateTime(dbValue).ToString("yyyy-MM-dd");

                #region Map Students_Audit
                if (studentsDs != null && studentsDs.Tables.Count > 0)
                {
                    foreach (DataRow row in studentsDs.Tables[0].Rows)
                    {
                        auditList.Add(new StudentDTO
                        {
                            StudentID = row["StudentID"]?.ToString(),
                            AdmissionNo = row["AdmissionNo"]?.ToString(),
                            StudentName = row["StudentName"]?.ToString(),
                            DOB = SafeDate(row["DOB"]),
                            FatherName = row["FathersName"]?.ToString(),
                            MontherName = row["MothersName"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            MobileFather = row["PhoneNo"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PermanentAddress = row["PerminantAddress"]?.ToString(),
                            Discharged = row["Discharged"]?.ToString(),
                            PEN = row["PEN"]?.ToString(),
                            WEIGHT = row["Weight"]?.ToString(),
                            Height = row["Height"]?.ToString(),
                            BloodGroup = row["BloodGroup"]?.ToString(),
                            SEmail = row["SEmail"]?.ToString(),
                            PinCode = row["Pincode"]?.ToString(),
                            Religion = row["Religion"]?.ToString(),
                            MotherTounge = row["MotherTounge"]?.ToString(),
                            BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
                            SDisability = row["SDisability"]?.ToString(),
                            BPLCategory = row["BPLCategory"]?.ToString(),
                            CWSNStatus = row["CWSNStatus"] != DBNull.Value ? Convert.ToBoolean(row["CWSNStatus"]) : (bool?)null,
                            HouseName = row["HouseName"]?.ToString(),
                            HID = row["HID"]?.ToString(),
                            AcademicNo = row["APAARSTUDENTID"]?.ToString(),
                            NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
                            DOBASPERADHAAR = SafeDate(row["DOBASPERADHAAR"]),
                            FAdhaar = row["Faadhaarcard"]?.ToString(),
                            MAdhaar = row["Maadhaarcard"]?.ToString(),
                            OldSchoolName = row["OldSchoolName"]?.ToString(),
                            OldYear = row["OldYear"]?.ToString(),
                            OldLastDay = SafeDate(row["OldLastDay"]),
                            OldGrade = row["OldGrade"]?.ToString(),
                            OldMarks = row["OldMarks"]?.ToString(),
                            OldAcademicNo = row["OldAcademicNo"]?.ToString(),
                            UpdatedOn = row["UpdatedOn"]?.ToString(),
                            DBy = row["UpdatedBy"]?.ToString(),
                            DRemarks = row["Endpoint"]?.ToString(),
                            Category = row["Category"]?.ToString(),
                            SourceTable = "Students_Audit"
                        });
                    }
                }
                #endregion

                #region Map StudentInfo_Audit
                if (studentInfoDs != null && studentInfoDs.Tables.Count > 0)
                {
                    foreach (DataRow row in studentInfoDs.Tables[0].Rows)
                    {
                        auditList.Add(new StudentDTO
                        {
                            StudentInfoID = row["StudentInfoID"]?.ToString(),
                            StudentID = row["StudentID"]?.ToString(),
                            Session = row["Current_Session"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SectionID"]?.ToString(),
                            RollNo = row["RollNo"]?.ToString(),
                            RouteID = row["RouteID"]?.ToString(),
                            busstopid = row["BusStopID"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            IsDischarged = row["IsDischarged"]?.ToString(),
                            BusFee = row["BusFee"]?.ToString(),
                            DDate = SafeDate(row["DDate"]),
                            DSession = row["DSession"]?.ToString(),
                            DBy = row["DBy"]?.ToString(),
                            DRemarks = row["DRemarks"]?.ToString(),
                            UpdatedOn = row["UpdatedOn"]?.ToString(),
                           // Category = row["Category"]?.ToString(),
                            PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
                            PrePrimaryDate = SafeDate(row["PrePrimaryDate"]),
                            PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
                            PrimaryDate = SafeDate(row["PrimaryDate"]),
                            MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
                            MiddleDate = SafeDate(row["MiddleDate"]),
                            HighBoardNo = row["HighBoardNo"]?.ToString(),
                            HighDate = SafeDate(row["HighDate"]),
                            HigherBoardNo = row["HigherBoardNo"]?.ToString(),
                            HigherDate = SafeDate(row["HigherDate"]),
                            SourceTable = "StudentInfo_Audit"
                        });
                    }
                }
                #endregion

                if (auditList.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Audit records fetched successfully.";
                    response.ResponseData = auditList;
                }

                return response;
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentAuditByDateAsync", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while fetching audit details.";
                response.ResponseData = null;
                response.Error = ex.Message;
                return response;
            }
        }


        //        public async Task<ResponseModel> GetStudentAuditByDateAsync(string date, string clientId)
        //        {
        //            var response = new ResponseModel
        //            {
        //                IsSuccess = true,
        //                Status = 0,
        //                Message = "No audit records found."
        //            };

        //            try
        //            {
        //                #region Get Connection String
        //                var connectionStringHelper = new ConnectionStringHelper(_configuration);
        //                string connectionString = connectionStringHelper.GetConnectionString(clientId);

        //                if (string.IsNullOrEmpty(connectionString))
        //                {
        //                    response.IsSuccess = false;
        //                    response.Status = -1;
        //                    response.Message = "Invalid ClientId.";
        //                    return response;
        //                }
        //                #endregion

        //                #region Parameters
        //                var parameters = new SqlParameter[]
        //                {
        //            new SqlParameter("@AuditDate", SqlDbType.Date) { Value = Convert.ToDateTime(date) }
        //                };
        //                #endregion

        //                #region SQL Query
        //                var sqlQuery = @"
        //SELECT 
        //    StudentID, AdmissionNo, StudentName, DOB, FathersName, MothersName, Gender,
        //    PhoneNo, PresentAddress, PerminantAddress, Discharged, PEN, Weight, Height,
        //    BloodGroup, SEmail, Pincode, Religion, MotherTounge, BPLStatus, SDisability, 
        //    BPLCategory, CWSNStatus, HouseName, HID, APAARSTUDENTID,
        //    NAMEASPERADHAAR, DOBASPERADHAAR, Faadhaarcard, Maadhaarcard, 
        //    OldSchoolName, OldYear, OldLastDay, OldGrade, OldMarks, OldAcademicNo,
        //    NULL AS StudentInfoID, NULL AS Current_Session, NULL AS ClassID, NULL AS SectionID, 
        //    NULL AS RollNo, NULL AS RouteID, NULL AS BusStopID, 
        //    NULL AS Remarks, NULL AS IsDischarged, NULL AS BusFee, NULL AS WithdrawnOn, 
        //    NULL AS DSession, UpdatedOn, UpdatedBy, Endpoint,Category,NULL AS PrePrimaryBoardNo, NULL AS PrePrimaryDate,
        //    NULL AS PrimaryBoardNo, NULL AS PrimaryDate,
        //    NULL AS MiddleBoardNo, NULL AS MiddleDate,
        //    NULL AS HighBoardNo, NULL AS HighDate,
        //    NULL AS HigherBoardNo, NULL AS HigherDate,
        //    'Students_Audit' AS SourceTable
        //FROM Students_Audit
        //WHERE CAST(UpdatedOn AS DATE) = @AuditDate

        //UNION ALL

        //SELECT
        //    StudentID, NULL AS AdmissionNo, NULL AS StudentName, NULL AS DOB, 
        //    NULL AS FathersName, NULL AS MothersName, NULL AS Gender,
        //    NULL AS PhoneNo, NULL AS PresentAddress, NULL AS PerminantAddress, NULL AS Discharged, 
        //    NULL AS PEN, NULL AS Weight, NULL AS Height,
        //    NULL AS BloodGroup, NULL AS SEmail, NULL AS Pincode, NULL AS Religion, 
        //    NULL AS MotherTounge, NULL AS BPLStatus, NULL AS SDisability, 
        //    NULL AS BPLCategory, NULL AS CWSNStatus, NULL AS HouseName, NULL AS HID, NULL AS APAARSTUDENTID,
        //    NULL AS NAMEASPERADHAAR, NULL AS DOBASPERADHAAR, NULL AS Faadhaarcard, NULL AS Maadhaarcard, 
        //    NULL AS OldSchoolName, NULL AS OldYear, NULL AS OldLastDay, NULL AS OldGrade, 
        //    NULL AS OldMarks, NULL AS OldAcademicNo,
        //    StudentInfoID, Current_Session, ClassID, SectionID, 
        //    RollNo, RouteID, BusStopID,
        //    Remarks, IsDischarged, BusFee, WithdrawnOn, DSession, UpdatedOn, UpdatedBy, Endpoint,Category, PrePrimaryBoardNo, PrePrimaryDate,
        //    PrimaryBoardNo, PrimaryDate,
        //    MiddleBoardNo, MiddleDate,
        //    HighBoardNo, HighDate,
        //    HigherBoardNo, HigherDate,
        //    'StudentInfo_Audit' AS SourceTable
        //FROM StudentInfo_Audit
        //WHERE CAST(UpdatedOn AS DATE) = @AuditDate

        //ORDER BY UpdatedOn DESC";
        //                #endregion

        //                #region Execute Query
        //                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlQuery, parameters);

        //                if (ds == null || ds.Tables.Count == 0)
        //                    return response;

        //                var dt = ds.Tables[0];
        //                #endregion

        //                #region Map to DTO
        //                var auditList = new List<StudentDTO>();

        //                string SafeDate(object dbValue) => dbValue == DBNull.Value ? null : Convert.ToDateTime(dbValue).ToString("yyyy-MM-dd");

        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    auditList.Add(new StudentDTO
        //                    {
        //                        // Students_Audit fields
        //                        StudentID = row["StudentID"]?.ToString(),
        //                        AdmissionNo = row["AdmissionNo"]?.ToString(),
        //                        StudentName = row["StudentName"]?.ToString(),
        //                        DOB = SafeDate(row["DOB"]),
        //                        FatherName = row["FathersName"]?.ToString(),
        //                        MontherName = row["MothersName"]?.ToString(),
        //                        Gender = row["Gender"]?.ToString(),
        //                        MobileFather = row["PhoneNo"]?.ToString(),
        //                        PresentAddress = row["PresentAddress"]?.ToString(),
        //                        PermanentAddress = row["PerminantAddress"]?.ToString(),
        //                        Discharged = row["Discharged"]?.ToString(),
        //                        BloodGroup = row["BloodGroup"]?.ToString(),
        //                        SEmail = row["SEmail"]?.ToString(),
        //                        PinCode = row["Pincode"]?.ToString(),
        //                        Religion = row["Religion"]?.ToString(),
        //                        MotherTounge = row["MotherTounge"]?.ToString(),
        //                        BPLStatus = row["BPLStatus"] != DBNull.Value ? Convert.ToInt32(row["BPLStatus"]) : (int?)null,
        //                        SDisability = row["SDisability"]?.ToString(),
        //                        BPLCategory = row["BPLCategory"]?.ToString(),
        //                        CWSNStatus = row["CWSNStatus"] != DBNull.Value ? Convert.ToBoolean(row["CWSNStatus"]) : (bool?)null,
        //                        HouseName = row["HouseName"]?.ToString(),
        //                        HID = row["HID"]?.ToString(),
        //                        AcademicNo = row["APAARSTUDENTID"]?.ToString(),
        //                        NameAsPerAadhaar = row["NAMEASPERADHAAR"]?.ToString(),
        //                        DOBASPERADHAAR = SafeDate(row["DOBASPERADHAAR"]),
        //                        FAdhaar = row["Faadhaarcard"]?.ToString(),
        //                        MAdhaar = row["Maadhaarcard"]?.ToString(),
        //                        OldSchoolName = row["OldSchoolName"]?.ToString(),
        //                        OldYear = row["OldYear"]?.ToString(),
        //                        OldLastDay = SafeDate(row["OldLastDay"]),
        //                        OldGrade = row["OldGrade"]?.ToString(),
        //                        OldMarks = row["OldMarks"]?.ToString(),
        //                        OldAcademicNo = row["OldAcademicNo"]?.ToString(),
        //                        Category = row["Category"]?.ToString(),
        //                        // StudentInfo_Audit fields
        //                        StudentInfoID = row["StudentInfoID"]?.ToString(),
        //                        Session = row["Current_Session"]?.ToString(),
        //                        ClassID = row["ClassID"]?.ToString(),
        //                        SectionID = row["SectionID"]?.ToString(),
        //                        RollNo = row["RollNo"]?.ToString(),
        //                        RouteID = row["RouteID"]?.ToString(),
        //                        busstopid = row["BusStopID"]?.ToString(),
        //                        Remarks = row["Remarks"]?.ToString(),
        //                        IsDischarged = row["IsDischarged"]?.ToString(),
        //                        BusFee = row["BusFee"]?.ToString(),
        //                        DDate = SafeDate(row["WithdrawnOn"]),
        //                        DSession = row["DSession"]?.ToString(),
        //                        DBy = row["UpdatedBy"]?.ToString(),
        //                        DRemarks = row["Endpoint"]?.ToString(),
        //                        PrePrimaryBoardNo = row["PrePrimaryBoardNo"]?.ToString(),
        //                        PrePrimaryDate = SafeDate(row["PrePrimaryDate"]),
        //                        PrimaryBoardNo = row["PrimaryBoardNo"]?.ToString(),
        //                        PrimaryDate = SafeDate(row["PrimaryDate"]),
        //                        MiddleBoardNo = row["MiddleBoardNo"]?.ToString(),
        //                        MiddleDate = SafeDate(row["MiddleDate"]),
        //                        HighBoardNo = row["HighBoardNo"]?.ToString(),
        //                        HighDate = SafeDate(row["HighDate"]),
        //                        HigherBoardNo = row["HigherBoardNo"]?.ToString(),
        //                        UpdatedOn = row["UpdatedOn"]?.ToString(),

        //                        SourceTable = row["SourceTable"]?.ToString()
        //                    });
        //                }
        //                #endregion

        //                if (auditList.Count > 0)
        //                {
        //                    response.IsSuccess = true;
        //                    response.Status = 1;
        //                    response.Message = "Audit records fetched successfully.";
        //                    response.ResponseData = auditList;
        //                }

        //                return response;
        //            }
        //            catch (Exception ex)
        //            {
        //                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentAuditByDateAsync", ex.ToString());
        //                response.IsSuccess = false;
        //                response.Status = -1;
        //                response.Message = "Error occurred while fetching audit details.";
        //                response.ResponseData = null;
        //                response.Error = ex.Message;
        //                return response;
        //            }
        //        }





    }
}
