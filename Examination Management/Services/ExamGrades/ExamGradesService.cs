using Examination_Management.Repository;
using Examination_Management.Repository.SQL;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Examination_Management.Services.ExamGrades
{
    public class ExamGradesService: IExamGradesService
    {
        private  readonly IConfiguration _configuration;
        public ExamGradesService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetExamGrades(string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                string query = "SELECT * FROM ExamGrades ORDER BY GradeId";

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                var list = new List<ExamGradesDTO>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new ExamGradesDTO
                        {
                            GradeId = Convert.ToInt64(dr["GradeId"]).ToString(),
                            From = dr["From"] != DBNull.Value ? Convert.ToDecimal(dr["From"]) : 0m,
                            To = dr["To"] != DBNull.Value ? Convert.ToDecimal(dr["To"]) : 0m,
                            Grade = dr["Grade"].ToString(),
                            TeacherRemarks = dr["TeacherRemarks"]?.ToString(),
                            PrincipalRemarks = dr["PrincipalRemarks"]?.ToString(),
                            user = dr["user"]?.ToString(),
                            UpdatedOn = dr["UpdatedOn"] as DateTime?,
                            UpdatedBy = dr["UpdatedBy"]?.ToString(),
                            GradePoint = dr["GradePoint"] != DBNull.Value ? Convert.ToDecimal(dr["GradePoint"]) : null,
                            ranks = dr["ranks"] != DBNull.Value ? Convert.ToInt32(dr["ranks"]) : null
                        });
                    }

                    response.Status = 1;
                    response.Message = "Exam grades fetched successfully.";
                    response.ResponseData = list;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ExamGradesService", "GetExamGrades", ex.ToString());
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetExamGradeById(long id, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                string query = "SELECT * FROM ExamGrades WHERE GradeId = @GID";
                SqlParameter param = new SqlParameter("@GID", id);

                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    var dr = ds.Tables[0].Rows[0];
                    var grade = new ExamGradesDTO
                    {
                        GradeId = Convert.ToInt64(dr["GradeId"]).ToString(),
                        From = dr["From"] != DBNull.Value ? Convert.ToDecimal(dr["From"]) : 0m,
                        To = dr["To"] != DBNull.Value ? Convert.ToDecimal(dr["To"]) : 0m,
                        Grade = dr["Grade"].ToString(),
                        TeacherRemarks = dr["TeacherRemarks"].ToString(),
                        PrincipalRemarks = dr["PrincipalRemarks"].ToString(),
                        user = dr["user"].ToString(),
                        UpdatedOn = dr["UpdatedOn"] as DateTime?,
                        UpdatedBy = dr["UpdatedBy"].ToString(),
                        GradePoint = dr["GradePoint"] as decimal?,
                        ranks = dr["ranks"] as int?
                    };
                    response.Status = 1;
                    response.Message = "Exam grade found.";
                    response.ResponseData = grade;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ExamGradesService", "GetExamGradeById", ex.ToString());
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddExamGradeAsync(ExamGradesDTO grade, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Failed to add exam grade." };
            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);

                SqlParameter[] parameters = {
                    new SqlParameter("@From", grade.From),
                    new SqlParameter("@To", grade.To),
                    new SqlParameter("@Grade", grade.Grade),
                    new SqlParameter("@TeacherRemarks", grade.TeacherRemarks ?? ""),
                    new SqlParameter("@PrincipalRemarks", grade.PrincipalRemarks ?? ""),
                    new SqlParameter("@user", grade.user ?? ""),
                    new SqlParameter("@UpdatedBy", grade.UpdatedBy ?? ""),
                    new SqlParameter("@GradePoint", grade.GradePoint ?? 0),
                    new SqlParameter("@ranks", grade.ranks ?? 0)
                };

                string insertQuery = @"INSERT INTO ExamGrades ([From],[To],[Grade],[TeacherRemarks],[PrincipalRemarks],[user],[GradePoint],[UpdatedBy],[ranks]) 
                                       VALUES (@From,@To,@Grade,@TeacherRemarks,@PrincipalRemarks,@user,@GradePoint,@UpdatedBy,@ranks)";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);
                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Exam grade added successfully.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ExamGradesService", "AddExamGradeAsync", ex.ToString());
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateExamGradeAsync(ExamGradesDTO grade, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Failed to update exam grade." };
            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);

                SqlParameter[] parameters = {
                    new SqlParameter("@GID", grade.GradeId),
                    new SqlParameter("@From", grade.From),
                    new SqlParameter("@To", grade.To),
                    new SqlParameter("@Grade", grade.Grade),
                    new SqlParameter("@TeacherRemarks", grade.TeacherRemarks ?? ""),
                    new SqlParameter("@PrincipalRemarks", grade.PrincipalRemarks ?? ""),
                    new SqlParameter("@user", grade.user ?? ""),
                    new SqlParameter("@GradePoint", grade.GradePoint ?? 0),
                     new SqlParameter("@UpdatedBy", grade.UpdatedBy ?? ""),
                    new SqlParameter("@ranks", grade.ranks ?? 0)
                };

                string updateQuery = @"UPDATE ExamGrades SET 
                                        [From]=@From, [To]=@To, [Grade]=@Grade, 
                                        [TeacherRemarks]=@TeacherRemarks, [PrincipalRemarks]=@PrincipalRemarks,
                                        [user]=@user, [GradePoint]=@GradePoint,[UpdatedBy]=@UpdatedBy,[ranks]=@ranks
                                        WHERE GradeId=@GID";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Exam grade updated successfully.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ExamGradesService", "UpdateExamGradeAsync", ex.ToString());
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteExamGradeAsync(long id, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Failed to delete exam grade." };
            try
            {
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                SqlParameter param = new SqlParameter("@GID", id);

                string deleteQuery = "DELETE FROM ExamGrades WHERE GradeId=@GID";
                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, deleteQuery, param);

                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Exam grade deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("ExamGradesService", "DeleteExamGradeAsync", ex.ToString());
            }
            return response;
        }
    }
}

