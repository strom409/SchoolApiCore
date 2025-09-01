using ClassTest.Repository;
using ClassTest.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassTest.Services.Subject
{
    public class SubjectService : ISubjectService
    {
        private readonly IConfiguration _configuration;
        public SubjectService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subname"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> InsertNewSubject(SubjectDTO subname, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subname.SubjectName) || string.IsNullOrEmpty(subname.ClassID))
                {
                    rp.Message = "Subject Name or Class ID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate Subject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subname", subname.SubjectName.Trim()),
            new SqlParameter("@classid", Convert.ToInt64(subname.ClassID))
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(*) AS DuplicateCount FROM Subjects WHERE ClassID = @classid AND SubjectName = @subname",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["DuplicateCount"]);
                    if (count > 0)
                    {
                        rp.Message = $"Subject: {subname.SubjectName} already exists!!";
                        return rp;
                    }
                }
                #endregion

                #region Insert New Subject
                SqlParameter[] insertParams =
                {
            new SqlParameter("@subname", subname.SubjectName.Trim()),
            new SqlParameter("@classid", Convert.ToInt64(subname.ClassID)),
            new SqlParameter("@sessid", Convert.ToInt32(subname.SessionID ?? "0")),
            new SqlParameter("@currentsession", subname.Current_Session),
            new SqlParameter("@IsActive", "1"),

        };

                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"INSERT INTO Subjects (SubjectName, ClassID, SessionID, Current_Session,IsActive) 
              VALUES (@subname, @classid, @sessid, @currentsession,@IsActive)",
                    insertParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Added Successfully.";
                }
                else
                {
                    rp.Message = "Not Added.";
                    return rp;
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "InsertNewSubjectAsync", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subname"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> InsertNewOptionalSubject(SubjectDTO subname, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subname.OptionalSubjectName) || string.IsNullOrEmpty(subname.ClassID))
                {
                    rp.Message = "Subject Name or Class ID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate Optional Subject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subname", subname.OptionalSubjectName.Trim()),
            new SqlParameter("@classid", Convert.ToInt64(subname.ClassID))
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(*) AS DuplicateCount FROM OptionalSubjects WHERE ClassID = @classid AND OptionalSubjectName = @subname",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["DuplicateCount"]);
                    if (count > 0)
                    {
                        rp.Message = $"OptionalSubject: {subname.OptionalSubjectName} already exists!!";
                        return rp;
                    }
                }
                #endregion

                #region Insert New Optional Subject
                SqlParameter[] insertParams =
                {
            new SqlParameter("@subname", subname.OptionalSubjectName.Trim()),
            new SqlParameter("@classid", Convert.ToInt64(subname.ClassID)),
            new SqlParameter("@sessid", Convert.ToInt32(subname.SessionID ?? "0")),
            new SqlParameter("@currentsession", subname.Current_Session)
        };

                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"INSERT INTO OptionalSubjects (OptionalSubjectName, ClassID, SessionID, Current_Session, IsActive) 
              VALUES (@subname, @classid, @sessid, @currentsession, 1)",
                    insertParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Added Successfully.";
                }
                else
                {
                    rp.Message = "Not Added.";
                    return rp;
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "InsertNewOptionalSubject", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subname"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> InsertNewSubSubject(SubjectDTO subname, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subname.SubSubjectName) || string.IsNullOrEmpty(subname.SubjectID))
                {
                    rp.Message = "SubSubject Name or SubjectID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate SubSubject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subname", subname.SubSubjectName.Trim()),
            new SqlParameter("@subjectid", Convert.ToInt64(subname.SubjectID))
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(*) AS DuplicateCount FROM SubSubjects WHERE SubjectID = @subjectid AND SubSubjectName = @subname",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int count = Convert.ToInt32(ds.Tables[0].Rows[0]["DuplicateCount"]);
                    if (count > 0)
                    {
                        rp.Message = $"SubSubject: {subname.SubSubjectName} already exists!!";
                        return rp;
                    }
                }
                #endregion

                #region Insert New SubSubject (Hardcoding Removed = 0)
                SqlParameter[] insertParams =
                {
            new SqlParameter("@subname", subname.SubSubjectName.Trim()),
            new SqlParameter("@subjectid", Convert.ToInt64(subname.SubjectID)),
            new SqlParameter("@currentsession", subname.Current_Session)
        };

                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"INSERT INTO SubSubjects (SubSubjectName, SubjectID, Current_Session, Removed) 
              VALUES (@subname, @subjectid, @currentsession, 0)",
                    insertParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "SubSubject Added Successfully.";
                }
                else
                {
                    rp.Message = "Not Added.";
                    return rp;
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "InsertNewSubSubject", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpgradeSubjectAsync(SubjectDTO subject, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subject.ClassID) || string.IsNullOrEmpty(subject.Current_Session) || string.IsNullOrEmpty(subject.SessionID))
                {
                    rp.Message = "ClassID, CurrentSession or NextSession is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Fetch Subjects of Current Session
                SqlParameter[] fetchParams =
                {
            new SqlParameter("@classid", Convert.ToInt64(subject.ClassID)),
            new SqlParameter("@currentsession", subject.Current_Session)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT SubjectName FROM Subjects WHERE ClassID = @classid AND Current_Session = @currentsession AND IsActive = 1",
                    fetchParams
                );

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    rp.Message = "No Subjects found for upgrade.";
                    return rp;
                }
                #endregion

                #region Upgrade Subjects to Next Session
                int totalInserted = 0;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string subjectName = row["SubjectName"].ToString();

                    // Check Duplicate in Next Session
                    SqlParameter[] checkParams =
                    {
                new SqlParameter("@subname", subjectName.Trim()),
                new SqlParameter("@classid", Convert.ToInt64(subject.ClassID)),
                new SqlParameter("@nextsession", subject.SessionID)  // Next Session ID
            };

                    DataSet dsDup = await SQLHelperCore.ExecuteDatasetAsync(
                        connectionString,
                        CommandType.Text,
                        @"SELECT COUNT(*) AS DuplicateCount FROM Subjects 
                  WHERE ClassID = @classid AND SubjectName = @subname AND Current_Session = @nextsession",
                        checkParams
                    );

                    int count = Convert.ToInt32(dsDup.Tables[0].Rows[0]["DuplicateCount"]);
                    if (count == 0)
                    {
                        // Insert Subject into Next Session
                        SqlParameter[] insertParams =
                        {
                    new SqlParameter("@subname", subjectName.Trim()),
                    new SqlParameter("@classid", Convert.ToInt64(subject.ClassID)),
                   new SqlParameter("@sessid", "0"),
                    new SqlParameter("@currentsession", subject.SessionID),
                    new SqlParameter("@IsActive", "1")
                };

                        int inserted = await SQLHelperCore.ExecuteNonQueryAsync(
                            connectionString,
                            CommandType.Text,
                            @"INSERT INTO Subjects (SubjectName, ClassID, SessionID, Current_Session, IsActive) 
                      VALUES (@subname, @classid, @sessid, @currentsession, @IsActive)",
                            insertParams
                        );

                        if (inserted > 0)
                        {
                            totalInserted++;
                        }
                    }
                }
                #endregion

                rp.Status = 1;
                rp.Message = $"{totalInserted} Subjects upgraded successfully.";
            }
            catch (Exception ex)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "UpgradeSubjectAsync", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subs"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSubject(SubjectDTO subs, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subs.SubjectName) || string.IsNullOrEmpty(subs.ClassID))
                {
                    rp.Message = "Subject Name or Class ID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate Subject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subid", subs.SubjectID),
            new SqlParameter("@subjectname", subs.SubjectName.Trim()),
            new SqlParameter("@classid", subs.ClassID),
            new SqlParameter("@currentsession", subs.Current_Session)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT COUNT(SubjectID) AS val 
              FROM Subjects 
              WHERE SubjectName = @subjectname 
                AND Current_Session = @currentsession 
                AND SubjectID != @subid",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int valchk = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                    if (valchk > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Subject Name Already Added.";
                        return rp;
                    }
                }
                #endregion

                #region Update Subject
                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"UPDATE Subjects 
              SET SubjectName = @subjectname 
              WHERE ClassID = @classid 
                AND Current_Session = @currentsession 
                AND SubjectID = @subid",
                    checkParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Subject Updated Successfully.";
                }
                else
                {
                    rp.Message = "Not Updated.";
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "UpdateSubject", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subs"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateOptionalSubject(SubjectDTO subs, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subs.OptionalSubjectID) || string.IsNullOrEmpty(subs.ClassID))
                {
                    rp.Message = "Optional Subject ID or Class ID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate Optional Subject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subid", subs.OptionalSubjectID),
            new SqlParameter("@subname", subs.OptionalSubjectName.Trim()),
            new SqlParameter("@classid", subs.ClassID),
            new SqlParameter("@currentsession", subs.Current_Session)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT COUNT(Optionalsubjectid) AS val 
              FROM OptionalSubjects 
              WHERE OptionalSubjectName = @subname 
                AND Current_Session = @currentsession 
                AND Optionalsubjectid != @subid",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int valchk = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                    if (valchk > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Subject Name Already Added.";
                        return rp;
                    }
                }
                #endregion

                #region Update Optional Subject
                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"UPDATE OptionalSubjects 
              SET OptionalSubjectName = @subname 
              WHERE ClassID = @classid 
                AND Current_Session = @currentsession 
                AND Optionalsubjectid = @subid",
                    checkParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Optional Subject Updated Successfully.";
                }
                else
                {
                    rp.Message = "Not Updated.";
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "UpdateOptionalSubject", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subs"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSubSubject(SubjectDTO subs, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Inputs
                if (string.IsNullOrEmpty(subs.SubsubjectID) || string.IsNullOrEmpty(subs.SubSubjectName))
                {
                    rp.Message = "SubSubject ID or SubSubject Name is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Duplicate SubSubject
                SqlParameter[] checkParams =
                {
            new SqlParameter("@subid", subs.SubsubjectID),
            new SqlParameter("@subname", subs.SubSubjectName.Trim()),
            new SqlParameter("@SubjectID", subs.SubjectID),
            new SqlParameter("@currentsession", subs.Current_Session)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT COUNT(SubsubjectID) AS val 
              FROM SubSubjects 
              WHERE SubSubjectName = @subname 
                AND Current_Session = @currentsession 
                AND SubsubjectID != @subid",
                    checkParams
                );

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int valchk = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                    if (valchk > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Subject Name Already Added.";
                        return rp;
                    }
                }
                #endregion

                #region Update SubSubject
                int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    @"UPDATE SubSubjects 
              SET SubSubjectName = @subname 
              WHERE SubjectID = @SubjectID 
                AND Current_Session = @currentsession 
                AND SubsubjectID = @subid",
                    checkParams
                );

                if (chk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Sub Subject Updated Successfully.";
                }
                else
                {
                    rp.Message = "Not Updated.";
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "UpdateSubSubject", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSubjectsByClassId(string? param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Parameter
                if (string.IsNullOrEmpty(param))
                {
                    rp.Message = "ClassID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Fetch Data (with IsActive condition)
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT * FROM Subjects 
              WHERE ClassID = @Classid 
              AND (IsActive = 1 OR IsActive IS NULL) 
              ORDER BY SubjectID",
                    new SqlParameter("@Classid", Convert.ToInt64(param))
                );

                List<SubjectDTO> subjects = new List<SubjectDTO>();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new SubjectDTO
                        {
                            SubjectID = dr["SubjectID"].ToString(),
                            SubjectName = dr["SubjectName"].ToString(),
                            ClassID = dr["ClassID"].ToString(),
                            SessionID = dr["SessionID"].ToString(),
                            Current_Session = dr["Current_Session"].ToString()
                        });
                    }
                }
                #endregion

                #region Prepare Response
                if (subjects.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = subjects;
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "GetSubjectsByClassId", er.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetOptionalSubjectsByClassId(string? param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(param))
                {
                    rp.Message = "ClassID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Fetch Data with IsActive Condition
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT * FROM OptionalSubjects 
              WHERE ClassID = @Classid 
              AND (IsActive = 1 OR IsActive IS NULL) 
              ORDER BY OptionalSubjectID",
                    new SqlParameter("@Classid", Convert.ToInt64(param))
                );

                List<SubjectDTO> optionalSubjects = new List<SubjectDTO>();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        optionalSubjects.Add(new SubjectDTO
                        {
                            OptionalSubjectID = dr["OptionalSubjectID"].ToString(),
                            OptionalSubjectName = dr["OptionalSubjectName"].ToString(),
                            ClassID = dr["ClassID"].ToString(),
                            Current_Session = dr["Current_Session"].ToString()
                        });
                    }
                }
                #endregion

                #region Prepare Response
                if (optionalSubjects.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = optionalSubjects;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "GetOptionalSubjectsByClassId", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSubSubjectsBySubjectId(string? param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(param))
                {
                    rp.Message = "SubjectID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Fetch Data (with Removed check)
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT * FROM subsubjects 
              WHERE SubjectID = @SubjectID 
              AND (Removed IS NULL OR Removed = 0) 
              ORDER BY SubsubjectID",
                    new SqlParameter("@SubjectID", Convert.ToInt64(param))
                );

                List<SubjectDTO> subSubjects = new List<SubjectDTO>();
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subSubjects.Add(new SubjectDTO
                        {
                            SubsubjectID = dr["SubsubjectID"].ToString(),
                            SubSubjectName = dr["SubSubjectName"].ToString(),
                            SubjectID = dr["SubjectID"].ToString(),
                            Current_Session = dr["Current_Session"].ToString()
                        });
                    }
                }
                #endregion

                #region Prepare Response
                if (subSubjects.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = subSubjects;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "GetSubSubjectsBySubjectId", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteSubject(string subjectId, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(subjectId))
                {
                    rp.Message = "SubjectID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check If Already Deleted (IsActive)
                DataSet dsCheckActive = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT IsActive FROM Subjects WHERE SubjectID = @subid",
                    new SqlParameter("@subid", subjectId));

                if (dsCheckActive.Tables[0].Rows.Count == 0)
                {
                    rp.Message = "Subject not found!";
                    return rp;
                }

                bool isActive = Convert.ToBoolean(dsCheckActive.Tables[0].Rows[0]["IsActive"]);
                if (!isActive)
                {
                    rp.Message = "Subject is already deleted or inactive.";
                    return rp;
                }
                #endregion

                #region Check Dependencies & Delete
                // Check marks table
                DataSet dsMarks = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(marksid) AS Cnt FROM marks WHERE SubjectID = @subid",
                    new SqlParameter("@subid", subjectId));

                int chkMarks = Convert.ToInt32(dsMarks.Tables[0].Rows[0]["Cnt"]);
                if (chkMarks > 0)
                {
                    rp.Message = "This Subject can't be deleted because it's used in marks table.";
                    return rp;
                }

                // Check subsubjects table
                DataSet dsSubSubjects = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(SubSubjectID) AS Cnt FROM subsubjects WHERE SubjectID = @subid AND Removed = 0",
                    new SqlParameter("@subid", subjectId));

                int chkSubSubjects = Convert.ToInt32(dsSubSubjects.Tables[0].Rows[0]["Cnt"]);
                if (chkSubSubjects > 0)
                {
                    rp.Message = "This Subject can't be deleted because it's used in subsubjects table.";
                    return rp;
                }

                // Check teacherlog table
                DataSet dsTeacherLog = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(TeacherLogID) AS Cnt FROM teacherlog WHERE subjectid = @subid",
                    new SqlParameter("@subid", subjectId));

                int chkTeacherLog = Convert.ToInt32(dsTeacherLog.Tables[0].Rows[0]["Cnt"]);
                if (chkTeacherLog > 0)
                {
                    rp.Message = "This Subject can't be deleted because it's used in teacherlog table.";
                    return rp;
                }

                // Soft Delete: Set IsActive = 0 (instead of hard delete)
                int deleteChk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    "UPDATE Subjects SET IsActive = 0 WHERE SubjectID = @subid",
                    new SqlParameter("@subid", subjectId));

                if (deleteChk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Subject deleted successfully (marked inactive).";
                }
                else
                {
                    rp.Message = "Subject deletion failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "DeleteSubject", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionalSubjectId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteOptionalSubject(string optionalSubjectId, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(optionalSubjectId))
                {
                    rp.Message = "OptionalSubjectID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check If Already Deleted (IsActive)
                DataSet dsCheckActive = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT IsActive FROM OptionalSubjects WHERE OptionalSubjectID = @subid",
                    new SqlParameter("@subid", optionalSubjectId));

                if (dsCheckActive.Tables[0].Rows.Count == 0)
                {
                    rp.Message = "Optional Subject not found!";
                    return rp;
                }

                bool isActive = Convert.ToBoolean(dsCheckActive.Tables[0].Rows[0]["IsActive"]);
                if (!isActive)
                {
                    rp.Message = "Optional Subject is already deleted or inactive.";
                    return rp;
                }
                #endregion

                #region Check Dependencies (OptionalMarks Table)
                DataSet dsOptionalMarks = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(opmarksid) AS Cnt FROM optionalmarks WHERE SubjectID = @subid",
                    new SqlParameter("@subid", optionalSubjectId));

                int chkMarks = Convert.ToInt32(dsOptionalMarks.Tables[0].Rows[0]["Cnt"]);
                if (chkMarks > 0)
                {
                    rp.Message = "This Optional Subject can't be deleted because it's used in optionalmarks table.";
                    return rp;
                }
                #endregion

                #region Soft Delete Optional Subject
                int deleteChk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    "UPDATE OptionalSubjects SET IsActive = 0 WHERE OptionalSubjectID = @subid",
                    new SqlParameter("@subid", optionalSubjectId));

                if (deleteChk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Optional Subject deleted successfully (marked inactive).";
                }
                else
                {
                    rp.Message = "Optional Subject deletion failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "DeleteOptionalSubject", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subSubjectId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteSubSubject(string subSubjectId, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validation
                if (string.IsNullOrEmpty(subSubjectId))
                {
                    rp.Message = "SubsubjectID is null!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Already Deleted (Removed Column)
                DataSet dsCheckRemoved = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT Removed FROM subsubjects WHERE SubsubjectID = @subid",
                    new SqlParameter("@subid", subSubjectId));

                if (dsCheckRemoved.Tables[0].Rows.Count == 0)
                {
                    rp.Message = "SubSubject not found!";
                    return rp;
                }

                bool isRemoved = Convert.ToBoolean(dsCheckRemoved.Tables[0].Rows[0]["Removed"]);
                if (isRemoved)
                {
                    rp.Message = "SubSubject is already deleted or removed.";
                    return rp;
                }
                #endregion

                #region Check Dependencies (TeacherLog Table)
                DataSet dsTeacherLog = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(TeacherLogID) AS Cnt FROM teacherlog WHERE subsubjectid = @subid",
                    new SqlParameter("@subid", subSubjectId));

                int chkTeacherLog = Convert.ToInt32(dsTeacherLog.Tables[0].Rows[0]["Cnt"]);
                if (chkTeacherLog > 0)
                {
                    rp.Message = "This SubSubject can't be deleted because it's used in teacherlog table.";
                    return rp;
                }
                #endregion

                #region Check Dependencies (TimeTable Table)
                DataSet dsTT = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT COUNT(attid) AS Cnt FROM TTAssignPeroids WHERE subsubjectid = @subid",
                    new SqlParameter("@subid", subSubjectId));

                int chkTT = Convert.ToInt32(dsTT.Tables[0].Rows[0]["Cnt"]);
                if (chkTT > 0)
                {
                    rp.Message = "This SubSubject can't be deleted because it's used in Timetable table.";
                    return rp;
                }
                #endregion

                #region Soft Delete SubSubject (Set Removed = 1)
                int deleteChk = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    "UPDATE subsubjects SET Removed = 1 WHERE SubsubjectID = @subid",
                    new SqlParameter("@subid", subSubjectId));

                if (deleteChk > 0)
                {
                    rp.Status = 1;
                    rp.Message = "SubSubject deleted successfully (marked as removed).";
                }
                else
                {
                    rp.Message = "SubSubject deletion failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("SubjectService", "DeleteSubSubject", ex.ToString());
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }

    }
}
