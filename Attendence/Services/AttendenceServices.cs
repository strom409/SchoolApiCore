using Attendence.Repository;
using Attendence.Repository.SQL;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Attendence.Services
{
    public class AttendenceServices:IAttendenceServices
    {
        private readonly IConfiguration _configuration;

        public AttendenceServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendance"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddAttendance(AttendanceDTO attendance, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@classid", attendance.ClassID),
            new SqlParameter("@SectionID", attendance.SectionID),
            new SqlParameter("@StudentID", attendance.StudentID),
            new SqlParameter("@Status", attendance.Status),
            new SqlParameter("@session", attendance.Current_Session),
            new SqlParameter("@Date", attendance.Date)
        };
                #endregion

                #region Check if Attendance Already Exists
                string checkQuery = "SELECT COUNT(*) AS val FROM attendance WHERE studentid=@StudentID AND date=@Date";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
                int existingCount = 0;

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    existingCount = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }

                if (existingCount > 0)
                {
                    response.Message = "Attendance Already Added.";
                    return response;
                }
                #endregion

                #region Insert Attendance Record
                string insertQuery = @"
            INSERT INTO attendance (classid, sectionid, studentid, current_session, date, status)
            VALUES (@classid, @SectionID, @StudentID, @session, @Date, @Status)";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Attendance Added Successfully.";
                }
                else
                {
                    response.Message = "Attendance Not Added.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceList"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddAttendanceList(List<AttendanceDTO> attendanceList, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                var resultMessages = new List<string>();
                int successCount = 0;

                foreach (var attendance in attendanceList)
                {
                    SqlParameter[] parameters =
                    {
                new SqlParameter("@classid", attendance.ClassID),
                new SqlParameter("@SectionID", attendance.SectionID),
                new SqlParameter("@StudentID", attendance.StudentID),
                new SqlParameter("@Status", attendance.Status),
                new SqlParameter("@session", attendance.Current_Session),
                new SqlParameter("@Date", attendance.Date)
            };

                    string checkQuery = "SELECT COUNT(*) AS val FROM attendance WHERE studentid=@StudentID AND date=@Date";
                    DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);

                    int existingCount = 0;
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        existingCount = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                    }

                    if (existingCount > 0)
                    {
                        resultMessages.Add($"StudentID {attendance.StudentID}: Already Exists.");
                        continue;
                    }

                    string insertQuery = @"
                INSERT INTO attendance (classid, sectionid, studentid, current_session, date, status)
                VALUES (@classid, @SectionID, @StudentID, @session, @Date, @Status)";

                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);

                    if (rowsAffected > 0)
                    {
                        successCount++;
                        resultMessages.Add($"StudentID {attendance.StudentID}: Added Successfully.");
                    }
                    else
                    {
                        resultMessages.Add($"StudentID {attendance.StudentID}: Failed to Add.");
                    }
                }

                response.Status = successCount > 0 ? 1 : 0;
                response.Message = successCount > 0
                    ? $"Added {successCount} attendance record(s)."
                    : "No attendance records were added.";
                response.ResponseData = resultMessages;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendance"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateTodaysAttendance(AttendanceDTO attendance, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@SectionID", attendance.SectionID),
            new SqlParameter("@StudentID", attendance.StudentID),
            new SqlParameter("@Status", attendance.Status),
            new SqlParameter("@session", attendance.Current_Session),
            new SqlParameter("@Date", attendance.Date)
        };
                #endregion

                #region Update Attendance
                string updateQuery = @"
            UPDATE attendance 
            SET status = @Status
            WHERE sectionid = @SectionID 
              AND current_session = @session 
              AND date = @Date 
              AND studentid = @StudentID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Attendance Updated Successfully.";
                }
                else
                {
                    response.Status = -1;
                    response.Message = "Attendance Not Updated.";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTodaysAttendance(string sessionId, string date, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(date))
                {
                    response.Message = "Date or Session is null!";
                    return response;
                }

                #region SQL + Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@session", sessionId),
            new SqlParameter("@dat", date)
        };

                #endregion
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "TodaysAttendees", parameters);
                #endregion

                #region Map Data
                var attendanceList = new List<Attendance>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        attendanceList.Add(new Attendance
                        {
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionName = dr["Section"]?.ToString(),
                            Prescent = dr["Present"]?.ToString(),
                            Abscent = dr["Abscent"]?.ToString(),
                            Leave = dr["Leave"]?.ToString(),
                            Halfleave = dr["Halfleave"]?.ToString(),
                            SectionID = dr["SectionID"]?.ToString()
                        });
                    }
                }
                #endregion

                #region Set Response
                if (attendanceList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = attendanceList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceService", "GetTodaysAttendance", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEditAttendance(string sectionId, string date, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(sectionId) || string.IsNullOrEmpty(date))
                {
                    response.Message = "Section or Date is null!";
                    return response;
                }

                #region Prepare Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@sectionid", sectionId),
            new SqlParameter("@dat", date)
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string sqlQuery = @"
            SELECT * 
            FROM students 
            INNER JOIN attendance ON students.studentid = attendance.studentid 
            INNER JOIN studentinfo ON students.studentid = studentinfo.studentid  
            WHERE studentinfo.sectionid = @sectionid AND date = @dat 
            ORDER BY rollno";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlQuery, parameters);
                #endregion

                #region Map Results
                List<Attendance> attendanceList = new List<Attendance>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        attendanceList.Add(new Attendance
                        {
                            StudentName = dr["StudentName"]?.ToString(),
                            Rollno = dr["rollno"]?.ToString(),
                            Status = dr["Status"]?.ToString(),
                            StudentID = dr["StudentID"]?.ToString()
                        });
                    }
                }
                #endregion

                #region Set Response
                if (attendanceList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = attendanceList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceService", "GetEditAttendance", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAbsentList(string session, string date, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(session) || string.IsNullOrEmpty(date))
                {
                    response.Message = "Date or Session is null!";
                    return response;
                }

                #region SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@session", session),
            new SqlParameter("@dat", date)
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT 
                attendance.classid AS CID,
                attendance.sectionid AS SecID,
                studentname,
                Rollno,
                classes.classname AS ClassName,
                sectionname,
                phoneno,
                PhoneNo2,
                perminantaddress,
                PresentAddress,
                students.studentid AS SID
            FROM attendance
            INNER JOIN students ON students.studentid = attendance.studentid
            INNER JOIN classes ON attendance.classid = classes.classid
            INNER JOIN sections ON sections.sectionid = attendance.sectionid
            INNER JOIN studentinfo ON students.studentid = studentinfo.studentid
            WHERE 
                status = 'A' 
                AND attendance.date = @dat 
                AND studentinfo.current_session = @session";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Result
                List<Attendance> absentList = new List<Attendance>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        absentList.Add(new Attendance
                        {
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionName = dr["SectionName"]?.ToString(),
                            ClassID = dr["CID"]?.ToString(),
                            SectionID = dr["SecID"]?.ToString(),
                            StudentName = dr["StudentName"]?.ToString(),
                            FatherPhone = dr["PhoneNo"]?.ToString(),
                            MotherPhone = dr["PhoneNo2"]?.ToString(),
                            Address = dr["perminantaddress"]?.ToString(),
                            Rollno = dr["Rollno"]?.ToString(),
                            PresentAddress = dr["PresentAddress"]?.ToString(),
                            StudentID = dr["SID"]?.ToString()
                        });
                    }
                }
                #endregion

                #region Set Response
                if (absentList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = absentList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceService", "GetAbsentList", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="Session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getAttendanceListOnDates(string dateFrom, string dateTo, string Session, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            try
            {
                if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo) || string.IsNullOrEmpty(Session))
                {
                    response.Message = "Invalid parameter: dateFrom, dateTo, Session";
                    return response;
                }

                #region SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@datefrom", dateFrom),
            new SqlParameter("@dateto", dateTo),
            new SqlParameter("@Session", Session)
        };
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT 
                Rollno,
                attendance.studentid AS SID,
                attendance.date AS Date,
                attendance.status AS status,
                attendance.classid AS CID,
                attendance.sectionid AS SecID,
                studentname,
                classes.classname AS ClassName,
                sectionname,
                phoneno,
                PhoneNo2,
                perminantaddress
            FROM attendance
            INNER JOIN students ON students.studentid = attendance.studentid
            INNER JOIN classes ON attendance.classid = classes.classid
            INNER JOIN sections ON sections.sectionid = attendance.sectionid
            INNER JOIN studentinfo ON studentinfo.studentid = students.studentid
            WHERE 
                attendance.date BETWEEN @datefrom AND @dateto 
                AND studentinfo.current_session = @Session
            ORDER BY Rollno";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Results
                var attendanceList = new List<Attendance>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        attendanceList.Add(new Attendance
                        {
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionName = dr["SectionName"]?.ToString(),
                            ClassID = dr["CID"]?.ToString(),
                            SectionID = dr["SecID"]?.ToString(),
                            StudentName = dr["StudentName"]?.ToString(),
                            FatherPhone = dr["PhoneNo"]?.ToString(),
                            MotherPhone = dr["PhoneNo2"]?.ToString(),
                            Address = dr["perminantaddress"]?.ToString(),
                            Status = dr["status"]?.ToString(),
                            Rollno = dr["Rollno"]?.ToString(),
                            StudentID = dr["SID"]?.ToString(),
                            Date = Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy")
                        });
                    }
                }
                #endregion

                #region Return Response
                if (attendanceList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = attendanceList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceService", "GetAttendanceBetweenDates", ex.ToString());
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="sectionId"></param>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> CheckAttendanceAddedorNot(string classId, string sectionId, string date, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(sectionId) || string.IsNullOrEmpty(date))
                {
                    response.Message = "Invalid parameters: ClassID, SectionID, and Date are required.";
                    return response;
                }
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Query
                string query = @"SELECT COUNT(attendanceid) AS val 
                         FROM attendance 
                         WHERE classid = @ClassID AND sectionid = @SectionID AND date = @Date";

                SqlParameter[] sqlParams =
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@Date", date)
        };
                #endregion

                #region Execute Query using ExecuteDatasetAsync
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                    if (count > 0)
                    {
                        response.Message = "Attendance Already Added.";
                    }
                    else
                    {
                        response.Status = 1;
                        response.Message = "Attendance Not Added.";
                    }
                }
                else
                {
                    response.Status = 1;
                    response.Message = "Attendance Not Added.";
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="className"></param>
        /// <param name="sectionName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetMonthlyAttendance(string session, string dateFrom, string dateTo, string className, string sectionName, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(session) ||
                    string.IsNullOrEmpty(dateFrom) ||
                    string.IsNullOrEmpty(dateTo) ||
                    string.IsNullOrEmpty(className) ||
                    string.IsNullOrEmpty(sectionName))
                {
                    response.Message = "Invalid parameters: session, dateFrom, dateTo, className, sectionName are all required.";
                    return response;
                }
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters and Execute Stored Procedure
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@session", session),
            new SqlParameter("@datefrom", dateFrom),
            new SqlParameter("@dateto", dateTo),
            new SqlParameter("@classname", className),
            new SqlParameter("@sectionname", sectionName),
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "AttendenceDatewiseNew", sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = ConvertDataTableRowsOnly(ds.Tables[0]);
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static object ConvertDataTableRowsOnly(DataTable dt)
        {
            return dt.AsEnumerable()
                     .Select(row => dt.Columns.Cast<DataColumn>()
                         .ToDictionary(col => col.ColumnName, col => row[col]))
                     .ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="classId"></param>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getAttendanceListOnDateswithclassid( string dateFrom,string dateTo,
            string classId, string sectionId,string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo) ||
                    string.IsNullOrEmpty(classId) || string.IsNullOrEmpty(sectionId))
                {
                    response.Message = "Invalid parameters: dateFrom, dateTo, classId, and sectionId are required.";
                    return response;
                }
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@datefrom", dateFrom),
            new SqlParameter("@dateto", dateTo),
            new SqlParameter("@classid", classId),
            new SqlParameter("@sectionid", sectionId),
        };

                string query = @"
            SELECT 
                Rollno, 
                attendance.studentid AS SID, 
                attendance.date AS Date, 
                attendance.status AS status,
                attendance.classid AS cid, 
                attendance.sectionid AS secid, 
                studentname, 
                classes.classname AS classname,
                sectionname, 
                phoneno, 
                PhoneNo2, 
                perminantaddress
            FROM attendance
            INNER JOIN students ON students.studentid = attendance.studentid
            INNER JOIN classes ON attendance.classid = classes.classid
            INNER JOIN sections ON sections.sectionid = attendance.sectionid
            INNER JOIN studentinfo ON studentinfo.studentid = students.studentid
            WHERE attendance.date BETWEEN @datefrom AND @dateto
              AND studentinfo.classid = @classid
              AND studentinfo.sectionid = @sectionid
              AND discharged = 0
            ORDER BY Rollno";
                #endregion

                #region Execute & Map
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                var attendanceList = new List<Attendance>();
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        attendanceList.Add(new Attendance
                        {
                            ClassName = dr["ClassName"].ToString(),
                            SectionName = dr["SectionName"].ToString(),
                            ClassID = dr["CID"].ToString(),
                            SectionID = dr["SecID"].ToString(),
                            StudentName = dr["StudentName"].ToString(),
                            FatherPhone = dr["phoneno"].ToString(),
                            MotherPhone = dr["PhoneNo2"].ToString(),
                            Address = dr["perminantaddress"].ToString(),
                            Status = dr["status"].ToString(),
                            Rollno = dr["Rollno"].ToString(),
                            StudentID = dr["SID"].ToString(),
                            Date = Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy")
                        });
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = attendanceList;
                }

                return response;
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="sectionId"></param>
        /// <param name="session"></param>
        /// <param name="date"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetPendingAttendanceStudents(string classId, string sectionId, string session, string date, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No pending students found!"
            };

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(classId) ||
                    string.IsNullOrEmpty(sectionId) ||
                    string.IsNullOrEmpty(session) ||
                    string.IsNullOrEmpty(date))
                {
                    response.Message = "Missing required parameters: classId, sectionId, session, or date.";
                    return response;
                }
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query to Fetch Pending Attendance
                string query = @"
            SELECT 
                s.studentid AS StudentID, 
                s.studentname AS StudentName,
                si.classid AS ClassID,
                si.sectionid AS SectionID,
                si.current_session AS Session
            FROM students s
            INNER JOIN studentinfo si ON s.studentid = si.studentid
            WHERE si.classid = @ClassID
              AND si.sectionid = @SectionID
              AND si.current_session = @Session
              AND s.studentid NOT IN (
                  SELECT studentid FROM attendance 
                  WHERE date = @Date 
                    AND classid = @ClassID 
                    AND sectionid = @SectionID
              )
              AND si.Isdischarged = 0
            ORDER BY s.studentname";

                SqlParameter[] sqlParams =
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@Session", session),
            new SqlParameter("@Date", date)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var pendingList = ds.Tables[0].AsEnumerable()
                        .Select(row => new
                        {
                            ClassID = row["ClassID"].ToString(),
                            SectionID = row["SectionID"].ToString(),
                            StudentID = row["StudentID"].ToString(),
                            StudentName = row["StudentName"].ToString(),
                            Session = row["Session"].ToString(),
                            Date = date,
                            Status = "Pending"
                        }).ToList();

                    response.Status = 1;
                    response.Message = "Pending attendance students retrieved.";
                    response.ResponseData = pendingList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAttendanceMarkedOrNotAsPerDate(string session, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Records Found!",
                Status = 0
            };
            #endregion

            try
            {
                #region Input Validation
                if (string.IsNullOrEmpty(session))
                {
                    rp.Status = -1;
                    rp.IsSuccess = false;
                    rp.Message = "Session input is required.";
                    return rp;
                }

                if (string.IsNullOrEmpty(clientId))
                {
                    rp.Status = -1;
                    rp.IsSuccess = false;
                    rp.Message = "Client ID is required.";
                    return rp;
                }

                string[] parts = session.Split(',');
                if (parts.Length != 3)
                {
                    rp.Status = -1;
                    rp.IsSuccess = false;
                    rp.Message = "Invalid input format. Expected: 'session,fromDate,toDate'";
                    return rp;
                }

                string currentSession = parts[0];
                if (!DateTime.TryParse(parts[1], out DateTime startDate) || !DateTime.TryParse(parts[2], out DateTime endDate))
                {
                    rp.Status = -1;
                    rp.IsSuccess = false;
                    rp.Message = "Invalid date format.";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@StartDate", startDate),
            new SqlParameter("@EndDate", endDate),
            new SqlParameter("@Session", currentSession)
        };
                #endregion

                #region Execute Stored Procedure
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "GetPivotedAttendanceStatus", sqlParams);
                #endregion

                #region Process DataSet Result
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<Dictionary<string, object>> resultList = new List<Dictionary<string, object>>();

                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        Dictionary<string, object> rowDict = new Dictionary<string, object>();
                        foreach (DataColumn column in ds.Tables[0].Columns)
                        {
                            rowDict[column.ColumnName] = row[column];
                        }
                        resultList.Add(rowDict);
                    }

                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = resultList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.Status = -1;
                rp.IsSuccess = false;
                rp.Message = "Error: " + ex.Message;

                var errorLog = new Repository.Error.ErrorLog
                {
                    Title = "GetAttendanceMarkedOrNotAsPerDate",
                    PageName = "Attendance Service",
                    Error = ex.ToString()
                };
                Repository.Error.ErrorBLL.CreateErrorLog(errorLog);
                #endregion
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="sectionId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAttendanceReport(string classId, string sectionId, string dateFrom, string dateTo, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!",
                ResponseData = null
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                SqlParameter[] sqlParams = new SqlParameter[]
                {
            new SqlParameter("@ClassID", classId),
            new SqlParameter("@SectionID", sectionId),
            new SqlParameter("@DateFrom", dateFrom),
            new SqlParameter("@DateTo", dateTo)
                };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "SP_AttendanceReport", sqlParams);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var data = MapDataTableToList(ds.Tables[0]);

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Attendance Report fetched successfully.";
                    response.ResponseData = data; // Now this is serializable
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("AttendanceService", "GetAttendanceReport", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<Dictionary<string, object>> MapDataTableToList(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                }
                list.Add(dict);
            }

            return list;
        }

    }
}
