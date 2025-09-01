using Microsoft.Data.SqlClient;
using System.Data;
using System.Xml;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TimeTableArrangements;
using Timetable_Arrangement.Services.TTPeroids;

namespace Timetable_Arrangement.Services.TTAssignPeroids
{
    public class TTAssignPeroidsService:ITTAssignPeroidsService
    {
        private readonly IConfiguration _configuration;
        private readonly ITTPeroidsNoService _ttPeroidsNoService;
        public TTAssignPeroidsService(IConfiguration configuration, ITTPeroidsNoService ttPeroidsNoService)
        {
            _configuration= configuration;
            _ttPeroidsNoService= ttPeroidsNoService;
        }

        public async Task<ResponseModel> AssignTimetable(TimeTable td, string clientId)
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
                SqlParameter[] para =
                {
            new SqlParameter("@ClassID", td.ClassID),
            new SqlParameter("@SecID", td.SecID),
            new SqlParameter("@SubSubjectID", td.SubSubjectID),
            new SqlParameter("@SubSubjectName", td.SubSubjectName),
            new SqlParameter("@TeacherID", td.TeacherID),
            new SqlParameter("@TeacherName", td.TeacherName),
            new SqlParameter("@DayName", td.DayName),
            new SqlParameter("@PIDFK", td.PIDFK),
            new SqlParameter("@DIDFK", td.DIDFK),
            new SqlParameter("@DayIds", td.DayIDS),
            new SqlParameter("@PeroidFKduration", td.PeroidFKduration),
            new SqlParameter("@IsFree", td.IsFree)
        };
                #endregion

                #region Check Duplicate
                string checkQuery = @"SELECT attid FROM TTAssignPeroids 
                              WHERE classid=@ClassID 
                                AND secid=@SecID 
                                AND subsubjectid=@SubSubjectID 
                                AND teacherid=@TeacherID 
                                AND pidfk=@PIDFK 
                                AND DIDfk=@DIDFK";

                var dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, para);

                if (dsCheck != null && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    response.Message = "Already Added.";
                    return response;
                }
                #endregion

