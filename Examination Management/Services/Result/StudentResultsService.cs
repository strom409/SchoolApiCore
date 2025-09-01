using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Examination_Management.Services.Result
{
    public class StudentResultsService : IStudentResultsService
    {
        private readonly IConfiguration _configuration;
        public StudentResultsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentResultsAsync(StudentResultsRequestDto request, string clientId)
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

                List<StudentResultDto> results = new();

                #region Prepare Parameters
                var parameters = new[]
                {
            new SqlParameter("@StudentID", request.StudentID ?? (object)DBNull.Value),
            new SqlParameter("@AdmissionNo", request.AdmissionNo ?? (object)DBNull.Value),
            new SqlParameter("@ClassID", request.ClassID ?? (object)DBNull.Value),
            new SqlParameter("@SectionID", request.SectionID ?? (object)DBNull.Value),
            new SqlParameter("@RollNo", request.RollNo ?? (object)DBNull.Value),
            new SqlParameter("@SubjectIDs", request.SubjectIDs ?? (object)DBNull.Value),
            new SqlParameter("@UnitID", request.UnitIDs ?? (object)DBNull.Value),
            new SqlParameter("@Current_Session", request.Current_Session ?? (object)DBNull.Value),
            new SqlParameter("@GetAllResults", request.GetAllResults)
        };
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "sp_GetStudentResults_Universal", parameters);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        StudentResultDto result = new StudentResultDto
                        {
                            StudentID = dr["StudentID"]?.ToString(),
                            AdmissionNo = dr["AdmissionNo"]?.ToString(),
                            StudentName = dr["StudentName"]?.ToString(),
                            FathersName = dr["FathersName"]?.ToString(),
                            MothersName = dr["MothersName"]?.ToString(),

                            ClassID = dr["ClassID"] != DBNull.Value ? Convert.ToInt32(dr["ClassID"]) : 0,
                            ClassName = dr["ClassName"]?.ToString(),

                            SectionID = dr["SectionID"] != DBNull.Value ? Convert.ToInt32(dr["SectionID"]) : 0,
                            SectionName = dr["SectionName"]?.ToString(),

                            DepartmentName = dr["DepartmentName"]?.ToString(),

                            RollNo = dr["RollNo"] != DBNull.Value ? Convert.ToInt32(dr["RollNo"]) : 0,

                            ResultID = dr["ResultID"] != DBNull.Value ? Convert.ToInt32(dr["ResultID"]) : 0,

                            SubjectID = dr["SubjectID"] != DBNull.Value ? Convert.ToInt32(dr["SubjectID"]) : 0,
                            SubjectName = dr["SubjectName"]?.ToString(),

                            UnitID = dr["UnitID"] != DBNull.Value ? Convert.ToInt32(dr["UnitID"]) : 0,
                            UnitName = dr["UnitName"]?.ToString(),

                            Marks = dr["Marks"] != DBNull.Value ? Convert.ToDecimal(dr["Marks"]) : 0,
                            MaxMarks = dr["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(dr["MaxMarks"]) : 0,
                            Grade = dr["Grade"]?.ToString(),
                            Percentage = dr["Percentage"] != DBNull.Value ? Convert.ToDecimal(dr["Percentage"]) : 0,

                            TeacherRemarks = dr["TeacherRemarks"]?.ToString(),
                            PrincipalRemarks = dr["PrincipalRemarks"]?.ToString()
                        };

                        results.Add(result);
                    }

                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = results;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalResultsService", "GetStudentResultsAsync", ex.ToString());
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
        public async Task<ResponseModel> GetOptionalResultsAsync(OptionalResultsRequestDto request, string clientId)
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

                List<OptionalStudentResultDto> results = new();

                #region Prepare Parameters
                var parameters = new[]
                {
            new SqlParameter("@StudentID", request.StudentID ?? (object)DBNull.Value),
            new SqlParameter("@ClassID", request.ClassID ?? (object)DBNull.Value),
            new SqlParameter("@SectionID", request.SectionID ?? (object)DBNull.Value),
            new SqlParameter("@RollNo", request.RollNo ?? (object)DBNull.Value),
            new SqlParameter("@SubjectIDs", request.SubjectIDs ?? (object)DBNull.Value),
            new SqlParameter("@UnitID", request.UnitIDs ?? (object)DBNull.Value),
            new SqlParameter("@Current_Session", request.Current_Session ?? (object)DBNull.Value),
            new SqlParameter("@GetAllResults", request.GetAllResults)
        };
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure,
                            "sp_GetOptionalStudentResults_Universal", parameters);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var result = new OptionalStudentResultDto
                        {
                            OpMarksID = dr["OpMarksID"] != DBNull.Value ? Convert.ToInt64(dr["OpMarksID"]) : 0,
                            StudentID = dr["StudentID"] != DBNull.Value ? Convert.ToInt32(dr["StudentID"]) : 0,
                            StudentName = dr["StudentName"]?.ToString() ?? string.Empty,
                            ClassID = dr["ClassID"] != DBNull.Value ? Convert.ToInt32(dr["ClassID"]) : 0,
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionID = dr["SectionID"] != DBNull.Value ? Convert.ToInt32(dr["SectionID"]) : 0,
                            SectionName = dr["SectionName"]?.ToString(),
                            RollNo = dr["RollNo"] != DBNull.Value ? Convert.ToInt32(dr["RollNo"]) : 0,
                            OptionalSubjectID = dr["OptionalSubjectID"] != DBNull.Value ? Convert.ToInt32(dr["OptionalSubjectID"]) : 0,
                            OptionalSubjectName = dr["OptionalSubjectName"]?.ToString(),
                            UnitID = dr["UnitID"] != DBNull.Value ? Convert.ToInt32(dr["UnitID"]) : 0,
                            UnitName = dr["UnitName"]?.ToString(),
                            Marks = dr["Marks"] != DBNull.Value ? Convert.ToDecimal(dr["Marks"]) : 0,
                            MaxMarks = dr["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(dr["MaxMarks"]) : null,
                            Grade = dr["Grade"]?.ToString(),
                            Percentage = dr["Percentage"] != DBNull.Value ? Convert.ToDecimal(dr["Percentage"]) : null,
                            TeacherRemarks = dr["TeacherRemarks"]?.ToString(),
                            PrincipalRemarks = dr["PrincipalRemarks"]?.ToString()
                        };

                        results.Add(result);
                    }

                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = results;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalResultsService", "GetOptionalResultsAsync", ex.ToString());
            }

            return response;
        }
    }
}
