using login.Repository;
using login.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Login.Services.Users.UserAccessManagement
{
    public class UserAccessServices:IUserAccessServices
    {
        private readonly IConfiguration _configuration;
        public UserAccessServices(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public async Task<ResponseModel> AddToUserAccessAsync(UserAccessDto request, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "User Access not added!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid client connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@UIDFK", request.UIDFK),
            new SqlParameter("@UserName", request.UserName),
            new SqlParameter("@MasterIDs", Encryption.Encrypt(request.MasterIDs, Encryption.key)),
            new SqlParameter("@PageIDs", Encryption.Encrypt(request.PageIDs, Encryption.key))
        };
                #endregion

                #region Check Duplicate (using DataSet)
                string checkDuplicateQuery = "SELECT COUNT(*) AS Val FROM UserAccess WHERE UIDFK=@UIDFK";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateQuery, parameters);

                int duplicate = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    duplicate = Convert.ToInt32(ds.Tables[0].Rows[0]["Val"]);
                }
                #endregion

                #region Insert Query
                if (duplicate == 0)
                {
                    string insertQuery = @"INSERT INTO UserAccess (UIDFK, UserName, MasterIDs, PageIDs) 
                                   VALUES (@UIDFK, @UserName, @MasterIDs, @PageIDs)";

                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        response.IsSuccess = true;
                        response.Status = 1;
                        response.Message = "User Access added successfully.";
                    }
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "User Access already exists for this user.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -500;
                response.Message = "Exception occurred while adding user access.";
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserAccessService", "AddToUserAccess", ex.ToString());
            }

            return response;
        }
        public async Task<ResponseModel> UpdateUserAccessAsync(UserAccessDto request, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "User Access not updated!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid client connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            //new SqlParameter("@ID", request.ID),
            new SqlParameter("@UIDFK", request.UIDFK),
            //new SqlParameter("@UserName", request.UserName),
            new SqlParameter("@MasterIDs", Encryption.Encrypt(request.MasterIDs, Encryption.key)),
            new SqlParameter("@PageIDs", Encryption.Encrypt(request.PageIDs, Encryption.key))
        };
                #endregion

                #region Update Query
                string updateQuery = @"UPDATE UserAccess 
                               SET MasterIDs=@MasterIDs, PageIDs=@PageIDs 
                               WHERE UIDFK=@UIDFK";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "User Access updated successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -500;
                response.Message = "Exception occurred while updating user access.";
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserAccessService", "UpdateUserAccess", ex.ToString());
            }

            return response;
        }
        public async Task<ResponseModel> DeleteUserAccessAsync(string uIDFK, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "User Access not deleted!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid client connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@UIDFK", uIDFK)
        };
                #endregion

                #region Delete Query
                string deleteQuery = @"DELETE FROM UserAccess WHERE UIDFK=@UIDFK";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "User Access deleted successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -500;
                response.Message = "Exception occurred while deleting user access.";
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserAccessService", "DeleteUserAccess", ex.ToString());
            }

            return response;
        }

        public async Task<ResponseModel> GetUserTypesAsync(string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data!"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid client connection string.";
                    return response;
                }
                #endregion

                #region Query
                string query = "SELECT UTID, UTName FROM UserTypes";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var userTypes = ds.Tables[0].AsEnumerable().Select(row => new UserTypeDto
                    {
                        UTID = row.Field<long>("UTID"),
                        UTName = row.Field<string>("UTName")
                    }).ToList();

                    response.Status = 1;
                    response.Message = "Success";
                    response.ResponseData = userTypes;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -500;
                response.Message = "Error occurred while fetching user types.";
               login.Repository.Error.ErrorBLL.CreateErrorLog("UserTypeService", "GetUserTypesAsync", ex.ToString());
            }

            return response;
        }


    }
}
