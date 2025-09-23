using Microsoft.Data.SqlClient;
using Student.Repository;
using Student.Repository.SQL;
using Student.Services.ClassMaster;
using Student.Services.Students;
using System.Data;

namespace Student.Services.ClassMaster
{
    public class ClassMasterService : IClassMasterService
    {
        private readonly IConfiguration _configuration;

        public ClassMasterService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEducationalDepartments(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Records Found!"
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Query Execution
                string query = @"
            SELECT 
                EduDepartmentID,
                DepartmentName
            FROM EducationalDepartments";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, null);
                #endregion

                #region Map Results
                var departments = new List<EducationalDepartmentDto>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        departments.Add(new EducationalDepartmentDto
                        {
                            EduDepartmentID = row["EduDepartmentID"] != DBNull.Value ? Convert.ToInt32(row["EduDepartmentID"]) : 0,
                            DepartmentName = row["DepartmentName"]?.ToString()
                        });
                    }

                    if (departments.Any())
                    {
                        response.Status = 1;
                        response.ResponseData = departments;
                        response.Message = "Data retrieved successfully.";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("EducationalDepartmentService", "GetEducationalDepartments", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while retrieving departments.";
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSectionsByClassId(int classId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Records Found!"
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT 
                s.SectionID,
                s.SectionName,
                s.ClassId,
                c.ClassName,
                s.SessionId,
                s.Current_Session,
                s.EmpCode,
                s.EmployeName
            FROM Sections s
            INNER JOIN Classes c ON s.ClassId = c.ClassID
            WHERE s.ClassId = @ClassId";

                var parameters = new[]
                {
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = classId }
        };
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                var sections = new List<SectionDto>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        sections.Add(new SectionDto
                        {
                            SectionID = row["SectionID"] != DBNull.Value ? Convert.ToInt32(row["SectionID"]) : 0,
                            SectionName = row["SectionName"]?.ToString(),
                            ClassId = row["ClassId"] != DBNull.Value ? Convert.ToInt32(row["ClassId"]) : 0,
                            ClassName = row["ClassName"]?.ToString(),
                            SessionId = row["SessionId"] != DBNull.Value ? Convert.ToInt32(row["SessionId"]) : 0,
                            Current_Session = row["Current_Session"]?.ToString(),
                            EmpCode = row["EmpCode"]?.ToString(),
                            EmployeName = row["EmployeName"]?.ToString()
                        });
                    }

                    if (sections.Any())
                    {
                        response.Status = 1;
                        response.Message = "Data retrieved successfully.";
                        response.ResponseData = sections;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("SectionService", "GetSectionsByClassId", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while retrieving section data.";
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetClassesBySessionWithDepartment(string session, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Records Found!"
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT 
                c.ClassId, 
                c.ClassName, 
                c.Current_Session, 
                c.SessionID, 
                c.SubDepartmentID, 
                ed.DepartmentName
            FROM Classes c
            INNER JOIN EducationalDepartments ed ON c.SubDepartmentID = ed.EduDepartmentID
            WHERE c.Current_Session = @Session";

                var parameters = new[]
                {
            new SqlParameter("@Session", SqlDbType.NVarChar) { Value = session }
        };
                #endregion

                #region Execute and Map Results
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                var classes = new List<ClassDto>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        classes.Add(new ClassDto
                        {
                            ClassId = row["ClassId"] != DBNull.Value ? Convert.ToInt32(row["ClassId"]) : 0,
                            ClassName = row["ClassName"]?.ToString(),
                            Current_Session = row["Current_Session"]?.ToString(),
                            SessionID = row["SessionID"] != DBNull.Value ? Convert.ToInt32(row["SessionID"]) : 0,
                            SubDepartmentID = row["SubDepartmentID"] != DBNull.Value ? Convert.ToInt32(row["SubDepartmentID"]) : 0,
                        //    ClassIncharg = row["ClassIncharg"]?.ToString(),
                            DepartmentName = row["DepartmentName"]?.ToString()
                        });
                    }

                    if (classes.Any())
                    {
                        response.Status = 1;
                        response.ResponseData = classes;
                        response.Message = "Data retrieved successfully.";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "GetClassesBySessionWithDepartment", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while retrieving classes.";
                response.Error = ex.Message;
                #endregion
            }

            #region Return Response
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classDto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddClass(ClassDto classDto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Class not added."
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check for Duplicate
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM Classes 
            WHERE ClassName = @ClassName AND Current_Session = @Current_Session";

                var checkParams = new[]
                {
            new SqlParameter("@ClassName", SqlDbType.NVarChar) { Value = classDto.ClassName },
            new SqlParameter("@Current_Session", SqlDbType.NVarChar) { Value = classDto.Current_Session }
        };

                DataSet checkDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                int count = Convert.ToInt32(checkDs.Tables[0].Rows[0][0]);

                if (count > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 409;
                    response.Message = "Class with the same name already exists for this session.";
                    return response;
                }
                #endregion

                #region Insert Class
                string insertQuery = @"
            INSERT INTO Classes (ClassName, Current_Session, SubDepartmentID)
            VALUES (@ClassName, @Current_Session, @SubDepartmentID)";

                var insertParams = new[]
                {
            new SqlParameter("@ClassName", SqlDbType.NVarChar) { Value = classDto.ClassName },
            new SqlParameter("@Current_Session", SqlDbType.NVarChar) { Value = classDto.Current_Session },
           // new SqlParameter("@SessionID", SqlDbType.Int) { Value = classDto.SessionID },
            new SqlParameter("@SubDepartmentID", SqlDbType.Int) { Value = classDto.SubDepartmentID },
     
        };

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);

                if (rows > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Class added successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "AddClass", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while adding the class.";
                response.Error = ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="section"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddSection(SectionDto section, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Section not added."
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Duplicate Check
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM Sections 
            WHERE SectionName = @SectionName AND ClassID = @ClassId";

                var checkParams = new[]
                {
            new SqlParameter("@SectionName", SqlDbType.NVarChar) { Value = section.SectionName },
            new SqlParameter("@ClassId", SqlDbType.NVarChar) { Value = section.ClassId }
        };

                DataSet checkDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                int count = Convert.ToInt32(checkDs.Tables[0].Rows[0][0]);

                if (count > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 409;
                    response.Message = "A section with the same name and class already exists.";
                    return response;
                }
                #endregion

                #region Insert Section
                string insertQuery = @"
            INSERT INTO Sections 
            (SectionName, ClassId, ClassName, SessionId, Current_Session, EmpCode, EmployeName)
            VALUES 
            (@SectionName, @ClassId, @ClassName, @SessionId, @Current_Session, @EmpCode, @EmployeName)";

                var insertParams = new[]
                {
            new SqlParameter("@SectionName", SqlDbType.NVarChar) { Value = section.SectionName },
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = section.ClassId },
            new SqlParameter("@ClassName", SqlDbType.NVarChar) { Value = section.ClassName },
            new SqlParameter("@SessionId", SqlDbType.Int) { Value = section.SessionId },
            new SqlParameter("@Current_Session", SqlDbType.NVarChar) { Value = section.Current_Session },
            new SqlParameter("@EmpCode", SqlDbType.NVarChar) { Value = section.EmpCode ?? string.Empty },
            new SqlParameter("@EmployeName", SqlDbType.NVarChar) { Value = section.EmployeName ?? string.Empty }
        };

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, insertParams);

                if (rows > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Section added successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                Repository.Error.ErrorBLL.CreateErrorLog("SectionService", "AddSection", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while adding the section.";
                response.Error = ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSession"></param>
        /// <param name="newSession"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpgradeClass(string currentSession, string newSession, string clientId)
        {
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);

                // Fetch Classes from Current Session
                SqlParameter[] fetchParams =
                {
            new SqlParameter("@currentsession", currentSession)
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    @"SELECT ClassName, SubDepartmentID, ClassIncharg 
              FROM Classes WHERE Current_Session = @currentsession",
                    fetchParams
                );

                if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    rp.Message = "No Classes found for upgrade.";
                    return rp;
                }

                int totalInserted = 0;

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    SqlParameter[] insertParams =
                    {
                new SqlParameter("@classname", row["ClassName"].ToString()),
                new SqlParameter("@currentsession", newSession),
                new SqlParameter("@subdeptid", Convert.ToInt32(row["SubDepartmentID"])),
                new SqlParameter("@classincharg", row["ClassIncharg"].ToString() ?? "")
            };

                    int chk = await SQLHelperCore.ExecuteNonQueryAsync(
                        connectionString,
                        CommandType.Text,
                        @"INSERT INTO Classes (ClassName, Current_Session, SessionID, SubDepartmentID, ClassIncharg) 
                  VALUES (@classname, @currentsession, 0, @subdeptid, @classincharg)",
                        insertParams
                    );

                    if (chk > 0)
                    {
                        totalInserted++;
                    }
                }

                rp.Status = 1;
                rp.Message = $"{totalInserted} Classes upgraded successfully.";
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "UpgradeClassAsync", ex.ToString());
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSession"></param>
        /// <param name="newSession"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpgradeClassSubjectsSectionsAsync(UpgradeClassDto upgradeDto, string clientId)
        {
            #region Initialize
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            Dictionary<int, int> classIdMapping = new();
            bool isAnyUpgradeDone = false;
            #endregion

            try
            {
                #region Upgrade Classes
                DataSet dsClasses = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT ClassID, ClassName, SubDepartmentID, ClassIncharg FROM Classes WHERE Current_Session = @currentsession",
                    new SqlParameter("@currentsession", upgradeDto.CurrentSession));

                foreach (DataRow row in dsClasses.Tables[0].Rows)
                {
                    string className = row["ClassName"].ToString()!;

                    // Check duplicate class
                    DataSet dsDupClass = await SQLHelperCore.ExecuteDatasetAsync(
                        connectionString,
                        CommandType.Text,
                        "SELECT ClassID FROM Classes WHERE ClassName = @classname AND Current_Session = @newsession",
                        new[]
                        {
                    new SqlParameter("@classname", className),
                    new SqlParameter("@newsession", upgradeDto.NewSession)
                        });

                    int newClassId;
                    if (dsDupClass.Tables[0].Rows.Count > 0)
                    {
                        // Already exists, map old to existing
                        newClassId = Convert.ToInt32(dsDupClass.Tables[0].Rows[0]["ClassID"]);
                    }
                    else
                    {
                        // Insert new class
                        DataSet dsNew = await SQLHelperCore.ExecuteDatasetAsync(
                            connectionString,
                            CommandType.Text,
                            @"INSERT INTO Classes (ClassName, Current_Session, SessionID, SubDepartmentID, ClassIncharg)
                      VALUES (@classname, @newsession, 0, @subdeptid, @classincharg);
                      SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewClassID;",
                            new[]
                            {
                        new SqlParameter("@classname", className),
                        new SqlParameter("@newsession", upgradeDto.NewSession),
                        new SqlParameter("@subdeptid", row["SubDepartmentID"]),
                        new SqlParameter("@classincharg", row["ClassIncharg"])
                            });
                        newClassId = Convert.ToInt32(dsNew.Tables[0].Rows[0]["NewClassID"]);
                        isAnyUpgradeDone = true;
                    }

                    classIdMapping.Add(Convert.ToInt32(row["ClassID"]), newClassId);
                }
                #endregion

                #region Upgrade Subjects
                foreach (var mapping in classIdMapping)
                {
                    DataSet dsSubjects = await SQLHelperCore.ExecuteDatasetAsync(
                        connectionString,
                        CommandType.Text,
                        "SELECT SubjectName FROM Subjects WHERE ClassID = @oldclassid AND Current_Session = @currentsession",
                        new[]
                        {
                    new SqlParameter("@oldclassid", mapping.Key),
                    new SqlParameter("@currentsession",upgradeDto. CurrentSession)
                        });

                    foreach (DataRow subjectRow in dsSubjects.Tables[0].Rows)
                    {
                        string subjectName = subjectRow["SubjectName"].ToString()!;

                        // Check duplicate subject
                        DataSet dsDupSubj = await SQLHelperCore.ExecuteDatasetAsync(
                            connectionString,
                            CommandType.Text,
                            "SELECT 1 FROM Subjects WHERE ClassID = @newclassid AND SubjectName = @subjectname AND Current_Session = @newsession",
                            new[]
                            {
                        new SqlParameter("@newclassid", mapping.Value),
                        new SqlParameter("@subjectname", subjectName),
                        new SqlParameter("@newsession",upgradeDto.NewSession)
                            });

                        if (dsDupSubj.Tables[0].Rows.Count == 0)
                        {
                            await SQLHelperCore.ExecuteNonQueryAsync(
                                connectionString,
                                CommandType.Text,
                                @"INSERT INTO Subjects (SubjectName, ClassID, SessionID, Current_Session, IsActive)
                          VALUES (@subjectname, @newclassid, 0, @newsession, 1)",
                                new[]
                                {
                            new SqlParameter("@subjectname", subjectName),
                            new SqlParameter("@newclassid", mapping.Value),
                            new SqlParameter("@newsession", upgradeDto.NewSession)
                                });
                            isAnyUpgradeDone = true;
                        }
                    }
                }
                #endregion

                #region Upgrade Sections
                foreach (var mapping in classIdMapping)
                {
                    DataSet dsSections = await SQLHelperCore.ExecuteDatasetAsync(
                        connectionString,
                        CommandType.Text,
                        "SELECT SectionName, EmpCode, EmployeName, ClassName FROM Sections WHERE ClassID = @oldclassid AND Current_Session = @currentsession",
                        new[]
                        {
            new SqlParameter("@oldclassid", mapping.Key),
            new SqlParameter("@currentsession",upgradeDto.CurrentSession)
                        });

                    foreach (DataRow sectionRow in dsSections.Tables[0].Rows)
                    {
                        string sectionName = sectionRow["SectionName"].ToString()!;
                        string className = sectionRow["ClassName"].ToString()!;

                        // Check duplicate section
                        DataSet dsDupSec = await SQLHelperCore.ExecuteDatasetAsync(
                            connectionString,
                            CommandType.Text,
                            "SELECT 1 FROM Sections WHERE ClassID = @newclassid AND SectionName = @sectionname AND Current_Session = @newsession",
                            new[]
                            {
                new SqlParameter("@newclassid", mapping.Value),
                new SqlParameter("@sectionname", sectionName),
                new SqlParameter("@newsession", upgradeDto.NewSession)
                            });

                        if (dsDupSec.Tables[0].Rows.Count == 0)
                        {
                            await SQLHelperCore.ExecuteNonQueryAsync(
                                connectionString,
                                CommandType.Text,
                                @"INSERT INTO Sections 
                  (SectionName, ClassID, SessionID, Current_Session, EmpCode, EmployeName, ClassName)
                  VALUES 
                  (@sectionname, @newclassid, 0, @newsession, @empcode, @employename, @classname)",
                                new[]
                                {
                    new SqlParameter("@sectionname", sectionName),
                    new SqlParameter("@newclassid", mapping.Value),
                    new SqlParameter("@newsession", upgradeDto.NewSession),
                    new SqlParameter("@empcode", sectionRow["EmpCode"]),
                    new SqlParameter("@employename", sectionRow["EmployeName"]),
                    new SqlParameter("@classname", className)
                                });
                            isAnyUpgradeDone = true;
                        }
                    }
                }


                #endregion


                #region Final Result Message
                if (isAnyUpgradeDone)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Upgrade completed successfully.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "All Classes, Subjects, and Sections are already upgraded. No action taken.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "UpgradeClassSubjectsSectionsAsync", ex.ToString());
                #endregion
            }

            #region Return
            return response;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classDto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateClass(ClassDto classDto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Class not updated."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                var parameters = new[]
                {
            new SqlParameter("@ClassName", SqlDbType.NVarChar) { Value = classDto.ClassName ?? (object)DBNull.Value },
            new SqlParameter("@Current_Session", SqlDbType.NVarChar) { Value = classDto.Current_Session ?? (object)DBNull.Value },
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = classDto.ClassId },
            new SqlParameter("@SubDepartmentID", SqlDbType.Int) { Value = classDto.SubDepartmentID ?? (object)DBNull.Value }
        };
                #endregion

                #region Duplicate Check (Only ClassName + Current_Session)
                string duplicateCheckQuery = @"
            SELECT 1
            FROM Classes
            WHERE ClassName = @ClassName
              AND Current_Session = @Current_Session
              AND ClassId != @ClassId";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateCheckQuery, parameters);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Another class with the same name  already exists.";
                    return response;
                }
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE Classes 
            SET 
                ClassName = @ClassName,
                Current_Session = @Current_Session,
                SubDepartmentID = @SubDepartmentID
            WHERE ClassId = @ClassId";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Set Response
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Class updated successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "UpdateClass", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while updating the class.";
                response.Error = ex.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionDto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSection(SectionDto sectionDto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Section not updated."
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                var parameters = new[]
                {
            new SqlParameter("@SectionName", SqlDbType.NVarChar) { Value = sectionDto.SectionName ?? (object)DBNull.Value },
            new SqlParameter("@Current_Session", SqlDbType.NVarChar) { Value = sectionDto.Current_Session ?? (object)DBNull.Value },
            new SqlParameter("@SectionID", SqlDbType.Int) { Value = sectionDto.SectionID },
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = sectionDto.ClassId },
            new SqlParameter("@ClassName", SqlDbType.NVarChar) { Value = sectionDto.ClassName ?? (object)DBNull.Value },
            new SqlParameter("@EmpCode", SqlDbType.NVarChar) { Value = sectionDto.EmpCode ?? (object)DBNull.Value },
            new SqlParameter("@EmployeName", SqlDbType.NVarChar) { Value = sectionDto.EmployeName ?? (object)DBNull.Value }
        };
                #endregion

                // add classid
                #region Duplicate Check (Only SectionName + Current_Session) 
                string duplicateCheckQuery = @"
            SELECT 1
            FROM Sections
            WHERE SectionName = @SectionName
              AND Current_Session = @Current_Session
              AND SectionID != @SectionID";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateCheckQuery, parameters);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "Another section with the same name  already exists.";
                    return response;
                }
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE Sections
            SET 
                SectionName = @SectionName,
                ClassId = @ClassId,
                ClassName = @ClassName,
                Current_Session = @Current_Session,
                EmpCode = @EmpCode,
                EmployeName = @EmployeName
            WHERE SectionID = @SectionID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Set Response
                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Section updated successfully.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                Repository.Error.ErrorBLL.CreateErrorLog("SectionService", "UpdateSection", ex.ToString());
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "An error occurred while updating the section.";
                response.Error = ex.Message;
                #endregion
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteClass(int classId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Failed to delete class."
            };

