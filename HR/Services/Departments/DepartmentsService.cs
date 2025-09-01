using HR.Repository;
using HR.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.Deparments
{
    public class DepartmentsService :IDepartmentsService
    {

        private readonly IConfiguration _configuration;

        public DepartmentsService(IConfiguration configuration)
        {
            _configuration= configuration;  
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="department"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddDepartment(SubDepartment department, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to add department",
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
                SqlParameter[] para = {
            new SqlParameter("@DName", department.SubDepartmentName)
        };

                string checkDuplicateQuery = "SELECT COUNT(*) AS val FROM SubDepartments WHERE SubDepartmentName = @DName";
                string insertQuery = "INSERT INTO SubDepartments (SubDepartmentName, Current_Session, SessionID, DepartmentID) VALUES (@DName, 'NA', 0, 0)";
                #endregion

                #region Check Duplicate using ExecuteDatasetAsync
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateQuery, para);
                int IsAvail = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    IsAvail = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }
                #endregion

                #region Insert if Not Duplicate
                if (IsAvail == 0)
                {
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, para);
                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Department added successfully.";
                        response.ResponseData = "1";
                    }
                    else
                    {
                        response.Message = "Failed to insert department.";
                        response.ResponseData = "0";
                    }
                }
                else
                {
                    response.Message = "Duplicate department name found.";
                    response.ResponseData = "0";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentService", "AddDepartmentAsync", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="department"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateDepartment(SubDepartment department, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to update department",
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
                SqlParameter[] para = {
            new SqlParameter("@DName", department.SubDepartmentName),
            new SqlParameter("@DID", department.SubDepartmentID)
        };

                string checkDuplicateQuery = "SELECT COUNT(*) AS val FROM SubDepartments WHERE SubDepartmentName = @DName AND SubDepartmentID != @DID";
                string updateQuery = "UPDATE SubDepartments SET SubDepartmentName = @DName WHERE SubDepartmentID = @DID";
                #endregion

                #region Check Duplicate Using ExecuteDatasetAsync
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkDuplicateQuery, para);
                int IsAvail = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    IsAvail = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }
                #endregion

                #region Update if Not Duplicate
                if (IsAvail == 0)
                {
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, para);

                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Department updated successfully.";
                        response.ResponseData = "1";
                    }
                    else
                    {
                        response.Message = "Failed to update department.";
                        response.ResponseData = "0";
                    }
                }
                else
                {
                    response.Message = "Duplicate department name found.";
                    response.ResponseData = "0";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentService", "UpdateDepartment", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getDepartments(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                string query = "SELECT * FROM SubDepartments ORDER BY SubDepartmentName";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Map DataSet to List
                var departments = new List<SubDepartment>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        departments.Add(new SubDepartment
                        {
                            SubDepartmentID = Convert.ToInt64(dr["SubDepartmentID"]),
                            SubDepartmentName = dr["SubDepartmentName"].ToString() ?? string.Empty,
                            DepartmentID = 0 // Static as per your old logic
                        });
                    }

                    response.Status = 1;
                    response.Message = "Departments fetched successfully.";
                    response.ResponseData = departments;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentService", "getDepartments", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDepartmentById(long id, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameter
                string query = "SELECT * FROM SubDepartments WHERE SubDepartmentID = @DID";
                SqlParameter parameter = new SqlParameter("@DID", id);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameter);
                #endregion

                #region Map Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var department = new SubDepartment
                    {
                        SubDepartmentID = Convert.ToInt64(dr["SubDepartmentID"]),
                        SubDepartmentName = dr["SubDepartmentName"].ToString() ?? string.Empty
                        // DepartmentID is intentionally NOT set as per your old code
                    };

                    response.Status = 1;
                    response.Message = "Department found.";
                    response.ResponseData = department;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentService", "GetDepartmentById", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteDepartment(long id, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to delete department",
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
                SqlParameter[] para = {
            new SqlParameter("@DID", id)
        };

                string checkUsageQuery = "SELECT COUNT(*) AS val FROM EmployeeDetail WHERE SubDepartmentID = @DID";
                string deleteQuery = "DELETE FROM SubDepartments WHERE SubDepartmentID = @DID";
                #endregion

                #region Check if Department is Used
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkUsageQuery, para);
                int isUsed = 0;

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isUsed = Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);
                }
                #endregion

                #region Delete if Not Used
                if (isUsed == 0)
                {
                    int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, para);

                    if (rowsAffected > 0)
                    {
                        response.Status = 1;
                        response.Message = "Department deleted successfully.";
                        response.ResponseData = "1";
                    }
                    else
                    {
                        response.Message = "Failed to delete department.";
                        response.ResponseData = "0";
                    }
                }
                else
                {
                    response.Message = "Department is in use and cannot be deleted.";
                    response.ResponseData = "0";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("DepartmentService", "DeleteDepartment", ex.ToString());
            }

            return response;
        }

    }

}
