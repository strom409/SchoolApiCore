using HR.Repository;
using HR.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.EmployeeAttendance
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService
    {
        private readonly IConfiguration _configuration;
        public EmployeeAttendanceService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddEmployeeAttendance(EmpAttendance request, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new()
            {
                IsSuccess = true,
                Status = 0,
                Message = "Attendance not added!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                using SqlConnection connection = new SqlConnection(connectionString);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = new SqlParameter[]
                {
            new SqlParameter("@DateIn", request.DateIn),
            new SqlParameter("@Whr", request.Whr ?? (object)DBNull.Value),
            new SqlParameter("@AType", request.AType ?? (object)DBNull.Value),
            new SqlParameter("@DayN", request.DayN ?? (object)DBNull.Value),
            new SqlParameter("@SMonth", request.SMonth ?? (object)DBNull.Value),
            new SqlParameter("@SYear", request.SYear ?? (object)DBNull.Value),
            new SqlParameter("@FYear", request.FYear ?? (object)DBNull.Value),
            new SqlParameter("@CSession", request.CSession ?? (object)DBNull.Value),
            new SqlParameter("@UserName", request.UserName ?? (object)DBNull.Value),
            new SqlParameter("@Ecode", request.Ecode ?? (object)DBNull.Value)
                };
                #endregion

                #region Check for Duplicate Attendance
                string dupCheckQuery = "SELECT COUNT(EAID) AS Total FROM AttendanceEmp WHERE Ecode = @Ecode AND DateIn = @DateIn";
                DataSet dupResult = await SQLHelperCore.ExecuteDatasetAsync(connection, CommandType.Text, dupCheckQuery, sqlParams);

                if (dupResult != null && dupResult.Tables.Count > 0 && dupResult.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(dupResult.Tables[0].Rows[0]["Total"]);
                    if (count > 0)
                    {
                        response.Message = "Attendance already added!";
                        return response;
                    }
                }
                #endregion

                #region Insert Attendance Record
                string insertQuery = @"
            INSERT INTO AttendanceEmp 
            (DateIn, Whr, AType, DayN, SMonth, SYear, FYear, CSession, UserName, Ecode)
            VALUES 
            (@DateIn, @Whr, @AType, @DayN, @SMonth, @SYear, @FYear, @CSession, @UserName, @Ecode)";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connection, CommandType.Text, insertQuery, sqlParams);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Attendance added successfully!";
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

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceService", "AddEmployeeAttendance", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateEmployeeAttendance(EmpAttendance value, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new()
            {
                IsSuccess = true,
                Status = 0,
                Message = "Attendance not updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = new SqlParameter[]
                {
            new SqlParameter("@EAID", value.EAID),
            new SqlParameter("@DateIn", value.DateIn),
            new SqlParameter("@Whr", value.Whr ?? (object)DBNull.Value),
            new SqlParameter("@AType", value.AType ?? (object)DBNull.Value),
            new SqlParameter("@DayN", value.DayN ?? (object)DBNull.Value),
            new SqlParameter("@SMonth", value.SMonth ?? (object)DBNull.Value),
            new SqlParameter("@SYear", value.SYear ?? (object)DBNull.Value),
            new SqlParameter("@FYear", value.FYear ?? (object)DBNull.Value),
            new SqlParameter("@CSession", value.CSession ?? (object)DBNull.Value),
            new SqlParameter("@UserName", value.UserName ?? (object)DBNull.Value),
            new SqlParameter("@Ecode", value.Ecode ?? (object)DBNull.Value)
                };
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE AttendanceEmp 
            SET 
                Whr = @Whr,
                AType = @AType,
                UserName = @UserName
            WHERE 
                EAID = @EAID";
                #endregion

                #region Execute Query
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, sqlParams);
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Attendance updated successfully!";
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

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceService", "UpdateEmployeeAttendance", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmpAttendanceByCodeOnDateRange(string employeeCode, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new()
            {
                IsSuccess = false,
                Status = 0,
                Message = "No data found!"
            };
            #endregion

            try
            {
                #region Parse Input (expecting format "Ecode,DateFrom,DateTo")
                var split = employeeCode.Split(',');
                if (split.Length != 3)
                {
                    response.Message = "Invalid parameter format. Expected: 'Ecode,DateFrom,DateTo'";
                    return response;
                }

                string ecode = split[0];
                string dateFrom = split[1];
                string dateTo = split[2];
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = new SqlParameter[]
                {
            new SqlParameter("@Ecode", ecode),
            new SqlParameter("@DateFrom", dateFrom),
            new SqlParameter("@DateTo", dateTo)
                };
                #endregion

                #region SQL Query
                string query = @"
            SELECT EAID, DateIn, Whr, AType, DayN, SMonth, SYear, FYear, CSession, UserName, Ecode 
            FROM AttendanceEmp 
            WHERE Ecode = @Ecode AND DateIn BETWEEN @DateFrom AND @DateTo";
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                List<EmpAttendance> list = new();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new EmpAttendance
                        {
                            EAID = dr["EAID"].ToString(),
                            DateIn = Convert.ToDateTime(dr["DateIn"]).ToString("dd-MM-yyyy"),
                            DayN = dr["DayN"].ToString(),
                            Whr = dr["Whr"].ToString(),
                            AType = dr["AType"].ToString(),
                            SMonth = dr["SMonth"].ToString(),
                            SYear = dr["SYear"].ToString(),
                            FYear = dr["FYear"].ToString(),
                            CSession = dr["CSession"].ToString(),
                            UserName = dr["UserName"].ToString(),
                            Ecode = dr["Ecode"].ToString()
                        });
                    }
                }
                #endregion

                #region Return Response
                if (list.Any())
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Data found.";
                    response.ResponseData = list;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceService", "GetEmpAttendanceByCodeOnDateRange", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteEmployeeAttendance(EmpAttendance request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Attendance not deleted!"
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

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams = new SqlParameter[]
                {
            new SqlParameter("@EAID", request.EAID ?? (object)DBNull.Value),
            new SqlParameter("@DateIn", request.DateIn ?? (object)DBNull.Value),
            new SqlParameter("@Whr", request.Whr ?? (object)DBNull.Value),
            new SqlParameter("@AType", request.AType ?? (object)DBNull.Value),
            new SqlParameter("@DayN", request.DayN ?? (object)DBNull.Value),
            new SqlParameter("@SMonth", request.SMonth ?? (object)DBNull.Value),
            new SqlParameter("@SYear", request.SYear ?? (object)DBNull.Value),
            new SqlParameter("@FYear", request.FYear ?? (object)DBNull.Value),
            new SqlParameter("@CSession", request.CSession ?? (object)DBNull.Value),
            new SqlParameter("@UserName", request.UserName ?? (object)DBNull.Value),
            new SqlParameter("@Ecode", request.Ecode ?? (object)DBNull.Value)
                };
                #endregion

                #region Delete Query Execution
                string deleteQuery = "DELETE FROM AttendanceEmp WHERE EAID = @EAID AND DateIn = @DateIn";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    deleteQuery,
                    sqlParams);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Attendance deleted successfully!";
                }
                else
                {
                    response.Message = "Past attendance can't be deleted!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeAttendanceService", "DeleteEmployeeAttendance", ex.ToString());
            }

            return response;
        }

    }
}
