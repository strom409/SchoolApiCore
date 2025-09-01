using Azure.Core;
using HR.Repository;
using HR.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.Designation
{
    public class DesignationServices :IDesignationServices
    {
        private readonly IConfiguration _configuration;
        public DesignationServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDesignations(string clientId)
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
                string query = "SELECT * FROM Designations ORDER BY Designation";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Map DataSet to List
                var designations = new List<Designations>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        designations.Add(new Designations
                        {
                            DesignationID = Convert.ToInt64(dr["DesignationID"]),
                            Designation = dr["Designation"].ToString() ?? string.Empty
                        });
                    }

                    response.Status = 1;
                    response.Message = "Designations fetched successfully.";
                    response.ResponseData = designations;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("DesignationService", "GetDesignations", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDesignationById(long id, string clientId)
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

                #region Prepare SQL Query
                string query = "SELECT * FROM Designations WHERE DesignationID = @DID";
                SqlParameter param = new SqlParameter("@DID", id);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var designation = new Designations
                    {
                        DesignationID = Convert.ToInt64(dr["DesignationID"]),
                        Designation = dr["Designation"].ToString() ?? string.Empty
                    };

                    response.Status = 1;
                    response.Message = "Designation found.";
                    response.ResponseData = designation;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("DesignationService", "GetDesignationById", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="designation"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddDesignationAsync(Designations designation, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to add designation.",
                ResponseData = null
            };

            try
            {
                if (long.TryParse(designation.Designation, out _))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid input: Designation name cannot be a number.";
                    return response;
                }
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters = {
            new SqlParameter("@DName", designation.Designation)
        };
                #endregion

                #region Check Duplicate
                string checkDuplicateQuery = "SELECT COUNT(*) AS val FROM Designations WHERE Designation = @DName";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateQuery, parameters);

                int existingCount = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    existingCount = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }

                if (existingCount > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 2;
                    response.Message = "Designation already exists.";
                    return response;
                }
                #endregion

                #region Insert New Designation
                string insertQuery = "INSERT INTO Designations (Designation, Current_Session, SessionID) VALUES (@DName, 'NA', 0)";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);

 
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Designation added successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to add designation.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("DesignationService", "AddDesignationAsync", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="designation"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateDesignationAsync(Designations designation, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to update designation.",
                ResponseData = null
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters = {
            new SqlParameter("@DName", designation.Designation),
            new SqlParameter("@DID", designation.DesignationID)
        };
                #endregion

                #region Check Duplicate
                string checkDuplicateQuery = "SELECT COUNT(*) AS val FROM Designations WHERE Designation = @DName AND DesignationID != @DID";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateQuery, parameters);
                int existingCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    existingCount = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }

                if (existingCount > 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Designation already exists.";
                    return response;
                }
                #endregion

                #region Update Designation
                string updateQuery = "UPDATE Designations SET Designation = @DName WHERE DesignationID = @DID";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Designation updated successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to update designation.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("DesignationService", "UpdateDesignationAsync", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteDesignationAsync(long id, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to delete designation.",
                ResponseData = null
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters = {
            new SqlParameter("@DID", id)
        };
                #endregion

                #region Check If Designation Used
                string checkUsageQuery = "SELECT COUNT(*) AS val FROM EmployeeDetail WHERE DesignationID = @DID";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkUsageQuery, parameters);
                int usageCount = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    usageCount = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }

                if (usageCount > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Cannot delete designation because it is assigned to employees.";
                    return response;
                }
                #endregion

                #region Delete Designation
                string deleteQuery = "DELETE FROM Designations WHERE DesignationID = @DID";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Designation deleted successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to delete designation.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("DesignationService", "DeleteDesignationAsync", ex.ToString());
            }

            return response;
        }

    }
}
