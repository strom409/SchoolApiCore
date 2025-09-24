using login.Repository;
using login.Repository.SQL;
using Login.Services.Login;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace Login.Services.Users
{
    public class UserService : IUserService
    {

        private readonly IConfiguration _configuration;
        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddUserAsync(RequestUserDto user, string clientId)
        {
            #region Initialize
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "User not added!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Validation
                if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.UserPassword))
                {
                    response.IsSuccess = false;
                    response.Message = "Required fields are missing (UserName, UserPassword).";
                    return response;
                }
                #endregion

                #region Duplicate Check
                string duplicateQuery = "SELECT COUNT(*) FROM Users WHERE UserName = @UserName";
                var duplicateParam = new SqlParameter("@UserName", user.UserName ?? string.Empty);
                var dsDup = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateQuery, new[] { duplicateParam });

                int exists = 0;
                if (dsDup != null && dsDup.Tables.Count > 0 && dsDup.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(dsDup.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Duplicate UserName not allowed!";
                    return response;
                }
                #endregion

                #region Insert User
                // Encrypt sensitive fields
                string encryptedUserName = Encryption.Encrypt(user.UserName ?? string.Empty, Encryption.key);
                string encryptedPassword = Encryption.Encrypt(user.UserPassword ?? string.Empty, Encryption.key);

                SqlParameter[] parameters =
                {
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@PW", encryptedPassword),
            new SqlParameter("@Activation", user.Activation),
            new SqlParameter("@ClassIDS", user.ClassIDS),
            new SqlParameter("@Current_Session", user.Current_Session ?? string.Empty),
            new SqlParameter("@Dashboard", user.Dashboard ?? string.Empty),
            new SqlParameter("@UserAddress", user.UserAddress ?? string.Empty),
            new SqlParameter("@UserEmail", user.UserEmail ?? string.Empty),
            new SqlParameter("@UserFullName", user.UserFullName ?? string.Empty),
            new SqlParameter("@UserLogoPath", user.UserLogoPath ?? string.Empty),
            new SqlParameter("@UserPhoneNo", user.UserPhoneNo ?? string.Empty),
            new SqlParameter("@UserRemarks", user.UserRemarks ?? string.Empty),
            new SqlParameter("@UserTypeID", user.UserTypeID)
        };

                string insertQuery = @"
        INSERT INTO Users 
            (UserName, UserPassword, Activation,ClassIDS, Current_Session, Dashboard, 
             UserAddress, UserEmail, UserFullName, UserLogoPath, UserPhoneNo, UserRemarks, UserTypeID) 
        VALUES 
            (@UserName, @PW, @Activation,@ClassIDS, @Current_Session, @Dashboard, 
             @UserAddress, @UserEmail, @UserFullName, @UserLogoPath, @UserPhoneNo, @UserRemarks, @UserTypeID);
        SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewUserId;";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, insertQuery, parameters);

                string userId = null;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    userId = ds.Tables[0].Rows[0]["NewUserId"].ToString();
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "User added successfully.";
                    response.ResponseData = new { UserId = userId };
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to retrieve UserId.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserService", "AddUserAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(user)}");

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while adding user.";
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateUserAsync(RequestUserDto user, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "User not updated!"
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                // Encrypt sensitive fields
                string encryptedUserName = Encryption.Encrypt(user.UserName ?? string.Empty, Encryption.key);
                string encryptedPassword = Encryption.Encrypt(user.UserPassword ?? string.Empty, Encryption.key);

                SqlParameter[] parameters =
                {
            new SqlParameter("@UserId", user.UserId),   
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@PW", encryptedPassword),
            new SqlParameter("@Activation", user.Activation),
            new SqlParameter("@Current_Session", user.Current_Session ?? string.Empty),
            new SqlParameter("@Dashboard", user.Dashboard ?? string.Empty),
            new SqlParameter("@UserAddress", user.UserAddress ?? string.Empty),
            new SqlParameter("@UserEmail", user.UserEmail ?? string.Empty),
            new SqlParameter("@UserFullName", user.UserFullName ?? string.Empty),
            new SqlParameter("@UserLogoPath", user.UserLogoPath ?? string.Empty),
            new SqlParameter("@UserPhoneNo", user.UserPhoneNo ?? string.Empty),
            new SqlParameter("@UserRemarks", user.UserRemarks ?? string.Empty)
            
        };

                string updateQuery = @"
            UPDATE Users
            SET 
                UserName = @UserName,
                UserPassword = @PW,
                Activation = @Activation,
                ClassIDS=@ClassIDS,
                Current_Session = @Current_Session,
                Dashboard = @Dashboard,
                UserAddress = @UserAddress,
                UserEmail = @UserEmail,
                UserFullName = @UserFullName,
                UserLogoPath = @UserLogoPath,
                UserPhoneNo = @UserPhoneNo,
                UserRemarks = @UserRemarks,
            WHERE UserId = @UserId";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    updateQuery,
                    parameters
                );

                response.IsSuccess = result > 0;
                response.Status = result > 0 ? 1 : 0;
                response.Message = result > 0 ? "User updated successfully." : "Failed to update user.";
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog(
                    "UserService",
                    "UpdateUserAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(user)}"
                );

                response = new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Message = "An error occurred while updating user.",
                    Error = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteUserAsync(int userId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "User not deleted!"
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                SqlParameter[] parameters =
                {
            new SqlParameter("@UserId", userId)
        };

                // Soft delete (set Activation = 0 instead of deleting record)
                string updateQuery = @"UPDATE Users 
                               SET Activation = 0 
                               WHERE UserId = @UserId AND Activation = 1";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    updateQuery,
                    parameters
                );

                response.IsSuccess = result > 0;
                response.Status = result > 0 ? 1 : 0;
                response.Message = result > 0 ? "User deleted (soft delete) successfully." : "User not found or already deleted.";
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog(
                    "UserService",
                    "DeleteUserAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | UserId: {userId}"
                );

                response = new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Message = "An error occurred while deleting user.",
                    Error = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ChangeUserPasswordAsync(RequestUserDto request, string clientId)
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

                #region Encrypt Credentials
                string encryptedUserName = Encryption.Encrypt(request.UserName, Encryption.key);
                string encryptedOldPassword = Encryption.Encrypt(request.OldPassword, Encryption.key);
                string encryptedNewPassword = Encryption.Encrypt(request.UserPassword, Encryption.key);
                #endregion

                #region Check Old Password
                SqlParameter[] checkParams =
                {
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@OldPassword", encryptedOldPassword)
        };

                string checkQuery = "SELECT COUNT(UserID) AS Val FROM Users WHERE UserName=@UserName AND UserPassword=@OldPassword";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);

                int check = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    check = Convert.ToInt32(ds.Tables[0].Rows[0]["Val"]);
                }

                if (check == 0)
                {
                    response.Message = "Old Password is incorrect!";
                    return response;
                }
                #endregion

                #region Update Password
                SqlParameter[] updateParams =
                {
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@PW", encryptedNewPassword)
        };

                string updateQuery = "UPDATE Users SET UserPassword=@PW WHERE UserName=@UserName";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, updateParams);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Password changed successfully.";
                }
                else
                {
                    response.Message = "Password not changed!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -500;
                response.Message = "Error occurred while changing password.";
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserService", "ChangeUserPasswordAsync", ex.ToString());
            }

            return response;
        }


    }
}
