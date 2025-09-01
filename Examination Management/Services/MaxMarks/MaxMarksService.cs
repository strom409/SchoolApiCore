using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.MaxMarks
{
    public class MaxMarksService:IMaxMarksService
    {
        private readonly IConfiguration _configuration;
        public MaxMarksService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddMaxMarks(MaxMarksDto dto, string clientId)
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
                SqlParameter[] param =
                {
            new SqlParameter("@MaxMarks", dto.MaxMarks ?? (object)DBNull.Value),
            new SqlParameter("@MinMarks", dto.MinMarks ?? (object)DBNull.Value),
            new SqlParameter("@Classid", dto.Classid ?? (object)DBNull.Value),
            new SqlParameter("@Subjectid", dto.Subjectid ?? (object)DBNull.Value),
            new SqlParameter("@Sectionid", dto.Sectionid ?? (object)DBNull.Value),
            new SqlParameter("@Unitid", dto.Unitid ?? (object)DBNull.Value),
            new SqlParameter("@Current_Session", dto.Current_Session ?? (object)DBNull.Value)
        };
                #endregion

                #region Check Duplicate
                string dupQuery = @"
            SELECT COUNT(1) FROM MaxMarks 
            WHERE Classid = @Classid AND Subjectid = @Subjectid 
              AND Unitid = @Unitid AND Current_Session = @Current_Session";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, param);
                int count = (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    ? Convert.ToInt32(ds.Tables[0].Rows[0][0])
                    : 0;

                if (count > 0)
                {
                    response.Message = "MaxMarks already exists for given criteria.";
                    return response;
                }
                #endregion

                #region Insert Query
                string insertQuery = @"
            INSERT INTO MaxMarks (MaxMarks, MinMarks, Classid, Subjectid, Unitid,Sectionid, Current_Session)
            VALUES (@MaxMarks, @MinMarks, @Classid, @Subjectid, @Unitid,@Sectionid, @Current_Session)";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                if (rows > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "MaxMarks inserted successfully.";
                }
                else
                {
                    response.Message = "Insert failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while inserting MaxMarks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MaxMarksService", "AddMaxMarks", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateMaxMarks(MaxMarksDto dto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@MaxID", dto.MaxID ?? (object)DBNull.Value),
            new SqlParameter("@MaxMarks", dto.MaxMarks ?? (object)DBNull.Value),
            new SqlParameter("@MinMarks", dto.MinMarks ?? (object)DBNull.Value)
           
        };
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE MaxMarks SET
                MaxMarks = @MaxMarks,
                MinMarks = @MinMarks
            WHERE MaxID = @MaxID";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, param);

                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "MaxMarks updated successfully.";
                }
                else
                {
                    response.Message = "Update failed or record not found.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while updating MaxMarks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MaxMarksService", "UpdateMaxMarks", ex.ToString());
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
        public async Task<ResponseModel> GetMaxMarksByClassAndSubject(string param, string clientId)
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
                var parts = param?.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (parts == null || parts.Length < 3)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid param format. Expected: 'classId,subjectId'";
                    return response;
                }

                string classId = parts[0].Trim();
                string subjectId = parts[1].Trim();
                string Unitid = parts[2].Trim();   

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] paramArray =
                {
            new SqlParameter("@Classid", classId ?? (object)DBNull.Value),
            new SqlParameter("@Subjectid", subjectId ?? (object)DBNull.Value),
            new SqlParameter("@Unitid", Unitid ?? (object)DBNull.Value)
        };
                #endregion

                #region Select Query with JOINs and WHERE filter
                string selectQuery = @"
        SELECT 
            m.MaxID, m.MaxMarks, m.MinMarks, m.Classid, m.Subjectid, 
            m.SubDepartmentid, m.Sectionid, m.Unitid, m.Optionalid, 
            m.Current_Session, m.SessionID,
            c.ClassName,
            s.SubjectName
        FROM MaxMarks m
        LEFT JOIN Classes c ON m.Classid = c.Classid
        LEFT JOIN Subjects s ON m.Subjectid = s.Subjectid
        LEFT JOIN Units u ON m.Unitid = u.Unitid
        WHERE m.Classid = @Classid AND m.Subjectid = @Subjectid  AND m.Unitid = @Unitid";
                #endregion

                #region Execute and Read Data
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, selectQuery, paramArray);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<MaxMarksDto>();

                    foreach (System.Data.DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new MaxMarksDto
                        {
                            MaxID = row["MaxID"]?.ToString(),
                            MaxMarks = row["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(row["MaxMarks"]) : null,
                            MinMarks = row["MinMarks"] != DBNull.Value ? Convert.ToDecimal(row["MinMarks"]) : null,
                            Classid = row["Classid"]?.ToString(),
                            Subjectid = row["Subjectid"]?.ToString(),
                            SubDepartmentid = row["SubDepartmentid"]?.ToString(),
                            Sectionid = row["Sectionid"]?.ToString(),
                            Unitid = row["Unitid"]?.ToString(),
                            Optionalid = row["Optionalid"]?.ToString(),
                            Current_Session = row["Current_Session"]?.ToString(),
                            SessionID = row["SessionID"]?.ToString(),
                            ClassName = row["ClassName"]?.ToString(),
                            SubjectName = row["SubjectName"]?.ToString(),
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
                response.Message = "Error occurred while fetching MaxMarks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MaxMarksService", "GetMaxMarksByClassAndSubject", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSession"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllMaxMarksByCurrentSession(string currentSession, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Select Query using string interpolation for Current_Session
                // WARNING: Make sure currentSession is sanitized or trusted to avoid SQL injection
                string selectQuery = $@"
            SELECT MaxID, MaxMarks, MinMarks, Classid, Subjectid, 
                   SubDepartmentid, Sectionid, Unitid, Optionalid, 
                   Current_Session, SessionID
            FROM MaxMarks
            WHERE Current_Session = '{currentSession.Replace("'", "''")}'";
                #endregion

                #region Execute and Read Data
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, selectQuery);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<MaxMarksDto>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new MaxMarksDto
                        {
                            MaxID = row["MaxID"].ToString(),
                            MaxMarks = row["MaxMarks"] != DBNull.Value ? Convert.ToDecimal(row["MaxMarks"]) : null,
                            MinMarks = row["MinMarks"] != DBNull.Value ? Convert.ToDecimal(row["MinMarks"]) : null,
                            Classid = row["Classid"].ToString(),
                            Subjectid = row["Subjectid"].ToString(),
                            SubDepartmentid = row["SubDepartmentid"].ToString(),
                            Sectionid = row["Sectionid"].ToString(),
                            Unitid = row["Unitid"].ToString(),
                            Optionalid = row["Optionalid"].ToString(),
                            Current_Session = row["Current_Session"].ToString(),
                            SessionID = row["SessionID"].ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "Records fetched successfully.";
                    response.ResponseData = list;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while fetching MaxMarks list.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MaxMarksService", "GetAllMaxMarksByCurrentSession", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteMaxMarks(string maxId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Record cannot be deleted as it is in use."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameter
                SqlParameter[] param =
                {
            new SqlParameter("@MaxID", maxId ?? (object)DBNull.Value)
        };
                #endregion

                #region Check if MaxID is Used in OptionalMarks
                string checkQuery = @"SELECT COUNT(1) FROM Marks WHERE MaxID = @MaxID";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);

                int count = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }

                if (count > 0)
                {
                    response.Message = "Cannot delete. MaxID is used in Marks.";
                    return response;
                }
                #endregion

                #region Delete Query
                string deleteQuery = @"DELETE FROM MaxMarks WHERE MaxID = @MaxID";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, param);
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Record deleted successfully.";
                }
                else
                {
                    response.Message = "Record not found or already deleted.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while deleting MaxMarks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MaxMarksService", "DeleteMaxMarksById", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}
