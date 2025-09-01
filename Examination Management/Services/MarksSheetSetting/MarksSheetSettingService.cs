using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.MarksSheetSetting
{
    public class MarksSheetSettingService :IMarksSheetSettingService
    {
        private readonly IConfiguration _configuration;
        public MarksSheetSettingService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SaveMarksSheetSetting(MarksSheetSettingDto dto, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Marks sheet setting update successfully"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters Dynamically (exclude ID if identity)
                var props = typeof(MarksSheetSettingDto).GetProperties()
                                .Where(p => p.Name != "ID")
                                .ToArray();

                var parameters = props.Select(p =>
                    new SqlParameter("@" + p.Name, p.GetValue(dto) ?? DBNull.Value)).ToList();
                
                parameters.Add(new SqlParameter("@ID", dto.ID));
                #endregion

                #region Build UPDATE SQL Dynamically
                var setClause = string.Join(", ", props.Select(p => p.Name + " = @" + p.Name));

                string query = $@"
            UPDATE MarksSheetSetting
            SET {setClause}
            WHERE ID = @ID;
        ";
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters.ToArray());
                if (result <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = "Failed to update marks sheet setting.";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Exception occurred!";
                response.Error = ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("SaveMarksSheetSettingService", "SaveMarksSheetSetting", ex.ToString());
            }

            return response;
        }
    }
}
