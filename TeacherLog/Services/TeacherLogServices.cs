using Microsoft.Data.SqlClient;
using System.Data;
using TeacherLog.Repository;
using TeacherLog.Repository.SQL;

namespace TeacherLog.Services
{
    public class TeacherLogServices : ITeacherLogServices
    {
        private readonly IConfiguration _configuration;
        public TeacherLogServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddTeacherLogDataOnSectionIDandDate(TeacherLogData td, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Record added!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param = {
            new SqlParameter("@EmpCode", td.EmployeeCode),
            new SqlParameter("@Period", td.Period),
            new SqlParameter("@Date", td.Date),
            new SqlParameter("@ClassID", td.ClassID),
            new SqlParameter("@SectionID", td.SectionID),
            new SqlParameter("@SubjectID", td.SubjectID),
            new SqlParameter("@SubSubjectID", td.SubSubjectID),
            new SqlParameter("@TotalStrength", td.TotalStrength),
            new SqlParameter("@TotalPresent", td.TotalPresent),
            new SqlParameter("@PresentNow", td.PresentNow),
            new SqlParameter("@Day", td.Day),
            new SqlParameter("@AbsenceReason", td.AbsenceReason),
            new SqlParameter("@Topic", td.Topic),
            new SqlParameter("@HomeWork", td.HomeWork),
            new SqlParameter("@Aids", td.Aids),
            new SqlParameter("@Remarks", td.Remarks),
            new SqlParameter("@SmartClass", "0"),
            new SqlParameter("@StudentIDFK", td.studentid),
            new SqlParameter("@SubSubjectName", td.SubSubjectName),
            new SqlParameter("@SubjectName", td.SubjectName),
            new SqlParameter("@FilePath", td.FilePaths != null ? string.Join(",", td.FilePaths) : (object)DBNull.Value),

        };
                #endregion

                #region Check Duplicate
                string dupQuery = "SELECT COUNT([TeacherLogID]) FROM TeacherLog WHERE SectionID = @SectionID AND Date = @Date AND Period = @Period";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, param);

                int count = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }

                if (count > 0)
                {
                    response.Message = "Logs already added!";
                    return response;
                }
                #endregion


