using FeeManagement.Repository;
using FeeManagement.Repository.SQL;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace FeeManagement.Services.FeeDue
{
    public class FeeDueService :IFeeDueService
    {
        private readonly IConfiguration _configuration;
        public FeeDueService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddFeeDue(FeeDueDTO request, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeDue Not Added!" };

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

                #region Prepare DataTable with single row
                var dt = new DataTable();
                dt.Columns.Add("FeeDueID", typeof(long));
                dt.Columns.Add("ClassIDFK", typeof(long));
                dt.Columns.Add("SectionIDFK", typeof(long));
                dt.Columns.Add("StudentIDFK", typeof(long));
                dt.Columns.Add("StudentInfoIDFK", typeof(long));
                dt.Columns.Add("FHIDFK", typeof(int));
                dt.Columns.Add("DIDFK", typeof(int));
                dt.Columns.Add("FSIDFK", typeof(long));
                dt.Columns.Add("FCategoryID", typeof(int));
                dt.Columns.Add("FeeHeadType", typeof(int));
                dt.Columns.Add("FeeHeadName", typeof(string));
                dt.Columns.Add("Current_Session", typeof(string));
                dt.Columns.Add("FeeMonth", typeof(string));
                dt.Columns.Add("FeeMonthIDFK", typeof(int));
                dt.Columns.Add("FeeYear", typeof(string));
                dt.Columns.Add("SystemYear", typeof(string));
                dt.Columns.Add("SystemMonth", typeof(string));
                dt.Columns.Add("TransactionType", typeof(int));
                dt.Columns.Add("DueDate", typeof(DateTime));
                dt.Columns.Add("BillDate", typeof(DateTime));
                dt.Columns.Add("UserName", typeof(string));
                dt.Columns.Add("IsPaid", typeof(int));
                dt.Columns.Add("RebateIDFK", typeof(long));
                dt.Columns.Add("FeeRate", typeof(decimal));
                dt.Columns.Add("Rebate", typeof(decimal));
                dt.Columns.Add("LateFee", typeof(decimal));
                dt.Columns.Add("ToPay", typeof(decimal));
                dt.Columns.Add("IsDeleted", typeof(int));
                dt.Columns.Add("Remarks", typeof(string));

                dt.Rows.Add(
                    request.FeeDueID,
                    request.ClassIDFK,
                    request.SectionIDFK,
                    request.StudentIdFk,
                    request.StudentInfoIdFk,
                    request.FhIdFk ?? (object)DBNull.Value,
                    request.DidFk ?? (object)DBNull.Value,
                    request.FsIdFk ?? (object)DBNull.Value,
                    request.FCategoryId ?? (object)DBNull.Value,
                    request.FeelHeadType ?? (object)DBNull.Value,
                    request.FeelHeadName ?? string.Empty,
                    request.CurrentSession ?? string.Empty,
                    request.FeelMonth ?? string.Empty,
                    request.FeelMonthIdFk ?? (object)DBNull.Value,
                    request.FeeYear ?? string.Empty,
                    request.SystemYear ?? string.Empty,
                    request.SystemMonth ?? string.Empty,
                    0,
                    request.DueDate ?? (object)DBNull.Value,
                    request.BillDate ?? (object)DBNull.Value,
                    request.UserName ?? string.Empty,
                    request.IsPaid ?? 0,
                    request.RebateIdFk ?? (object)DBNull.Value,
                    request.FeeRate ?? 0,
                    request.Rebate ?? 0,
                    request.LateFee ?? 0,
                    request.ToPay ?? 0,
                    0,
                    request.Remarks ?? string.Empty
                );
                #endregion

                #region Bulk Insert
                using (var sqlBulk = new SqlBulkCopy(connectionString))
                {
                    sqlBulk.DestinationTableName = "FeeDue";

                    foreach (DataColumn col in dt.Columns)
                        sqlBulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

                    await sqlBulk.WriteToServerAsync(dt);
                }
                #endregion

                response.IsSuccess = true;
                response.Status = 1;
                response.Message = "FeeDue record added successfully!";
                response.ResponseData = request;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding FeeDue record";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("FeeDueService", "AddFeeDue",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}");
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetFeeDueByStudentName(string studentName, string clientId)
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

                #region Query FeeDue by Student Name
                string query = @"
            SELECT 
                fd.FeeDueID,
                fd.ClassIDFK,
                fd.SectionIDFK,
                fd.StudentIDFK,
                s.StudentName,
                si.AdmissionNo,
                fd.StudentInfoIDFK,
                fd.FHIDFK,
                fd.DIDFK,
                fd.FSIDFK,
                fd.FCategoryID,
                fd.FeeHeadType,
                fd.FeeHeadName,
                fd.Current_Session,
                fd.FeeMonth,
                fd.FeeMonthIDFK,
                fd.FeeYear,
                fd.SystemYear,
                fd.SystemMonth,
                fd.TransactionType,
                fd.DueDate,
                fd.BillDate,
                fd.UserName,
                fd.IsPaid,
                fd.RebateIDFK,
                fd.DiscountIDFK,
                fd.BulkAdvanceIDFK,
                fd.FeeAdvanceIDFK,
                fd.FeeRate,
                fd.Rebate,
                fd.Balance,
                fd.BulkAdvance,
                fd.Advance,
                fd.Discount,
                fd.LateFee,
                fd.AdvanceType,
                fd.ToPay,
                fd.IsDeleted,
                fd.IsApplicable,
                fd.Remarks,
                fd.adjustment,
                fd.AdjID,
                fd.IsFlat,
                fd.Percentage
            FROM FeeDue fd
            INNER JOIN Students s ON fd.StudentIDFK = s.StudentID
            INNER JOIN StudentInfo si ON fd.StudentInfoIDFK = si.StudentInfoID
            WHERE s.StudentName LIKE '%' + @StudentName + '%'
            AND fd.IsDeleted = 0
           
        ";

                // Assume frontend always passes current session
                var sqlParams = new[]
                {
            new SqlParameter("@StudentName", studentName ?? string.Empty),
           // new SqlParameter("@CurrentSession", HttpContext.cu.Request.Headers["Current-Session"] ?? string.Empty)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeDueList = ds.Tables[0].AsEnumerable().Select(row => new FeeDueDTO
                    {
                        FeeDueID = row.Field<long>("FeeDueID"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIdFk = row.Field<long>("StudentIDFK"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIDFK"),
                      //  st = row.Field<string>("StudentName"),
                       // AdmissionNo = row.Field<string>("AdmissionNo"),
                        FhIdFk = row.Field<int?>("FHIDFK"),
                        DidFk = row.Field<int?>("DIDFK"),
                        FsIdFk = row.Field<long?>("FSIDFK"),
                        FCategoryId = row.Field<int?>("FCategoryID"),
                        FeelHeadType = row.Field<int?>("FeeHeadType"),
                        FeelHeadName = row.Field<string>("FeeHeadName"),
                        CurrentSession = row.Field<string>("Current_Session"),
                        FeelMonth = row.Field<string>("FeeMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIDFK"),
                        FeeYear = row.Field<string>("FeeYear"),
                        SystemYear = row.Field<string>("SystemYear"),
                        SystemMonth = row.Field<string>("SystemMonth"),
                        TransactionType = row.Field<int?>("TransactionType"),
                        DueDate = row.Field<DateTime?>("DueDate"),
                        BillDate = row.Field<DateTime?>("BillDate"),
                        UserName = row.Field<string>("UserName"),
                        IsPaid = row.Field<int?>("IsPaid"),
                        RebateIdFk = row.Field<long?>("RebateIDFK"),
                        DiscountIdFk = row.Field<long?>("DiscountIDFK"),
                        BulkAdvancedDfk = row.Field<long?>("BulkAdvanceIDFK"),
                        FeeAdvancedDfk = row.Field<long?>("FeeAdvanceIDFK"),
                        FeeRate = row.Field<decimal?>("FeeRate"),
                        Rebate = row.Field<decimal?>("Rebate"),
                        Balance = row.Field<decimal?>("Balance"),
                        BulkAdvance = row.Field<decimal?>("BulkAdvance"),
                        Advance = row.Field<decimal?>("Advance"),
                        Discount = row.Field<decimal?>("Discount"),
                        LateFee = row.Field<decimal?>("LateFee"),
                        AdvanceType = row.Field<int?>("AdvanceType"),
                        ToPay = row.Field<decimal?>("ToPay"),
                        IsDeleted = row.Field<int?>("IsDeleted"),
                        IsApplicable = row.Field<int?>("IsApplicable"),
                        Remarks = row.Field<string>("Remarks"),
                        Adjustment = row.Field<decimal?>("adjustment"),
                        AdjId = row.Field<long?>("AdjID"),
                        IsFlat = row.Field<int?>("IsFlat"),
                        Percentage = row.Field<decimal?>("Percentage")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = feeDueList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDue by student name.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "GetFeeDueByStudentName",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | StudentName: {studentName}"
                );

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="admissionNo"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetFeeDueByAdmissionNo(string param, string clientId)
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
                #region Parse Parameters (AdmissionNo, CurrentSession)
                if (string.IsNullOrEmpty(param))
                {
                    response.IsSuccess = false;
                    response.Message = "Parameter (AdmissionNo|CurrentSession) missing.";
                    return response;
                }
                #endregion
                // Assuming param comes as "AdmissionNo|CurrentSession"
                #region Parse Parameters (AdmissionNo, CurrentSession)
                var parts = param.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid parameter format. Use AdmissionNo,CurrentSession.";
                    return response;
                }


                string admissionNo = parts[0].Trim();
                string currentSession = parts[1].Trim();
                #endregion

                #region Get Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }
                #endregion

                #region Query FeeDue by AdmissionNo and CurrentSession
                string query = @"
        SELECT 
            fd.FeeDueID,
            fd.ClassIDFK,
            fd.SectionIDFK,
            fd.StudentIDFK,
            s.StudentName,
            si.AdmissionNo,
            fd.StudentInfoIDFK,
            fd.FHIDFK,
            fd.FCategoryID,
            fd.FeeHeadType,
            fd.FeeHeadName,
            fd.Current_Session,
            fd.FeeMonth,
            fd.FeeMonthIDFK,
            fd.FeeYear,
            fd.DueDate,
            fd.BillDate,
            fd.FeeRate,
            fd.Rebate,
            fd.LateFee,
            fd.ToPay,
            fd.IsPaid,
            fd.Remarks
        FROM FeeDue fd
        INNER JOIN StudentInfo si ON fd.StudentInfoIDFK = si.StudentInfoID
        INNER JOIN Students s ON fd.StudentIDFK = s.StudentID
        WHERE si.AdmissionNo = @AdmissionNo
          AND fd.Current_Session = @CurrentSession
          AND fd.IsDeleted = 0";

                var sqlParams = new[]
                {
            new SqlParameter("@AdmissionNo", admissionNo),
            new SqlParameter("@CurrentSession", currentSession)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeDueList = ds.Tables[0].AsEnumerable().Select(row => new FeeDueDTO
                    {
                        FeeDueID = row.Field<long>("FeeDueID"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIdFk = row.Field<long>("StudentIDFK"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIDFK"),
                        FhIdFk = row.Field<int?>("FHIDFK"),
                        FCategoryId = row.Field<int?>("FCategoryID"),
                        FeelHeadType = row.Field<int?>("FeeHeadType"),
                        FeelHeadName = row.Field<string>("FeeHeadName"),
                        CurrentSession = row.Field<string>("Current_Session"),
                        FeelMonth = row.Field<string>("FeeMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIDFK"),
                        FeeYear = row.Field<string>("FeeYear"),
                        DueDate = row.Field<DateTime?>("DueDate"),
                        BillDate = row.Field<DateTime?>("BillDate"),
                        FeeRate = row.Field<decimal?>("FeeRate"),
                        Rebate = row.Field<decimal?>("Rebate"),
                        LateFee = row.Field<decimal?>("LateFee"),
                        ToPay = row.Field<decimal?>("ToPay"),
                        IsPaid = row.Field<int?>("IsPaid"),
                        Remarks = row.Field<string>("Remarks")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = feeDueList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDue by AdmissionNo.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "GetFeeDueByAdmissionNo",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Param: {param}"
                );

                return response;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetFeeDueByClassId(long classId, string clientId)
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

                #region Query FeeDue by ClassID
                string query = @"
    SELECT 
        fd.FeeDueID,
        fd.ClassIDFK,
        fd.SectionIDFK,
        fd.StudentIDFK,
        s.StudentName,
        si.AdmissionNo,
        fd.StudentInfoIDFK,
        fd.FHIDFK,
        fd.DIDFK,
        fd.FSIDFK,
        fd.FCategoryID,
        fd.FeeHeadType,
        fd.FeeHeadName,
        fd.Current_Session,
        fd.FeeMonth,
        fd.FeeMonthIDFK,
        fd.FeeYear,
        fd.SystemYear,
        fd.SystemMonth,
        fd.TransactionType,
        fd.DueDate,
        fd.BillDate,
        fd.UserName,
        fd.IsPaid,
        fd.RebateIDFK,
        fd.DiscountIDFK,
        fd.BulkAdvanceIDFK,
        fd.FeeAdvanceIDFK,
        fd.FeeRate,
        fd.Rebate,
        fd.Balance,
        fd.BulkAdvance,
        fd.Advance,
        fd.Discount,
        fd.LateFee,
        fd.AdvanceType,
        fd.ToPay,
        fd.IsDeleted,
        fd.IsApplicable,
        fd.Remarks,
        fd.adjustment,
        fd.AdjID,
        fd.IsFlat,
        fd.Percentage
    FROM FeeDue fd
    INNER JOIN Students s ON fd.StudentIDFK = s.StudentID
    INNER JOIN StudentInfo si ON fd.StudentInfoIDFK = si.StudentInfoID
    WHERE fd.ClassIDFK = @ClassID
    AND fd.IsDeleted = 0
";


                var sqlParams = new[]
                {
            new SqlParameter("@ClassID", classId)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var feeDueList = ds.Tables[0].AsEnumerable().Select(row => new FeeDueDTO
                    {
                        FeeDueID = row.Field<long>("FeeDueID"),
                        ClassIDFK = row.Field<long>("ClassIDFK"),
                        SectionIDFK = row.Field<long>("SectionIDFK"),
                        StudentIdFk = row.Field<long>("StudentIDFK"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIDFK"),
                        FhIdFk = row.Field<int?>("FHIDFK"),
                        DidFk = row.Field<int?>("DIDFK"),
                        FsIdFk = row.Field<long?>("FSIDFK"),
                        FCategoryId = row.Field<int?>("FCategoryID"),
                        FeelHeadType = row.Field<int?>("FeeHeadType"),
                        FeelHeadName = row.Field<string>("FeeHeadName"),
                        CurrentSession = row.Field<string>("Current_Session"),
                        FeelMonth = row.Field<string>("FeeMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIDFK"),
                        FeeYear = row.Field<string>("FeeYear"),
                        SystemYear = row.Field<string>("SystemYear"),
                        SystemMonth = row.Field<string>("SystemMonth"),
                        TransactionType = row.Field<int?>("TransactionType"),
                        DueDate = row.Field<DateTime?>("DueDate"),
                        BillDate = row.Field<DateTime?>("BillDate"),
                        UserName = row.Field<string>("UserName"),
                        IsPaid = row.Field<int?>("IsPaid"),
                        RebateIdFk = row.Field<long?>("RebateIDFK"),
                        DiscountIdFk = row.Field<long?>("DiscountIDFK"),
                        BulkAdvancedDfk = row.Field<long?>("BulkAdvanceIDFK"),
                        FeeAdvancedDfk = row.Field<long?>("FeeAdvanceIDFK"),
                        FeeRate = row.Field<decimal?>("FeeRate"),
                        Rebate = row.Field<decimal?>("Rebate"),
                        Balance = row.Field<decimal?>("Balance"),
                        BulkAdvance = row.Field<decimal?>("BulkAdvance"),
                        Advance = row.Field<decimal?>("Advance"),
                        Discount = row.Field<decimal?>("Discount"),
                        LateFee = row.Field<decimal?>("LateFee"),
                        AdvanceType = row.Field<int?>("AdvanceType"),
                        ToPay = row.Field<decimal?>("ToPay"),
                        IsDeleted = row.Field<int?>("IsDeleted"),
                        IsApplicable = row.Field<int?>("IsApplicable"),
                        Remarks = row.Field<string>("Remarks"),
                        Adjustment = row.Field<decimal?>("adjustment"),
                        AdjId = row.Field<long?>("AdjID"),
                        IsFlat = row.Field<int?>("IsFlat"),
                        Percentage = row.Field<decimal?>("Percentage")
                    }).ToList();


                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = feeDueList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving FeeDue by ClassID.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "GetFeeDueByClassId",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | ClassID: {classId}"
                );

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllMonths(string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No records found." };

            try
            {
                // Get connection string for client
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID.";
                    return response;
                }

                // Query
                string query = "SELECT MonthID, [Month] FROM Months";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, null);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var months = ds.Tables[0].AsEnumerable().Select(row => new MonthDTO
                    {
                        MonthID = row.Field<int>("MonthID"),
                        Month = row.Field<string>("Month")
                    }).ToList();

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records retrieved successfully.";
                    response.ResponseData = months;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error retrieving months.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("MonthService", "GetAllMonths", ex.ToString());
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateFeeDue(FeeDueDTO request, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "FeeDue not updated."
            };

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

                #region Update Query (only FeeRate)
                string query = @"
            UPDATE FeeDue
            SET FeeRate = @FeeRate
            WHERE FeeDueID = @FeeDueID AND IsDeleted = 0";

                var sqlParams = new[]
                {
            new SqlParameter("@FeeDueID", request.FeeDueID),
            new SqlParameter("@FeeRate", request.FeeRate ?? 0)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeDue rate updated successfully.";
                    response.ResponseData = request;
                }
                else
                {
                    response.Message = "FeeDue not found or already deleted.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error updating FeeDue.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "UpdateFeeDue",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | Request: {JsonConvert.SerializeObject(request)}"
                );
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="feeDueID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteFeeDue(long feeDueID, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "FeeDue not deleted." };

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

                #region Soft Delete Query
                string query = @"
            UPDATE FeeDue
            SET IsDeleted = 1
            WHERE FeeDueID = @FeeDueID AND IsDeleted = 0";

                var sqlParams = new[]
                {
            new SqlParameter("@FeeDueID", feeDueID)
        };

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, sqlParams);

                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "FeeDue deleted successfully (soft delete).";
                }
                else
                {
                    response.Message = "FeeDue not found or already deleted.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error deleting FeeDue.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "DeleteFeeDue",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | FeeDueID: {feeDueID}"
                );
            }

            return response;
        }


    }
}
