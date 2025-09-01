using Azure.Core;
using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.Marks
{
    public class MarksService : IMarksService
    {
        private readonly IConfiguration _configuration;
        public MarksService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="marksDto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddMarks(MarksDto marksDto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to insert record."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters for Duplicate Check
                SqlParameter[] duplicateCheckParams =
                {
            new SqlParameter("@ClassID", marksDto.ClassID ?? (object)DBNull.Value),
            new SqlParameter("@SectionID", marksDto.SectionID ?? (object)DBNull.Value),
            new SqlParameter("@SubjectID", marksDto.SubjectID ?? (object)DBNull.Value),
              new SqlParameter("@StudentID", marksDto.StudentID ?? (object)DBNull.Value),
            new SqlParameter("@UnitID", marksDto.UnitID ?? (object)DBNull.Value)

        };
                #endregion

                #region Duplicate Check Query
                string duplicateCheckQuery = @"
            SELECT COUNT(*) FROM Marks 
            WHERE ClassID = @ClassID AND SectionID = @SectionID AND SubjectID = @SubjectID AND StudentID = @StudentID AND UnitID = @UnitID";
                #endregion

                #region Execute Duplicate Check
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateCheckQuery, duplicateCheckParams);

                int duplicateCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                if (duplicateCount > 0)
                {
                    response.Status = 1;
                    response.Message = "Duplicate record found for the given ClassID, SectionID, SubjectID, and UnitID.";
                    return response;
                }
                #endregion
                #region Set Status based on Absence
                // Default from DTO (0 = Present, 1 = Absent, etc.)
                int statusValue = marksDto.Status ?? 0;

                // Rule: If student is absent (Marks null OR Marks = "A") -> set Status = 1
                if (marksDto.Marks == null || marksDto.Marks.ToString().Trim().ToUpper() == "A")
                {
                    statusValue = 1;
                }
                #endregion
                #region Prepare SQL Parameters for Insert (without OptionalID)

                // use here list of dto createbject  after that add multiple
                SqlParameter[] insertParams =
                {
            new SqlParameter("@StudentID", marksDto.StudentID ?? (object)DBNull.Value),
            new SqlParameter("@ClassID", marksDto.ClassID ?? (object)DBNull.Value),
            new SqlParameter("@SectionID", marksDto.SectionID ?? (object)DBNull.Value),
            new SqlParameter("@Rollno", marksDto.Rollno ?? (object)DBNull.Value),
            new SqlParameter("@MaxID", marksDto.MaxID ?? (object)DBNull.Value),
            new SqlParameter("@UnitID", marksDto.UnitID ?? (object)DBNull.Value),
            new SqlParameter("@SubjectID", marksDto.SubjectID ?? (object)DBNull.Value),
            new SqlParameter("@Current_Session", marksDto.Current_Session ?? (object)DBNull.Value),
            new SqlParameter("@SessionID", marksDto.SessionID ?? (object)DBNull.Value),
            new SqlParameter("@Status", marksDto.Status ?? (object)DBNull.Value),
            new SqlParameter("@Marks", marksDto.Marks ?? (object)DBNull.Value),
            new SqlParameter("@Date", marksDto.Date ?? (object)DBNull.Value)
        };
                #endregion

                #region Insert Query (without OptionalID)
                string insertQuery = @"
            INSERT INTO Marks
            (StudentID, ClassID, SectionID, Rollno, MaxID, UnitID, SubjectID,
             Current_Session, SessionID, Status, Marks, Date)
            VALUES
            (@StudentID, @ClassID, @SectionID, @Rollno, @MaxID, @UnitID, @SubjectID,
             @Current_Session, @SessionID, @Status, @Marks, @Date)";
                #endregion

                #region Execute Insert
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Marks added successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while adding marks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MarksService", "AddMarks", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="marksDto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateMarks(MarksDto marksDto, string clientId)
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

                #region Prepare SQL Parameters (only Marks, Date, MarksID)
                SqlParameter[] parameters =
                {
            new SqlParameter("@MarksID", marksDto.MarksID),
            new SqlParameter("@Marks", marksDto.Marks ?? (object)DBNull.Value),
            new SqlParameter("@Date", marksDto.Date ?? (object)DBNull.Value),
            new SqlParameter("@Status", marksDto.Status)
        };
                #endregion

                #region Update Query (only update Marks and Date)
                string updateQuery = @"
            UPDATE Marks SET
                Marks = @Marks,
                Date = @Date,
                Status = @Status
            WHERE MarksID = @MarksID";
                #endregion

                #region Execute Update
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Marks updated successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while updating marks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MarksService", "UpdateMarks", ex.ToString());
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
        /// <exception cref="ArgumentException"></exception>
        public async Task<ResponseModel> GetMarksWithNames(string param, string clientId)
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
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Split Param
                var parts = param?.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (parts == null || parts.Length < 4)
                {
                    throw new ArgumentException("Invalid param format. Expected: classId,subjectId,sectionId,unitId");
                }

                string classId = parts[0];
                string subjectId = parts[1];
                string sectionId = parts[2];
                string unitId = parts[3];
                #endregion


                #region Prepare Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SubjectID", subjectId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@UnitID", unitId)
        };
                #endregion

                #region SQL Query
                string query = @"
        SELECT 
            m.MarksID,
            m.StudentID,
            m.ClassID,
            m.SectionID,
            m.Rollno,
            m.MaxID,
            m.UnitID,
            m.SubjectID,
            m.OptionalID,
            m.Current_Session,
            m.SessionID,
            m.Status,
            m.Marks,
            m.Date,
            c.ClassName,
            s.SubjectName,
            sec.SectionName,
            u.UnitName
        FROM Marks m
        INNER JOIN Students st ON st.StudentID = m.StudentID
        INNER JOIN Classes c ON m.ClassID = c.ClassID
        INNER JOIN Subjects s ON m.SubjectID = s.SubjectID
        INNER JOIN Sections sec ON m.SectionID = sec.SectionID
        LEFT JOIN Units u ON m.UnitID = u.UnitID
        WHERE m.ClassID = @ClassID 
          AND m.SubjectID = @SubjectID 
          AND m.SectionID = @SectionID 
          AND m.UnitID = @UnitID";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Convert Data
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<MarksDto> list = new List<MarksDto>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(new MarksDto
                        {
                            MarksID = row["MarksID"]?.ToString(),
                            StudentID = row["StudentID"]?.ToString(),
                            ClassID = row["ClassID"]?.ToString(),
                            SectionID = row["SectionID"]?.ToString(),
                            Rollno = row["Rollno"]?.ToString(),
                            MaxID = row["MaxID"]?.ToString(),
                            UnitID = row["UnitID"]?.ToString(),
                            SubjectID = row["SubjectID"]?.ToString(),
                            OptionalID = row["OptionalID"]?.ToString(),
                            Current_Session = row["Current_Session"]?.ToString(),
                            SessionID = row["SessionID"]?.ToString(),
                            Status = row["Status"] != DBNull.Value ? Convert.ToInt32(row["Status"]) : (int?)null,
                            Marks = row["Marks"] != DBNull.Value ? Convert.ToDecimal(row["Marks"]) : (decimal?)null,
                            Date = row["Date"] != DBNull.Value ? Convert.ToDateTime(row["Date"]) : (DateTime?)null,
                            ClassName = row["ClassName"]?.ToString(),
                            SubjectName = row["SubjectName"]?.ToString(),
                            SectionName = row["SectionName"]?.ToString(),
                            UnitName = row["UnitName"]?.ToString(),
                           // StudentName=row["StudentName"]?.ToString()
                        });
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Records found.";
                    response.ResponseData = list;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MarksService", "GetMarksWithNames", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="marksId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteMarks(string marksId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to delete record."
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Perform Hard Delete
                string deleteQuery = "DELETE FROM Marks WHERE MarksID = @MarksID";
                SqlParameter[] deleteParams = { new SqlParameter("@MarksID", marksId) };

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, deleteParams);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Marks record deleted successfully.";
                }
                else
                {
                    response.Message = "No record found with the given MarksID.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while deleting marks.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("MarksService", "DeleteMarks", ex.ToString());
            }

            return response;
        }

    }
}