            try
            {
                #region Connection String
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                #region Check if class exists
                const string classExistsQuery = "SELECT COUNT(1) FROM Classes WHERE ClassId = @ClassId";
                var classExistsParams = new[]
                {
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = classId }
        };

                var classExistsDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, classExistsQuery, classExistsParams);
                int classExistsCount = Convert.ToInt32(classExistsDs?.Tables[0]?.Rows[0][0] ?? 0);

                if (classExistsCount == 0)
                {
                    response.Message = "Class not found.";
                    return response;
                }
                #endregion

                #region Check if students are assigned
                const string checkQuery = @"
            SELECT COUNT(1) AS StudentCount 
            FROM StudentInfo 
            WHERE ClassId = @ClassId 
            AND Current_Session = (
                SELECT TOP 1 Current_Session 
                FROM StudentInfo 
                WHERE ClassId = @ClassId
                ORDER BY Current_Session DESC
            )";

                var checkParams = new[]
                {
            new SqlParameter("@ClassId", SqlDbType.Int) { Value = classId }
        };

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                int studentCount = Convert.ToInt32(ds?.Tables[0]?.Rows[0]["StudentCount"] ?? 0);

                if (studentCount > 0)
                {
                    response.Message = "Class is currently in use and has assigned students.";
                    return response;
                }
                #endregion

