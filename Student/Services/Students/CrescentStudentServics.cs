


using Microsoft.Data.SqlClient;
using Student.Repository.SQL;
using Student.Repository;
using System.Data;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Routing;
using Azure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace Student.Services.Students
{

    public class CrescentStudentServics: ICrescentStudentServics
    {
        private readonly IConfiguration _configuration;

        public CrescentStudentServics(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddNewStudentWithUID(AddStudentRequestDTO request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Student Not Added!" };
            #endregion

            try
            {
                #region Validation

                if (string.IsNullOrEmpty(request.StudentName) ||
                    string.IsNullOrEmpty(request.UID) ||
                    string.IsNullOrEmpty(request.AdmissionNo) ||
                    string.IsNullOrEmpty(request.classid) ||
                    string.IsNullOrEmpty(request.sectionid) ||
                    request.DOA == default ||
                    request.DOB == default ||
                    string.IsNullOrEmpty(request.Session) ||
                    string.IsNullOrEmpty(request.FatherName))
                {
                    response.Message = "Some params missing!";
                    return response;
                }

                #endregion

                #region Parameters

                SqlParameter[] parameters = {
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
    //new SqlParameter("@IsDischarged", request.IsDischarged ?? string.Empty),
    new SqlParameter("@DSession", request.DSession ?? string.Empty),
    new SqlParameter("@DDate", request.DDate ?? string.Empty),
    new SqlParameter("@DRemarks", request.DRemarks ?? string.Empty),
    new SqlParameter("@DBy", request.DBy ?? string.Empty),
    new SqlParameter("@UserName", request.UserName ?? string.Empty),
    new SqlParameter("@UpdatedOn", request.UpdatedOn ?? string.Empty),
  //  new SqlParameter("@Discharged", request.Discharged ?? string.Empty),
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

                #endregion

                #region Duplicate Checks

                string sqlCheckDuplicate = "SELECT COUNT(*) AS Count FROM Students WHERE StudentName=@StudentName AND FathersName=@FatherName AND MothersName=@MontherName AND PresentAddress=@PresentAddress AND PhoneNo=@MobileFather";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sqlCheckDuplicate, parameters.ToArray());
                if (ds != null && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["Count"]) > 0)
                {
                    response.Status = 0;
                    response.Message = "Duplicate Entry Not Allowed!";
                    return response;
                }

                sqlCheckDuplicate = "SELECT COUNT(*) AS Count FROM Students WHERE AdmissionNo=@AdmissionNo";
                ds = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sqlCheckDuplicate, parameters.ToArray());
                if (ds != null && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["Count"]) > 0)
                {
                    response.Status = 0;
                    response.Message = "AdmissionNo already allotted to other student!";
                    return response;
                }

                sqlCheckDuplicate = "SELECT COUNT(StudentID) AS Count FROM Students WHERE UID=@UID";
                ds = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sqlCheckDuplicate, parameters.ToArray());
                if (ds != null && ds.Tables[0].Rows.Count > 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["Count"]) > 0)
                {
                    response.Status = 0;
                    response.Message = "UID already allotted to other student!";
                    return response;
                }

                #endregion

                #region Insert Student

                int inserted = await SQLHelperCore.ExecuteNonQueryAsync(SQLHelperCore.Connect, CommandType.StoredProcedure, "AddstudentAPICrescent", parameters);
                if (inserted > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student Added Successfully";
                }

                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling

                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "AddNewStudentWithUID", ex.ToString());

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                return response;

                #endregion
            }
        }
        /// <returns></returns>
        #region Get All Active Students On SectionID Crescent School
        
        public async Task<ResponseModel> GetAllActiveStudentsOnSectionIDCrescentSchool(string sectionID, string clientId)
        {
            ResponseModel response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Get Connection String
                // Get dynamic connection string
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                List<StudentDTO> studentList = new List<StudentDTO>();
                SqlParameter param = new SqlParameter("@SectionID", sectionID);

                #region Get Current Session
                string sessionQuery = "SELECT MAX(Current_Session) FROM Sections WHERE SectionID = @SectionID";
                DataSet sessionDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, param);

                if (sessionDs.Tables.Count == 0 || sessionDs.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(sessionDs.Tables[0].Rows[0][0]?.ToString()))
                {
                    response.Message = "Invalid Section ID";
                    return response;
                }

                string currentSession = sessionDs.Tables[0].Rows[0][0].ToString();
                #endregion

                #region Student Query
                string sql = @"
        SELECT 
            Students.StudentID,
            StudentInfoID,
            Students.AdmissionNo,
            UID,
            StudentName,
            DOB,
            BOA,
            dbo.AcademicNo(@CS, Students.StudentID, SubDepartmentID) AS AcademicNo,
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
            Students.Discharged,
            Phoneno2,
            landlineno,
            WithdrawDate,
            Saadhaarcard,
            Faadhaarcard,
            Maadhaarcard,
            SEmail,
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
            StudentInfo.RouteID,
            StudentInfo.BusStopID,
            StudentInfo.IsDischarged,
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
            Students.Discharged,
            Classes.ClassName,
            Sections.SectionName,
            bs.BusStop,
            bs.BusRate,
            t.RouteName
        FROM Students
        INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
        INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
        INNER JOIN Sections ON StudentInfo.SectionID = Sections.SectionID
        LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
        LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
        WHERE StudentInfo.SectionID = @SectionID AND StudentInfo.IsDischarged = 0
        ORDER BY StudentInfo.ClassID, StudentInfo.SectionID, Rollno";
                #endregion

                SqlParameter[] parameters =
                {
            new SqlParameter("@SectionID", sectionID),
            new SqlParameter("@CS", currentSession)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StudentDTO student = new StudentDTO
                    {
                        StudentID = dr["StudentID"]?.ToString(),
                        StudentCode = dr["UID"]?.ToString(),
                        StudentInfoID = dr["StudentInfoID"]?.ToString(),
                        AdmissionNo = dr["AdmissionNo"]?.ToString(),
                        AcademicNo = dr["AcademicNo"]?.ToString(),
                        StudentName = dr["StudentName"]?.ToString(),
                        DOB = TryFormatDate(dr["DOB"]?.ToString()),
                        DOA = TryFormatDate(dr["BOA"]?.ToString()),
                        PhotoPath = dr["PhotoPath"]?.ToString(),
                        Aadhaar = dr["Saadhaarcard"]?.ToString(),
                        SEmail = dr["SEmail"]?.ToString(),
                        Gender = dr["Gender"]?.ToString(),
                        Remarks = dr["Remarks"]?.ToString(),
                        Discharged = dr["Discharged"]?.ToString(),
                        ClassID = dr["ClassID"]?.ToString(),
                        SectionID = dr["SectionID"]?.ToString(),
                        RollNo = dr["Rollno"]?.ToString(),
                        RouteID = dr["RouteID"]?.ToString(),
                        busstopid = dr["BusStopID"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        BusStopName = dr["BusStop"]?.ToString(),
                        BusFee = dr["BusRate"]?.ToString(),
                        ClassName = dr["ClassName"]?.ToString(),
                        SectionName = dr["SectionName"]?.ToString(),
                        Session = dr["Current_Session"]?.ToString(),
                        SessionOfAdmission = dr["SessionOfAdmission"]?.ToString(),
                        FatherName = dr["FathersName"]?.ToString(),
                        FatherQualification = dr["FathersQualification"]?.ToString(),
                        FatherOccupation = dr["FathersJob"]?.ToString(),
                        MobileFather = dr["PhoneNo"]?.ToString(),
                        MontherName = dr["MothersName"]?.ToString(),
                        MotherQualification = dr["MothersQualification"]?.ToString(),
                        MotherOccupation = dr["MothersJob"]?.ToString(),
                        MobileMother = dr["Phoneno2"]?.ToString(),
                        PresentAddress = dr["PresentAddress"]?.ToString(),
                        PermanentAddress = dr["PerminantAddress"]?.ToString(),
                        PinCode = dr["Pincode"]?.ToString(),
                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"]?.ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"]?.ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"]?.ToString(),
                        HighBoardNo = dr["HighBoardNo"]?.ToString(),
                        HigherBoardNo = dr["HigherBoardNo"]?.ToString(),
                        IsDischarged = dr["IsDischarged"]?.ToString(),
                        DRemarks = DischargeStatus(dr["IsDischarged"]?.ToString()),
                        DBy = dr["DBy"]?.ToString(),
                        DSession = dr["DSession"]?.ToString(),
                        DDate = TryFormatDate(dr["DDate"]?.ToString()),
                        GuardianName = dr["GuardianName"]?.ToString(),
                        GuardialAccupation = dr["GuardialAccupation"]?.ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"]?.ToString(),
                        GuardianQualification = dr["GuardianQualification"]?.ToString(),
                        DistrictID = dr["DistrictID"]?.ToString(),
                        StudentCatID = dr["StudentCatID"]?.ToString(),
                        DistrictName = dr["DistrictName"]?.ToString(),
                        StudentCatName = dr["StudentCatName"]?.ToString()
                    };

                    studentList.Add(student);
                }

                if (studentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = studentList;
                }

                return response;
            }
            catch (Exception ex)
            {
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetAllActiveStudentsOnSectionIDCrescentSchool", ex.Message + " | " + ex.StackTrace);
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllDischargedStudentsOnSectionIDCrescentSchool(string sectionID, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };
            var sdL = new List<StudentDTO>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Get Current Session
                SqlParameter param = new SqlParameter("@SectionID", sectionID);
                string sessionQuery = "SELECT MAX(Current_Session) FROM Sections WHERE SectionID = @SectionID";
                DataSet dssCS = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, param);

                if (dssCS.Tables.Count == 0 || dssCS.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(dssCS.Tables[0].Rows[0][0]?.ToString()))
                    return response;

                string cs = dssCS.Tables[0].Rows[0][0].ToString();
                #endregion

                #region Main Student Query
                string sql = @"SELECT 
    Students.StudentID, StudentInfoID, Students.AdmissionNo, UID, StudentName, DOB, BOA,
    dbo.AcademicNo(@CS, Students.StudentID, SubDepartmentID) AS AcademicNo,
    FathersName, FathersQualification, FathersJob, MothersName, MothersQualification, MothersJob,
    PresentAddress, PerminantAddress, SessionOfAdmission, PhoneNo, Gender, Students.Discharged,
    Phoneno2, landlineno, WithdrawDate, Saadhaarcard, Faadhaarcard, Maadhaarcard, SEmail,
    BloodGroup, Pincode, StudentInfo.ClassID, StudentInfo.SectionID, StudentInfo.Current_Session, Rollno,
    PhotoPath, StudentInfo.RouteID, StreamID, StudentInfo.Remarks,
    BoardNo, PrePrimaryBoardNo, PrePrimaryDate, PrimaryBoardNo, PrimaryDate,
    MiddleBoardNo, MiddleDate, HighBoardNo, HighDate, HigherBoardNo, HigherDate,
    StudentInfo.RouteID, StudentInfo.BusStopID, StudentInfo.IsDischarged, DSession, DDate, DRemarks, DBy, BusFee,
    GuardianName, GuardialAccupation, GuardianPhoneNo, GuardianQualification, DistrictID,
    StudentCatID, StudentCatName, DistrictName, Students.Discharged,
    Classes.ClassName, Sections.SectionName, bs.BusStop, bs.BusRate, t.RouteName 
