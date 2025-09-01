using HR.Repository;
using HR.Services.Salary;
using Microsoft.Data.SqlClient;

namespace HR.Services.GrossPay
{
    public class GrossPayService : IGrossPayService
    {
        private readonly IConfiguration _configuration;
        public GrossPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grossPayInput"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> CalculateSalaryDetailsOnGrossPay(string grossPayInput, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Salary not updated!"
            };
            #endregion

            try
            {
                #region Validate & Parse Input
                string[] v = grossPayInput.Split(',');

                if (v.Length != 3)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid Parameters";
                    return response;
                }

                GrossPaySalary gp = new GrossPaySalary();

                gp.GrossPay = Convert.ToDecimal(v[0]);
                gp.OtherAllow = Convert.ToDecimal(v[1]);
                gp.EPFStatus = (v[2] == "1") ? "1" : "0";
                #endregion

                #region Business Logic (Same as your original code)
                gp.GP63 = (gp.GrossPay / 100) * 63; // 63%
                gp.GP37 = (gp.GrossPay / 100) * 37; // 37%

                gp.BP = Math.Round((gp.GP63 / 100) * 60);
                gp.DA = Math.Round((gp.GP63 / 100) * 40);
                gp.Pay = Math.Round(gp.BP + gp.DA);

                gp.HRA = Math.Round((gp.GP37 / 100) * 37);
                gp.Medical = Math.Round((gp.GP37 / 100) * 15);

                if (gp.EPFStatus == "1")
                {
                    gp.EPF = Math.Round((gp.Pay / 100) * 12);
                    gp.LI = Math.Round((decimal)((Convert.ToDouble(gp.Pay / 100) * 0.5)));
                }
                else
                {
                    gp.EPF = 0;
                    gp.LI = 0;
                }

                gp.ELeave = Math.Round(((gp.Pay + gp.HRA + gp.Medical + gp.OtherAllow) / 30) * 2);
                gp.GrossPay = Math.Round(gp.Pay + gp.HRA + gp.Medical + gp.OtherAllow + gp.EPF + gp.LI + gp.ELeave);
                gp.EPFDeduction = Math.Round((gp.EPF * 1) + gp.LI);
                gp.NetPay = gp.GrossPay - ((gp.EPF) + gp.LI);
                #endregion

                #region Return Success
                response.Status = 1;
                response.Message = "Salary calculated successfully.";
                response.ResponseData = gp;
                return response;
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "CalculateSalaryDetailsOnGrossPay", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSalaryDetailsOnGrossPay(string year, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
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

                #region Prepare Data List
                List<GrossPaySalary> GPS = new List<GrossPaySalary>();
                #endregion

                #region SQL Query
                string sql = @"
            SELECT EDID, EmployeeDetail.EmployeeID, BasicPay, DarenessAllownce, HouseRentAllownce, MedicalAllownce, 
                   AdditionslAllownce, EmployeeCPShare, Year, GrossPay, EarnedLeave, NetPay, Pay, EmployeeCode, 
                   EmployeeName, SubDepartmentID, DesignationID, Insurance1PercentAmt, CPFundStatus
            FROM EmployeeDetail
            INNER JOIN Employees ON EmployeeDetail.EmployeeID = Employees.EmployeeID
            WHERE Year = @Year AND Withdrawn = 'false'
            ORDER BY EmployeeCode";
                #endregion

                #region Execute SQL and Read Data
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Year", year);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                decimal employeeCPShare = reader["EmployeeCPShare"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["EmployeeCPShare"]);
                                decimal insuranceAmt = reader["Insurance1PercentAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Insurance1PercentAmt"]);

                                decimal epfDeduction = employeeCPShare + insuranceAmt;
                                string epfStatus = (reader["CPFundStatus"] != DBNull.Value && Convert.ToBoolean(reader["CPFundStatus"])) ? "1" : "0";

