using login.Repository;
using login.Repository.SQL;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace Login.Services.Login
{
    public class LoginServices : ILoginService
    {
        private readonly IConfiguration _configuration;

        public LoginServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResponseModel> LoginAsync(LoginDto request)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid credentials."
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(request.ClientId);
                //    string connectionString = GetConnectionString(request.ClientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid ClientId.";
                    return response;
                }
                #endregion

                #region Validate Static Key
                string staticKey = _configuration["ApiSettings:StaticJwt"];
                if (string.IsNullOrWhiteSpace(staticKey) || request.Key != staticKey)
                {

                    response.Message = "Invalid  key.";
                    return response;
                }
                #endregion

                #region Encrypt Credentials
                string encryptedUserName = Encryption.Encrypt(request.UserName, Encryption.key);
                string encryptedPassword = Encryption.Encrypt(request.UserPassword, Encryption.key);
                #endregion

                #region Prepare SQL and Parameters
                string query = @"
SELECT * FROM Users 
WHERE UserName = @UserName 
  AND UserPassword = @UserPassword 
  AND Activation = 1";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@UserPassword", encryptedPassword)
                };
                #endregion

                #region Execute SQL
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Validate User
                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid username or password.";
                    return response;
                }

                DataRow dr = ds.Tables[0].Rows[0];

                UserDTO user = new UserDTO
                {
                    UserID = Convert.ToInt64(dr["UserID"]),
                    UserName = Encryption.Decrypt(dr["UserName"].ToString(), Encryption.key),
                    UserFullName = dr["UserFullName"].ToString(),
                    UserLogoPath = dr["UserLogoPath"].ToString(),
                    UserPhoneNo = dr["UserPhoneNo"].ToString(),
                    UserRemarks = dr["UserRemarks"].ToString(),
                    UserTypeID = Convert.ToInt64(dr["UserTypeID"]),
                    UserAddress = dr["UserAddress"].ToString(),
                    UserEmail = dr["UserEmail"].ToString(),
                    Current_Session = dr["Current_Session"].ToString(),
                    Activation = true,
                    Control1Id = dr["Control1Id"].ToString(),
                    ControlId = dr["ControlId"].ToString(),
                    Dashboard = dr["Dashboard"].ToString()
                };
                #endregion

                #region Get Page Access
                UserAccessDto access = await getUserAccessDataAsync(dr["UserID"].ToString(), connectionString);
                user.MasterIDs = access.MasterIDs;
                user.PageIDs = access.PageIDs;
                #endregion

                #region Generate Token
                string token = await GenerateToken.CreateToken(connectionString, user.UserID.ToString(), request.ClientId, _configuration);
                user.Token = token;
                #endregion

                #region Final Response
                response.Status = 1;
                response.Message = "Login successful.";
                response.ResponseData = user;
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserService", "LoginAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during login.";
                response.Error = ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="UID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static async Task<UserAccessDto> getUserAccessDataAsync(string UID, string connectionString)
        {
            #region Variable Declaration and SQL Setup
            UserAccessDto ua = new UserAccessDto();
            SqlParameter p = new SqlParameter("@UID", UID);
            string sql = "SELECT * FROM UserAccess WHERE UIDFK = @UID";
            #endregion

            #region Execute SQL and Fetch Data
            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, p);
            #endregion

            #region Populate DTO from DataRow
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                ua.MasterIDs = Encryption.Decrypt(dr["MasterIDs"].ToString(), Encryption.key);
                ua.PageIDs = Encryption.Decrypt(dr["PageIDs"].ToString(), Encryption.key);
                ua.UserName = dr["UserName"].ToString();
                ua.ID = Convert.ToInt64(dr["ID"]);
                ua.UIDFK = Convert.ToInt64(dr["UIDFK"]);
            }
            else
            {
                ua.MasterIDs = "0";
                ua.PageIDs = "0";
                ua.ID = 0;
                ua.UIDFK = 0;
            }
            #endregion

            #region Return Result
            return ua;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ValidateToken(string token, string clientId)
        {
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid or expired token."
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid ClientId.";
                    return response;
                }

                string query = @"
            SELECT * FROM tokens WHERE TokenID = @TokenID AND IsExpired = 0 AND ExpiresOn > GETUTCDATE()";

                SqlParameter[] parameters = new SqlParameter[]
                {
            new SqlParameter("@TokenID", token)
                };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid or expired token.";
                    return response;
                }

                string userId = ds.Tables[0].Rows[0]["UserName"].ToString();

                // Fetch user details
                string userQuery = "SELECT * FROM Users WHERE UserID = @UserID AND Activation = 1";

                SqlParameter[] userParams = new SqlParameter[]
                {
            new SqlParameter("@UserID", userId)
                };

                DataSet userDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, userQuery, userParams);

                if (userDs.Tables.Count == 0 || userDs.Tables[0].Rows.Count == 0)
                {
                    response.Message = "User not found.";
                    return response;
                }

                DataRow dr = userDs.Tables[0].Rows[0];
                UserDTO user = new UserDTO
                {
                    UserID = Convert.ToInt64(dr["UserID"]),
                    UserName = Encryption.Decrypt(dr["UserName"].ToString(), Encryption.key),
                    UserFullName = dr["UserFullName"].ToString(),
                    UserLogoPath = dr["UserLogoPath"].ToString(),
                    UserPhoneNo = dr["UserPhoneNo"].ToString(),
                    UserRemarks = dr["UserRemarks"].ToString(),
                    UserTypeID = Convert.ToInt64(dr["UserTypeID"]),
                    UserAddress = dr["UserAddress"].ToString(),
                    UserEmail = dr["UserEmail"].ToString(),
                    Current_Session = dr["Current_Session"].ToString(),
                    Activation = true,
                    Control1Id = dr["Control1Id"].ToString(),
                    ControlId = dr["ControlId"].ToString(),
                    Dashboard = dr["Dashboard"].ToString()
                };

                UserAccessDto access = await getUserAccessDataAsync(userId, connectionString);
                user.MasterIDs = access.MasterIDs;
                user.PageIDs = access.PageIDs;
                user.Token = token; // Already provided token

                response.Status = 1;
                response.Message = "Token is valid.";
                response.ResponseData = user;
                return response;
            }
            catch (Exception ex)
            {
                login.Repository.Error.ErrorBLL.CreateErrorLog("UserService", "ValidateTokenAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Token: {token} | ClientId: {clientId}");

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during token validation.";
                response.Error = ex.Message;
                return response;
            }
        }
        //private string GetConnectionString(string clientId)
        //{
        //    Lowercase to handle case -insensitive client id
        //    switch (clientId?.ToLower())
        //        {
        //            case "client1":
        //                return _configuration.GetConnectionString("Client1");
        //            case "client2":
        //                return _configuration.GetConnectionString("Client2");
        //                Add more clients here as needed
        //        default:
        //                return null; // Unknown client
        //        }
        //    }

    }
}