                #region Delete class
                const string deleteQuery = "DELETE FROM Classes WHERE ClassId = @ClassId";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, checkParams);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Class deleted successfully.";
                }
                else
                {
                    response.Message = "Class deletion failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("ClassService", "DeleteClass", ex.ToString());
                response.Status = -1;
                response.Message = "An error occurred during class deletion.";
                response.Error = ex.Message;
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteSection(int sectionId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Failed to delete section."
            };

            try
            {
                #region Connection String
                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                #endregion

                #region Check if section exists
                const string sectionExistsQuery = "SELECT COUNT(1) FROM Sections WHERE SectionID = @SectionID";
                var sectionExistsParams = new[]
                {
            new SqlParameter("@SectionID", sectionId)
        };

                var sectionExistsDs = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    sectionExistsQuery,
                    sectionExistsParams);

                int sectionExistsCount = Convert.ToInt32(sectionExistsDs?.Tables[0]?.Rows[0][0] ?? 0);

                if (sectionExistsCount == 0)
                {
                    response.Message = "Section not found.";
                    return response;
                }
                #endregion

                #region Check if students are assigned
                const string checkQuery = @"
            SELECT COUNT(1) AS StudentCount 
            FROM StudentInfo 
            WHERE SectionID = @SectionID 
            AND Current_Session = (
                SELECT TOP 1 Current_Session 
                FROM StudentInfo 
                WHERE SectionID = @SectionID
                ORDER BY Current_Session DESC
            )";

                var checkParams = new[]
                {
            new SqlParameter("@SectionID", sectionId)
        };

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                int studentCount = Convert.ToInt32(ds?.Tables[0]?.Rows[0]["StudentCount"] ?? 0);

                if (studentCount > 0)
                {
                    response.Message = "Students are assigned to this section.";
                    return response;
                }
                #endregion

                #region Delete section
                const string deleteQuery = "DELETE FROM Sections WHERE SectionID = @SectionID";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString,
                    CommandType.Text,
                    deleteQuery,
                    new SqlParameter("@SectionID", sectionId));

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Section deleted successfully.";
                }
                else
                {
                    response.Message = "Section deletion failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("SectionService", "DeleteSection", ex.ToString());
                response.Status = -1;
                response.Message = "An error occurred while deleting section.";
                response.Error = ex.Message;
            }

            return response;
        }


    }
}
