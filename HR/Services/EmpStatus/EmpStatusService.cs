using HR.Repository;
using HR.Repository.SQL;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Data;

namespace HR.Services.EmpStatus
{
    public class EmpStatusService : IEmpStatusService   
    {
        private readonly IConfiguration _configuration;
        public EmpStatusService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeStatus(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                string query = "SELECT * FROM EmployeeStatus ORDER BY Status";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Map DataSet to List
                var statuses = new List<EmployeeStatus>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        statuses.Add(new EmployeeStatus
                        {
                            StatusID = Convert.ToInt64(dr["StatusID"]),
                            Status = dr["Status"].ToString() ?? string.Empty
                        });
                    }

                    response.Status = 1;
                    response.Message = "Employee statuses fetched successfully.";
                    response.ResponseData = statuses;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeStatusRepository", "GetEmployeeStatusAsync", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}

