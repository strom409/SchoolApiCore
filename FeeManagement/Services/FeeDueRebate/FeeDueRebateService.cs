using FeeManagement.Repository;
using FeeManagement.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FeeManagement.Services.FeeDueRebate
{
    public class FeeDueRebateService:IFeeDueRebateService
    {
        private readonly IConfiguration _configuration;
        public FeeDueRebateService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        public async Task<ResponseModel> AddFeeDueRebate(FeeDueRebateDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeDueRebate not added!" };

            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                string query = @"INSERT INTO FeeDueRebate
                                (FeeDueIDFK, FeeHeadIDFK, FeeCatgIDFK, ClassIDFK, SectionIDFK, StudentIDFK, AdmissionNoIDFK,
                                 IsYearly, IsFlat, FlatAmount, RebatePercent, RebatePercentAmount, OnMonth, MonthIDFK, IsDeleted, Status,
                                 Current_Session, UserName, OnDate, Remarks)
                                 VALUES
                                (@FeeDueIDFK, @FeeHeadIDFK, @FeeCatgIDFK, @ClassIDFK, @SectionIDFK, @StudentIDFK, @AdmissionNoIDFK,
                                 @IsYearly, @IsFlat, @FlatAmount, @RebatePercent, @RebatePercentAmount, @OnMonth, @MonthIDFK,
                                 0, @Status, @Current_Session, @UserName, GETDATE(), @Remarks)";

                var parameters = new[]
                {
                    new SqlParameter("@FeeDueIDFK", request.FeeDueIDFK),
                    new SqlParameter("@FeeHeadIDFK", request.FeeHeadIDFK),
                    new SqlParameter("@FeeCatgIDFK", request.FeeCatgIDFK),
                    new SqlParameter("@ClassIDFK", request.ClassIDFK),
                    new SqlParameter("@SectionIDFK", request.SectionIDFK),
                    new SqlParameter("@StudentIDFK", request.StudentIDFK),
                    new SqlParameter("@AdmissionNoIDFK", request.AdmissionNoIDFK),
                    new SqlParameter("@IsYearly", request.IsYearly),
                    new SqlParameter("@IsFlat", request.IsFlat),
                    new SqlParameter("@FlatAmount", (object?)request.FlatAmount ?? DBNull.Value),
                    new SqlParameter("@RebatePercent", (object?)request.RebatePercent ?? DBNull.Value),
                    new SqlParameter("@RebatePercentAmount", (object?)request.RebatePercentAmount ?? DBNull.Value),
                    new SqlParameter("@OnMonth", request.OnMonth ?? (object)DBNull.Value),
                    new SqlParameter("@MonthIDFK", request.MonthIDFK),
                    new SqlParameter("@IsDeleted", request.IsDeleted),
                    new SqlParameter("@Status", request.Status),
                    new SqlParameter("@Current_Session", request.Current_Session ?? (object)DBNull.Value),
                    new SqlParameter("@UserName", request.UserName ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", request.Remarks ?? (object)DBNull.Value)
                };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeDueRebate added successfully.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while adding FeeDueRebate.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateService", "AddFeeDueRebate", ex.ToString());
            }

            return response;
        }

        public async Task<ResponseModel> UpdateFeeDueRebate(FeeDueRebateDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeDueRebate not updated!" };

            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                    return new ResponseModel { IsSuccess = false, Message = "Invalid client ID." };

                string query = @"UPDATE FeeDueRebate SET 
                                    
                                    FlatAmount=@FlatAmount,
                                    OnMonth=@OnMonth,
                                    MonthIDFK=@MonthIDFK,
                                    Current_Session=@Current_Session,
                                    UbdatedBy=@UbdatedBy,
                                    UpdatedOn=GETDATE(),
                                    Remarks=@Remarks
                                WHERE RebateID=@RebateID";

                var parameters = new[]
                {
                    //new SqlParameter("@RebateID", request.RebateID),
                    //new SqlParameter("@FeeDueIDFK", request.FeeDueIDFK),
                    //new SqlParameter("@FeeHeadIDFK", request.FeeHeadIDFK),
                    //new SqlParameter("@FeeCatgIDFK", request.FeeCatgIDFK),
                    //new SqlParameter("@ClassIDFK", request.ClassIDFK),
                    //new SqlParameter("@SectionIDFK", request.SectionIDFK),
                    //new SqlParameter("@StudentIDFK", request.StudentIDFK),
                    //new SqlParameter("@AdmissionNoIDFK", request.AdmissionNoIDFK),
                    //new SqlParameter("@IsYearly", request.IsYearly),
                    //new SqlParameter("@IsFlat", request.IsFlat),
                    new SqlParameter("@FlatAmount", (object?)request.FlatAmount ?? DBNull.Value),
                    //new SqlParameter("@RebatePercent", (object?)request.RebatePercent ?? DBNull.Value),
                    //new SqlParameter("@RebatePercentAmount", (object?)request.RebatePercentAmount ?? DBNull.Value),
                    new SqlParameter("@OnMonth", request.OnMonth ?? (object)DBNull.Value),
                    new SqlParameter("@MonthIDFK", request.MonthIDFK),
                    new SqlParameter("@IsDeleted", request.IsDeleted),
                    //new SqlParameter("@Status", request.Status ?? (object)DBNull.Value),
                    //new SqlParameter("@Current_Session", request.Current_Session ?? (object)DBNull.Value),
                    new SqlParameter("@UbdatedBy", request.UpdatedBy ?? (object)DBNull.Value),
                    new SqlParameter("@Remarks", request.Remarks ?? (object)DBNull.Value)
                };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeDueRebate updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while updating FeeDueRebate.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateService", "UpdateFeeDueRebate", ex.ToString());
            }

            return response;
        }

        public async Task<ResponseModel> GetFeeDueRebateByStudentName(string studentName, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No records found." };

            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                string query = @"
            SELECT fdr.*, s.StudentName, si.AdmissionNo
            FROM FeeDueRebate fdr
            INNER JOIN Students s ON fdr.StudentIDFK = s.StudentID
            INNER JOIN StudentInfo si ON fdr.AdmissionNoIDFK = si.StudentInfoID
            WHERE s.StudentName LIKE '%' + @StudentName + '%'
            AND fdr.IsDeleted = 0";

                var sqlParams = new[] { new SqlParameter("@StudentName", studentName ?? string.Empty) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row => new FeeDueRebateDTO
                    {
                        RebateID = row.Field<long>("RebateID"),
                        FeeDueIDFK = row.Field<long>("FeeDueIDFK"),
                        FeeHeadIDFK = row.Field<long>("FeeHeadIDFK"),
                        FeeCatgIDFK = row.Field<int>("FeeCatgIDFK"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIDFK = row.Field<long>("StudentIDFK"),
                        AdmissionNoIDFK = row.Field<long>("AdmissionNoIDFK"),
                        IsYearly = row.Field<int>("IsYearly"),
                        IsFlat = row.Field<int>("IsFlat"),
                        FlatAmount = row.Field<decimal?>("FlatAmount"),
                        RebatePercent = row.Field<decimal?>("RebatePercent"),
                        RebatePercentAmount = row.Field<decimal?>("RebatePercentAmount"),
                        OnMonth = row.Field<string>("OnMonth"),
                        MonthIDFK = row.Field<int>("MonthIDFK"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        Status = row.Field<bool>("Status"),
                        Current_Session = row.Field<string>("Current_Session"),
                        UserName = row.Field<string>("UserName"),
                        OnDate = row.Field<DateTime>("OnDate"),
                        UpdatedBy = row.Field<string>("UbdatedBy"),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn"),
                        Remarks = row.Field<string>("Remarks")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = list;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDueRebate by student name.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueRebateService", "GetFeeDueRebateByStudentName",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | StudentName: {studentName}"
                );
            }

            return response;
        }

        public async Task<ResponseModel> GetFeeDueRebateByAdmissionNo(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No records found." };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.IsSuccess = false;
                    response.Message = "Parameter (AdmissionNo,CurrentSession) missing.";
                    return response;
                }

                var parts = param.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid parameter format. Use AdmissionNo,CurrentSession.";
                    return response;
                }

                string admissionNo = parts[0].Trim();
                string currentSession = parts[1].Trim();

                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                string query = @"
            SELECT fdr.*, s.StudentName, si.AdmissionNo
            FROM FeeDueRebate fdr
            INNER JOIN Students s ON fdr.StudentIDFK = s.StudentID
            INNER JOIN StudentInfo si ON fdr.AdmissionNoIDFK = si.StudentInfoID
            WHERE si.AdmissionNo = @AdmissionNo
              AND fdr.Current_Session = @CurrentSession
              AND fdr.IsDeleted = 0";

                var sqlParams = new[]
                {
            new SqlParameter("@AdmissionNo", admissionNo),
            new SqlParameter("@CurrentSession", currentSession)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row => new FeeDueRebateDTO
                    {
                        RebateID = row.Field<long>("RebateID"),
                        FeeDueIDFK = row.Field<long>("FeeDueIDFK"),
                        FeeHeadIDFK = row.Field<long>("FeeHeadIDFK"),
                        FeeCatgIDFK = row.Field<int>("FeeCatgIDFK"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIDFK = row.Field<long>("StudentIDFK"),
                        AdmissionNoIDFK = row.Field<long>("AdmissionNoIDFK"),
                        IsYearly = row.Field<int>("IsYearly"),
                        IsFlat = row.Field<int>("IsFlat"),
                        FlatAmount = row.Field<decimal?>("FlatAmount"),
                        RebatePercent = row.Field<decimal?>("RebatePercent"),
                        RebatePercentAmount = row.Field<decimal?>("RebatePercentAmount"),
                        OnMonth = row.Field<string>("OnMonth"),
                        MonthIDFK = row.Field<int>("MonthIDFK"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        Status = row.Field<bool>("Status"),
                        Current_Session = row.Field<string>("Current_Session"),
                        UserName = row.Field<string>("UserName"),
                        OnDate = row.Field<DateTime>("OnDate"),
                        UpdatedBy = row.Field<string>("UbdatedBy"),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn"),
                        Remarks = row.Field<string>("Remarks")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = list;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDueRebate by AdmissionNo.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueRebateService", "GetFeeDueRebateByAdmissionNo",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Param: {param}"
                );
            }

            return response;
        }
        public async Task<ResponseModel> GetFeeDueRebateByClassId(long classId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
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

                #region Query FeeDueRebate by ClassID
                string query = @"
            SELECT fdr.*, s.StudentName, si.AdmissionNo
            FROM FeeDueRebate fdr
            INNER JOIN Students s ON fdr.StudentIDFK = s.StudentID
            INNER JOIN StudentInfo si ON fdr.AdmissionNoIDFK = si.StudentInfoID
            WHERE fdr.ClassIDFK = @ClassID
            AND fdr.IsDeleted = 0
        ";

                var sqlParams = new[] { new SqlParameter("@ClassID", classId) };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = ds.Tables[0].AsEnumerable().Select(row => new FeeDueRebateDTO
                    {
                        RebateID = row.Field<long>("RebateID"),
                        FeeDueIDFK = row.Field<long>("FeeDueIDFK"),
                        FeeHeadIDFK = row.Field<long>("FeeHeadIDFK"),
                        FeeCatgIDFK = row.Field<int>("FeeCatgIDFK"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIDFK = row.Field<long>("StudentIDFK"),
                        AdmissionNoIDFK = row.Field<long>("AdmissionNoIDFK"),
                        IsYearly = row.Field<int>("IsYearly"),
                        IsFlat = row.Field<int>("IsFlat"),
                        FlatAmount = row.Field<decimal?>("FlatAmount"),
                        RebatePercent = row.Field<decimal?>("RebatePercent"),
                        RebatePercentAmount = row.Field<decimal?>("RebatePercentAmount"),
                        OnMonth = row.Field<string>("OnMonth"),
                        MonthIDFK = row.Field<int>("MonthIDFK"),
                        IsDeleted = row.Field<bool>("IsDeleted"),
                        Status = row.Field<bool>("Status"),
                        Current_Session = row.Field<string>("Current_Session"),
                        UserName = row.Field<string>("UserName"),
                        OnDate = row.Field<DateTime>("OnDate"),
                        UpdatedBy = row.Field<string>("UbdatedBy"),
                        UpdatedOn = row.Field<DateTime?>("UpdatedOn"),
                        Remarks = row.Field<string>("Remarks")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = list;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDueRebate by ClassID.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueRebateService", "GetFeeDueRebateByClassId",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | ClassID: {classId}"
                );

                return response;
            }
        }

        public async Task<ResponseModel> DeleteFeeDueRebate(long rebateId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeDueRebate not deleted!" };

            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                    return new ResponseModel { IsSuccess = false, Message = "Invalid client ID." };

                string query = @"UPDATE FeeDueRebate SET IsDeleted = 1, UpdatedOn = GETDATE() WHERE RebateID=@RebateID";

                var parameters = new[] { new SqlParameter("@RebateID", rebateId) };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeDueRebate deleted successfully.";
                }
                else
                {
                    response.Message = "FeeDueRebate not found or already deleted.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while deleting FeeDueRebate.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueRebateService", "DeleteFeeDueRebate", ex.ToString());
            }

            return response;
        }

    }
}

