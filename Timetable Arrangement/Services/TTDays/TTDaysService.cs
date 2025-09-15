using Microsoft.Data.SqlClient;
using System.Data;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TTDays
{
    public class TTDaysService :ITTDaysService
    {
        private readonly IConfiguration _configuration;
        public TTDaysService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="did"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DaydName(string did, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Record not found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@DID", did)
        };
                #endregion

                #region Execute Query
                string query = "SELECT DayName FROM TTDays WHERE DID=@DID";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var dayName = ds.Tables[0].Rows[0][0]?.ToString();
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Record fetched successfully.";
                    response.ResponseData = string.IsNullOrEmpty(dayName) ? "NA" : dayName;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching record.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TTDaysService", "DaydName", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getweekdays(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                string query = "SELECT DID, DayName FROM TTDays";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<TimeTable> weekdays = new List<TimeTable>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        weekdays.Add(new TimeTable
                        {
                            DID = dr["DID"].ToString(),
                            DayName = dr["DayName"].ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = weekdays;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching weekdays.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TTDaysService", "getweekdays", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="years"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetwholetimetableProc(string years, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@year", years)
        };
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getTT2", param);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ds.Tables[0]; // returning DataTable as in old method
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TTDaysService", "GetwholetimetableProc", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}
