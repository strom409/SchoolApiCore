using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Examination_Management.Services.OptionalMarks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.OptionalMaxMarks
{
    public class OptionalMaxMarksService :IOptionalMaxMarksService
    {
        private readonly IConfiguration _configuration;
        public OptionalMaxMarksService(IConfiguration configuration)
        {
            _configuration= configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddOptionalMaxMarks(OptionalMaxMarksDto dto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record inserted!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] insertParams = new SqlParameter[]
                {
            new SqlParameter("@MaxMarks", dto.MaxMarks),
            new SqlParameter("@MinMarks", dto.MinMarks),
            new SqlParameter("@ClassId", dto.ClassId),
            new SqlParameter("@UnitID", dto.UnitID),
            new SqlParameter("@Current_Session", dto.Current_Session),
            new SqlParameter("@SessionID", dto.SessionID),
            new SqlParameter("@OptionalSubjectid", dto.OptionalSubjectid)
           
                };
                #endregion

                #region Check Duplicate
                string checkQuery = @"SELECT * FROM OptionalMaxMarks 
                              WHERE ClassId = @ClassId AND UnitID = @UnitID 
                              AND OptionalSubjectid = @OptionalSubjectid 
                              AND Current_Session = @Current_Session";

                SqlParameter[] checkParams = new SqlParameter[]
                {
            new SqlParameter("@ClassId", dto.ClassId),
            new SqlParameter("@UnitID", dto.UnitID),
            new SqlParameter("@OptionalSubjectid", dto.OptionalSubjectid),
            new SqlParameter("@Current_Session", dto.Current_Session)
                };

                var checkResult = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                if (checkResult != null && checkResult.Tables.Count > 0 && checkResult.Tables[0].Rows.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 2;
                    response.Message = "Duplicate record found.";
                    return response;
                }
                #endregion

                #region Insert Query
                string insertQuery = @"INSERT INTO OptionalMaxMarks (MaxMarks, MinMarks, ClassId, UnitID, Current_Session, SessionID, OptionalSubjectid)
                               VALUES (@MaxMarks, @MinMarks, @ClassId, @UnitID, @Current_Session, @SessionID, @OptionalSubjectid)";

                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Record inserted successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Failed to insert record.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while inserting.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("optionalMaxMarksService", "addptionalMaxMarks", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateOptionalMaxMarks(OptionalMaxMarksDto model, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to update record."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] updateParams = new SqlParameter[]
                {
            new SqlParameter("@OpMaxID", model.OpMaxID),
            new SqlParameter("@MaxMarks", model.MaxMarks),
            new SqlParameter("@MinMarks", model.MinMarks)
                };
                #endregion

                #region Update Query
                string updateQuery = @"UPDATE OptionalMaxMarks 
                               SET MaxMarks = @MaxMarks, MinMarks = @MinMarks 
                               WHERE OpMaxID = @OpMaxID";
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, updateParams);
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Record updated successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = 0;
                response.Message = "An error occurred while updating record.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("optionalMaxMarksService", "UpdateOptionalMaxMarks", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetOptionalMaxMarksByFilter(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record found!"
            };
            #endregion

            try
            {
                #region Parse Parameters
                var parts = param?.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (parts == null || parts.Length < 3)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid param format. Expected: 'ClassId,OptionalSubjectId,UnitId'";
                    return response;
                }

                string classId = parts[0].Trim();
                string optionalSubjectId = parts[1].Trim();
                string unitId = parts[2].Trim();
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] paramArray =
                {
            new SqlParameter("@ClassId", classId),
            new SqlParameter("@OptionalSubjectId", optionalSubjectId),
            new SqlParameter("@UnitId", unitId)
        };
                #endregion

                #region Select Query with JOINs
                string selectQuery = @"
SELECT 
    o.OpMaxID, o.MaxMarks, o.MinMarks, o.ClassId, o.UnitID, o.Current_Session, 
    o.SessionID, o.OptionalSubjectid,
    c.ClassName,
    u.UnitName,
    s.OptionalSubjectName
FROM OptionalMaxMarks o
LEFT JOIN Classes c ON o.ClassId = c.ClassId
LEFT JOIN Units u ON o.UnitID = u.UnitID
LEFT JOIN OptionalSubjects s ON o.OptionalSubjectid = s.OptionalSubjectID
WHERE o.ClassId = @ClassId 
    AND o.OptionalSubjectid = @OptionalSubjectId
    AND o.UnitID = @UnitId";
                #endregion

                #region Execute and Read Data
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, selectQuery, paramArray);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<OptionalMaxMarksDto>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new OptionalMaxMarksDto
                        {
                            OpMaxID = row["OpMaxID"]?.ToString(),
                            MaxMarks = row["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(row["MaxMarks"]) : null,
                            MinMarks = row["MinMarks"] != DBNull.Value ? Convert.ToDecimal(row["MinMarks"]) : null,
                            ClassId = row["ClassId"]?.ToString(),
                            UnitID = row["UnitID"]?.ToString(),
                            Current_Session = row["Current_Session"]?.ToString() ?? string.Empty,
                            SessionID = row["SessionID"]?.ToString(),
                            OptionalSubjectid = row["OptionalSubjectid"]?.ToString(),
                            ClassName = row["ClassName"]?.ToString(),
                            UnitName = row["UnitName"]?.ToString(),
                            SubjectName = row["OptionalSubjectName"]?.ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "Records found.";
                    response.ResponseData = list;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while fetching OptionalMaxMarks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMaxMarksService", "GetOptionalMaxMarksByFilter", ex.ToString());
                #endregion
            }

            return response;
        }


    }
}
