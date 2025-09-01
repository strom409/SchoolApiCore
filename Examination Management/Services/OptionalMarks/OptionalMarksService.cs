using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Examination_Management.Services.Marks;
using Examination_Management.Services.MaxMarks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.OptionalMarks
{
    public class OptionalMarksService : IOptionalMarksService
    {
        private readonly IConfiguration _configuration;
        public OptionalMarksService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddOptionalMark(OptionalMarksDto dto, string clientId)
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
                    new SqlParameter("@StudentID", dto.StudentID),
                    new SqlParameter("@ClassID", dto.ClassID),
                    new SqlParameter("@SectionID", dto.SectionID),
                    new SqlParameter("@Rollno", dto.Rollno),
                    new SqlParameter("@MaxID", dto.MaxID),
                    new SqlParameter("@UnitID", dto.UnitID),
                    new SqlParameter("@SubjectID", dto.SubjectID),
                    new SqlParameter("@Current_Session", dto.Current_Session),
                    new SqlParameter("@SessionID", dto.SessionID),
                    new SqlParameter("@Status", dto.Status),
                    new SqlParameter("@Marks", dto.Marks),
                    //new SqlParameter("@Date", dto.Date),
                    new SqlParameter("@Date", new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))
        };
                #endregion

                #region Check Duplicate
                // Optional: implement duplicate logic if needed
                // For example, check if a record for same StudentID, ClassID, SubjectID, Current_Session exists
                string dupQuery = @"
            SELECT COUNT(1) FROM OptionalMarks
            WHERE StudentID = @StudentID AND ClassID = @ClassID AND SubjectID = @SubjectID AND Current_Session = @Current_Session";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, param);
                int count = (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    ? Convert.ToInt32(ds.Tables[0].Rows[0][0])
                    : 0;

                if (count > 0)
                {
                    response.Message = "Record already exists for the given criteria.";
                    return response;
                }
                #endregion

                #region Insert Query
                string insertQuery = @"
            INSERT INTO OptionalMarks 
            (StudentID, ClassID, SectionID, Rollno, MaxID, UnitID, SubjectID, Current_Session, SessionID, Status, Marks, Date)
            VALUES
            (@StudentID, @ClassID, @SectionID, @Rollno, @MaxID, @UnitID, @SubjectID, @Current_Session, @SessionID, @Status, @Marks, @Date)";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                if (rows > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "OptionalMark inserted successfully.";
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
                response.Message = "Error occurred while inserting OptionalMark.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksService", "AddOptionalMark", ex.ToString());
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
        public async Task<ResponseModel> UpdateOptionalMarks(OptionalMarksDto dto, string clientId)
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
            new SqlParameter("@OpMarksID", dto.OpMarksID),
            new SqlParameter("@Marks", dto.Marks),
            new SqlParameter("@Date", dto.Date),
            new SqlParameter("@Status", dto.Status)

        };
                #endregion

                #region Update Query
                string updateQuery = @"
        UPDATE OptionalMarks
        SET Marks = @Marks, Date = @Date,Status = @Status
        WHERE OpMarksID = @OpMarksID";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, param);

                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Optional marks updated successfully.";
                }
                else
                {
                    response.Message = "No matching record found for update.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while updating optional marks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksService", "UpdateOptionalMarksByIdAsync", ex.ToString());
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
        public async Task<ResponseModel> GetMaxMarksByClassSectionSubjectUnit(string param, string clientId)
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

                if (parts == null || parts.Length < 4)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid param format. Expected: 'classId,subjectId,sectionId,unitId'";
                    return response;
                }

                string classId = parts[0].Trim();
                string subjectId = parts[1].Trim();
                string sectionId = parts[2].Trim();
                string unitId = parts[3].Trim();
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SubjectID", subjectId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@UnitID", unitId)
        };
                #endregion

                #region Query
                string query = @"
SELECT 
    om.OpMarksID, om.StudentID, om.ClassID, om.SectionID, om.Rollno, om.MaxID, om.UnitID, om.SubjectID, 
    om.Current_Session, om.SessionID, om.Status, om.Marks, om.Date,
    c.ClassName,
    s.OptionalSubjectName AS SubjectName,
    u.UnitName,
    sec.SectionName
FROM OptionalMarks om
LEFT JOIN Classes c ON om.ClassID = c.ClassID
LEFT JOIN OptionalSubjects s ON om.SubjectID = s.OptionalSubjectID
LEFT JOIN Units u ON om.UnitID = u.UnitID
LEFT JOIN Sections sec ON om.SectionID = sec.SectionID
WHERE 
    om.ClassID = @ClassID
    AND om.SubjectID = @SubjectID
    AND om.SectionID = @SectionID
    AND om.UnitID = @UnitID 
    AND om.Status = 1";
                #endregion

                #region Execute and Map
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<OptionalMarksDto>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new OptionalMarksDto
                        {
                            OpMarksID = row["OpMarksID"]?.ToString(),
                            StudentID = row["StudentID"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SectionID"]?.ToString(),
                            Rollno = row["Rollno"]?.ToString(),
                            MaxID = row["MaxID"]?.ToString(),
                            UnitID = row["UnitID"]?.ToString(),
                            SubjectID = row["SubjectID"]?.ToString(),
                            Current_Session = row["Current_Session"]?.ToString(),
                            SessionID = row["SessionID"]?.ToString(),
                            Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : null,
                            Marks = row["Marks"] != DBNull.Value ? Convert.ToDecimal(row["Marks"]) : null,
                            Date = row["Date"] != DBNull.Value ? Convert.ToDateTime(row["Date"]) : DateTime.MinValue,
                            ClassName = row["ClassName"]?.ToString(),
                            SubjectName = row["SubjectName"]?.ToString(),
                            UnitName = row["UnitName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString()
                        });
                    }

                    response.Status = 1;
                    response.IsSuccess = true;
                    response.Message = "Records found.";
                    response.ResponseData = list;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while fetching optional marks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("OptionalMarksService", "GetOptionalMarksByParams", ex.ToString());
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="opMarksId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteOptionalMarks(string opMarksId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to delete record."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Perform Hard Delete
                string deleteQuery = "DELETE FROM OptionalMarks WHERE OpMarksID = @OpMarksID";
                SqlParameter[] deleteParams = { new SqlParameter("@OpMarksID", opMarksId) };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    deleteQuery,
                    deleteParams
                );

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "OptionalMarks record deleted successfully.";
                }
                else
                {
                    response.Message = "No record found to delete.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while deleting OptionalMarks.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "OptionalMarksService",
                    "DeleteOptionalMarks",
                    ex.ToString()
                );
                #endregion
            }

            return response;
        }

    }
}
