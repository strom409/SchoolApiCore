using Azure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TTAssignPeroids;

namespace Timetable_Arrangement.Services.TimeTableArrangements
{
    public class TimeTableArrangementsServices :ITimeTableArrangementsServices
    {
        private readonly IConfiguration _configuration;
        public TimeTableArrangementsServices(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<ResponseModel> ArrangeTimetable(TimeTableDto td, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Time Table Not added"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@ClassID", td.ClassID),
            new SqlParameter("@SectionID", td.SecID),
            new SqlParameter("@SubjectID", td.SubjectID),
            new SqlParameter("@TeacherID", td.TeacherID),
            new SqlParameter("@PIDFK", td.PIDFK),
            new SqlParameter("@DIDFK", td.DIDFK),
            new SqlParameter("@OnDate", Convert.ToDateTime(td.OnDate)),
            new SqlParameter("@InsertedOn", DateTime.Now.Date),
            new SqlParameter("@User", td.UserName),
            new SqlParameter("@AbscentTeacherID", td.AbscentTeacherID)
        };
                #endregion

                #region SQL Queries
                string checkQuery = @"SELECT COUNT(*) AS Cnt
                              FROM TimeTableArrangements 
                              WHERE DayID = @DIDFK 
                                AND PeriodID = @PIDFK 
                                AND ClassID = @ClassID 
                                AND SectionID = @SectionID 
                                AND OnDate = @OnDate 
                                AND AbscentTeacherID = @AbscentTeacherID";

                string insertQuery = @"INSERT INTO [dbo].[TimeTableArrangements]
                               ([TeacherID],[DayID],[PeriodID],[ClassID],[SectionID],[SubjectID],[OnDate],[InsertedOn],[InsertedBy],[UpdatedBy],[AbscentTeacherID])   
                               VALUES(@TeacherID,@DIDFK,@PIDFK,@ClassID,@SectionID,@SubjectID,@OnDate,@InsertedOn,@User,@User,@AbscentTeacherID)";

                string updateQuery = @"UPDATE TimeTableArrangements 
                               SET TeacherID = @TeacherID, UpdatedBy = @User  
                               WHERE DayID = @DIDFK 
                                 AND PeriodID = @PIDFK 
                                 AND ClassID = @ClassID 
                                 AND SectionID = @SectionID 
                                 AND SubjectID = @SubjectID 
                                 AND OnDate = @OnDate 
                                 AND AbscentTeacherID = @AbscentTeacherID";
                #endregion

                #region Execute Check Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
                int count = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    count = Convert.ToInt32(ds.Tables[0].Rows[0]["Cnt"]);
                }
                #endregion