                #region Insert Record
                string insertQuery = @"INSERT INTO TTAssignPeroids
                               (classid, secid, subsubjectid, teacherid, TeacherName, DayName, PIDFK, DIDFK, PeroidFKduration, SubsubjectName, IsFree, DayIds)
                               VALUES
                               (@ClassID, @SecID, @SubSubjectID, @TeacherID, @TeacherName, @DayName, @PIDFK, @DIDFK, @PeroidFKduration, @SubSubjectName, @IsFree, @DayIds)";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, para);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Added Successfully.";
                }
                else
                {
                    response.Message = "Not Added.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error adding timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "AssignTimetable", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetassignedTT(string teacherId, string clientId)
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
                if (string.IsNullOrEmpty(teacherId))
                {
                    response.Message = "Teacher ID is null!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@teacherID", teacherId)
        };
                #endregion

                #region Execute Query
                string query = "SELECT * FROM TTAssignPeroids WHERE TeacherID=@teacherID";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                List<TimeTable> listTimeTable = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var peroidResponse = await _ttPeroidsNoService.PeroidName(dr["PIDFK"]?.ToString(), clientId);
                        TimeTable ttable = new TimeTable
                        {
                            ATTID = dr["ATTID"].ToString(),
                            ClassID = dr["ClassID"].ToString(),
                            SecID = dr["SecID"].ToString(),
                            SubSubjectID = dr["SubSubjectID"].ToString(),
                            TeacherName = dr["TeacherName"].ToString(),
                            DayName = dr["DayName"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            PeroidFKduration = dr["PeroidFKduration"].ToString(),
                            SubSubjectName = dr["SubSubjectName"].ToString(),
                            DayIDS = dr["DayIDS"].ToString(),
                            peroid = peroidResponse.ResponseData?.ToString() ?? "NA",
                            IsFree = dr["IsFree"].ToString()
                        };

                        listTimeTable.Add(ttable);
                    }
                }
                #endregion

                #region Set Response
                if (listTimeTable.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = listTimeTable;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.Error = ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "GetassignedTT", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> Getwholetimetable(string clientId)
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

                #region Execute Query
                string query = @"
            SELECT 
                TTAssignPeroids.TeacherName,
                Classes.ClassName,
                Sections.SectionName,
                TTAssignPeroids.SubSubjectName,
                TTAssignPeroids.DayName,
                TTAssignPeroids.ClassID,
                TTAssignPeroids.SecID,
                TTAssignPeroids.SubSubjectID,
                TTAssignPeroids.PIDFK AS PID,
                TTAssignPeroids.TeacherID,
                TTAssignPeroids.DIDFK,
                TTAssignPeroids.DayIDS,
                TTAssignPeroids.IsFree,
                TTAssignPeroids.PIDFK
            FROM TTAssignPeroids
            INNER JOIN Classes ON TTAssignPeroids.ClassID = Classes.ClassID
            INNER JOIN Sections ON Sections.SectionID = TTAssignPeroids.SecID";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    List<TimeTable> Listtimetable = new List<TimeTable>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TimeTable ttable = new TimeTable
                        {
                            TeacherName = dr["TeacherName"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            SectionName = dr["SectionName"].ToString(),
                            SubSubjectName = dr["SubSubjectName"].ToString(),
                            DayName = dr["DayName"].ToString(),
                            ClassID = dr["ClassID"].ToString(),
                            TeacherID = dr["TeacherID"].ToString(),
                            SecID = dr["SecID"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            DayIDS = dr["DayIDS"].ToString(),
                            IsFree = dr["IsFree"].ToString(),
                            SubSubjectID = dr["SubSubjectID"].ToString(),
                            PID = dr["PID"].ToString(),
                            PIDFK = dr["PIDFK"].ToString()
                            // You can call additional methods here if needed, e.g., Alldata
                            // Alldata = TimeTableBLL.GetAlldata(dr["TeacherID"].ToString(), dr["PID"].ToString())
                        };

                        Listtimetable.Add(ttable);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = Listtimetable;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "Getwholetimetable", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetTeacherTimeTable(string teacherId, string clientId)
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
                if (string.IsNullOrEmpty(teacherId))
                {
                    response.Message = "Teacher ID is null!";
                    return response;
                }

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@teacherID", teacherId)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT TTAssignPeroids.*, 
                   Classes.ClassName, 
                   Sections.SectionName 
            FROM TTAssignPeroids
            INNER JOIN Classes ON Classes.ClassId = TTAssignPeroids.ClassID
            INNER JOIN Sections ON Sections.SectionID = TTAssignPeroids.SecID
            WHERE TeacherID = @teacherID";


                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                List<TTAssignPeroidsDto> timetableList = new List<TTAssignPeroidsDto>();
                #endregion

                #region Process Result

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var peroidResponse = await _ttPeroidsNoService.PeroidName(dr["PIDFK"]?.ToString(), clientId);
                        var ttable = new TTAssignPeroidsDto
                        {
                            ATTID = dr["ATTID"]?.ToString(),
                            ClassID = dr["ClassID"]?.ToString(),
                            SecID = dr["SecID"]?.ToString(),
                            SubSubjectID = dr["SubSubjectID"]?.ToString(),
                            TeacherName = dr["TeacherName"]?.ToString(),
                            DayName = dr["DayName"]?.ToString(),
                            PIDFK = dr["PIDFK"]?.ToString(),
                            DIDFK = dr["DIDFK"]?.ToString(),
                            PeroidFKduration = dr["PeroidFKduration"]?.ToString(),
                            SubSubjectName = dr["SubSubjectName"]?.ToString(),
                            DayIDS = dr["DayIDS"]?.ToString(),
                            Per = peroidResponse.ResponseData?.ToString() ?? "NA",
                            IsFree = dr["IsFree"]?.ToString(),
                            ClassName = dr["ClassName"]?.ToString(),
                            SectionName = dr["SectionName"]?.ToString()
                        };
                        timetableList.Add(ttable);
                    }

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
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "GetTeacherTimeTableAsync", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> GetTimeTableWithCurrentSession(string session, string clientId)
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
                SqlParameter[] mainParams =
         {
            new SqlParameter("@session", session)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT 
                E.EmployeeName AS TeacherName,
                C.ClassName,
                S.SectionName,
                SS.SubSubjectName,
                T.DayName,
                T.ClassID,
                T.SecID AS sectionID,
                T.SubSubjectID,
                T.PIDFK AS PID,
                T.TeacherID,
                T.DIDFK,
                T.DayIDS,
                T.IsFree,
                T.PIDFK,
                E.Gender
            FROM TTAssignPeroids T
            INNER JOIN Employees E ON E.EmployeeCode = T.TeacherID
            LEFT JOIN Classes C ON T.ClassID = C.ClassID
            LEFT JOIN Sections S ON S.SectionID = T.SecID
            LEFT JOIN SubSubjects SS ON SS.SubSubjectID = T.SubSubjectID AND SS.Current_Session = @session";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, mainParams);

                List<TimeTable> Listtimetable = new List<TimeTable>();

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TimeTable ttable = new TimeTable
                        {
                            TeacherName = dr["TeacherName"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            SectionName = dr["SectionName"].ToString(),
                            SubSubjectName = dr["SubSubjectName"].ToString(),
                            DayName = dr["DayName"].ToString(),
                            ClassID = dr["ClassID"].ToString(),
                            TeacherID = dr["TeacherID"].ToString(),
                            SecID = dr["sectionID"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            DayIDS = dr["DayIDS"].ToString(),
                            IsFree = dr["IsFree"].ToString(),
                            SubSubjectID = dr["SubSubjectID"].ToString(),
                            PID = dr["PID"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            Gender = dr["Gender"].ToString()
                            // Alldata = TimeTableBLL.GetAlldata(dr["TeacherID"].ToString(), dr["PID"].ToString())
                        };

                        Listtimetable.Add(ttable);
                    }

                    #region Assign Designations

                    // Prepare separate parameters for designation query
                    SqlParameter[] designationParams =
                    {
                new SqlParameter("@session", session)
            };
                    string designationQuery = @"
                WITH CTE(TeacherID, DesignationID, [Year], RowNumber) AS
                (
                    SELECT 
                        E.EmployeeCode AS TeacherID,
                        ED.DesignationID,
                        ED.[Year],
                        ROW_NUMBER() OVER(PARTITION BY E.EmployeeID ORDER BY ED.[Year] DESC) AS RowNumber
                    FROM EmployeeDetail ED
                    INNER JOIN Employees E ON E.EmployeeID = ED.EmployeeID
                    INNER JOIN TTAssignPeroids T ON E.EmployeeCode = T.TeacherID
                    LEFT JOIN SubSubjects SS ON SS.SubSubjectID = T.SubSubjectID AND SS.Current_Session = @session
                )
                SELECT 
                    CTE.TeacherID,
                    CTE.DesignationID,
                    D.Designation
                FROM CTE
                INNER JOIN Designations D ON D.DesignationID = CTE.DesignationID
                WHERE RowNumber = 1
                ORDER BY TeacherID";

                    var dsDesignations = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, designationQuery, designationParams);

                    if (dsDesignations != null && dsDesignations.Tables.Count > 0 && dsDesignations.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsDesignations.Tables[0].Rows)
                        {
                            string teacherId = dr["TeacherID"].ToString();
                            var teacherEntries = Listtimetable.FindAll(x => x.TeacherID == teacherId);
                            teacherEntries.ForEach(x =>
                            {
                                x.Designation = dr["Designation"].ToString();
                                x.DesignationID = dr["DesignationID"].ToString();
                            });
                        }
                    }
                    #endregion
                }
                #endregion

                if (Listtimetable.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = Listtimetable;
                }
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error fetching timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "GetTimeTableWithCurrentSession", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> SwapTimeTable(TimeTable ttval, string clientId)
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
                SqlParameter[] para =
                {
            new SqlParameter("@TeacherFrom", ttval.AbscentTeacherID),
            new SqlParameter("@TeacherTo", ttval.TeacherID),
            new SqlParameter("@User", ttval.UserName)
        };
                #endregion

                #region Update and Insert History
                string sqlQuery = @"
            UPDATE TTAssignPeroids 
            SET TeacherID = @TeacherTo 
            WHERE TeacherID = @TeacherFrom;

            INSERT INTO SwappingTTHistory (TeacherFrom, TeacherTo, [User]) 
            VALUES (@TeacherFrom, @TeacherTo, @User);";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, para);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Swap Completed Successfully.";
                }
                else
                {
                    response.Message = "Swap Not Completed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error swapping timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "SwapTimeTable", ex.ToString());
                #endregion
            }

            return response;
        }
        public async Task<ResponseModel> UpdateAssignedTimetable(TimeTable ttval, string clientId)
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
                SqlParameter[] para =
                {
            new SqlParameter("@ATTID", ttval.ATTID),
            new SqlParameter("@ClassID", string.IsNullOrEmpty(ttval.ClassID) ? "0" : ttval.ClassID),
            new SqlParameter("@SecID", string.IsNullOrEmpty(ttval.SecID) ? "0" : ttval.SecID),
            new SqlParameter("@SubSubjectID", string.IsNullOrEmpty(ttval.SubSubjectID) ? "0" : ttval.SubSubjectID),
            new SqlParameter("@PIDFK", ttval.PIDFK),
            new SqlParameter("@DIDFK", ttval.DIDFK),
            new SqlParameter("@PeroidFKduration", ttval.PeroidFKduration),
            new SqlParameter("@SubSubjectName", ttval.SubSubjectName),
            new SqlParameter("@IsFree", ttval.IsFree),
            new SqlParameter("@TeacherID", ttval.TeacherID),
            new SqlParameter("@TeacherName", ttval.TeacherName),
            new SqlParameter("@DayName", ttval.DayName)
        };
                #endregion

                #region Check Existing Record
                string sqlQueryFound = "SELECT COUNT(*) AS cnt FROM TTAssignPeroids WHERE DIDFK=@DIDFK AND PIDFK=@PIDFK AND TeacherID=@TeacherID";
                var dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlQueryFound, para);
                int count = 0;
                if (dsCheck != null && dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                    count = Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]);
                #endregion

                #region Update or Insert
                if (count > 0)
                {
                    string sqlQueryUpdate = @"
                UPDATE TTAssignPeroids
                SET ClassID=@ClassID, SecID=@SecID, SubSubjectID=@SubSubjectID, DayName=@DayName,
                    PIDFK=@PIDFK, DIDFK=@DIDFK, PeroidFKduration=@PeroidFKduration, 
                    SubsubjectName=@SubSubjectName, IsFree=@IsFree
                WHERE ATTID=@ATTID";

                    int rowsUpdated = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQueryUpdate, para);
                    if (rowsUpdated > 0)
                    {
                        response.Status = 1;
                        response.Message = "Updated Successfully.";
                    }
                    else
                    {
                        response.Message = "Not Updated.";
                    }
                }
                else
                {
                    string sqlQueryInsert = @"
                INSERT INTO TTAssignPeroids
                (ClassID, SecID, SubSubjectID, TeacherID, TeacherName, DayName, PIDFK, DIDFK, PeroidFKduration, SubsubjectName, IsFree, DayIds)
                VALUES
                (@ClassID, @SecID, @SubSubjectID, @TeacherID, @TeacherName, @DayName, @PIDFK, @DIDFK, @PeroidFKduration, @SubSubjectName, @IsFree, @DIDFK)";

                    int rowsInserted = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQueryInsert, para);
                    if (rowsInserted > 0)
                    {
                        response.Status = 1;
                        response.Message = "Inserted Successfully.";
                    }
                    else
                    {
                        response.Message = "Not Inserted.";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error updating assigned timetable.";
                response.Error = ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableService", "UpdateAssignedTimetable", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}
