using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Student.Repository.SQL;
using Student.Repository;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Student.Services.User
{
    public class UserService:IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ResponseModel> AddUserAsync(RequestUserDto user, string clientId)
        {
            var response = new ResponseModel();

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
            new SqlParameter("@UserName", encryptedUserName),
            new SqlParameter("@PW", encryptedPassword),
            new SqlParameter("@Activation", user.Activation),
            new SqlParameter("@Control1Id", user.Control1Id ?? string.Empty),
            new SqlParameter("@ControlId", user.ControlId ?? string.Empty),
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
                (UserName, UserPassword, Activation, Control1Id, ControlId, Current_Session, Dashboard, 
                 UserAddress, UserEmail, UserFullName, UserLogoPath, UserPhoneNo, UserRemarks, UserTypeID) 
            VALUES 
                (@UserName, @PW, @Activation, @Control1Id, @ControlId, @Current_Session, @Dashboard, 
                 @UserAddress, @UserEmail, @UserFullName, @UserLogoPath, @UserPhoneNo, @UserRemarks, @UserTypeID)";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);

                response.IsSuccess = result > 0;
                response.Status = result > 0 ? 1 : 0;
                response.Message = result > 0 ? "User added successfully." : "Failed to add user.";
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("UserService", "AddUserAsync",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(user)}");

                response = new ResponseModel
                {
                    IsSuccess = false,
                    Status = -1,
                    Message = "An error occurred while adding user.",
                    Error = ex.Message
                };
            }

            return response;
        }

    }
}
