using Microsoft.Data.SqlClient;
using System.Data;
using Timetable_Arrangement.Repository;
using Timetable_Arrangement.Repository.SQL;
using Timetable_Arrangement.Services.TimeTableArrangements;
using Timetable_Arrangement.Services.TTPeroids;

namespace Timetable_Arrangement.Services.TimeTableHistory
{
    public class TimeTableHistoryServices:ITimeTableHistoryServices
    {
        private readonly IConfiguration _configuration;
        private readonly ITTPeroidsNoService _ttPeroidsNoService;
        public TimeTableHistoryServices(IConfiguration configuration, ITTPeroidsNoService ttPeroidsNoService)
        {
            _configuration= configuration;
            _ttPeroidsNoService= ttPeroidsNoService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeTable"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddTimeTableTeacherHistory(TimeTableDto timeTable, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records added."
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters for Existing Timetable
                SqlParameter[] param =
                {
            new SqlParameter("@TeacherID", timeTable.TeacherID),
            new SqlParameter("@DayID", timeTable.DayIDS)
        };
                #endregion

                #region Get Existing Timetable
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT * FROM TTAssignPeroids WHERE TeacherID=@TeacherID AND DayIDS=@DayID", param);

                DataTable dt = ds.Tables[0];
                #endregion

                #region Get Max UID for History
                SqlParameter[] uidParam = { new SqlParameter("@TeacherID", timeTable.TeacherID) };
                DataSet uidDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT ISNULL(MAX(UID),0)+1 AS MaxUID FROM TimeTableHistory WHERE TeacherID=@TeacherID", uidParam);

                string maxTeacherUID = uidDs.Tables[0].Rows[0]["MaxUID"].ToString();
                #endregion

                #region Insert into History
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SqlParameter[] insertParams =
                        {
                    new SqlParameter("@UID", maxTeacherUID),
                    new SqlParameter("@ATTID", dr["ATTID"].ToString()),
                    new SqlParameter("@ClassiD", dr["ClassID"].ToString()),
                    new SqlParameter("@SecID", dr["SecID"].ToString()),
                    new SqlParameter("@SubSubjectID", dr["SubSubjectID"].ToString()),
                    new SqlParameter("@TeacherID", dr["TeacherID"].ToString()),
                    new SqlParameter("@TeacherName", dr["TeacherName"].ToString()),
                    new SqlParameter("@DayName", dr["DayName"].ToString()),
                    new SqlParameter("@PIDFK", dr["PIDFK"].ToString()),
                    new SqlParameter("@DIDFK", dr["DIDFK"].ToString()),
                    new SqlParameter("@PeroidFKduration", dr["PeroidFKduration"].ToString()),
                    new SqlParameter("@SubsubjectName", dr["SubSubjectName"].ToString()),
                    new SqlParameter("@DayIDS", dr["DayIDS"].ToString()),
                    new SqlParameter("@IsFree", dr["IsFree"].ToString()),
                    new SqlParameter("@UpdatedBy", timeTable.UserName)
                };

                        await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text,
                            @"INSERT INTO TimeTableHistory
                    (UID, ATTID, ClassiD, SecID, SubSubjectID, TeacherID, TeacherName, DayName, PIDFK, DIDFK, PeroidFKduration, SubsubjectName, DayIDS, IsFree, UpdatedBy) 
                    VALUES(@UID,@ATTID,@ClassiD,@SecID,@SubSubjectID,@TeacherID,@TeacherName,@DayName,@PIDFK,@DIDFK,@PeroidFKduration,@SubsubjectName,@DayIDS,@IsFree,@UpdatedBy)",
                            insertParams);
                    }

                    response.Status = 1;
                    response.Message = "TimeTable history added successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "AddTimeTableTeacherHistoryAsync", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="dayId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTimeTableTeacherHistory(string teacherId, string dayId, string clientId)
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
                if (string.IsNullOrEmpty(teacherId) || string.IsNullOrEmpty(dayId))
                {
                    response.Message = "Teacher ID or DayID is null!";
                    return response;
                }

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@teacherID", teacherId),
            new SqlParameter("@dayID", dayId)
        };
                #endregion

                #region Execute Query
                string query = @"
            SELECT TH.*, C.ClassName, S.SectionName
            FROM TimeTableHistory TH
            LEFT JOIN Classes C ON C.ClassId = TH.ClassiD
            LEFT JOIN Sections S ON S.SectionID = TH.SecID
            WHERE TH.TeacherID = @teacherID AND TH.DayIDS = @dayID";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                List<TimeTableHistoryDTO> timetableList = new List<TimeTableHistoryDTO>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var peroidResponse = await _ttPeroidsNoService.PeroidName(dr["PIDFK"]?.ToString(), clientId);
                        var ttable = new TimeTableHistoryDTO
                        {
                            ID = dr["ID"].ToString(),
                            UID = dr["UID"].ToString(),
                            ATTID = dr["ATTID"].ToString(),
                            ClassiD = dr["ClassiD"].ToString(),
                            SecID = dr["SecID"].ToString(),
                            SubSubjectID = dr["SubSubjectID"].ToString(),
                            TeacherID = dr["TeacherID"].ToString(),
                            TeacherName = dr["TeacherName"].ToString(),
                            DayName = dr["DayName"].ToString(),
                            PIDFK = dr["PIDFK"].ToString(),
                            DIDFK = dr["DIDFK"].ToString(),
                            PeroidFKduration = dr["PeroidFKduration"].ToString(),
                            SubsubjectName = dr["SubsubjectName"].ToString(),
                            DayIDS = dr["DayIDS"].ToString(),
                            IsFree = dr["IsFree"].ToString(),
                            Year = dr["Year"].ToString(),
                            MonthID = dr["MonthID"].ToString(),
                            PeroidName = peroidResponse.ResponseData?.ToString() ?? "NA",
                            OnDate = dr["OnDate"].ToString(),
                            UpdatedBy = dr["UpdatedBy"].ToString(),
                            SectionName = dr["SectionName"].ToString(),
                            ClassName = dr["ClassName"].ToString()
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
                Repository.Error.ErrorBLL.CreateErrorLog("TimeTableArrangementsServices", "GetTimeTableTeacherHistoryAsync", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}
