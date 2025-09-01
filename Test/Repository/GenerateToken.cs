using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Student.Repository.Error;
using Student.Repository.SQL;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Student.Repository
{
    public class GenerateToken
    {

        public static async Task<string> CreateToken(string connectionString, string userId, string clientId, IConfiguration configuration)
        {
            try
            {
                // 1. Prepare JWT claims (add more claims if needed)
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId),
                // Add roles or other claims here if needed
                new Claim(ClaimTypes.Role, "User"),
                new Claim("ClientId", clientId)
                // example role
            };

                // 2. Get JWT settings from config
                var secretKey = configuration["JwtSettings:SecretKey"];
                var issuer = configuration["JwtSettings:Issuer"];
                var audience = configuration["JwtSettings:Audience"];
                var expirySeconds = Convert.ToDouble(configuration["AuthTokenExpiry"]);

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // 3. Create JWT token
                var tokenDescriptor = new JwtSecurityToken(
                    issuer,
                    audience,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: creds);

                string jwtToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

                // 4. Save JWT token and metadata to database
                string issuedOn = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                string deleteDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime expiredOn = DateTime.UtcNow.AddSeconds(expirySeconds);

                SqlParameter[] param =
                {
                new SqlParameter("@TokenID", jwtToken),
                new SqlParameter("@UserName", userId),
                new SqlParameter("@IssuedOn", issuedOn),
                new SqlParameter("@ExpiresOn", expiredOn),
                new SqlParameter("@deleteDate", deleteDate),
                new SqlParameter("@IsExpired", "0"),
            };

                string sql = "DELETE FROM tokens WHERE IssuedOn <= @deleteDate; " +
                             "INSERT INTO tokens (TokenID, UserName, IssuedOn, ExpiresOn, IsExpired) " +
                             "VALUES (@TokenID, @UserName, @IssuedOn, @ExpiresOn, @IsExpired)";
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql, param);

                //int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString ,SQLHelperCore.Connect, CommandType.Text, sql, param);

                // 5. Return JWT token if inserted successfully, else "NA"
                return result > 0 ? jwtToken : "NA";
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    Title = "Token Generation Error",
                    PageName = "GenerateToken.CreateToken",
                    Error = ex.ToString()
                };
                ErrorBLL.CreateErrorLog(log);
                return "NA";
            }
        }


        public static async Task<bool> IsTokenValid(string tokenId)
        {
            try
            {
                SqlParameter param = new SqlParameter("@TokenNo", tokenId);
                string sql = "SELECT ExpiresOn FROM tokens WHERE TokenID = @TokenNo";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sql, param);

                if (ds.Tables[0].Rows.Count == 0)
                    return false;

                DateTime expiresOn = Convert.ToDateTime(ds.Tables[0].Rows[0]["ExpiresOn"]);
                return expiresOn > DateTime.Now;
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    Title = "Token Validation Error",
                    PageName = "GenerateToken.IsTokenValid",
                    Error = ex.ToString()
                };
                ErrorBLL.CreateErrorLog(log);
                return false;
            }
        }
    }
}