FROM Students
INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
INNER JOIN Sections ON StudentInfo.SectionID = Sections.SectionID
LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
WHERE StudentInfo.SectionID = @SectionID AND StudentInfo.IsDischarged = 1
ORDER BY StudentInfo.ClassID, StudentInfo.SectionID, Rollno";

                SqlParameter[] parameters = {
            new SqlParameter("@SectionID", sectionID),
            new SqlParameter("@CS", cs)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                    return response;
                #endregion

                #region Map Data to DTO
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentCode = dr["UID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        AcademicNo = dr["AcademicNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        DOB = TryFormatDate(dr["DOB"].ToString()),
                        DOA = TryFormatDate(dr["BOA"].ToString()),
                        PhotoPath = dr["PhotoPath"].ToString(),
                        Aadhaar = dr["Saadhaarcard"].ToString(),
                        SEmail = dr["SEmail"].ToString(),
                        Gender = dr["Gender"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        Discharged = dr["Discharged"].ToString(),
                        ClassID = dr["ClassID"].ToString(),
                        SectionID = dr["SectionID"].ToString(),
                        ClassName = dr["ClassName"]?.ToString(),
                        SectionName = dr["SectionName"]?.ToString(),
                        RollNo = dr["Rollno"].ToString(),
                        RouteID = dr["RouteID"].ToString(),
                        busstopid = dr["BusStopID"].ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        BusStopName = dr["BusStop"]?.ToString(),
                        BusFee = dr["BusRate"]?.ToString(),
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
                        DRemarks = DischargeStatus(dr["IsDischarged"].ToString()),
                        DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = TryFormatDate(dr["DDate"].ToString()),
                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString()
                    };

                    sdL.Add(sd);
                }
                #endregion

                #region Final Response
                if (sdL.Count > 0)
                {
                    response.Message = "OK";
                    response.Status = 1;
                    response.ResponseData = sdL;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetAllDischargedStudentsOnSectionIDCrescentSchool", ex.Message + " | " + ex.StackTrace);
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllStudentsOnSectionIDCrescent(string sectionID, string clientId)
        {
            #region Initialize
            var response = new ResponseModel
            {
                IsSuccess = false,
                Message = "No Records Found",
                Status = 0
            };

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            var studentList = new List<StudentDTO>();
            #endregion

            try
            {
                #region SQL Query and Parameters
                var param = new SqlParameter("@SecID", sectionID);

                var sql = @"SELECT 
                        Students.StudentID, UID, StudentInfoID, Students.AdmissionNo, 
                        dbo.AcademicNo((SELECT Current_Session FROM Sections WHERE SectionID = @SecID), Students.StudentID, SubDepartmentID) AS AcademicNo,
                        StudentName, DOB, BOA, FathersName, FathersQualification, FathersJob,
                        MothersName, MothersQualification, MothersJob, PresentAddress, PerminantAddress,
                        SessionOfAdmission, PhoneNo, Gender, Discharged, Phoneno2, landlineno, WithdrawDate,
                        Saadhaarcard, Faadhaarcard, Maadhaarcard, SEmail, BloodGroup, Pincode,
                        StudentInfo.ClassID, SectionID, StudentInfo.Current_Session, Rollno, PhotoPath,
                        RouteID, StreamID, Remarks, BoardNo, PrePrimaryBoardNo, PrePrimaryDate,
                        PrimaryBoardNo, PrimaryDate, MiddleBoardNo, MiddleDate, HighBoardNo,
                        HighDate, HigherBoardNo, HigherDate, busstopid, IsDischarged, DSession,
                        DDate, DRemarks, DBy, BusFee, GuardianName, GuardialAccupation,
                        GuardianPhoneNo, GuardianQualification, DistrictID, StudentCatID,
                        StudentCatName, DistrictName
                    FROM Students
                    INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
                    INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
                    WHERE StudentInfo.SectionID = @SecID
                    ORDER BY StudentInfo.ClassID, SectionID, Rollno";
                #endregion

               

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param);
                var dt = ds.Tables[0];
                #endregion

                #region Map DataTable to DTOs
                foreach (DataRow dr in dt.Rows)
                {
                    studentList.Add(new StudentDTO
                    {
                        StudentID = dr["StudentID"]?.ToString(),
                        StudentCode = dr["UID"]?.ToString(),
                        StudentInfoID = dr["StudentInfoID"]?.ToString(),
                        AdmissionNo = dr["AdmissionNo"]?.ToString(),
                        AcademicNo = dr["AcademicNo"]?.ToString(),
                        StudentName = dr["StudentName"]?.ToString(),
                        DOB = TryFormatDate(dr["DOB"]),
                        DOA = TryFormatDate(dr["BOA"]),
                        PhotoPath = dr["PhotoPath"]?.ToString(),
                        Aadhaar = dr["Saadhaarcard"]?.ToString(),
                        SEmail = dr["SEmail"]?.ToString(),
                        Gender = dr["Gender"]?.ToString(),
                        Remarks = dr["Remarks"]?.ToString(),
                        Discharged = dr["Discharged"]?.ToString(),
                        ClassID = dr["ClassID"]?.ToString(),
                        SectionID = dr["SectionID"]?.ToString(),
                        RollNo = dr["Rollno"]?.ToString(),
                        Session = dr["Current_Session"]?.ToString(),
                        SessionOfAdmission = dr["SessionOfAdmission"]?.ToString(),
                        FatherName = dr["FathersName"]?.ToString(),
                        FatherQualification = dr["FathersQualification"]?.ToString(),
                        FatherOccupation = dr["FathersJob"]?.ToString(),
                        MobileFather = dr["PhoneNo"]?.ToString(),
                        MontherName = dr["MothersName"]?.ToString(),
                        MotherQualification = dr["MothersQualification"]?.ToString(),
                        MotherOccupation = dr["MothersJob"]?.ToString(),
                        MobileMother = dr["Phoneno2"]?.ToString(),
                        PresentAddress = dr["PresentAddress"]?.ToString(),
                        PermanentAddress = dr["PerminantAddress"]?.ToString(),
                        PinCode = dr["Pincode"]?.ToString(),
                        PrePrimaryBoardNo = dr["PrePrimaryBoardNo"]?.ToString(),
                        PrimaryBoardNo = dr["PrimaryBoardNo"]?.ToString(),
                        MiddleBoardNo = dr["MiddleBoardNo"]?.ToString(),
                        HighBoardNo = dr["HighBoardNo"]?.ToString(),
                        HigherBoardNo = dr["HigherBoardNo"]?.ToString(),
                        IsDischarged = dr["IsDischarged"]?.ToString(),
                        DRemarks = DischargeStatus(dr["IsDischarged"]?.ToString()),
                        DBy = dr["DBy"]?.ToString(),
                        DSession = dr["DSession"]?.ToString(),
                        DDate = TryFormatDate(dr["DDate"]),
                        GuardianName = dr["GuardianName"]?.ToString(),
                        GuardialAccupation = dr["GuardialAccupation"]?.ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"]?.ToString(),
                        GuardianQualification = dr["GuardianQualification"]?.ToString(),
                        DistrictID = dr["DistrictID"]?.ToString(),
                        StudentCatID = dr["StudentCatID"]?.ToString(),
                        DistrictName = dr["DistrictName"]?.ToString(),
                        StudentCatName = dr["StudentCatName"]?.ToString()
                    });
                }
                #endregion

                #region Set Response
                if (studentList.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Message = "OK";
                    response.Status = 1;
                    response.ResponseData = studentList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Logging
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetAllStudentsOnSectionIDCrescentAsync", ex.Message + " " + ex.StackTrace);
                response.Message = "An error occurred";
                response.Status = -1;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllStudentsOnClassIDCrescent(string classId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };
            var studentList = new List<StudentDTO>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Get Current Session
                SqlParameter classIdParam = new SqlParameter("@ClassID", classId);
                string sessionSql = "SELECT MAX(Current_Session) FROM Classes WHERE ClassId = @ClassID";

                DataSet sessionDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionSql, classIdParam);
                if (sessionDs.Tables[0].Rows.Count == 0 || string.IsNullOrEmpty(sessionDs.Tables[0].Rows[0][0].ToString()))
                {
                    response.Message = "Invalid Class ID";
                    return response;
                }

                string currentSession = sessionDs.Tables[0].Rows[0][0].ToString();
                #endregion

                #region Fetch Students
                string sql = @"
SELECT 
    Students.StudentID, UID, StudentInfoID, Students.AdmissionNo,
    dbo.AcademicNo(@CurrentSession, Students.StudentID, SubDepartmentID) AS AcademicNo,
    StudentName, DOB, BOA, FathersName, FathersQualification, FathersJob, 
    MothersName, MothersQualification, MothersJob, PresentAddress, PerminantAddress, 
    SessionOfAdmission, PhoneNo, Gender, Discharged, Phoneno2, landlineno, 
    WithdrawDate, Saadhaarcard, Faadhaarcard, Maadhaarcard, SEmail, BloodGroup, 
    Pincode, StudentInfo.ClassID, SectionID, StudentInfo.Current_Session, Rollno, 
    PhotoPath, RouteID, StreamID, Remarks, BoardNo, PrePrimaryBoardNo, PrePrimaryDate, 
    PrimaryBoardNo, PrimaryDate, MiddleBoardNo, MiddleDate, HighBoardNo, HighDate, 
    HigherBoardNo, HigherDate, RouteID, busstopid, IsDischarged, DSession, DDate, 
    DRemarks, DBy, BusFee, GuardianName, GuardialAccupation, GuardianPhoneNo, 
    GuardianQualification, DistrictID, StudentCatID, StudentCatName, DistrictName 
FROM 
    Students 
    INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId 
    INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId 
WHERE 
    StudentInfo.ClassID = @ClassID 
    AND Discharged = 'false' 
    AND (IsDischarged = 0 OR IsDischarged IS NULL) 
ORDER BY 
    StudentInfo.ClassID, SectionID, Rollno";

                SqlParameter[] parameters = {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@CurrentSession", currentSession)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);
                #endregion

                #region Map Data
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentCode = dr["UID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        AcademicNo = dr["AcademicNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        DOB = Convert.ToDateTime(string.IsNullOrEmpty(dr["DOB"].ToString()) ? DateTime.Now.ToString() : dr["DOB"].ToString()).ToString("dd-MM-yyyy"),
                        DOA = Convert.ToDateTime(string.IsNullOrEmpty(dr["BOA"].ToString()) ? DateTime.Now.ToString() : dr["BOA"].ToString()).ToString("dd-MM-yyyy"),
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
                        DRemarks = DischargeStatus(dr["IsDischarged"].ToString()),
                        DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = string.IsNullOrEmpty(dr["DDate"].ToString()) ? null : Convert.ToDateTime(dr["DDate"]).ToString("dd-MM-yyyy"),

                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString()
                    };

                    studentList.Add(sd);
                }
                #endregion

                #region Return Response
                if (studentList.Count > 0)
                {
                    response.Message = "OK";
                    response.Status = 1;
                    response.ResponseData = studentList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                Student.Repository.Error.ErrorBLL.CreateErrorLog(
                    "StudentServices",
                    "GetAllStudentsOnClassIDCrescentAsync",
                    ex.Message + " | " + ex.StackTrace
                );
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
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetInvalidDischargeListOnClassIDCrescent(string classId, string clientId)
        {
            #region Initialize and Prepare
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };
            var studentList = new List<StudentDTO>();

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            try
            {
                SqlParameter param = new SqlParameter("@ClassID", classId);

                #endregion

                #region Get Current Session
                string sessionSql = "SELECT MAX(Current_Session) FROM Classes WHERE ClassId = @ClassID";
                DataSet dssCS = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionSql, param);

                string currentSession = "";
                if (dssCS.Tables.Count > 0 && dssCS.Tables[0].Rows.Count > 0)
                    currentSession = dssCS.Tables[0].Rows[0][0]?.ToString() ?? "";
                #endregion

                #region Query Student Records
                string sql = @"
SELECT 
    Students.StudentID,
    UID,
    StudentInfoID,
    Students.AdmissionNo,
    dbo.AcademicNo(@CurrentSession, Students.StudentID, SubDepartmentID) AS AcademicNo,
    StudentName,
    DOB,
    BOA,
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
    Pincode,
    StudentInfo.ClassID,
    SectionID,
    StudentInfo.Current_Session,
    Rollno,
    PhotoPath,
    RouteID,
    StreamID,
    Remarks,
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
    RouteID,
    busstopid,
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
    Discharged
FROM 
    Students
    INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId
    INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId
WHERE
    IsDischarged IS NULL AND
    StudentInfo.ClassID = @ClassID
ORDER BY
    StudentInfo.ClassID, SectionID, Rollno";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@CurrentSession", currentSession)
                };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);
                #endregion

                #region Map StudentDTO List
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO();

                    sd.StudentID = dr["StudentID"].ToString();
                    sd.StudentCode = dr["UID"].ToString();
                    sd.StudentInfoID = dr["StudentInfoID"].ToString();
                    sd.AdmissionNo = dr["AdmissionNo"].ToString();
                    sd.AcademicNo = dr["AcademicNo"].ToString();
                    sd.StudentName = dr["StudentName"].ToString();
                    sd.DOB = Convert.ToDateTime(string.IsNullOrEmpty(dr["DOB"].ToString()) ? DateTime.Now.ToString() : dr["DOB"].ToString()).ToString("dd-MM-yyyy");
                    sd.DOA = Convert.ToDateTime(string.IsNullOrEmpty(dr["BOA"].ToString()) ? DateTime.Now.ToString() : dr["BOA"].ToString()).ToString("dd-MM-yyyy");
                    sd.PhotoPath = dr["PhotoPath"].ToString();
                    sd.Aadhaar = dr["Saadhaarcard"].ToString();
                    sd.SEmail = dr["SEmail"].ToString();
                    sd.Gender = dr["Gender"].ToString();
                    sd.Remarks = dr["Remarks"].ToString();
                    sd.Discharged = dr["Discharged"].ToString();

                    sd.ClassID = dr["ClassID"].ToString();
                    sd.SectionID = dr["SectionID"].ToString();
                    sd.RollNo = dr["Rollno"].ToString();
                    sd.Session = dr["Current_Session"].ToString();
                    sd.SessionOfAdmission = dr["SessionOfAdmission"].ToString();

                    sd.FatherName = dr["FathersName"].ToString();
                    sd.FatherQualification = dr["FathersQualification"].ToString();
                    sd.FatherOccupation = dr["FathersJob"].ToString();
                    sd.MobileFather = dr["PhoneNo"].ToString();

                    sd.MontherName = dr["MothersName"].ToString();
                    sd.MotherQualification = dr["MothersQualification"].ToString();
                    sd.MotherOccupation = dr["MothersJob"].ToString();
                    sd.MobileMother = dr["Phoneno2"].ToString();

                    sd.PresentAddress = dr["PresentAddress"].ToString();
                    sd.PermanentAddress = dr["PerminantAddress"].ToString();
                    sd.PinCode = dr["Pincode"].ToString();

                    sd.PrePrimaryBoardNo = dr["PrePrimaryBoardNo"].ToString();
                    sd.PrimaryBoardNo = dr["PrimaryBoardNo"].ToString();
                    sd.MiddleBoardNo = dr["MiddleBoardNo"].ToString();
                    sd.HighBoardNo = dr["HighBoardNo"].ToString();
                    sd.HigherBoardNo = dr["HigherBoardNo"].ToString();

                    sd.IsDischarged = dr["IsDischarged"].ToString();
                    sd.DRemarks = DischargeStatus(dr["IsDischarged"].ToString());

                    sd.DBy = dr["DBy"].ToString();
                    sd.DSession = dr["DSession"].ToString();
                    if (!string.IsNullOrEmpty(dr["DDate"].ToString()))
                        sd.DDate = Convert.ToDateTime(dr["DDate"].ToString()).ToString("dd-MM-yyyy");

                    sd.GuardianName = dr["GuardianName"].ToString();
                    sd.GuardialAccupation = dr["GuardialAccupation"].ToString();
                    sd.GuardianPhoneNo = dr["GuardianPhoneNo"].ToString();
                    sd.GuardianQualification = dr["GuardianQualification"].ToString();
                    sd.DistrictID = dr["DistrictID"].ToString();
                    sd.StudentCatID = dr["StudentCatID"].ToString();
                    sd.DistrictName = dr["DistrictName"].ToString();
                    sd.StudentCatName = dr["StudentCatName"].ToString();

                    studentList.Add(sd);
                }
                #endregion

                #region Prepare Final Response
                if (studentList.Any())
                {
                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = studentList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = $"Error: {ex.Message}";
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentServices", "GetInvalidDischargeListOnClassIDCrescentAsync", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSession"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMaxUID(string currentSession, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };
            #endregion

            try
            {
                #region SQL Query and Parameters
                string sessionQuery = @"SELECT ISNULL(MAX(UID), 0) AS MaxUID FROM Students WHERE SessionOfAdmission = @CurrentSession";

                SqlParameter[] param = new SqlParameter[]
                {
            new SqlParameter("@CurrentSession", currentSession)
                };
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sessionQuery, param);
                #endregion

                #region Process Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int maxUID = Convert.ToInt32(ds.Tables[0].Rows[0]["MaxUID"]);

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Max UID retrieved successfully.";
                    response.ResponseData = maxUID;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetMaxUIDAsync", ex.Message + " | " + ex.StackTrace);

                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while retrieving max UID.";
                response.ResponseData = null;
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMaxRollNo(string classId, string sectionId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };
            #endregion

            try
            {
                #region SQL Query and Parameters
                string query = @"
SELECT ISNULL(MAX(TRY_CAST(StudentInfo.RollNo AS INT)), 0) + 1 AS NextRollNo
FROM Students
INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentID
WHERE StudentInfo.ClassID = @ClassID AND StudentInfo.SectionID = @SectionID";

                SqlParameter[] param = new SqlParameter[]
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", sectionId)
                };
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Process Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int nextRollNo = Convert.ToInt32(ds.Tables[0].Rows[0]["NextRollNo"]);

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Next RollNo retrieved successfully.";
                    response.ResponseData = nextRollNo;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetMaxRollNoAsync", ex.Message + " | " + ex.StackTrace);

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while retrieving max roll number.";
                response.ResponseData = null;
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetActiveStudentsOnUID(string UID, string clientId)
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
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query and Parameters
                string sql = @"SELECT Students.StudentID, StudentInfoID, Students.AdmissionNo, UID, StudentName, DOB, BOA, FathersName, 
                        FathersQualification, FathersJob, MothersName, MothersQualification, MothersJob, PresentAddress, 
                        PerminantAddress, SessionOfAdmission, PhoneNo, Gender, Discharged, Phoneno2, landlineno, WithdrawDate, 
                        Saadhaarcard, Faadhaarcard, Maadhaarcard, SEmail, BloodGroup, Pincode, StudentInfo.ClassID,StudentInfo.SectionID, 
                        StudentInfo.Current_Session, Rollno, PhotoPath,StudentInfo.RouteID, StreamID, StudentInfo.Remarks, BoardNo, PrePrimaryBoardNo, 
                        PrePrimaryDate, PrimaryBoardNo, PrimaryDate, MiddleBoardNo, MiddleDate, HighBoardNo, HighDate, 
                        HigherBoardNo, HigherDate,StudentInfo.busstopid, IsDischarged, DSession, DDate, DRemarks, DBy, BusFee, 
                        GuardianName, GuardialAccupation, GuardianPhoneNo, GuardianQualification, DistrictID, StudentCatID, Classes.ClassName, se.SectionName, bs.BusStop,
                          bs.BusRate, t.RouteName,
                        StudentCatName, DistrictName, Discharged 
                        FROM Students 
                        INNER JOIN StudentInfo ON Students.StudentID = StudentInfo.StudentId 
                        INNER JOIN Classes ON StudentInfo.ClassID = Classes.ClassId 
                        LEFT JOIN Sections se ON StudentInfo.SectionID = se.SectionID
                       LEFT JOIN Transport t ON t.RouteID = StudentInfo.RouteID
                        LEFT JOIN BusStops bs ON bs.BusStopID = StudentInfo.BusStopID
                        WHERE UID = @UID AND (IsDischarged = 0 OR IsDischarged IS NULL)
                        ORDER BY StudentInfo.ClassID, SectionID, Rollno";

                SqlParameter[] param = new SqlParameter[]
                {
            new SqlParameter("@UID", UID)
                };
                #endregion

                #region Execute SQL
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param);
                #endregion

                #region Process Result
                List<StudentDTO> sdL = new List<StudentDTO>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var sd = new StudentDTO
                    {
                        StudentID = dr["StudentID"].ToString(),
                        StudentCode = dr["UID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdmissionNo = dr["AdmissionNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        DOB = Convert.ToDateTime(string.IsNullOrEmpty(dr["DOB"].ToString()) ? DateTime.Now.ToString() : dr["DOB"].ToString()).ToString("dd-MM-yyyy"),
                        DOA = Convert.ToDateTime(string.IsNullOrEmpty(dr["BOA"].ToString()) ? DateTime.Now.ToString() : dr["BOA"].ToString()).ToString("dd-MM-yyyy"),
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
                        ClassName = dr["ClassName"]?.ToString(),
                        SectionName = dr["SectionName"]?.ToString(),
                        busstopid = dr["BusStopID"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        RouteID = dr["RouteID"]?.ToString(),
                        BusFee = dr["BusRate"]?.ToString(),
                        BusStopName = dr["BusStop"]?.ToString(),
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
                        DRemarks = DischargeStatus(dr["IsDischarged"].ToString()),
                        DBy = dr["DBy"].ToString(),
                        DSession = dr["DSession"].ToString(),
                        DDate = string.IsNullOrEmpty(dr["DDate"].ToString()) ? null : Convert.ToDateTime(dr["DDate"].ToString()).ToString("dd-MM-yyyy"),
                        GuardianName = dr["GuardianName"].ToString(),
                        GuardialAccupation = dr["GuardialAccupation"].ToString(),
                        GuardianPhoneNo = dr["GuardianPhoneNo"].ToString(),
                        GuardianQualification = dr["GuardianQualification"].ToString(),
                        DistrictID = dr["DistrictID"].ToString(),
                        StudentCatID = dr["StudentCatID"].ToString(),
                        DistrictName = dr["DistrictName"].ToString(),
                        StudentCatName = dr["StudentCatName"].ToString()
                    };

                    sdL.Add(sd);
                }

                if (sdL.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data retrieved successfully.";
                    response.ResponseData = sdL;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Student.Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetActiveStudentsOnUID", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while retrieving student data.";
                response.Error = ex.Message;
                #endregion\
            }

            #region Return Response
            return response;
            #endregion
        }


        public async Task<ResponseModel> GetStudentsByPhoneNumber(string phoneNo, string clientId)
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
    s.pp, s.p, s.m, s.h,
    s.ppan, s.pan, s.man, s.han,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
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
                        UID = row["UID"]?.ToString(),
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

        public async Task<ResponseModel> GetStudentsByAddress(string address, string clientId)
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
    s.UID,
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
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.pp, s.p, s.m, s.h,
    s.ppan, s.pan, s.man, s.han,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,
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
WHERE s.PresentAddress LIKE '%' + @Address + '%' OR s.PerminantAddress LIKE '%' + @Address + '%'";
                #endregion

                #region Execute Query
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Address", address)
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
                            UID = row["UID"]?.ToString(),
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
                        };

                        students.Add(student);
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Students found.";
                    response.ResponseData = students;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentsByAddress", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while fetching students by address.";
                response.Error = ex.Message;
                response.ResponseData = null;
            }

            return response;
        }


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
    s.UID,
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
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.pp, s.p, s.m, s.h,
    s.ppan, s.pan, s.man, s.han,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,
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
                            UID = row["UID"]?.ToString(), // ✅ Added UID here
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
                        };

                        students.Add(student);
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Students found.";
                    response.ResponseData = students;
                }
                #endregion
            }
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

        public async Task<ResponseModel> GetStudentsByAcademicNo(string academicNo, string clientId)
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
    s.UID,
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
    s.Discharged,
    s.TransportFee,
    s.StudentFeeRebate,
    s.WithdrawDate,
    s.Withdrawnarration,
    s.Detnewadmission,
    s.PhoneNo2,
    s.pp, s.p, s.m, s.h,
    s.ppan, s.pan, s.man, s.han,
    s.Ledgerid, s.landlineno,
    s.FeeRemarks, s.BloodGroup,
    s.Pincode, s.SEmail,
    s.Saadhaarcard, s.Faadhaarcard, s.Maadhaarcard,
    s.Fphn, s.Mphn,
    s.GuardianName, s.GuardianPhoneNo, s.GuardianQualification, s.GuardialAccupation,
    s.DistrictID, s.StudentCatID, s.DistrictName, s.StudentCatName,
    s.Scategory, s.ScategoryID, s.categoryID, s.category,
    s.HID, 
    s.PEN, s.WEIGHT, s.Height, s.NAMEASPERADHAAR, s.DOBASPERADHAAR,
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
WHERE dbo.AcademicNo(si.Current_Session, s.StudentID, c.SubDepartmentID) = @AcademicNo";
                #endregion

                #region Execute Query
                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@AcademicNo", academicNo)
        };

                //    DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters.ToArray());
                DataSet ds = await ExecuteLongQueryWithTimeout(connectionString, query, parameters.ToArray(), 120);

                #endregion

                #region Map to DTO
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
                            UID = row["UID"]?.ToString(),
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
                            ClassName = row["ClassName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PermanentAddress = row["PerminantAddress"]?.ToString(),
                            SessionOfAdmission = row["SessionOfAdmission"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            Discharged = row["Discharged"]?.ToString(),
                            DSession = row["DSession"]?.ToString(),
                            DDate = row["DDate"] != DBNull.Value ? Convert.ToDateTime(row["DDate"]).ToString("yyyy-MM-dd") : null,
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
                        };

                        students.Add(student);
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Student(s) found.";
                    response.ResponseData = students;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("StudentService", "GetStudentsByAcademicNo", ex.Message + " | " + ex.StackTrace);
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while fetching students.";
                response.ResponseData = null;
                response.Error = ex.Message;
            }

            return response;
        }


        public async Task<DataSet> ExecuteLongQueryWithTimeout(string connectionString, string query, SqlParameter[] parameters, int timeoutInSeconds)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = timeoutInSeconds;
                cmd.CommandType = CommandType.Text;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();

                var ds = new DataSet();
                await Task.Run(() => da.Fill(ds)); // ensure async compatibility
                return ds;
            }
        }



        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isDischarged"></param>
        /// <returns></returns>
        private string DischargeStatus(string isDischarged)
        {
            try
            {
                if (isDischarged == "0")
                {
                    return "Active";
                }
                else
                    if (isDischarged == "1")
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
        /// <param name="input"></param>
        /// <returns></returns>
        private static string TryFormatDate(string input)
        {
            return DateTime.TryParse(input, out DateTime dt)
                ? dt.ToString("dd-MM-yyyy")
                : string.Empty;
        }
        // Helper method to parse and format dates safely
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbValue"></param>
        /// <returns></returns>
        private string TryFormatDate(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value)
                return null;

            if (DateTime.TryParse(dbValue.ToString(), out DateTime dt))
                return dt.ToString("dd-MM-yyyy");

            return null;
        }

       
    }
}