                                GPS.Add(new GrossPaySalary
                                {
                                    EDID = reader["EDID"].ToString(),
                                    EmployeeID = reader["EmployeeID"].ToString(),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    BP = reader["BasicPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BasicPay"]),
                                    DA = reader["DarenessAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DarenessAllownce"]),
                                    Pay = reader["Pay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Pay"]),
                                    HRA = reader["HouseRentAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["HouseRentAllownce"]),
                                    Medical = reader["MedicalAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MedicalAllownce"]),
                                    OtherAllow = reader["AdditionslAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AdditionslAllownce"]),
                                    ELeave = reader["EarnedLeave"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["EarnedLeave"]),
                                    EPF = employeeCPShare,
                                    LI = insuranceAmt,
                                    NetPay = reader["NetPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NetPay"]),
                                    GrossPay = reader["GrossPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["GrossPay"]),
                                    EPFStatus = epfStatus,
                                    EPFDeduction = epfDeduction,
                                    DepartmentID = reader["SubDepartmentID"].ToString(),
                                    Year = reader["Year"].ToString(),
                                });
                            }
                        }
                    }
                }
                #endregion

                #region Prepare Response
                if (GPS.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = GPS;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GrossService", "GetSalaryDetailsOnGrossPay", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="edid"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSalaryDetailsOnGrossPayEDID(string edid, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
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

                #region Prepare Data List
                List<GrossPaySalary> GPS = new List<GrossPaySalary>();
                #endregion

                #region SQL Query
                string sql = @"
            SELECT EDID, EmployeeDetail.EmployeeID, BasicPay, DarenessAllownce, HouseRentAllownce, MedicalAllownce, 
                   AdditionslAllownce, EmployeeCPShare, Year, GrossPay, EarnedLeave, NetPay, Pay, EmployeeCode, 
                   EmployeeName, SubDepartmentID, DesignationID, Insurance1PercentAmt, CPFundStatus
            FROM EmployeeDetail
            INNER JOIN Employees ON EmployeeDetail.EmployeeID = Employees.EmployeeID
            WHERE EDID = @EDID AND Withdrawn = 'false'
            ORDER BY EmployeeCode";
                #endregion

                #region Execute SQL and Read Data
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@EDID", edid);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                string epfStatus = (reader["CPFundStatus"] != DBNull.Value && Convert.ToBoolean(reader["CPFundStatus"])) ? "1" : "0";

                                GPS.Add(new GrossPaySalary
                                {
                                    EDID = reader["EDID"].ToString(),
                                    EmployeeID = reader["EmployeeID"].ToString(),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    BP = reader["BasicPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BasicPay"]),
                                    DA = reader["DarenessAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DarenessAllownce"]),
                                    Pay = reader["Pay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Pay"]),
                                    HRA = reader["HouseRentAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["HouseRentAllownce"]),
                                    Medical = reader["MedicalAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MedicalAllownce"]),
                                    OtherAllow = reader["AdditionslAllownce"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AdditionslAllownce"]),
                                    ELeave = reader["EarnedLeave"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["EarnedLeave"]),
                                    EPF = reader["EmployeeCPShare"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["EmployeeCPShare"]),
                                    LI = reader["Insurance1PercentAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Insurance1PercentAmt"]),
                                    NetPay = reader["NetPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NetPay"]),
                                    GrossPay = reader["GrossPay"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["GrossPay"]),
                                    EPFStatus = epfStatus,
                                    DepartmentID = reader["SubDepartmentID"].ToString(),
                                    Year = reader["Year"].ToString(),
                                });
                            }
                        }
                    }
                }
                #endregion

                #region Prepare Response
                if (GPS.Count > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = GPS;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GrossService", "GetSalaryDetailsOnGrossPayEDID", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salaries"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public Task<ResponseModel> UpdateSalaryDetailsOnGrossPay(List<GrossPaySalary> salaries)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salaries"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSalaryDetailsOnGrossPay(List<GrossPaySalary> salaries, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = false,
                Status = 0,
                Message = "Salary not updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                int updateCount = 0;

                #region Update Each Salary Record
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                UPDATE EmployeeDetail
                SET BasicPay = @BasicPay,
                    MedicalAllownce = @MedicalAllownce,
                    AdditionslAllownce = @AdditionslAllownce,
                    HouseRentAllownce = @HouseRentAllownce,
                    DarenessAllownce = @DarenessAllownce,
                    UpdatedBy = @UpdatedBy,
                    UpdatedOn = @UpdatedOn,
                    GrossPay = @GrossPay,
                    EarnedLeave = @EarnedLeave,
                    NetPay = @NetPay,
                    Pay = @Pay,
                    Insurance1PercentAmt = @LI,
                    EmployeeCPShare = @EPFEmployeeShare,
                    CPFundStatus = @CPFundStatus
                WHERE EDID = @EDID";

                    foreach (var gp in salaries)
                    {
                        bool CPFundStatus = gp.EPFStatus == "1";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@EDID", gp.EDID);
                            cmd.Parameters.AddWithValue("@BasicPay", validationBLL.IsDecimalNotNull(gp.BP.ToString(), 0));
                            cmd.Parameters.AddWithValue("@MedicalAllownce", gp.Medical);
                            cmd.Parameters.AddWithValue("@AdditionslAllownce", gp.OtherAllow);
                            cmd.Parameters.AddWithValue("@HouseRentAllownce", gp.HRA);
                            cmd.Parameters.AddWithValue("@DarenessAllownce", validationBLL.IsDecimalNotNull(gp.DA.ToString(), 0));
                            cmd.Parameters.AddWithValue("@UpdatedBy", gp.UpdatedBy ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@UpdatedOn", gp.UpdatedOn ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@GrossPay", gp.GrossPay);
                            cmd.Parameters.AddWithValue("@EarnedLeave", gp.ELeave);
                            cmd.Parameters.AddWithValue("@NetPay", gp.NetPay);
                            cmd.Parameters.AddWithValue("@Pay", gp.Pay);
                            cmd.Parameters.AddWithValue("@LI", gp.LI);
                            cmd.Parameters.AddWithValue("@EPFEmployeeShare", gp.EPF);
                            cmd.Parameters.AddWithValue("@CPFundStatus", CPFundStatus);

                            int affectedRows = await cmd.ExecuteNonQueryAsync();
                            if (affectedRows > 0)
                            {
                                updateCount++;
                            }
                        }
                    }
                }
                #endregion

                #region Set Response
                if (updateCount > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = $"({updateCount}) Records Updated";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("GrossService", "UpdateSalaryDetailsOnGrossPay", ex.ToString());
                return response;
                #endregion
            }
        }

    }
}
