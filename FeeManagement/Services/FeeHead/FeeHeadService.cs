using FeeManagement.Repository;
using FeeManagement.Repository.SQL;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace FeeManagement.Services.FeeHead
{
    public class FeeHeadService : IFeeHeadService
    {
        private readonly IConfiguration _configuration;
        public FeeHeadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ResponseModel> AddFeeHead(FeeHeadDto request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeHead Not Added!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Validate Request
                if (string.IsNullOrWhiteSpace(request.FHName))
                {
                    response.Message = "FeeHead Name is required.";
                    return response;
                }
                #endregion

                #region Duplicate Check
                var dupParams = new[] { new SqlParameter("@FHName", request.FHName ?? string.Empty) };
                string dupQuery = "SELECT COUNT(*) FROM FeeHeads WHERE FHName = @FHName AND IsDeleted = 0";

                DataSet dsDup = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, dupParams);
                int exists = dsDup?.Tables[0].Rows.Count > 0 ? Convert.ToInt32(dsDup.Tables[0].Rows[0][0]) : 0;

                if (exists > 0)
                {
                    response.Status = 0;
                    response.Message = "FeeHead name already exists!";
                    return response;
                }
                #endregion

                #region Insert FeeHead
                string insertQuery = @"
            INSERT INTO FeeHeads
            (FHName, FHType, UserName, IsDeleted, UpdatedOn, UpdatedBy, Frate, isprimay)
            VALUES
            (@FHName, @FHType, @UserName, 0, @UpdatedOn, @UpdatedBy, @Frate, @isprimay)";

                var insertParams = new[]
                {
            new SqlParameter("@FHName", request.FHName ?? string.Empty),
            new SqlParameter("@FHType", request.FHType),
            new SqlParameter("@UserName", request.UserName ?? string.Empty),
            new SqlParameter("@UpdatedOn", DateTime.Now),
            new SqlParameter("@UpdatedBy", request.UpdatedBy ?? string.Empty),
            new SqlParameter("@Frate", request.Frate),
            new SqlParameter("@isprimay", request.IsPrimary)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHead Added Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding FeeHead";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "AddFeeHead",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");
                return response;
                #endregion
            }
        }

        public async Task<ResponseModel> UpdateFeeHead(FeeHeadDto request, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeHead Not Updated!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Validate Request
                if (request.FHID <= 0)
                {
                    response.Message = "Invalid FeeHead ID.";
                    return response;
                }

                if (string.IsNullOrWhiteSpace(request.FHName))
                {
                    response.Message = "FeeHead Name is required.";
                    return response;
                }
                #endregion

                #region Duplicate Check
                var dupParams = new[]
                {
            new SqlParameter("@FHName", request.FHName ?? string.Empty),
            new SqlParameter("@FHID", request.FHID)
        };
                string dupQuery = "SELECT COUNT(*) FROM FeeHeads WHERE FHName = @FHName AND FHID <> @FHID AND IsDeleted = 0";

                DataSet dsDup = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, dupParams);
                int exists = dsDup?.Tables[0].Rows.Count > 0 ? Convert.ToInt32(dsDup.Tables[0].Rows[0][0]) : 0;

                if (exists > 0)
                {
                    response.Status = 0;
                    response.Message = "FeeHead name already exists!";
                    return response;
                }
                #endregion

                #region Update FeeHead
                string updateQuery = @"
            UPDATE FeeHeads
            SET FHName = @FHName,
                FHType = @FHType,
                UserName = @UserName,
                UpdatedOn = @UpdatedOn,
                UpdatedBy = @UpdatedBy,
                Frate = @Frate,
                isprimay = @isprimay
            WHERE FHID = @FHID";

                var updateParams = new[]
                {
            new SqlParameter("@FHID", request.FHID),
            new SqlParameter("@FHName", request.FHName ?? string.Empty),
            new SqlParameter("@FHType", request.FHType),
            new SqlParameter("@UserName", request.UserName ?? string.Empty),
            new SqlParameter("@UpdatedOn", DateTime.Now),
            new SqlParameter("@UpdatedBy", request.UpdatedBy ?? string.Empty),
            new SqlParameter("@Frate", request.Frate),
            new SqlParameter("@IsPrimay", request.IsPrimary)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, updateParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHead Updated Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error updating FeeHead";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "UpdateFeeHead",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");

                return response;
                #endregion
            }
        }

        public async Task<ResponseModel> GetFeeHeadById(long fHID, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeHead not found." };
            #endregion

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Query FeeHead
                string query = "SELECT * FROM FeeHeads WHERE FHID = @FHID AND IsDeleted = 0";
                var sqlParams = new[] { new SqlParameter("@FHID", fHID) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    var feeHead = new FeeHeadDto
                    {
                        FHID = row.Field<long>("FHID"),
                        FHName = row.Field<string>("FHName"),
                        FHType = row.Field<int>("FHType"),
                        UserName = row.Field<string>("UserName"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn"),
                        UpdatedBy = row.Field<string>("UpdatedBy"),
                        Frate = row.Field<decimal>("Frate"),
                        IsPrimary = row.Field<int>("IsPrimay")
                    };

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHead retrieved successfully.";
                    response.ResponseData = feeHead;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeHead.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "GetFeeHeadById",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | FHID: {fHID}");

                return response;
                #endregion
            }
        }

        public async Task<ResponseModel> GetFeeHeadsByType(int fHType, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No FeeHeads found for this type." };
            #endregion

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Query FeeHeads by Type
                string query = "SELECT * FROM FeeHeads WHERE FHType = @FHType AND IsDeleted = 0";
                var sqlParams = new[] { new SqlParameter("@FHType", fHType) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeHeads = ds.Tables[0].AsEnumerable().Select(row => new FeeHeadDto
                    {
                        FHID = row.Field<long>("FHID"),
                        FHName = row.Field<string>("FHName"),
                        FHType = row.Field<int>("FHType"),
                        UserName = row.Field<string>("UserName"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn"),
                        UpdatedBy = row.Field<string>("UpdatedBy"),
                        Frate = row.Field<decimal>("Frate"),
                        IsPrimary = row.Field<int>("IsPrimay") // match your DTO type
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHeads retrieved successfully.";
                    response.ResponseData = feeHeads;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeHeads by type.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "GetFeeHeadsByType",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | FHType: {fHType}");

                return response;
                #endregion
            }
        }

        public async Task<ResponseModel> GetAllFeeHeads(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No FeeHeads Found." };
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

                #region Fetch FeeHeads
                string query = "SELECT * FROM FeeHeads WHERE IsDeleted = 0";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeHeads = ds.Tables[0].AsEnumerable().Select(r => new FeeHeadDto
                    {
                        FHID = r.Field<long>("FHID"),
                        FHName = r.Field<string>("FHName"),
                        FHType = r.Field<int>("FHType"),
                        UserName = r.Field<string>("UserName"),
                        IsDeleted = r.Field<bool>("IsDeleted"),
                        UpdatedOn = r.Field<DateTime?>("UpdatedOn"),
                        UpdatedBy = r.Field<string>("UpdatedBy"),
                        Frate = r.Field<decimal>("Frate"),
                        IsPrimary = r.Field<int>("IsPrimay")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHeads retrieved successfully.";
                    response.ResponseData = feeHeads;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeHeads.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "GetAllFeeHeads",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace}");

                return response;
                #endregion
            }
        }

        public async Task<ResponseModel> DeleteFeeHead(long fHID, string clientId, string updatedBy)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeHead Not Deleted!" };
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

                #region Delete FeeHead
                string query = "UPDATE FeeHeads SET IsDeleted = 1, UpdatedOn = GETDATE(), UpdatedBy = @UpdatedBy WHERE FHID = @FHID";

                var sqlParams = new[]
                {
            new SqlParameter("@FHID", fHID),
            new SqlParameter("@UpdatedBy", updatedBy ?? string.Empty)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, sqlParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeHead Deleted Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error deleting FeeHead.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeHeadService", "DeleteFeeHead",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | FHID: {fHID}");

                return response;
                #endregion
            }
        }

    }
}
