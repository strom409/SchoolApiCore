using Microsoft.Data.SqlClient;
using System.Data;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TTPeroids
{
    public class TTPeroidsNoService : ITTPeroidsNoService
    {
        private readonly IConfiguration _configuration;
        public TTPeroidsNoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> PeroidName(string pid, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record found."
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
            new SqlParameter("@PID", pid)
        };
                #endregion

                #region Execute Query
                string query = "SELECT PeroidName FROM TTPeroidsNo WHERE PID = @PID";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var peroidName = ds.Tables[0].Rows[0][0]?.ToString();
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Record fetched successfully.";
                    response.ResponseData = string.IsNullOrEmpty(peroidName) ? "NA" : peroidName;
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

                Repository.Error.ErrorBLL.CreateErrorLog("TTPeroidsNoService", "PeroidName", ex.ToString());
                #endregion
            }

            return response;
        }

    }

}

