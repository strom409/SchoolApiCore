using HR.Repository;
using HR.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.Subjects
{
    public class EmployeeSubjectsService : IEmployeeSubjectsService
    {
        private readonly IConfiguration _configuration;
        public EmployeeSubjectsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeSubjects(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No employee subjects found.",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Query
                string query = "SELECT * FROM EmployeeSubjects WHERE IsDeleted = 0 ORDER BY ESubjectName";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Process Data
                var subjects = new List<EmployeeSubjects>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new EmployeeSubjects
                        {
                            ESID = dr["ESID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ESID"]),
                            ESubjectName = dr["ESubjectName"]?.ToString() ?? string.Empty,
                            UserName = dr["UserName"]?.ToString() ?? string.Empty
                        });
                    }

                    response.Status = 1;
                    response.Message = "Employee subjects fetched successfully.";
                    response.ResponseData = subjects;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectsService", "GetEmployeeSubjects", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ESID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeSubjectById(string ESID, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No employee subject found.",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                string query = "SELECT * FROM EmployeeSubjects WHERE ESID = @ESID AND IsDeleted = 0";
                SqlParameter[] param = { new SqlParameter("@ESID", ESID) };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var subject = new EmployeeSubjects
                    {
                        ESID = dr["ESID"] == DBNull.Value ? 0 : Convert.ToInt64(dr["ESID"]),
                        ESubjectName = dr["ESubjectName"]?.ToString() ?? string.Empty,
                        UserName = dr["UserName"]?.ToString() ?? string.Empty
                    };

                    response.Status = 1;
                    response.Message = "Employee subject fetched successfully.";
                    response.ResponseData = subject;
                }
                #endregion
            }
            #region Exception Handling
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectService", "GetEmployeeSubjectById", ex.ToString());
            }
            #endregion

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddEmployeeSubject(EmployeeSubjects dto, string clientId)
        {
            #region Initialize Response
            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Unable to add employee subject."
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
            new SqlParameter("@ESubjectName", dto.ESubjectName ?? string.Empty),
            new SqlParameter("@UserName", dto.UserName ?? string.Empty)
        };
                #endregion

                #region Duplicate Check using ExecuteDatasetAsync
                string checkQuery = "SELECT COUNT(*) AS Total FROM EmployeeSubjects WHERE ESubjectName = @ESubjectName";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);

                int isDuplicate = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isDuplicate = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                }
                #endregion

                #region Insert If Not Duplicate
                if (isDuplicate == 0)
                {
                    string insertQuery = "INSERT INTO EmployeeSubjects (ESubjectName, UserName, IsDeleted) VALUES (@ESubjectName, @UserName, 0)";
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Employee subject added successfully.";
                        response.ResponseData = rowsAffected.ToString();
                    }
                    else
                    {
                        response.Message = "Failed to insert employee subject.";
                    }
                }
                else
                {
                    response.Status = 2;
                    response.Message = "Duplicate employee subject found.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectService", "AddEmployeeSubject", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateEmployeeSubject(EmployeeSubjects Q, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No employee subjects found.",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] param = {
            new SqlParameter("@ESID", Q.ESID),
            new SqlParameter("@ESubjectName", Q.ESubjectName ?? string.Empty),
            new SqlParameter("@UserName", Q.UserName ?? string.Empty)
        };
                #endregion

                #region Duplicate Check using ExecuteDatasetAsync
                string checkQuery = "SELECT COUNT(*) AS Total FROM EmployeeSubjects WHERE ESubjectName = @ESubjectName AND ESID != @ESID";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);

                int isDuplicate = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isDuplicate = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                }
                #endregion

                #region Execute Update or Set Duplicate Response
                if (isDuplicate == 0)
                {
                    string updateQuery = "UPDATE EmployeeSubjects SET ESubjectName = @ESubjectName, UserName = @UserName WHERE ESID = @ESID";
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, param);

                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Employee subject updated successfully.";
                        response.ResponseData = rowsAffected.ToString();
                    }
                    else
                    {
                        response.Message = "Failed to update employee subject.";
                    }
                }
                else
                {
                    response.Status = 2;
                    response.Message = "Duplicate employee subject found.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectService", "UpdateEmployeeSubjectAsync", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ESID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteEmployeeSubject(string ESID, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Unable to delete employee subject."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameter
                SqlParameter param = new SqlParameter("@ESID", ESID);
                #endregion

                #region Check Usage in EmployeeDetail (using ExecuteDatasetAsync)
                string checkQuery = "SELECT COUNT(*) AS Total FROM EmployeeDetail WHERE ESIDFK = @ESID";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);

                int isInUse = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isInUse = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
                }
                #endregion

                if (isInUse == 0)
                {
                    #region Delete EmployeeSubject
                    string deleteQuery = "DELETE FROM EmployeeSubjects WHERE ESID = @ESID";
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, param);

                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Employee subject deleted successfully.";
                        response.ResponseData = rowsAffected.ToString();
                    }
                    else
                    {
                        response.Message = "Failed to delete employee subject.";
                    }
                    #endregion
                }
                else
                {
                    response.Status = 2;
                    response.Message = "Employee subject cannot be deleted because it is in use.";
                }
            }
            catch (Exception ex)
            {
                #region Error Logging
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeSubjectService", "DeleteEmployeeSubjectAsync", ex.ToString());
                #endregion
            }

            return response;
        }

    }
}