                #region Insert Data
                string insertQuery = @"
        INSERT INTO TeacherLog
        (EmpCode, Period, ClassID, SectionID, SubjectID, SubSubjectID, TotalStrength, TotalPresent, PresentNow, Day, Date, AbsenceReason, Topic, HomeWork, Aids, Remarks, SmartClass,StudentIDFK,SubSubjectName,SubjectName, FilePath)
        VALUES
        (@EmpCode, @Period, @ClassID, @SectionID, @SubjectID, @SubSubjectID, @TotalStrength, @TotalPresent, @PresentNow, @Day, @Date, @AbsenceReason, @Topic, @HomeWork, @Aids, @Remarks, @SmartClass,@StudentIDFK, @SubSubjectName,@SubjectName, @FilePath)";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                if (rowsAffected > 0)
                {
                    response.Message = "Log Added Successfully";
                    response.Status = 1;
                }
                else
                {
                    response.Message = "Log Not Added";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "AddTeacherLogDataOnSectionIDandDate", ex.Message + " | " + ex.StackTrace);
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddTeacherPerformance(TeacherLogData td, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Record added!"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters for Lookup
                SqlParameter[] lookupParams = {
            new SqlParameter("@EmployeeCode", td.EmployeeCode)
        };
                #endregion

                #region Check if Employee Performance Exists
                string checkQuery = "SELECT edi FROM Employeeperformance WHERE ECODE = @EmployeeCode";
                object result = null;

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, lookupParams);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result = ds.Tables[0].Rows[0]["edi"];
                }
                #endregion

                if (result != null && int.TryParse(result.ToString(), out int edi))
                {
                    #region Update Performance
                    SqlParameter[] updateParams = {
                new SqlParameter("@EmployeeCode", td.EmployeeCode),
                new SqlParameter("@Performance", td.Performance),
                new SqlParameter("@EDI", edi)
            };

                    string updateQuery = "UPDATE Employeeperformance SET Performance = @Performance WHERE EDI = @EDI AND ECODE = @EmployeeCode";
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, updateParams);

                    response.Message = rowsAffected > 0 ? "Performance updated successfully." : "Update failed.";
                    response.Status = rowsAffected > 0 ? 1 : 0;
                    #endregion
                }
                else
                {
                    #region Insert Performance
                    SqlParameter[] insertParams = {
                new SqlParameter("@EmployeeCode", td.EmployeeCode),
                new SqlParameter("@Performance", td.Performance)
            };

                    string insertQuery = "INSERT INTO Employeeperformance (ECODE, Performance) VALUES (@EmployeeCode, @Performance)";
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);

                    response.Message = rowsAffected > 0 ? "Performance inserted successfully." : "Insert failed.";
                    response.Status = rowsAffected > 0 ? 1 : 0;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "AddTeacherPerformance", ex.Message + " | " + ex.StackTrace);
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddTeacherLogForNewTiny(TeacherLogData td, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Record added!"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters = {
            new SqlParameter("@date", td.Date),
            new SqlParameter("@SubjectID", td.SubjectID),
            new SqlParameter("@SubjectName", td.SubjectName),
            new SqlParameter("@topic", td.Topic),
            new SqlParameter("@classwork", td.classwork),
            new SqlParameter("@homework", td.HomeWork),
            new SqlParameter("@studentid", td.studentid),
            new SqlParameter("@Session", td.current_session),
            new SqlParameter("@Remarks", td.Remarks),
            new SqlParameter("@Ecode", td.EmployeeCode),
            new SqlParameter("@TeacherName", td.EmployeeName),
            new SqlParameter("@classid", td.ClassID),
            new SqlParameter("@sectionid", td.SectionID),
            new SqlParameter("@SubSubjectid", td.SubSubjectID),
            new SqlParameter("@SubSubjectName", td.SubSubjectName),
            new SqlParameter("@classname", td.ClassName),
            new SqlParameter("@sectionname", td.SectionName),
            new SqlParameter("@FilePath", td.FilePaths != null ? string.Join(",", td.FilePaths) : (object)DBNull.Value),

        };
                #endregion

                #region Execute Stored Procedure
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,   // Use connection string here instead of clientId
                    CommandType.StoredProcedure,
                    "InsertIntoTeacherLog2",
                    parameters
                );

                if (rowsAffected > 0)
                {
                    response.Message = "Log Added Successfully";
                    response.Status = 1;
                }
                else
                {
                    response.Message = "No log was added.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "AddTeacherLogForNewTiny", ex.Message + " | " + ex.StackTrace);
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trd"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateTeacherLog(TeacherLogData trd, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] para =
                {
            new SqlParameter("@ClassID", trd.ClassID),
            new SqlParameter("@SectionID", trd.SectionID),
            new SqlParameter("@SubjectID", trd.SubjectID),
            new SqlParameter("@SubSubjectID", trd.SubSubjectID),
            new SqlParameter("@EmpCode", trd.EmployeeCode),
            new SqlParameter("@Date", trd.Date),
            new SqlParameter("@Topic", trd.Topic),
            new SqlParameter("@classwork", trd.classwork),
            new SqlParameter("@HomeWork", trd.HomeWork),
            new SqlParameter("@Remarks", trd.Remarks),
            //new SqlParameter("@studentid", trd.studentid),
        };
                #endregion

                #region Execute Update Query
                string sqlquery = @"
            UPDATE teacherlog
            SET Topic = @Topic,
                classwork = @classwork,
                HomeWork = @HomeWork,
                Remarks = @Remarks
            WHERE ClassID = @ClassID
              AND SectionID = @SectionID
              AND SubSubjectID = @SubSubjectID
              AND empcode = @EmpCode
              AND Date = @Date";

                int chk = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlquery, para);

                if (chk > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully.";
                }
                else
                {
                    response.Message = "Not Updated.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "UpdateTeacherLog", ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionIdAndDate"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeacherLogDataOnSectionIDandDate(string sectionIdAndDate, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Validate Input
                var data = sectionIdAndDate.Split(',');

                if (data.Length != 2)
                {
                    response.Message = "Invalid data format: SectionID,Date";
                    return response;
                }
                string sectionId = data[0];
                string date = data[1];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@Date", date),
        };
                #endregion

                #region SQL Query
                string sql = @"
            SELECT * FROM TeacherLog
            INNER JOIN Classes ON Classes.ClassID = TeacherLog.ClassID
            INNER JOIN Sections ON Sections.SectionID = TeacherLog.SectionID
            INNER JOIN Employees ON Employees.EmployeeCode = TeacherLog.EmpCode
            WHERE TeacherLog.SectionID = @SectionID AND Date = @Date ";
                #endregion

                #region Execute Query and Map Results
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);

                var teacherLogList = new List<TeacherLogData>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var classResponse = await GetClassName(dr["ClassID"].ToString(), clientId);
                    var sectionResponse = await GetSectionName(dr["SectionID"].ToString(), clientId);
                    var sd = new TeacherLogData
                    {
                        LogId = dr["TeacherLogId"].ToString(),
                        EmployeeCode = dr["EmpCode"].ToString(),
                        EmployeeName = dr["EmployeeName"].ToString(),
                        //Period = dr["Period"].ToString(),
                        ClassID = dr["ClassID"].ToString(),
                        Date = string.IsNullOrEmpty(dr["Date"].ToString())
                            ? DateTime.Now.ToString("dd-MM-yyyy")
                            : Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy"),
                        SectionID = dr["SectionID"].ToString(),
                        SubjectID = dr["SubjectID"].ToString(),
                        SubSubjectID = dr["SubSubjectID"].ToString(),
                        //TotalStrength = dr["TotalStrength"].ToString(),
                        //TotalPresent = dr["TotalPresent"].ToString(),
                        //AbsenceReason = dr["AbsenceReason"].ToString(),
                        HomeWork = dr["HomeWork"].ToString(),
                        Topic = dr["Topic"].ToString(),
                        //PresentNow = dr["PresentNow"].ToString(),
                        //Day = dr["Day"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        ClassName = classResponse.IsSuccess ? classResponse.ResponseData?.ToString() : "NA",
                        SectionName = sectionResponse.IsSuccess ? sectionResponse.ResponseData?.ToString() : "NA"
                    };

                    teacherLogList.Add(sd);
                }

                if (teacherLogList.Count > 0)
                {
                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = teacherLogList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetTeacherLogDataOnSectionIDandDate", ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eCodeAndDate"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeacherLogDataOnECodeandDate(string eCodeAndDate, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Message = "No Records Found!", Status = 0 };

            try
            {
                #region Validate Input
                var data = eCodeAndDate.Split(',');

                if (data.Length != 2)
                {
                    response.Message = "Invalid data format: ECode,Date";
                    return response;
                }
                string eCode = data[0];
                string date = data[1];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@ECode", eCode),
            new SqlParameter("@Date", date),
        };
                #endregion

                #region SQL Query
                string sql = @"
            SELECT * FROM TeacherLog
            INNER JOIN Classes ON Classes.ClassID = TeacherLog.ClassID
            INNER JOIN Sections ON Sections.SectionID = TeacherLog.SectionID
            INNER JOIN Employees ON Employees.EmployeeCode = TeacherLog.EmpCode
            WHERE TeacherLog.EmpCode = @ECode AND Date = @Date
            ORDER BY Period";
                #endregion

                #region Execute Query and Map Results
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, parameters);

                var teacherLogList = new List<TeacherLogData>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    var classResponse = await GetClassName(dr["ClassID"].ToString(), clientId);
                    var sectionResponse = await GetSectionName(dr["SectionID"].ToString(), clientId);
                    var sd = new TeacherLogData
                    {
                        LogId = dr["TeacherLogId"].ToString(),
                        EmployeeCode = dr["EmpCode"].ToString(),
                        EmployeeName = dr["EmployeeName"].ToString(),
                        //Period = dr["Period"].ToString(),
                        ClassID = dr["ClassID"].ToString(),
                        Date = string.IsNullOrEmpty(dr["Date"].ToString())
                            ? DateTime.Now.ToString("dd-MM-yyyy")
                            : Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy"),
                        SectionID = dr["SectionID"].ToString(),
                        SubjectID = dr["SubjectID"].ToString(),
                        SubSubjectID = dr["SubSubjectID"].ToString(),
                       // TotalStrength = dr["TotalStrength"].ToString(),
                       // TotalPresent = dr["TotalPresent"].ToString(),
                       // AbsenceReason = dr["AbsenceReason"].ToString(),
                        HomeWork = dr["HomeWork"].ToString(),
                        Topic = dr["Topic"].ToString(),
                      //  PresentNow = dr["PresentNow"].ToString(),
                        Day = dr["Day"].ToString(),
                        Remarks = dr["Remarks"].ToString(),
                        SubjectName = dr["SubjectName"].ToString(),
                        ClassName = classResponse.IsSuccess ? classResponse.ResponseData?.ToString() : "NA",
                        SectionName = sectionResponse.IsSuccess ? sectionResponse.ResponseData?.ToString() : "NA"
                    };

                    teacherLogList.Add(sd);
                }

                if (teacherLogList.Count > 0)
                {
                    response.Message = "ok";
                    response.Status = 1;
                    response.ResponseData = teacherLogList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetTeacherLogDataOnECodeandDate", ex.Message + " | " + ex.StackTrace);
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSubjectList(string classId, string clientId)
        {
            ResponseModel response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No subjects found." };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                List<TeacherLogData> subjectList = new List<TeacherLogData>();
                SqlParameter[] param = {
            new SqlParameter("@CID", classId ?? string.Empty)
        };
                #endregion

                #region SQL Query
                string sql = @"SELECT SubjectID, SubjectName FROM Subjects WHERE ClassId = @CID";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TeacherLogData subject = new TeacherLogData
                        {
                            SubjectID = dr["SubjectID"]?.ToString(),
                            SubjectName = dr["SubjectName"]?.ToString()
                        };
                        subjectList.Add(subject);
                    }
                }
                #endregion

                #region Set Response
                if (subjectList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = subjectList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetSubjectListAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSubSubjectList(string subjectId, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No sub-subjects found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                List<TeacherLogData> subSubjectList = new List<TeacherLogData>();
                SqlParameter[] param = {
            new SqlParameter("@SID", subjectId ?? string.Empty)
        };
                #endregion

                #region SQL Query
                string sql = @"SELECT SubjectID, SubSubjectID, SubSubjectName 
                       FROM SubSubjects 
                       WHERE SubjectID = @SID";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TeacherLogData subSubject = new TeacherLogData
                        {
                            SubjectID = dr["SubjectID"]?.ToString(),
                            SubSubjectID = dr["SubSubjectID"]?.ToString(),
                            SubSubjectName = dr["SubSubjectName"]?.ToString()
                        };
                        subSubjectList.Add(subSubject);
                    }
                }
                #endregion

                #region Set Response
                if (subSubjectList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = subSubjectList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetSubSubjectListAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetOptSubjectList(string classId, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No optional subjects found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                List<TeacherLogData> subjectList = new List<TeacherLogData>();
                SqlParameter[] param = {
            new SqlParameter("@CID", classId ?? string.Empty)
        };
                #endregion

                #region SQL Query
                string sql = @"SELECT OptionalSubjectID, OptionalSubjectName 
                       FROM OptionalSubjects 
                       WHERE ClassID = @CID";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, param);

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TeacherLogData subject = new TeacherLogData
                        {
                            SubjectID = dr["OptionalSubjectID"]?.ToString(),
                            SubSubjectName = dr["OptionalSubjectName"]?.ToString()
                        };
                        subjectList.Add(subject);
                    }
                }
                #endregion

                #region Set Response
                if (subjectList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = subjectList;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetOptSubjectListAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="date"></param>
       /// <param name="clientId"></param>
       /// <returns></returns>
        public async Task<ResponseModel> GetTeacherLogOnDateList(string date, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@Date", date ?? string.Empty)
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getTeacherLog", param);
                #endregion

                #region Set Response
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ds.Tables[0];
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeacherLogOnDateListAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="empcode"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeacherLogOnDateAndCodeList(string empcode, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@EmpcodeT", empcode ?? string.Empty)
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getTeacherLogoncode", param);
                #endregion

                #region Set Response
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var table = ds.Tables[0];
                    var resultList = new List<Dictionary<string, object>>();

                    foreach (DataRow row in table.Rows)
                    {
                        var rowData = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            rowData[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                        }
                        resultList.Add(rowData);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = resultList;
                }
                return response;

                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeacherLogOnDateAndCodeListAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeachersLog(string date, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@Date", Convert.ToDateTime(date))
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getTeacherLogoondate", param);
                #endregion

                #region Set Response
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var table = ds.Tables[0];
                    var resultList = new List<Dictionary<string, object>>();

                    foreach (DataRow row in table.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                        }
                        resultList.Add(dict);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = resultList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeachersLogAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeachersLogfromTT(string date, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Validate Input
                string[] data = date.Split(',');
                if (data.Length != 3)
                {
                    response.Message = "Invalid format. Expected: date,session,totPeriods";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@date", data[0]),
            new SqlParameter("@session", data[1]),
            new SqlParameter("@totPeriods", data[2])
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "prHomeWorkOnTimeTable", param);
                #endregion

                #region Set Response
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ds.Tables[0];
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeachersLogfromTTAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeachersLogFromTTEmpty(string date, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Validate Input
                string[] data = date.Split(',');
                if (data.Length != 3)
                {
                    response.Message = "Invalid format. Expected: date,session,totPeriods";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@date", data[0]),
            new SqlParameter("@session", data[1]),
            new SqlParameter("@totPeriods", data[2])
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "prHomeWorkOnTimeTable2", param);
                #endregion

                #region Set Response
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ds.Tables[0];
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeachersLogfromTTEmptyAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeacherPerformance(string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL and Execute
                string sql = "SELECT EDI, ECODE, Performance FROM Employeeperformance";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql);
                #endregion

                #region Map Results
                List<TeacherLogData> result = new List<TeacherLogData>();
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new TeacherLogData
                        {
                            EDI = dr["EDI"]?.ToString(),
                            EmployeeCode = dr["ECODE"]?.ToString(),
                            Performance = dr["Performance"]?.ToString()
                        });
                    }
                }
                #endregion

                #region Set Response
                if (result.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = result;
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeacherPerformanceAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateRange"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTeachersLogRangeWise(string dateRange, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Parse Input
                string[] data = dateRange.Split(',');
                if (data.Length != 2 || !DateTime.TryParse(data[0], out DateTime dateFrom) || !DateTime.TryParse(data[1], out DateTime dateTo))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid date format. Expected: 'startDate,endDate'";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter[] sqlParam = new SqlParameter[]
                {
            new SqlParameter("@Datefrom", dateFrom),
            new SqlParameter("@DateTo", dateTo)
                };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, "getTeacherLogoondateRANGE", sqlParam);
                #endregion

                #region Map Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ds.Tables[0];
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "GetTeachersLogRangewiseAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetClassName(string classId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Class not found",
                ResponseData = "NA"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter param = new SqlParameter("@CID", classId ?? (object)DBNull.Value);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT ClassName FROM Classes WHERE ClassId = @CID", param);
                #endregion

                #region Extract Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var val = ds.Tables[0].Rows[0][0]?.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        response.Status = 1;
                        response.Message = "ok";
                        response.ResponseData = val;
                        return response;
                    }
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Log Exception
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetClassNameAsync", ex.Message + " | " + ex.StackTrace);
                #endregion
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = "NA";
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSectionName(string sectionId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Section not found",
                ResponseData = "NA"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or connection string.";
                    return response;
                }
                #endregion

                #region Prepare Parameters
                SqlParameter param = new SqlParameter("@CID", sectionId ?? (object)DBNull.Value);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT SectionName FROM Sections WHERE SectionID = @CID", param);
                #endregion

                #region Extract Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var val = ds.Tables[0].Rows[0][0]?.ToString();
                    if (!string.IsNullOrEmpty(val))
                    {
                        response.Status = 1;
                        response.Message = "ok";
                        response.ResponseData = val;
                        return response;
                    }
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Log Exception
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogServices", "GetSectionNameNameAsync", ex.Message + " | " + ex.StackTrace);
                #endregion
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = "NA";
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="td"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteTeacherLogOnLogID(TeacherLogData td, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Logs not deleted!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionHelper.GetConnectionString(clientId);

                if (string.IsNullOrEmpty(connectionString))
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid client ID or missing connection string.";
                    return response;
                }
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param = {
            new SqlParameter("@date", td.Date),
            new SqlParameter("@empcode", td.EmployeeCode),
            new SqlParameter("@classid", td.ClassID),
            new SqlParameter("@sectionid", td.SectionID),
            new SqlParameter("@subsubjectId", td.SubSubjectID)
        };
                #endregion

                #region Execute Delete Query
                string sql = @"
            DELETE FROM TeacherLog 
            WHERE date = @date AND classid = @classid AND sectionid = @sectionid AND subsubjectid = @subsubjectId AND empcode = @empcode;
            
            DELETE FROM TeacherHistory
            WHERE date = @date AND classid = @classid AND sectionid = @sectionid AND subsubjectid = @subsubjectId AND empcode = @empcode;
        ";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql, param);
                #endregion

                #region Set Response
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Logs deleted successfully!";
                }
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.Status = -1;
                response.IsSuccess = false;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TeacherLogService", "DeleteTeacherLogOnLogIDAsync", ex.Message + " | " + ex.StackTrace);
                return response;
                #endregion
            }
        }
    }
}


