using ClassTest.Repository;
using ClassTest.Repository.SQL;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace ClassTest.Services
{
    public class ClassTestService : IClassTestService
    {
        private readonly IConfiguration _configuration;
        public ClassTestService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddClassTestMaxMarks(List<ClassTestDTO> list, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Class Test not added!" };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                int insertedCount = 0;

                foreach (var ts in list)
                {
                    SqlParameter[] param =
                    {
                new SqlParameter("@SSMaxMarks", ts.SSMaxMarks),
                new SqlParameter("@SSMinMarks", ts.SSMinMarks),
                new SqlParameter("@Classid", ts.Classid),
                new SqlParameter("@Subjectid", ts.Subjectid),
                new SqlParameter("@SubSubjectid", ts.SubSubjectid),
                new SqlParameter("@Date", ts.ClassTestName),
                new SqlParameter("@Current_Session", ts.Current_Session),
            };

                    #region Check for Duplicacy using Dataset
                    string checkQuery = @"
                SELECT COUNT(SSMaxID) AS val 
                FROM SubSubjectMaxMarks 
                WHERE Classid = @Classid 
                    AND Current_Session = @Current_Session 
                    AND Subjectid = @Subjectid 
                    AND SubSubjectid = @SubSubjectid 
                    AND ClassTestName = @Date";

                    var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);
                    int exists = 0;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[0].Rows[0]["val"]?.ToString(), out exists);
                    }

                    if (exists > 0)
                    {
                        response.Message = "Maxmarks Already Added.";
                        continue;
                    }
                    #endregion

                    #region Insert Max Marks Record
                    string insertQuery = @"
                INSERT INTO SubSubjectMaxMarks 
                (SSMaxMarks, SSMinMarks, Classid, Subjectid, SubSubjectid, ClassTestName, Current_Session)
                VALUES 
                (@SSMaxMarks, @SSMinMarks, @Classid, @Subjectid, @SubSubjectid, @Date, @Current_Session)";

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);
                    if (result > 0)
                    {
                        insertedCount++;
                    }
                    else
                    {
                        response.Message = "Not Added.";
                    }
                    #endregion
                }

                #region Final Response
                if (insertedCount > 0)
                {
                    response.Status = 1;
                    response.Message = $"({insertedCount}) records added.";
                }
                else if (response.Message == "Class Test not added!")
                {
                    response.Message = "No record added!";
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "AddClassTestMaxMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddClassTestMarks(List<ClassTestDTO> list, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Class Test not added!" };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                foreach (var ts in list)
                {
                    // Use two separate parameters for ClassTest and Date
                    SqlParameter[] param =
                    {
                new SqlParameter("@SSMaxID", ts.SSMaxID ?? (object)DBNull.Value),
                new SqlParameter("@SSMaxMarks", ts.SSMaxMarks ?? (object)DBNull.Value),
                new SqlParameter("@SSMinMarks", ts.SSMinMarks ?? (object)DBNull.Value),
                new SqlParameter("@Classid", ts.Classid ?? (object)DBNull.Value),
                new SqlParameter("@SectionID", ts.SectionID ?? (object)DBNull.Value),
                new SqlParameter("@Subjectid", ts.Subjectid ?? (object)DBNull.Value),
                new SqlParameter("@SubSubjectid", ts.SubSubjectid ?? (object)DBNull.Value),
                new SqlParameter("@ClassTest", ts.ClassTestName?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value),
                new SqlParameter("@Date", ts.ClassTestName?.ToString("yyyy-MM-dd") ?? (object)DBNull.Value),
                new SqlParameter("@Current_Session", ts.Current_Session ?? (object)DBNull.Value),
                new SqlParameter("@StudentID", ts.StudentID ?? (object)DBNull.Value),
                new SqlParameter("@Rollno", ts.Rollno ?? (object)DBNull.Value),
                new SqlParameter("@PAStatus", ts.PAStatus ?? (object)DBNull.Value),
                new SqlParameter("@Marks", ts.Marks ?? (object)DBNull.Value)
            };

                    #region Check Duplicacy
                    string checkQuery = @"
                SELECT COUNT(ssmarksid) AS val 
                FROM SubSubjectMarks 
                WHERE StudentID = @StudentID AND ClassID = @Classid 
                    AND SectionID = @SectionID AND Current_Session = @Current_Session 
                    AND SubSubjectID = @SubSubjectid AND ClassTest = @ClassTest";

                    var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);
                    int exists = 0;
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[0].Rows[0]["val"]?.ToString(), out exists);
                    }

                    if (exists > 0)
                    {
                        response.Message = "Class Test Marks Already Added.";
                        return response;
                    }
                    #endregion

                    #region Insert Record
                    string insertQuery = @"
                INSERT INTO SubSubjectMarks 
                (StudentID, ClassID, SectionID, Rollno, SSMaxID, ClassTest, SubjectID, SubSubjectID, 
                 Current_Session, Status, Date, Marks) 
                VALUES 
                (@StudentID, @Classid, @SectionID, @Rollno, @SSMaxID, @ClassTest, @Subjectid, @SubSubjectid, 
                 @Current_Session, @PAStatus, @Date, @Marks)";

                    int inserted = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                    if (inserted > 0)
                    {
                        response.Status = 1;
                        response.Message = "Marks Added Successfully.";
                    }
                    else
                    {
                        response.Message = "Not Updated.";
                    }
                    #endregion
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "AddClassTestMarks", ex.ToString());
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateClassTestMaxMarks(List<ClassTestDTO> list, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                var connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                foreach (var trd in list)
                {
                    #region SQL Parameters
                    SqlParameter[] parameters =
                    {
                new SqlParameter("@SSMaxID", trd.SSMaxID),
                new SqlParameter("@SSMaxMarks", trd.SSMaxMarks),
                new SqlParameter("@SSMinMarks", trd.SSMinMarks)
            };
                    #endregion

                    #region SQL Execution
                    string sqlQuery = @"UPDATE SubSubjectMaxMarks 
                                SET SSMaxMarks = @SSMaxMarks, SSMinMarks = @SSMinMarks 
                                WHERE SSMaxID = @SSMaxID";

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, parameters);
                    #endregion

                    if (result > 0)
                    {
                        response.Status = 1;
                        response.Message = "Updated Successfully.";
                        return response;
                    }
                    else
                    {
                        response.Message = "Not Updated.";
                        return response;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "UpdateClassTestMaxMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditUpdateClassTestMarks(List<ClassTestDTO> list, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                var connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                foreach (var trd in list)
                {
                    #region Parameters
                    SqlParameter[] parameters =
                    {
                new SqlParameter("@SSMarksid", trd.SSMarksid),
                new SqlParameter("@Marks", trd.Marks),
                new SqlParameter("@PAStatus", trd.PAStatus)
            };
                    #endregion

                    #region SQL Execution
                    string sqlQuery = @"UPDATE SubSubjectMarks 
                                SET Marks = @Marks, Status = @PAStatus 
                                WHERE SSMarksid = @SSMarksid";

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, parameters);
                    #endregion

                    if (result > 0)
                    {
                        response.Status = 1;
                        response.Message = "Updated Successfully.";
                        return response;
                    }
                    else
                    {
                        response.Message = "Not Updated.";
                        return response;
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "EditUpdateClassTestMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSubjectForMaxMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value is null!";
                    return response;
                }

                string[] values = param.Split(',');

                if (values.Length != 5)
                {
                    response.Message = "Invalid parameter count. Required: classId,subjectId,subSubjectId,date,session";
                    return response;
                }

                #region Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@classid", values[0]),
            new SqlParameter("@subjectid", values[1]),
            new SqlParameter("@Subsubjectid", values[2]),
            new SqlParameter("@date", values[3]),
            new SqlParameter("@session", values[4])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Already Added
                string checkQuery = @"
    SELECT COUNT(SSMaxID)
    FROM SubSubjectMaxMarks
    WHERE 
        Classid = @classid AND
        Current_Session = @session AND
        Subjectid = @subjectid AND
        SubSubjectid = @Subsubjectid AND
        ClassTestName = @date";

                int existingCount = 0;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand(checkQuery, conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddRange(parameters);

                        object result = await cmd.ExecuteScalarAsync();
                        existingCount = result != null ? Convert.ToInt32(result) : 0;
                    }
                }

                if (existingCount > 0)
                {
                    response.Message = "Maxmarks already added.";
                    return response;
                }
                #endregion


                #region Fetch Subjects
                string dataQuery = @"
            SELECT subsubjectid, SubSubjectName 
            FROM SubSubjects 
            WHERE 
                subsubjectid = @Subsubjectid AND 
                subjectid = @subjectid";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dataQuery, parameters);
                var subjectList = new List<ClassTest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjectList.Add(new ClassTest
                        {
                            SubSubjectid = dr["subsubjectid"]?.ToString(),
                            SubSubjectName = dr["SubSubjectName"]?.ToString()
                        });
                    }

                    if (subjectList.Count > 0)
                    {
                        response.Status = 1;
                        response.Message = "ok";
                        response.ResponseData = subjectList;
                    }
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetSubjectForMaxMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMaxMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Input is null!";
                    return response;
                }

                string[] values = param.Split(',');

                if (values.Length != 5)
                {
                    response.Message = "Invalid parameter count. Required: classId, subjectId, subSubjectId, date, session";
                    return response;
                }

                #region Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@classid", values[0]),
            new SqlParameter("@subjectid", values[1]),
            new SqlParameter("@Subsubjectid", values[2]),
            new SqlParameter("@date", values[3]),
            new SqlParameter("@session", values[4])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Execution
                var query = @"
            SELECT 
                SSMaxID,
                SubSubjectName,
                SSMaxMarks,
                SSMinMarks
            FROM 
                SubSubjectMaxMarks 
            INNER JOIN 
                SubSubjects ON SubSubjectMaxMarks.SubSubjectid = SubSubjects.SubSubjectID
            WHERE 
                classid = @classid AND
                SubSubjectMaxMarks.subsubjectid = @Subsubjectid AND
                SubSubjectMaxMarks.Subjectid = @subjectid AND
                SubSubjectMaxMarks.Current_Session = @session AND
                classtestname = @date";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Result
                List<ClassTest> maxMarksList = new List<ClassTest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        maxMarksList.Add(new ClassTest
                        {
                            SSMaxID = dr["SSMaxID"]?.ToString(),
                            SubSubjectName = dr["SubSubjectName"]?.ToString(),
                            SSMaxMarks = dr["SSMaxMarks"]?.ToString(),
                            SSMinMarks = dr["SSMinMarks"]?.ToString()
                        });
                    }
                }

                if (maxMarksList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = maxMarksList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetMaxMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudents(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                string[] parts = param.Split(',');
                if (parts.Length < 6)
                {
                    response.Message = "Invalid parameter format!";
                    return response;
                }

                #region Parameters
                string classId = parts[0];
                string sectionId = parts[1];
                string session = parts[2];
                string date = parts[3];
                string subjectId = parts[4];
                string subSubjectId = parts[5];
                #endregion

                #region Connection
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL + Param
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@sectionid", sectionId),
            new SqlParameter("@session", session),
            new SqlParameter("@Date", date),
            new SqlParameter("@subjectID", subjectId),
            new SqlParameter("@SubsubjectID", subSubjectId),
        };

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT Students.StudentID AS id, StudentName, RollNo " +
                    "FROM Students " +
                    "INNER JOIN Studentinfo ON Studentinfo.StudentId = Students.StudentID " +
                    "WHERE classid = @classid AND sectionid = @sectionid AND Studentinfo.Current_Session = @session " +
                    "AND discharged = 'False' AND Isdischarged = 0 ORDER BY RollNo", sqlParams);
                #endregion

                #region Map Result
                var studentList = new List<ClassTest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var student = new ClassTest
                        {
                            StudentID = dr["id"]?.ToString(),
                            StudentName = dr["StudentName"]?.ToString(),
                            Rollno = dr["RollNo"]?.ToString()
                        };

                        // Get max marks range
                        string markRange = await GetMaxMarksrange(classId, date, subjectId, subSubjectId, session, clientId);
                        var marks = markRange.Split(',');

                        student.SSMaxID = marks[0];
                        student.SSMaxMarks = marks[1];
                        student.SSMinMarks = marks[2];

                        studentList.Add(student);
                    }
                }
                #endregion

                #region Set Response
                if (studentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetStudents", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentsWithMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Parameters
                string[] parts = param.Split(',');
                if (parts.Length < 6)
                {
                    response.Message = "Invalid parameter format!";
                    return response;
                }

                string classId = parts[0];
                string sectionId = parts[1];
                string session = parts[2];
                string date = parts[3];
                string subjectId = parts[4];
                string subSubjectId = parts[5];
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL + Params
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@sectionid", sectionId),
            new SqlParameter("@session", session),
            new SqlParameter("@Date", date),
            new SqlParameter("@subjectID", subjectId),
            new SqlParameter("@SubsubjectID", subSubjectId)
        };

                string query = @"SELECT 
                            Students.StudentID AS id,
                            StudentName,
                            Studentinfo.RollNo AS RollNo,
                            Status,
                            marks,
                            ssmarksid,
                            phoneno,
                            ssmaxmarks,
                            ssminmarks,
                            SubSubjectMaxMarks.ssmaxid AS SSmaxid
                         FROM Students 
                         INNER JOIN Studentinfo ON Studentinfo.StudentId = Students.StudentID 
                         INNER JOIN SubSubjectMarks ON Studentinfo.StudentId = SubSubjectMarks.studentid 
                         INNER JOIN SubSubjectMaxMarks ON SubSubjectMaxMarks.ssmaxid = SubSubjectMarks.ssmaxid 
                         WHERE 
                             Studentinfo.classid = @classid AND 
                             Studentinfo.sectionid = @sectionid AND 
                             Studentinfo.Current_Session = @session AND 
                             discharged = 'False' AND 
                             Isdischarged = 0 AND 
                             SubSubjectMarks.SubSubjectID = @SubsubjectID AND 
                             classtest = @Date 
                         ORDER BY RollNo";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Data
                var classTestList = new List<ClassTest>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        classTestList.Add(new ClassTest
                        {
                            SSMarksid = dr["SSMarksid"]?.ToString(),
                            StudentID = dr["id"]?.ToString(),
                            StudentName = dr["StudentName"]?.ToString(),
                            Rollno = dr["RollNo"]?.ToString(),
                            SSMaxMarks = dr["ssmaxmarks"]?.ToString(),
                            SSMinMarks = dr["ssminmarks"]?.ToString(),
                            SSMaxID = dr["SSMaxID"]?.ToString(),
                            PAStatus = dr["Status"]?.ToString(),
                            Marks = dr["marks"]?.ToString(),
                            PhobeNo = dr["phoneno"]?.ToString()
                        });
                    }
                }
                #endregion

                #region Set Response
                if (classTestList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = classTestList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetStudentsWithMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ViewDateWiseResult(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Params
                string[] values = param.Split(',');
                if (values.Length != 7)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                SqlParameter[] parameters =
                {
            new SqlParameter("@session", values[0]),
            new SqlParameter("@datefrom", values[1]),
            new SqlParameter("@dateto", values[2]),
            new SqlParameter("@classID", values[3]),
            new SqlParameter("@sectionID", values[4]),
            new SqlParameter("@subjectID", values[5]),
            new SqlParameter("@subsubjectID", values[6])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "GetClasstetdatewise", parameters);
                #endregion
                #region Inline Conversion to List<Dictionary<string, object>>
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "ViewDateWiseResult", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ClassTestReport(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Params
                string[] parts = param.Split(',');
                if (parts.Length != 7)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                SqlParameter[] sqlParams =
                {
            new SqlParameter("@studentid", parts[0]),
            new SqlParameter("@classid", parts[1]),
            new SqlParameter("@sectionid", parts[2]), // Not used in SP but passed for compatibility
            new SqlParameter("@unitid", parts[3]),
            new SqlParameter("@Current_Session", parts[4]),
            new SqlParameter("@Datefrom", parts[5]),
            new SqlParameter("@Dateto", parts[6])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute SP
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "PrintOneStudentClassTest", sqlParams);
                #endregion

                #region Inline Conversion to List<Dictionary<string, object>>
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "ClassTestReport", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ViewDateWiseResultForAllSubjects(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Params
                string[] parts = param.Split(',');
                if (parts.Length < 5)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                SqlParameter[] parameters =
                {
            new SqlParameter("@datefrom", parts[0]),
            new SqlParameter("@dateto", parts[1]),
            new SqlParameter("@classID", parts[2]),
            new SqlParameter("@sectionID", parts[3]),
            new SqlParameter("@session", parts[4])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute SP
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "GetClasstetdatewiseNew2", parameters);
                #endregion

                #region Convert DataTable to List<Dictionary<string, object>>
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "ViewDateWiseResultForAllSubjects", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ViewDateWiseResultForTotalMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Params
                string[] parts = param.Split(',');
                if (parts.Length < 5)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                SqlParameter[] parameters =
                {
            new SqlParameter("@datefrom", parts[0]),
            new SqlParameter("@dateto", parts[1]),
            new SqlParameter("@classID", parts[2]),
            new SqlParameter("@sectionID", parts[3]),
            new SqlParameter("@session", parts[4])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "GetClasstetdatewisePercentage", parameters);
                #endregion

                #region Convert DataTable to List<Dictionary<string, object>>
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "ViewDateWiseResultForTotalMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> ViewDateWiseTotalMMandObtMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Value null!";
                    return response;
                }

                #region Split & Validate Params
                string[] parts = param.Split(',');
                if (parts.Length < 5)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                SqlParameter[] parameters =
                {
            new SqlParameter("@datefrom", parts[0]),
            new SqlParameter("@dateto", parts[1]),
            new SqlParameter("@classID", parts[2]),
            new SqlParameter("@sectionID", parts[3]),
            new SqlParameter("@session", parts[4])
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getclassMMOBTsubjetwiseTotal", parameters);
                #endregion

                #region Convert DataTable to List<Dictionary<string, object>>
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "ViewDateWiseTotalMMandObtMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMissingClassTestMarks(string param, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(param))
                {
                    response.Message = "Invalid Parameters!";
                    return response;
                }

                string[] parts = param.Split(',');
                if (parts.Length != 6)
                {
                    response.Message = "Invalid parameter count!";
                    return response;
                }

                // Parse parameters
                var classId = int.Parse(parts[0]);
                var sectionId = int.Parse(parts[1]);
                var subjectId = int.Parse(parts[2]);
                var subSubjectId = int.Parse(parts[3]);
                var date = parts[4];
                var session = parts[5];

                #region SQL Query
                string query = @"
        SELECT s.StudentId, s.StudentName, si.RollNo
        FROM Students s
        INNER JOIN StudentInfo si ON s.StudentId = si.StudentId
        WHERE si.ClassID = @classid 
          AND si.SectionID = @sectionid 
          AND si.Current_Session = @session 
          AND s.Discharged = 'false'
          AND NOT EXISTS (
              SELECT 1 
              FROM SubSubjectMarks sm
              WHERE sm.StudentID = s.StudentId
                AND sm.ClassID = @classid
                AND sm.SectionID = @sectionid
                AND sm.SubjectID = @subjectid
                AND sm.SubSubjectID = @subsubjectid
                AND sm.ClassTest = @date
                AND sm.Current_Session = @session
          );
        ";
                #endregion

                SqlParameter[] sqlParams =
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@sectionid", sectionId),
            new SqlParameter("@subjectid", subjectId),
            new SqlParameter("@subsubjectid", subSubjectId),
            new SqlParameter("@date", date),
            new SqlParameter("@session", session)
        };

                #region Execute Query
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Set Response
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var list = new List<Dictionary<string, object>>();
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in ds.Tables[0].Columns)
                        {
                            dict[col.ColumnName] = row[col] is DBNull ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = list;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetMissingClassTestMarks", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="date"></param>
        /// <param name="subjectID"></param>
        /// <param name="subSubjectID"></param>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<string> GetMaxMarksrange(string classId, string date, string subjectID, string subSubjectID, string session, string clientId)
        {
            string val = "0,0,0";
            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@date", date),
            new SqlParameter("@subjectID", subjectID),
            new SqlParameter("@SubsubjectID", subSubjectID),
            new SqlParameter("@session", session),
        };
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT ssmaxid, ssmaxmarks, SSMinMarks FROM SubSubjectMaxMarks WHERE classid=@classid AND ClassTestName=@date AND subjectid=@subjectID AND SubSubjectid=@SubsubjectID AND Current_Session=@session", param);
                #endregion

                #region Parse Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && !string.IsNullOrEmpty(ds.Tables[0].Rows[0][0]?.ToString()))
                {
                    val = ds.Tables[0].Rows[0][0].ToString() + "," +
                          ds.Tables[0].Rows[0][1].ToString() + "," +
                          ds.Tables[0].Rows[0][2].ToString();
                }

                return val;
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("ClassTestService", "GetMaxMarksrange", ex.ToString());
                return "N,N,N";
            }
        }

    }
}
