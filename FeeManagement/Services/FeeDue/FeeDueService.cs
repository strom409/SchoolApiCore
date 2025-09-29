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
                dt.Columns.Add("ClassIDFK", typeof(long));
                dt.Columns.Add("SectionIDFK", typeof(long));
                dt.Columns.Add("StudentIdFk", typeof(long));
                dt.Columns.Add("StudentInfoIdFk", typeof(long));
                dt.Columns.Add("FhIdFk", typeof(int));
                dt.Columns.Add("DidFk", typeof(int));
                dt.Columns.Add("FsIdFk", typeof(long));
                dt.Columns.Add("FCategoryId", typeof(int));
                dt.Columns.Add("FeelHeadType", typeof(int));
                dt.Columns.Add("FeelHeadName", typeof(string));
                dt.Columns.Add("CurrentSession", typeof(string));
                dt.Columns.Add("FeelMonth", typeof(string));
                dt.Columns.Add("FeeMonthIdFk", typeof(int));
                dt.Columns.Add("FeeYear", typeof(string));
                dt.Columns.Add("SystemYear", typeof(string));
                dt.Columns.Add("SystemMonth", typeof(string));
                dt.Columns.Add("TransactionType", typeof(int));
                dt.Columns.Add("DueDate", typeof(DateTime));
                dt.Columns.Add("BillDate", typeof(DateTime));
                dt.Columns.Add("UserName", typeof(string));
                dt.Columns.Add("IsPaid", typeof(int));
                dt.Columns.Add("RebateIdFk", typeof(long));
                dt.Columns.Add("FeeRate", typeof(decimal));
                dt.Columns.Add("Rebate", typeof(decimal));
                dt.Columns.Add("LateFee", typeof(decimal));
                dt.Columns.Add("ToPay", typeof(decimal));
                dt.Columns.Add("IsDeleted", typeof(int));
                dt.Columns.Add("Remarks", typeof(string));

                dt.Rows.Add(
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
                fd.StudentIdFk,
                s.StudentName,
                si.AdmissionNo,
                fd.StudentInfoIdFk,
                fd.FhIdFk,
                fd.FCategoryId,
                fd.FeelHeadType,
                fd.FeelHeadName,
                fd.CurrentSession,
                fd.FeelMonth,
                fd.FeeMonthIdFk,
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
            INNER JOIN Students s ON fd.StudentIdFk = s.StudentID
            INNER JOIN StudentInfo si ON fd.StudentInfoIdFk = si.StudentInfoID
            WHERE s.StudentName LIKE '%' + @StudentName + '%'
            AND fd.IsDeleted = 0
        ";

                var sqlParams = new[]
                {
            new SqlParameter("@StudentName", studentName ?? string.Empty)
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
                        StudentIdFk = row.Field<long>("StudentIdFk"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIdFk"),
                        FhIdFk = row.Field<int?>("FhIdFk"),
                        FCategoryId = row.Field<int?>("FCategoryId"),
                        FeelHeadType = row.Field<int?>("FeelHeadType"),
                        FeelHeadName = row.Field<string>("FeelHeadName"),
                        CurrentSession = row.Field<string>("CurrentSession"),
                        FeelMonth = row.Field<string>("FeelMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIdFk"),
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
                response.Message = "Error retrieving FeeDue by student name.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "GetFeeDueByStudentName",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | StudentName: {studentName}"
                );

                return response;
            }
        }

        public async Task<ResponseModel> GetFeeDueByAdmissionNo(string admissionNo, string clientId)
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

                #region Query FeeDue by AdmissionNo
                string query = @"
            SELECT 
                fd.FeeDueID,
                fd.ClassIDFK,
                fd.SectionIDFK,
                fd.StudentIdFk,
                s.StudentName,
                si.AdmissionNo,
                fd.StudentInfoIdFk,
                fd.FhIdFk,
                fd.FCategoryId,
                fd.FeelHeadType,
                fd.FeelHeadName,
                fd.CurrentSession,
                fd.FeelMonth,
                fd.FeeMonthIdFk,
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
            INNER JOIN StudentInfo si ON fd.StudentInfoIdFk = si.StudentInfoID
            INNER JOIN Students s ON fd.StudentIdFk = s.StudentID
            WHERE si.AdmissionNo = @AdmissionNo
            AND fd.IsDeleted = 0
        ";

                var sqlParams = new[]
                {
            new SqlParameter("@AdmissionNo", admissionNo ?? string.Empty)
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
                        StudentIdFk = row.Field<long>("StudentIdFk"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIdFk"),
                        FhIdFk = row.Field<int?>("FhIdFk"),
                        FCategoryId = row.Field<int?>("FCategoryId"),
                        FeelHeadType = row.Field<int?>("FeelHeadType"),
                        FeelHeadName = row.Field<string>("FeelHeadName"),
                        CurrentSession = row.Field<string>("CurrentSession"),
                        FeelMonth = row.Field<string>("FeelMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIdFk"),
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
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | AdmissionNo: {admissionNo}"
                );

                return response;
            }
        }

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
                fd.StudentIdFk,
                s.StudentName,
                si.AdmissionNo,
                fd.StudentInfoIdFk,
                fd.FhIdFk,
                fd.FCategoryId,
                fd.FeelHeadType,
                fd.FeelHeadName,
                fd.CurrentSession,
                fd.FeelMonth,
                fd.FeeMonthIdFk,
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
            INNER JOIN Students s ON fd.StudentIdFk = s.StudentID
            INNER JOIN StudentInfo si ON fd.StudentInfoIdFk = si.StudentInfoID
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
                        StudentIdFk = row.Field<long>("StudentIdFk"),
                        StudentInfoIdFk = row.Field<long>("StudentInfoIdFk"),
                        FhIdFk = row.Field<int?>("FhIdFk"),
                        FCategoryId = row.Field<int?>("FCategoryId"),
                        FeelHeadType = row.Field<int?>("FeelHeadType"),
                        FeelHeadName = row.Field<string>("FeelHeadName"),
                        CurrentSession = row.Field<string>("CurrentSession"),
                        FeelMonth = row.Field<string>("FeelMonth"),
                        FeelMonthIdFk = row.Field<int?>("FeeMonthIdFk"),
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
                response.Message = "Error retrieving FeeDue by ClassID.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "FeeDueService", "GetFeeDueByClassId",
                    $"Error: {ex.Message} | StackTrace: {ex.StackTrace} | ClassID: {classId}"
                );

                return response;
            }
        }
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
            SET IsDeleted = 1,
                UpdatedOn = GETDATE()
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
