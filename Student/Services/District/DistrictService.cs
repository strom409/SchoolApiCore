using Microsoft.Data.SqlClient;
using Student.Repository;
using Student.Repository.SQL;
using System.Data;

namespace Student.Services.District
{
    public class DistrictService : IDistrictService
    {
        private readonly IConfiguration _configuration;
        public DistrictService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel> GetAllDistricts(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "No districts found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
                SELECT DistrictID, StateIDFK, DistrictName
                FROM District
                ORDER BY DistrictName
            ";
                #endregion

                #region Execute Query
                var dataSet = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var districts = dataSet.Tables[0].AsEnumerable()
                        .Select(row => new DistrictDto
                        {
                            DistrictID = row["DistrictID"] != DBNull.Value ? Convert.ToInt32(row["DistrictID"]) : 0,
                            StateIDFK = row["StateIDFK"] != DBNull.Value ? Convert.ToInt32(row["StateIDFK"]) : 0,
                            DistrictName = row["DistrictName"] != DBNull.Value ? row["DistrictName"].ToString() : string.Empty
                        }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Districts fetched successfully";
                    response.ResponseData = districts;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.Message = "An error occurred while fetching districts.";
                Repository.Error.ErrorBLL.CreateErrorLog("districtservice", "GetAllDistricts", ex.ToString());
                #endregion
            }

            return response;
        }

        public async Task<ResponseModel> GetDistrictsByStateId(int stateId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "No districts found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT DistrictID, StateIDFK, DistrictName
            FROM District
            WHERE StateIDFK = @StateIDFK
            ORDER BY DistrictName";
                #endregion

                #region Prepare Parameters
                var parameters = new[]
                {
            new SqlParameter("@StateIDFK", stateId)
        };
                #endregion

                #region Execute Query
                var dataSet = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var districts = dataSet.Tables[0].AsEnumerable()
                        .Select(row => new DistrictDto
                        {
                            DistrictID = Convert.ToInt32(row["DistrictID"]),
                            StateIDFK = Convert.ToInt32(row["StateIDFK"]),
                            DistrictName = row["DistrictName"].ToString()
                        }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Districts fetched successfully";
                    response.ResponseData = districts;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while fetching districts.";
                Repository.Error.ErrorBLL.CreateErrorLog("districtservice", "GetDistrictsByStateId", ex.ToString());
            }

            return response;
        }

        public async Task<ResponseModel> GetAllStates(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "No states found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT StateID, StateName
            FROM States
            ORDER BY StateName";
                #endregion

                #region Execute Query
                var dataSet = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                {
                    var states = dataSet.Tables[0].AsEnumerable()
                        .Select(row => new StateDto
                        {
                            StateID = Convert.ToInt32(row["StateID"]),
                            StateName = row["StateName"].ToString()
                        }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "States fetched successfully";
                    response.ResponseData = states;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred while fetching states.";
                Repository.Error.ErrorBLL.CreateErrorLog("districtservice", "GetAllStates", ex.ToString());
            }

            return response;
        }



    }

}

