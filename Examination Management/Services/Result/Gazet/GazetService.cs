using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.Result.Gazet
{
    public class GazetService :IGazetService
    {
        private readonly IConfiguration _configuration;
        public GazetService(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetGazetResults(string? param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record found!"
            };
            #endregion

            try
            {
                #region Parse Parameters
                string? classId = null;
                string? sectionId = null;
                string? currentSession = null;
                string? unitIds = null;

                if (!string.IsNullOrEmpty(param))
                {
                    var parts = param.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1) classId = parts[0].Trim();
                    if (parts.Length >= 2) sectionId = parts[1].Trim();
                    if (parts.Length >= 3) currentSession = parts[2].Trim();
                    if (parts.Length >= 4) unitIds = parts[3].Trim();
                }

                if (string.IsNullOrEmpty(classId))
                {
                    response.IsSuccess = false;
                    response.Message = "classId is required!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", string.IsNullOrEmpty(sectionId) ? DBNull.Value : (object)sectionId),
            new SqlParameter("@Current_Session", string.IsNullOrEmpty(currentSession) ? DBNull.Value : (object)currentSession),
            new SqlParameter("@UnitIDs", string.IsNullOrEmpty(unitIds) ? DBNull.Value : (object)unitIds)
        };
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "sp_GetGazetUnits", parameters);
                #endregion

                #region Map Data
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<GazetResultDto>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new GazetResultDto
                        {
                            StudentID = row["StudentID"] != DBNull.Value ? Convert.ToInt32(row["StudentID"]) : 0,
                            AdmissionNo = row["AdmissionNo"]?.ToString(),
                            RollNo = row["RollNo"] != DBNull.Value ? Convert.ToInt32(row["RollNo"]) : 0,
                            Current_Session = row["Current_Session"]?.ToString(),
                            SessionID = row["SessionID"] != DBNull.Value ? Convert.ToInt32(row["SessionID"]) : 0,
                            StudentName = row["StudentName"]?.ToString(),
                            FathersName = row["FathersName"]?.ToString(),
                            MothersName = row["MothersName"]?.ToString(),
                            Gender = row["Gender"]?.ToString(),
                            DOB = row["DOB"] != DBNull.Value ? Convert.ToDateTime(row["DOB"]) : null,
                            BloodGroup = row["BloodGroup"]?.ToString(),
                            StudentAadhaar = row["StudentAadhaar"]?.ToString(),
                            FatherAadhaar = row["FatherAadhaar"]?.ToString(),
                            MotherAadhaar = row["MotherAadhaar"]?.ToString(),
                            PresentAddress = row["PresentAddress"]?.ToString(),
                            PerminantAddress = row["PerminantAddress"]?.ToString(),
                            PhoneNo = row["PhoneNo"]?.ToString(),
                            PhoneNo2 = row["PhoneNo2"]?.ToString(),
                            SEmail = row["SEmail"]?.ToString(),
                            Category = row["Category"]?.ToString(),
                            Remarks = row["Remarks"]?.ToString(),
                            RegistrationNo = row["RegistrationNo"]?.ToString(),
                            StudentCode = row["StudentCode"]?.ToString(),
                            FeeBookNo = row["FeeBookNo"]?.ToString(),
                            BusFeeBookNo = row["BusFeeBookNo"]?.ToString(),
                            PhotoPath = row["PhotoPath"]?.ToString(),
                            BusRoute = row["BusRoute"]?.ToString(),
                            ClassID = row["ClassID"] != DBNull.Value ? Convert.ToInt32(row["ClassID"]) : 0,
                            ClassName = row["ClassName"]?.ToString(),
                            SectionID = row["SectionID"] != DBNull.Value ? Convert.ToInt32(row["SectionID"]) : 0,
                            SectionName = row["SectionName"]?.ToString(),
                            SubjectID = row["SubjectID"] != DBNull.Value ? Convert.ToInt32(row["SubjectID"]) : 0,
                            SubjectName = row["SubjectName"]?.ToString(),
                            UnitID = row["UnitID"] != DBNull.Value ? Convert.ToInt32(row["UnitID"]) : 0,
                            UnitName = row["UnitName"]?.ToString(),
                            Marks = row["Marks"] != DBNull.Value ? Convert.ToDecimal(row["Marks"]) : 0,
                            MaxMarks = row["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(row["MaxMarks"]) : 0,
                            Percentage = row["Percentage"] != DBNull.Value ? Convert.ToDecimal(row["Percentage"]) : 0,
                            Grade = row["Grade"]?.ToString(),
                            GradePoint = row["GradePoint"] != DBNull.Value ? Convert.ToDecimal(row["GradePoint"]) : 0,
                            TeacherRemarks = row["TeacherRemarks"]?.ToString(),
                            PrincipalRemarks = row["PrincipalRemarks"]?.ToString(),
                            Rank = row["Rank"] != DBNull.Value ? Convert.ToInt32(row["Rank"]) : 0
                        });
                    }

                    response.Status = 1;
                    response.IsSuccess = true;
                    response.Message = "Records found.";
                    response.ResponseData = list;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while fetching Gazet results.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GazetService", "GetGazetResults", ex.ToString());
                #endregion
            }

            return response;
        }


    }
}
