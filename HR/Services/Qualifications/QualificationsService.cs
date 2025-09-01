using HR.Repository;
using HR.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.Qualifications
{
    public class QualificationsService:IQualificationsService
    {
        private readonly IConfiguration _configuration;
        public QualificationsService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualification"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddQualification(QualificationModel qualification, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not added!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL and Parameters
                SqlParameter[] param = {
            new SqlParameter("@Qualification", qualification.Qualification ?? string.Empty),
            new SqlParameter("@UserName", qualification.UserName ?? string.Empty)
        };

                string checkQuery = "SELECT COUNT(*) FROM Qualification WHERE Qualification = @Qualification";
                string insertQuery = "INSERT INTO Qualification (Qualification, UserName, IsDeleted) VALUES (@Qualification, @UserName, 'false')";
                #endregion

                #region Execute Check for Duplicate
                DataSet dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, param);
                int isAvailable = 0;

                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    isAvailable = Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]);
                }
                #endregion


                #region Insert If Not Exists
                if (isAvailable == 0)
                {
                    int affectedRows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);
                    if (affectedRows > 0)
                    {
                        response.Status = 1;
                        response.Message = "Qualification added successfully.";
                        response.ResponseData = affectedRows;
                    }
                }
                else
                {
                    response.Status = 2;
                    response.Message = "Qualification already exists.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;

                Repository.Error.ErrorBLL.CreateErrorLog("QualificationService", "AddQualification", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualification"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateQualification(QualificationModel qualification, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not updated.",
                ResponseData = null
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
            new SqlParameter("@QID", qualification.QID),
            new SqlParameter("@Qualification", qualification.Qualification ?? string.Empty),
            new SqlParameter("@UserName", qualification.UserName ?? string.Empty)
        };
                #endregion

                #region Check for Duplicate
                string sqlCheck = "SELECT COUNT(*) FROM Qualification WHERE Qualification = @Qualification AND QID != @QID";
                DataSet dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlCheck, param);

                int isAvailable = 0;
                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    isAvailable = Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]);
                }

                if (isAvailable > 0)
                {
                    response.Status = 0;
                    response.Message = "Duplicate qualification found.";
                    return response;
                }
                #endregion

                #region Update Qualification
                string sqlUpdate = "UPDATE Qualification SET Qualification = @Qualification, UserName = @UserName WHERE QID = @QID";
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlUpdate, param);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Qualification updated successfully.";
                    response.ResponseData = rowsAffected;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationService", "UpdateQualification", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetQualifications(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No qualifications found.",
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
                string query = "SELECT * FROM Qualification WHERE IsDeleted = 0 ORDER BY Qualification";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Process Result
                List<QualificationModel> qualifications = new List<QualificationModel>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        qualifications.Add(new QualificationModel
                        {
                            QID = Convert.ToInt64(dr["QID"]),
                            Qualification = dr["Qualification"].ToString(),
                            UserName = dr["UserName"].ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "Qualifications retrieved successfully.";
                    response.ResponseData = qualifications;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationService", "GetQualifications", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualificationId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetQualificationById(string qualificationId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No qualification found.",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Parameters
                string query = "SELECT * FROM Qualification WHERE QID = @QID AND IsDeleted = 0";

                SqlParameter[] param = {
            new SqlParameter("@QID", qualificationId)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Process Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    var qualification = new QualificationModel
                    {
                        QID = Convert.ToInt64(dr["QID"]),
                        Qualification = dr["Qualification"].ToString(),
                        UserName = dr["UserName"].ToString()
                    };

                    response.Status = 1;
                    response.Message = "Qualification retrieved successfully.";
                    response.ResponseData = qualification;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationService", "GetQualificationById", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qualificationId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteQualification(string qualificationId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Qualification not deleted.",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL and Parameters
                SqlParameter[] param = {
            new SqlParameter("@QID", qualificationId)
        };

                string checkUsageQuery = "SELECT COUNT(*) AS val FROM EmployeeDetail WHERE QidFk = @QID";
                string softDeleteQuery = "UPDATE Qualification SET IsDeleted = 1 WHERE QID = @QID";
                #endregion

                #region Execute Duplicate Check
                DataSet dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkUsageQuery, param);
                int isInUse = 0;

                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    isInUse = Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]);
                }
                #endregion

                if (isInUse == 0)
                {
                    #region Execute Soft Delete
                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, softDeleteQuery, param);

                    if (result > 0)
                    {
                        response.Status = 1;
                        response.Message = "Qualification deleted successfully (soft delete).";
                    }
                    #endregion
                }
                else
                {
                    response.Status = 0;
                    response.Message = "Cannot delete. Qualification is in use.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("QualificationService", "DeleteQualification", ex.ToString());
            }

            return response;
        }


    }
}
