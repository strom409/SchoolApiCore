using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Student.Repository;
using Student.Repository.SQL;
using System.Data;

namespace Student.Services.Students
{
    public class RPAlamStudentService : IRPAlamStudentService
    {
        private readonly IConfiguration _configuration;
        public RPAlamStudentService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<ResponseModel> GetAllStudentsOnStudentInfoIdAsync(string StudentID, string clientId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAttendanceMarkedOrNotAsPerDateAsync(string session, string clientId)
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
                #region Input Validation
                string[] parts = session.Split(',');
                if (parts.Length != 3)
                {
                    response.Status = -1;
                    response.IsSuccess = false;
                    response.Message = "Invalid input format. Expected: 'session,fromDate,toDate'";
                    return response;
                }

                string currentSession = parts[0];
                DateTime startDate = DateTime.Parse(parts[1]);
                DateTime endDate = DateTime.Parse(parts[2]);
                #endregion

                #region Build Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate),
            new SqlParameter("@Session", currentSession)
                };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "GetPivotedAttendanceStatus",
                    parameters
                );
                #endregion

                #region Process Results
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = ds.Tables[0];
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
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
        public async Task<ResponseModel> GetCopyCheckingOnSectionIdAsync(string session, string clientId)
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
                #region Validate Input
                string[] parts = session.Split(',');
                if (parts.Length != 3)
                {
                    response.Status = -1;
                    response.IsSuccess = false;
                    response.Message = "Invalid input format. Expected: 'ClassId,SectionID,Date'";
                    return response;
                }

                string classId = parts[0];
                string sectionId = parts[1];
                DateTime date = DateTime.Parse(parts[2]);
                #endregion

                #region Build Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Build Parameters
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@Classid", classId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@Date", date)
                };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "GetCopyCheckedBySectionID",
                    parameters
                );
                #endregion

                #region Process Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Message = "OK";
                    response.Status = 1;
                    response.ResponseData = ds.Tables[0];
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
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
        public async Task<ResponseModel> GetPTMReportAsync(string session, string clientId)
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
                #region Input Validation
                string[] parts = session.Split(',');
                if (parts.Length != 1)
                {
                    response.Status = -1;
                    response.IsSuccess = false;
                    response.Message = "Invalid input format. Expected: 'session'";
                    return response;
                }

                string sessionYear = parts[0];
                #endregion

                #region Build Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@SessionYear", sessionYear)
                };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "GetPTMDetailsForSession",
                    parameters
                );
                #endregion

                #region Process Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Message = "OK";
                    response.Status = 1;
                    response.ResponseData = ds.Tables[0];
                }

                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                return response;
                #endregion
            }
        }
    }
}
