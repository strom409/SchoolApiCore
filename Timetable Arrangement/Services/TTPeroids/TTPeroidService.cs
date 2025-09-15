using Microsoft.Data.SqlClient;
using System.Data;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TTAssignPeroids;
using Timetable_Arrangement.Services.TTDays;

namespace Timetable_Arrangement.Services.TTPeroids
{
    public class TTPeroidService : ITTPeroidService
    {
        private readonly IConfiguration _configuration;
        private readonly ITTPeroidsNoService _ttPeroidsNoService;
        private readonly ITTDaysService _ttDaysService;
        public TTPeroidService(IConfiguration configuration, ITTPeroidsNoService ttPeroidsNoService)
        {
            _configuration = configuration;
            _ttPeroidsNoService = ttPeroidsNoService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttval"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> addTTperoidtime(TimeTable ttval, string clientId)
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
                SqlParameter[] para =
                {
            new SqlParameter("@PIDFK", ttval.PIDFK),
            new SqlParameter("@DIDFK", ttval.DIDFK),
            new SqlParameter("@PeroidFrom", ttval.PeroidFrom),
            new SqlParameter("@PeroidTo", ttval.PeroidTo),
        };
                #endregion

                #region Check Duplicate
                string checkQuery = "SELECT * FROM TTperoid WHERE PIDFK=@PIDFK AND DIDFK=@DIDFK";
                var dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, para);

                if (dsCheck != null && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    response.Message = "Already Added.";
                    return response;
                }
                #endregion

                #region Insert Record
                string insertQuery = "INSERT INTO TTperoid(PIDFK, DIDFK, PeroidFrom, PeroidTo) VALUES(@PIDFK, @DIDFK, @PeroidFrom, @PeroidTo)";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, para);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Added Successfully.";
                }
                else
                {
                    response.Message = "Not Added.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding period time.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "addTTperoidtime", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ttval"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> updateTTperoidtime(TimeTable ttval, string clientId)
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
                SqlParameter[] para =
                {
            new SqlParameter("@PIDFK", ttval.PIDFK),
            new SqlParameter("@DIDFK", ttval.DIDFK),
            new SqlParameter("@PeroidFrom", ttval.PeroidFrom),
            new SqlParameter("@PeroidTo", ttval.PeroidTo)
        };
                #endregion

                #region Update Record
                string updateQuery = "UPDATE TTperoid SET PeroidFrom=@PeroidFrom, PeroidTo=@PeroidTo WHERE PIDFK=@PIDFK AND DIDFK=@DIDFK";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, para);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully.";
                }
                else
                {
                    response.Message = "Not Updated.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error updating period time.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "updateTTperoidtime", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> Gettimetable(string clientId)
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
                string query = @"
SELECT 
    pno.PeroidName,
    p.PeroidFrom,
    p.PeroidTo,
    p.PIDFK,
    p.PeroidID
FROM TTPeroid p
INNER JOIN TTPeroidsNo pno ON pno.PID = p.PIDFK";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                List<TimeTable> Listtimetable = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TimeTable ttable = new TimeTable
                        {
                            PeroidName = dr["PeroidName"].ToString(),
                            PeroidFrom = dr["PeroidFrom"].ToString(),
                            PeroidTo = dr["PeroidTo"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            PeroidID = dr["PeroidID"].ToString()
                        };

                        Listtimetable.Add(ttable);
                    }
                }
                #endregion

                if (Listtimetable.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = Listtimetable;
                }
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "Gettimetable", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getPeroidNo(string clientId)
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
                string query = "SELECT PID, Peroid, PeroidName FROM TTPeroidsNo";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                List<TimeTable> Listtimetable = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TimeTable ttable = new TimeTable
                        {
                            PID = dr["PID"].ToString(),
                            peroid = dr["Peroid"].ToString(),
                            PeroidName = dr["PeroidName"].ToString()
                        };

                        Listtimetable.Add(ttable);
                    }
                }
                #endregion

                if (Listtimetable.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = Listtimetable;
                }
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching period numbers.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "getPeroidNo", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getTimeTablePeriodsWithDuration(string clientId)
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
                string query = "SELECT * FROM TTPeroid";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                List<TimeTable> Listtimetable = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TimeTable ttable = new TimeTable
                        {
                            PeroidFKduration = dr["PeroidID"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            PeroidFrom = dr["PeroidFrom"].ToString(),
                            PeroidTo = dr["PeroidTo"].ToString()
                        };

                        Listtimetable.Add(ttable);
                    }
                }
                #endregion

                if (Listtimetable.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = Listtimetable;
                }
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching timetable periods.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "getTimeTablePeriodsWithDuration", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetPeriodList(string pid, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(pid))
                {
                    response.Message = "Year (pid) is null!";
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
            new SqlParameter("@pidfk", pid)
        };
                #endregion

                #region Execute Query
                string query = "SELECT * FROM TTPeroid WHERE PIDFK = @pidfk";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                List<TimeTable> periodList = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var peroidResponse = await _ttPeroidsNoService.PeroidName(dr["PIDFK"]?.ToString(), clientId);
                        var dayResponse = await _ttDaysService.DaydName(dr["DIDFK"]?.ToString(), clientId);

                        TimeTable ttable = new TimeTable
                        {
                            PeroidID = dr["PeroidID"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            PeroidFrom = dr["PeroidFrom"].ToString(),
                            PeroidTo = dr["PeroidTo"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            PeroidName = peroidResponse.ResponseData?.ToString() ?? "NA",
                            DayName = dayResponse.ResponseData?.ToString() ?? "NA",
                        };

                        periodList.Add(ttable);
                    }

                    if (periodList.Count > 0)
                    {
                        response.Status = 1;
                        response.Message = "Ok";
                        response.ResponseData = periodList;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "TimeTableArrangementsServices",
                    "GetPeriodList",
                    ex.ToString()
                );
                #endregion
            }

            return response;
        }


    }
}
