using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Student.Repository.SQL;
using Student.Repository;
using System.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Test.Services.User;

namespace Student.Services.User
{
    public class LoginServices : ILoginService
    {
        private readonly IConfiguration _configuration;

        public LoginServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<ResponseModel> LoginAsync(LoginDto request)
        {
            var response = new ResponseModel()
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid Creditionals.",
            };

            try
            {
                string connectionString = GetConnectionString(request.ClientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid ClientId.";
                    return response;
                }


                string encryptedUserName = Encryption.Encrypt(request.UserName, Encryption.key);
                string encryptedPassword = Encryption.Encrypt(request.UserPassword, Encryption.key);

                string query = @"SELECT * FROM Users 
                         WHERE UserName = @UserName AND UserPassword = @UserPassword AND Activation = 1";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@UserPassword", encryptedPassword)
                };

                DataSet ds = new DataSet();
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters);
                        using (var adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(ds);
                        }
                    }
                }

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    response.Message = "Invalid username or password.";
                    return response;
                }

                DataRow dr = ds.Tables[0].Rows[0];

                // Populate full user
                UserDTO Us = new UserDTO
                {
                    UserID = Convert.ToInt64(dr["UserID"]),
                    UserName = Encryption.Decrypt(dr["UserName"].ToString(), Encryption.key),
                    // UserPassword = Encryption.Decrypt(dr["UserPassword"].ToString(), Encryption.key),
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

                #region GET PAGE MASTERS AND PAGES
                UserAccessDto UA = await getUserAccessDataAsync(dr["UserID"].ToString(), connectionString);
                Us.MasterIDs = UA.MasterIDs;
                Us.PageIDs = UA.PageIDs;
                #endregion

                #region CREATE GWT TOKEN
                string token = await Repository.GenerateToken.CreateToken(connectionString, Us.UserID.ToString(), request.ClientId, _configuration);

                Us.Token = token;

                #endregion

                if (!string.IsNullOrWhiteSpace(Us.UserID.ToString()))
                {
                   
                    response.Status = 1;
                    response.Message = "Login successful.";
                    response.ResponseData = Us;
                }
                return response;
                
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("UserService", "LoginAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");

                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred during login.";
                response.Error = ex.Message;
                return response;
            }

        }
        public static async Task<UserAccessDto> getUserAccessDataAsync(string UID, string connectionString)
        {
            UserAccessDto ua = new UserAccessDto();
            SqlParameter p = new SqlParameter("@UID", UID);
            string sql = "SELECT * FROM UserAccess WHERE UIDFK = @UID";

            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, p);

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

            return ua;
        }

        //public static async Task<UserAccessDto> getUserAccessDataAsync(string UID)
        //{
        //    UserAccessDto ua = new UserAccessDto();
        //    SqlParameter p = new SqlParameter("@UID", UID);
        //    string sql = "SELECT * FROM UserAccess WHERE UIDFK = @UID";

        //    DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sql, p);

        //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //    {
        //        DataRow dr = ds.Tables[0].Rows[0];
        //        ua.MasterIDs = Encryption.Decrypt(dr["MasterIDs"].ToString(), Encryption.key);
        //        ua.PageIDs = Encryption.Decrypt(dr["PageIDs"].ToString(), Encryption.key);
        //        ua.UserName = dr["UserName"].ToString();
        //        ua.ID = Convert.ToInt64(dr["ID"]);
        //        ua.UIDFK = Convert.ToInt64(dr["UIDFK"]);
        //    }
        //    else
        //    {
        //        ua.MasterIDs = "0";
        //        ua.PageIDs = "0";
        //        ua.ID = 0;
        //        ua.UIDFK = 0;
        //    }

        //    return ua;
        //}

        private string GetConnectionString(string clientId)
        {
            // Lowercase to handle case-insensitive client id
            switch (clientId?.ToLower())
            {
                case "client1":
                    return _configuration.GetConnectionString("Client1");
                case "client2":
                    return _configuration.GetConnectionString("Client2");
                // Add more clients here as needed
                default:
                    return null; // Unknown client
            }
        }

        public Task<ResponseModel> CompareFacesAsync(CompareFacesRequestDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
