using login.Repository;
using login.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Login.Services.HT
{
    public class HTService:IHTService
    {
        private readonly IConfiguration _configuration;
        public HTService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getHT(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Setup
                string query = "SELECT * FROM HT";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, null);
                #endregion

                #region Map Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var ht = new HTModel
                    {
                        HID = Convert.ToInt64(dr["HID"]),
                        Name = Encryption.Decrypt(dr["Name"].ToString(), Encryption.key),
                        LiveSession = dr["LiveSession"]?.ToString(),
                        LiveYear = dr["LiveYear"]?.ToString().Trim(),
                        FYear = dr["FYear"]?.ToString(),
                        Address = Encryption.Decrypt(dr["Address"].ToString(), Encryption.key),
                        PhoneNo = Encryption.Decrypt(dr["PhoneNo"].ToString(), Encryption.key)
                    };

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ht;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                login.Repository.Error.ErrorBLL.CreateErrorLog("HTService", "getHT", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="htData"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> UpdateHT(HTModel htData, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Update failed!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Setup
                string query = @"
            UPDATE HT
            SET 
                Name = @Name,
                Address = @Address,
                PhoneNo = @PhoneNo,
                Email = @Email,
                FYear = @FYear,
                LiveSession = @LiveSession,
                LiveYear = @LiveYear
            WHERE HID = @HID";

                SqlParameter[] parameters = {
            new SqlParameter("@HID", htData.HID),
            new SqlParameter("@Name", Encryption.Encrypt(htData.Name, Encryption.key)),
            new SqlParameter("@Address", Encryption.Encrypt(htData.Address, Encryption.key)),
            new SqlParameter("@PhoneNo", Encryption.Encrypt(htData.PhoneNo, Encryption.key)),
            new SqlParameter("@Email", Encryption.Encrypt(htData.Email, Encryption.key)),
           // new SqlParameter("@Website", Encryption.Encrypt(htData.Website, Encryption.key)),
            new SqlParameter("@FYear", htData.FYear),
           // new SqlParameter("@DFrom", string.IsNullOrEmpty(htData.DFrom?.ToString()) ? DateTime.Now.Date : htData.DFrom),
           // new SqlParameter("@DTo", string.IsNullOrEmpty(htData.DTo?.ToString()) ? DateTime.Now.Date : htData.DTo),
            //new SqlParameter("@UserName", htData.UserName),
            new SqlParameter("@LiveSession", htData.LiveSession),
            new SqlParameter("@LiveYear", htData.LiveYear)
        };
                #endregion

                #region Execute Update
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Result
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated successfully!";
                }
                else
                {
                    response.Status = 0;
                    response.Message = "No rows updated.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
               login.Repository.Error.ErrorBLL.CreateErrorLog("HTService", "UpdateHT", ex.ToString());

                #endregion
            }

            return response;
        }



    }
}