                #region Execute Insert or Update
                if (count > 0)
                {
                    int chk = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                    if (chk > 0)
                    {
                        response.Message = "Updated Successfully.";
                    }
                    else
                    {
                        response.Status = 1;
                        response.Message = "Not Updated.";
                    }
                }
                else
                {
                    int chk = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);
                    if (chk > 0)
                    {
                        response.Message = "Added Successfully.";
                    }
                    else
                    {
                        response.Status = 1;
                        response.Message = "Not Added.";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "ArrangeTimetable", ex.ToString());
                #endregion
            }

            return response;
        }

        public async Task<ResponseModel> GetTimeTableArrangementsByDate(string onDate, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                DateTime OnDate = Convert.ToDateTime(onDate);
                SqlParameter[] param =
                {
            new SqlParameter("@OnDate", OnDate)
        };
                #endregion

                #region Define Query
                string query = @"SELECT TimeTableArrangements.*,
                                Employees.EmployeeName as TeacherName,
                                SubSubjects.SubSubjectName as SubjectName,
                                Classes.ClassName,
                                Sections.SectionName,
                                EE.EmployeeName as AbscentTeacherName
                         FROM TimeTableArrangements
                         INNER JOIN SubSubjects 
                                ON TimeTableArrangements.SubjectID = SubSubjects.SubSubjectID
                         INNER JOIN Sections 
                                ON TimeTableArrangements.SectionID = Sections.SectionID
                         INNER JOIN Classes 
                                ON TimeTableArrangements.ClassID = Classes.ClassId
                         INNER JOIN Employees 
                                ON TimeTableArrangements.TeacherID = Employees.EmployeeCode
                         INNER JOIN Employees EE 
                                ON TimeTableArrangements.AbscentTeacherID = EE.EmployeeCode
                         WHERE OnDate = @OnDate";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                List<TimeTableDto> timetableList = new List<TimeTableDto>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var ttable = new TimeTableDto
                        {
                            PeroidID = dr["PeriodID"].ToString(),
                            PIDFK = dr["PeriodID"].ToString(),
                            ClassID = dr["ClassId"].ToString(),
                            SecID = dr["SectionID"].ToString(),
                            SubjectID = dr["SubjectID"].ToString(),
                            DIDFK = dr["DayID"].ToString(),
                            TeacherID = dr["TeacherID"].ToString(),
                            AbscentTeacherID = dr["AbscentTeacherID"].ToString(),
                            TeacherName = dr["TeacherName"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            SectionName = dr["SectionName"].ToString(),
                            AbscentTeacherName = dr["AbscentTeacherName"].ToString(),
                            SubSubjectName = dr["SubjectName"].ToString(),
                            UserName = dr["InsertedBy"].ToString(),
                            OnDate = dr["OnDate"].ToString()
                        };
                        timetableList.Add(ttable);
                    }
                }
                #endregion

                #region Set Response
                if (timetableList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = timetableList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "GetTimeTableArrangementsByDate", ex.Message.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetTimeTableArrangementsOfAbsentTeacherToday(string input, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!"
            };
            #endregion

            try
            {

          
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Split Input
                var parts = input.Split(',');
                if (parts.Length != 2)
                    throw new ArgumentException("Input must be in the format 'yyyy-MM-dd,TeacherID'");

                DateTime onDate = DateTime.Parse(parts[0]);
                long teacherId = Convert.ToInt64(parts[1]);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param =
                {
    new SqlParameter("@OnDate", onDate), // corrected
    new SqlParameter("@TID", teacherId)
};
                #endregion

                #region Define SQL Query
                string query = @"
            SELECT 
                TA.*, 
                E.EmployeeName AS TeacherName,
                SS.SubSubjectName AS SubjectName,
                C.ClassName,
                S.SectionName,
                EE.EmployeeName AS AbscentTeacherName
            FROM TimeTableArrangements TA
            INNER JOIN SubSubjects SS ON TA.SubjectID = SS.SubSubjectID
            INNER JOIN Sections S ON TA.SectionID = S.SectionID
            INNER JOIN Classes C ON TA.ClassID = C.ClassId
            INNER JOIN Employees E ON TA.TeacherID = E.EmployeeCode
            INNER JOIN Employees EE ON TA.AbscentTeacherID = EE.EmployeeCode
            WHERE TA.AbscentTeacherID = @TID AND TA.OnDate = @OnDate";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Process Result
                List<TimeTableDto> timeTables = new List<TimeTableDto>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var ttable = new TimeTableDto
                        {
                            PeroidID = dr["PeriodID"]?.ToString(),
                            PIDFK = dr["PeriodID"]?.ToString(),
                            ClassID = dr["ClassId"]?.ToString(),
                            SecID = dr["SectionID"]?.ToString(),
                            SubjectID = dr["SubjectID"]?.ToString(),
                            DIDFK = dr["DayID"]?.ToString(),
                            TeacherID = dr["TeacherID"]?.ToString(),
                            AbscentTeacherID = dr["AbscentTeacherID"]?.ToString(),
                            TeacherName = dr["TeacherName"]?.ToString(),
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionName = dr["SectionName"]?.ToString(),
                            AbscentTeacherName = dr["AbscentTeacherName"]?.ToString(),
                            SubSubjectName = dr["SubjectName"]?.ToString(),
                            UserName = dr["InsertedBy"]?.ToString(),
                            OnDate = dr["OnDate"]?.ToString()
                        };
                        timeTables.Add(ttable);
                    }
                }

                if (timeTables.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = timeTables;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "GetTimeTableArrangementsOfAbsentTeacherTodayAsync", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> ResetTimeTable(string updatedBy, string session, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Time Table not reset."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@updatedby", updatedBy),
            new SqlParameter("@session", session)
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "ResetAllTimeTablesNew", parameters);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var result = ds.Tables[0].Rows[0][0].ToString();
                    if (result == "0")
                        response.Message = "TimeTables not reset!!";
                    else
                        response.Message = "TimeTables reset successfully.";
                }
                else
                {
                    response.Status = 1;
                    response.Message = "No data returned from stored procedure.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "ResetTimeTable", ex.ToString());
                #endregion
            }

            return response;
        }



        public async Task<ResponseModel> GetEmployeeList(string year, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(year))
                {
                    response.Message = "Year is required.";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@year", year)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT 
                employees.EmployeeCode AS EID,
                employeename,
                employees.Gender,
                Designations.Designation,
                Designations.DesignationID
            FROM employees
            INNER JOIN EmployeeDetail ON employees.EmployeeID = EmployeeDetail.EmployeeID
            INNER JOIN Designations ON Designations.DesignationID = EmployeeDetail.DesignationID
            WHERE Withdrawn = 'False' 
              AND IsTeacher = 1 
              AND Year = @year
            ORDER BY Designation";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                List<TimeTable> employeeList = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var ttable = new TimeTable
                        {
                            EmployeeID = dr["EID"].ToString(),
                            employeename = dr["employeename"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            DesignationID = dr["DesignationID"].ToString()
                        };
                        employeeList.Add(ttable);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = employeeList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeeList", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetEmployeeList(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                string query = @"
            SELECT 
                employees.EmployeeCode AS EID,
                employees.EmployeeCode AS TeacherID,
                employeename,
                employees.Gender,
                Designations.Designation,
                Designations.DesignationID
            FROM employees
            INNER JOIN EmployeeDetail ON employees.EmployeeID = EmployeeDetail.EmployeeID
            INNER JOIN Designations ON Designations.DesignationID = EmployeeDetail.DesignationID
            WHERE Withdrawn = 'False' AND IsTeacher = 1
            ORDER BY Designation";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var employeeList = new List<TimeTable>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        employeeList.Add(new TimeTable
                        {
                            EmployeeID = dr["EID"].ToString(),
                            TeacherID = dr["TeacherID"].ToString(),
                            employeename = dr["employeename"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            DesignationID = dr["DesignationID"].ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = employeeList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching employee list";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeeList", ex.ToString());
                #endregion
            }

            return response;
        }

        public async Task<ResponseModel> GetEmployeeListNotInTimeTable(string year, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(year))
                {
                    response.Message = "Year is required.";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@year", year)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT 
                employees.EmployeeCode AS EID,
                employeename,
                employees.Gender,
                Designations.Designation,
                Designations.DesignationID
            FROM employees
            INNER JOIN EmployeeDetail ON employees.EmployeeID = EmployeeDetail.EmployeeID
            INNER JOIN Designations ON Designations.DesignationID = EmployeeDetail.DesignationID
            WHERE Withdrawn = 'False' 
              AND IsTeacher = 1
              AND Year = @year
              AND employees.EmployeeCode NOT IN (SELECT TeacherID FROM TTAssignPeroids)
            ORDER BY Designation";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                List<TimeTable> employeeList = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var ttable = new TimeTable
                        {
                            EmployeeID = dr["EID"].ToString(),
                            employeename = dr["employeename"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            DesignationID = dr["DesignationID"].ToString()
                        };
                        employeeList.Add(ttable);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = employeeList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeeListNotInTimeTable", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetEmployeeListWhoAreInTimeTable(string year, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(year))
                {
                    response.Message = "Year is required.";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@year", year)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT 
                employees.EmployeeCode AS EID,
                employeename,
                employees.Gender,
                Designations.Designation,
                Designations.DesignationID
            FROM employees
            INNER JOIN EmployeeDetail ON employees.EmployeeID = EmployeeDetail.EmployeeID
            INNER JOIN Designations ON Designations.DesignationID = EmployeeDetail.DesignationID
            WHERE Withdrawn = 'False' 
              AND IsTeacher = 1
              AND Year = @year
              AND employees.EmployeeCode IN (SELECT TeacherID FROM TTAssignPeroids)
            ORDER BY Designation";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

                List<TimeTable> employeeList = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var ttable = new TimeTable
                        {
                            EmployeeID = dr["EID"].ToString(),
                            employeename = dr["employeename"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            DesignationID = dr["DesignationID"].ToString()
                        };
                        employeeList.Add(ttable);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = employeeList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeeListWhoAreInTimeTable", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> DeleteTimetable(TimeTableDelete td, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Time Table not deleted."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@TID", td.TeacherID),
            new SqlParameter("@DayID", td.DayID),
            new SqlParameter("@UpdatedBy", td.UserName ?? string.Empty),
            new SqlParameter("@Session", td.Session ?? string.Empty)
        };
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.StoredProcedure,
                    "ResetTeacherTimeTableNew",
                    parameters
                );

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    string result = ds.Tables[0].Rows[0][0].ToString();

                    if (result == "1")
                    {
                        response.Message = "Records cleared successfully.";
                        response.Status = 1;
                    }
                    else
                    {
                        response.Message = "Records not updated.";
                        response.Status = 0;
                    }
                }
                else
                {
                    response.Message = "No records found.";
                    response.Status = 0;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "TimeTableArrangementsServices",
                    "DeleteTimetable",
                    ex.ToString()
                );
                #endregion
            }

            return response;
        }

        public async Task<ResponseModel> DeleteArrangementTimetable(TimeTable td, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Time Table not deleted."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@ClassID", td.ClassID),
            new SqlParameter("@SectionID", td.SecID),
            new SqlParameter("@SubjectID", td.SubSubjectID),
            new SqlParameter("@TeacherID", td.TeacherID),
            new SqlParameter("@PIDFK", td.PIDFK),
            new SqlParameter("@DIDFK", td.DIDFK),
            new SqlParameter("@OnDate", Convert.ToDateTime(td.OnDate)),
            new SqlParameter("@InsertedOn", DateTime.Now.Date),
            new SqlParameter("@User", td.UserName ?? string.Empty),
            new SqlParameter("@AbscentTeacherID", td.AbscentTeacherID)
        };
                #endregion

                #region Check if Record Exists
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM TimeTableArrangements 
            WHERE TeacherId = @TeacherID 
              AND DayID = @DIDFK 
              AND PeriodID = @PIDFK 
              AND ClassID = @ClassID 
              AND SectionID = @SectionID 
              AND OnDate = @OnDate 
              AND AbscentTeacherID = @AbscentTeacherID";

                var dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
                int count = 0;
                if (dsCheck != null && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    count = Convert.ToInt32(dsCheck.Tables[0].Rows[0]["RecordCount"]);
                }

                if (count == 0)
                {
                    response.Message = "No matching record found to delete.";
                    return response;
                }
                #endregion

                #region Perform Delete
                string deleteQuery = @"
            DELETE FROM TimeTableArrangements 
            WHERE DayID = @DIDFK 
              AND PeriodID = @PIDFK 
              AND ClassID = @ClassID 
              AND SectionID = @SectionID 
              AND SubjectID = @SubjectID 
              AND OnDate = @OnDate 
              AND AbscentTeacherID = @AbscentTeacherID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Record deleted successfully.";
                }
                else
                {
                    response.Message = "Delete failed. Please try again.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog(
                    "TimeTableArrangementsServices",
                    "DeleteArrangementTimetable",
                    ex.ToString()
                );
                #endregion
            }

            return response;
        }


    }
}
