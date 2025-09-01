using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.Units
{
    public class UnitsService : IUnitsService
    {
        private readonly IConfiguration _configuration;
        public UnitsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddUnit(UnitDto unit, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record added!"
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
            new SqlParameter("@UnitName", unit.UnitName ?? (object)DBNull.Value),
            new SqlParameter("@Name", unit.Name ?? (object)DBNull.Value)
        };
                #endregion

                #region Check Duplicate
                string dupQuery = "SELECT COUNT(1) FROM units WHERE UnitName = @UnitName";
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupQuery, param);

                int count = 0;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    count = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                }

                if (count > 0)
                {
                    response.Message = "Unit with same UnitName already exists!";
                    return response;
                }
                #endregion

                #region Insert Data
                string insertQuery = @"
            INSERT INTO units (UnitName, Name)
            VALUES (@UnitName, @Name)";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, param);

                if (rowsAffected > 0)
                {
                    response.Message = "Unit added successfully.";
                    response.Status = 1;
                }
                else
                {
                    response.Message = "Insert failed.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("UnitService", "AddUnit", ex.ToString());
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateUnit(UnitDto unit, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record updated!"
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
            new SqlParameter("@UnitId", unit.UnitId ?? (object)DBNull.Value),
            new SqlParameter("@UnitName", unit.UnitName ?? (object)DBNull.Value),
            new SqlParameter("@Name", unit.Name ?? (object)DBNull.Value)
        };
                #endregion
                #region Check Duplicate
                string duplicateQuery = @"
                   SELECT COUNT(1) AS DuplicateCount
                        FROM Units 
                 WHERE UnitName = @UnitName AND UnitId <> @UnitId";

                DataSet dsDuplicate = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateQuery, param);

                int duplicateCount = 0;
                if (dsDuplicate != null && dsDuplicate.Tables.Count > 0 && dsDuplicate.Tables[0].Rows.Count > 0)
                {
                    duplicateCount = Convert.ToInt32(dsDuplicate.Tables[0].Rows[0]["DuplicateCount"]);
                }

                if (duplicateCount > 0)
                {
                    response.Message = "Duplicate UnitName already exists.";
                    response.Status = 0;
                    return response;
                }
                #endregion
                #region Update Data
                string updateQuery = @"
            UPDATE units SET
                UnitName = @UnitName,
                Name = @Name
            WHERE UnitId = @UnitId";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, param);

                if (rowsAffected > 0)
                {
                    response.Message = "Unit updated successfully.";
                    response.Status = 1;
                }
                else
                {
                    response.Message = "No record found to update.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while updating unit.";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("UnitService", "UpdateUnit", ex.ToString());
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllUnits(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No records found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Query
                string query = "SELECT UnitId, UnitName, UnitType, Name, SortOrder FROM units ORDER BY SortOrder";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Process Result and Convert DataTable to List Inline
                if (ds != null && ds.Tables.Count > 0)
                {
                    var dt = ds.Tables[0];
                    var list = new List<Dictionary<string, object>>();

                    foreach (DataRow row in dt.Rows)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                        }
                        list.Add(dict);
                    }

                    response.ResponseData = list;
                    response.Message = "Records retrieved successfully.";
                    response.Status = 1;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("UnitService", "GetAllUnits", ex.Message + " | " + ex.StackTrace);
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetUnitById(string? unitId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No record found."
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameter
                SqlParameter[] param = {
            new SqlParameter("@UnitId", unitId ?? (object)DBNull.Value),
        };
                #endregion

                #region Execute Query
                string query = "SELECT UnitId, UnitName, UnitType, Name, SortOrder FROM units WHERE UnitId = @UnitId";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Process Result and Convert Inline
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var dt = ds.Tables[0];
                    var row = dt.Rows[0];
                    var dict = new Dictionary<string, object>();

                    foreach (DataColumn col in dt.Columns)
                    {
                        dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
                    }

                    response.ResponseData = dict;
                    response.Message = "Record retrieved successfully.";
                    response.Status = 1;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("UnitService", "GetUnitById", ex.Message + " | " + ex.StackTrace);
            }

            return response;
        }

    }
}
