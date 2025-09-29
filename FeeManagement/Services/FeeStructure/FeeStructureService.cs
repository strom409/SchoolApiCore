using FeeManagement.Repository;
using FeeManagement.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FeeManagement.Services.FeeStructure
{
    public class FeeStructureService : IFeeStructureService
    {
        private readonly IConfiguration _configuration;
        public FeeStructureService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddFeeStructure(FeeStructureDto dto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeStructure not added." };
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

                #region Prepare SQL Parameters
                var insertParams = new[]
                {
                    new SqlParameter("@FHIDFK", dto.FHIDFK ?? (object)DBNull.Value),
                    new SqlParameter("@CIDFK", dto.CIDFK ?? (object)DBNull.Value),
                    new SqlParameter("@CSession", dto.CSession ?? string.Empty),
                    new SqlParameter("@Amount", dto.Amount ?? (object)DBNull.Value),
                    new SqlParameter("@UserName", dto.UserName ?? string.Empty),
                    new SqlParameter("@IsDeleted", false),
                    new SqlParameter("@Title", dto.Title ?? string.Empty),
                    new SqlParameter("@Remarks", dto.Remarks ?? string.Empty)

                };
                #endregion

                #region Execute Insert Query
                string query = @"INSERT INTO FeeStructure 
                                (FHIDFK, CIDFK, CSession, Amount, UserName, Title,IsDeleted, Remarks)
                                VALUES
                                (@FHIDFK, @CIDFK, @CSession, @Amount, @UserName, @Title,0, @Remarks)";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, insertParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeStructure added successfully.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding FeeStructure.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeStructureService", "AddFeeStructure", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateFeeStructure(FeeStructureDto dto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeStructure not updated." };
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

                #region Prepare Update Parameters
                var updateParams = new[]
                {
                    new SqlParameter("@FSID", dto.FSID),
                    new SqlParameter("@FHIDFK", dto.FHIDFK ?? (object)DBNull.Value),
                    new SqlParameter("@CIDFK", dto.CIDFK ?? (object)DBNull.Value),
                    new SqlParameter("@CSession", dto.CSession ?? string.Empty),
                    new SqlParameter("@Amount", dto.Amount ?? (object)DBNull.Value),
                    new SqlParameter("@UpdateOn", DateTime.Now),
                    new SqlParameter("@UpdateBy", dto.UpdateBy ?? string.Empty),
                    new SqlParameter("@Title", dto.Title ?? string.Empty),
                    new SqlParameter("@Remarks", dto.Remarks ?? string.Empty),

                };
                #endregion

                #region Execute Update Query
                string query = @"UPDATE FeeStructure SET 
                                FHIDFK = @FHIDFK,
                                CIDFK = @CIDFK,
                                CSession = @CSession,
                                Amount = @Amount,
                                UpdateOn = @UpdateOn,
                                UpdateBy = @UpdateBy,
                                Title = @Title,
                                Remarks = @Remarks
                                
                                WHERE FSID = @FSID AND IsDeleted = 0";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, updateParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeStructure updated successfully.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error updating FeeStructure.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeStructureService", "UpdateFeeStructure", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllFeeStructures(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No FeeStructures found." };
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

                #region Execute Select Query
                string query = "SELECT * FROM FeeStructure WHERE IsDeleted = 0";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                var list = new List<FeeStructureDto>();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new FeeStructureDto
                        {
                            FSID = row.Field<long>("FSID"),
                            FHIDFK = row.Field<long?>("FHIDFK"),
                            CIDFK = row.Field<long?>("CIDFK"),
                            CSession = row.Field<string>("CSession"),
                            Amount = row.Field<decimal?>("Amount"),
                            UserName = row.Field<string>("UserName"),
                            UpdateOn = row.Field<DateTime?>("UpdateOn"),
                            UpdateBy = row.Field<string>("UpdateBy"),
                            IsDeleted = row.Field<bool?>("IsDeleted"),
                            Title = row.Field<string>("Title"),
                            Remarks = row.Field<string>("Remarks"),
                            SecIDFK = row.Field<long?>("SecIDFK"),
                            FeeCatID = row.Field<long?>("FeeCatID"),
                           // FeeSID = row.Field<long?>("FeeSID"),
                           // Rate = row.Field<decimal?>("Rate")
                        });
                    }
                }

                response.IsSuccess = true;
                response.Status = 1;
                response.Message = "FeeStructures retrieved successfully.";
                response.ResponseData = list;
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching FeeStructures.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeStructureService", "GetAllFeeStructures", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetFeeStructureById(long fsId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeStructure not found." };
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

                #region Execute Select By ID
                string query = "SELECT * FROM FeeStructure WHERE FSID = @FSID AND IsDeleted = 0";
                var sqlParams = new[] { new SqlParameter("@FSID", fsId) };
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var row = ds.Tables[0].Rows[0];
                    var feeStruct = new FeeStructureDto
                    {
                        FSID = row.Field<long>("FSID"),
                        FHIDFK = row.Field<long?>("FHIDFK"),
                        CIDFK = row.Field<long?>("CIDFK"),
                        CSession = row.Field<string>("CSession"),
                        Amount = row.Field<decimal?>("Amount"),
                        UserName = row.Field<string>("UserName"),
                        UpdateOn = row.Field<DateTime?>("UpdateOn"),
                        UpdateBy = row.Field<string>("UpdateBy"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        Title = row.Field<string>("Title"),
                        Remarks = row.Field<string>("Remarks"),
                        SecIDFK = row.Field<long?>("SecIDFK"),
                        FeeCatID = row.Field<long?>("FeeCatID"),
                        FeeSID = row.Field<long?>("FeeSID"),
                        Rate = row.Field<decimal?>("Rate")
                    };

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeStructure retrieved successfully.";
                    response.ResponseData = feeStruct;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeStructure.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeStructureService", "GetFeeStructureById", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cIDFK"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetFeeStructuresByClassId(long cIDFK, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No FeeStructures found for this Class." };
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

                #region Execute Select By ClassID (CIDFK)
                string query = "SELECT * FROM FeeStructure WHERE CIDFK = @CIDFK AND IsDeleted = 0";
                var sqlParams = new[] { new SqlParameter("@CIDFK", cIDFK) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeStructures = ds.Tables[0].AsEnumerable().Select(row => new FeeStructureDto
                    {
                        FSID = row.Field<long>("FSID"),
                        FHIDFK = row.Field<long?>("FHIDFK"),
                        CIDFK = row.Field<long?>("CIDFK"),
                        CSession = row.Field<string>("CSession"),
                        Amount = row.Field<decimal?>("Amount"),
                        UserName = row.Field<string>("UserName"),
                        UpdateOn = row.Field<DateTime?>("UpdateOn"),
                        UpdateBy = row.Field<string>("UpdateBy"),
                        IsDeleted = row.Field<bool?>("IsDeleted"),
                        Title = row.Field<string>("Title"),
                        Remarks = row.Field<string>("Remarks"),
                        SecIDFK = row.Field<long?>("SecIDFK"),
                        FeeCatID = row.Field<long?>("FeeCatID"),
                        FeeSID = row.Field<long?>("FeeSID"),
                        Rate = row.Field<decimal?>("Rate")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeStructures retrieved successfully.";
                    response.ResponseData = feeStructures;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeStructures.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeStructureService", "GetFeeStructuresByClassId", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsId"></param>
        /// <param name="updateBy"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteFeeStructure(long fsId, string updateBy, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeStructure not deleted." };

            try
            {
                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Soft Delete Query
                string query = @"
        UPDATE FeeStructure 
        SET IsDeleted = 1, 
            UpdateOn = GETDATE(),
            UpdateBy = @UpdateBy
        WHERE FSID = @FSID AND IsDeleted = 0"; // only update if not already deleted

                var sqlParams = new[]
                {
            new SqlParameter("@FSID", fsId),
            new SqlParameter("@UpdateBy", updateBy ?? string.Empty)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, sqlParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeStructure deleted successfully (soft delete).";
                }
                else
                {
                    response.Message = "FeeStructure not found or already deleted.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error deleting FeeStructure.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeStructureService", "DeleteFeeStructure",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | FSID: {fsId}"
                );

                return response;
                #endregion
            }
        }


    }
}
