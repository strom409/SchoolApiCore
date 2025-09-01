using Azure;
using HR.Repository;
using HR.Repository.SQL;
using HR.Services.Salary;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.PayRoll
{
    public class PayRollService : IPayRollService
    {
        private readonly IConfiguration _configuration;
        public PayRollService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> PayLoan(LoanPayment request, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to pay installment!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (request == null || request.LoanID <= 0 || request.EmployeeID <= 0 || request.LoanInstallment <= 0)
                {
                    rp.Message = "Invalid input data!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Insert Loan Installment
                int rt = await Task.Run(() => SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text,
                    "INSERT INTO LoanCollection (LoanInstallment, LoanID, EmployeeID, Year, Month, DateOfInstallment) " +
                    "VALUES (@LoanInstallment, @LoanID, @EmployeeID, YEAR(GETDATE()), DATENAME(MONTH, GETDATE()), GETDATE())",
                    new SqlParameter("@LoanInstallment", request.LoanInstallment),
                    new SqlParameter("@LoanID", request.LoanID),
                    new SqlParameter("@EmployeeID", request.EmployeeID)));
                #endregion

                #region Set Response Based on Insert Result
                if (rt > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Installment Paid Successfully";

                    var loanDetails = await GetLoanDetails(request.EmployeeID, clientId);

                    if (loanDetails != null && loanDetails.Status == 1)
                    {
                        rp.ResponseData = loanDetails.ResponseData;
                    }
                    else
                    {
                        rp.Message = "Installment Paid Successfully but, no active loan details found.";
                        rp.ResponseData = null;
                    }
                }
                else
                {
                    rp.Message = "Installment Not Paid!";
                }

                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.Message;
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> PayCPFLoan(LoanPayment request, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to pay installment!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (request == null || request.LoanID <= 0 || request.EmployeeID <= 0 || request.LoanInstallment <= 0)
                {
                    rp.Message = "Invalid input data!";
                    return rp;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Insert CPF Loan Installment
                int rt = await Task.Run(() => SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text,
                    "INSERT INTO CPFLoanCollection (CPFLoanInstallment, CPFLoanID, EmployeeID, Year, Month, DateOfInstallment) " +
                    "VALUES (@LoanInstallment, @LoanID, @EmployeeID, YEAR(GETDATE()), DATENAME(MONTH, GETDATE()), GETDATE())",
                    new SqlParameter("@LoanInstallment", request.LoanInstallment),
                    new SqlParameter("@LoanID", request.LoanID),
                    new SqlParameter("@EmployeeID", request.EmployeeID)));
                #endregion

                #region Set Response Based on Insert Result
                if (rt > 0)
                {
                    rp.Status = 1;
                    rp.Message = "CPF Installment Paid Successfully";

                    // Optionally, fetch updated CPF loan details here if needed
                    // rp.ResponseData = await GetCPFLoanDetails(request.EmployeeID, clientId);
                }
                else
                {
                    rp.Message = "CPF Installment Not Paid!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.Message;
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SR"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> IssueNewLoan(SalaryModel SR, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Loan/Advance section transaction failed!"
            };
            #endregion

            try
            {

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion


                #region Prepare SQL Parameters
                SqlParameter[] sqlParam1 = {
            new SqlParameter("@EmployeeID", SR.EmployeeID),
            new SqlParameter("@EmployeeCode", SR.EmployeeCode),
            new SqlParameter("@LoanAmount", SR.LoanBalance),
            new SqlParameter("@Year", SR.Year),
            new SqlParameter("@Month", SR.Month),
            new SqlParameter("@Date", SR.SalaryDate),

        };
                #endregion

                #region Execute Stored Procedure Async
                int rt = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "issuenewloan", sqlParam1);
                #endregion

                #region Process Result
                if (rt > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Loan issued";
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SetLeavesTaken(string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "Employee Not Updated!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion
                #region SQL Update Query
                string query = "UPDATE EmployeeDetail SET PenaltyDeduction=0, LeavesTaken=0, AdditionslAllownce=0";
                #endregion

                #region Execute Query
                int x = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Set Response Based on Result
                if (x > 0)
                {
                    rp.IsSuccess = true;
                    rp.Status = 1;
                    rp.Message = "Updated Successfully";
                }
                else
                {
                    rp.Message = "Failed To Update !";
                }
                #endregion

                return rp;
            }
            catch (Exception e)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + e.ToString();
                #endregion
                return rp;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SR"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> IssueCPFLoan(SalaryModel SR, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Loan/Advance section transaction failed!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion
                #region Prepare SQL Parameters
                SqlParameter[] sqlParam1 = {
            new SqlParameter("@EmployeeID", SR.EmployeeID),
            new SqlParameter("@EmployeeCode", SR.EmployeeCode),
            new SqlParameter("@CPFLoanAmount", SR.LoanBalance),
            new SqlParameter("@Year", SR.Year),
            new SqlParameter("@Month", SR.Month)
        };
                #endregion

                #region Execute Stored Procedure Async
                int rt = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "issuecpfnewloan", sqlParam1);
                #endregion

                #region Process Result
                if (rt > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Loan issued";
                }
                #endregion
            }
            catch (Exception er)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + er.ToString();
                #endregion
            }

            #region Return Response
            return rp;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetLoanDetails(long employeeID, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to retrieve loan details!",
                ResponseData = null
            };
            #endregion

            List<SalaryModel> SDL = new List<SalaryModel>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                string loanQuery = $"SELECT CONVERT(VARCHAR, DateOfLoanIssue, 106) AS DateOfLoanIssue, LoanID, LoanAmount FROM Loan WHERE EmployeeId={employeeID} AND LoanPaid='false'";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, loanQuery);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string empQuery = $"SELECT EmployeeName, PhotoPath, EmployeeCode FROM Employees WHERE EmployeeId={employeeID}";
                    DataSet dsEmp = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, empQuery);

                    if (dsEmp.Tables[0].Rows.Count == 0)
                    {
                        rp.Message = "Employee not found!";
                        return rp;
                    }

                    DataRow drEmp = dsEmp.Tables[0].Rows[0];
                    string empName = drEmp["EmployeeName"].ToString();
                    string photoPath = drEmp["PhotoPath"].ToString();
                    string empCode = drEmp["EmployeeCode"].ToString();

                    DataRow drTaken = ds.Tables[0].Rows[0];
                    long loanID = Convert.ToInt64(drTaken["LoanID"]);
                    decimal loanAmount = Convert.ToDecimal(drTaken["LoanAmount"]);
                    string dateOfLoanIssue = drTaken["DateOfLoanIssue"].ToString();

                    string recoveryQuery = $"SELECT SUM(LoanInstallment) AS Recovery FROM LoanCollection INNER JOIN Loan ON Loan.LoanID = LoanCollection.LoanID " +
                                           $"WHERE LoanCollection.EmployeeID = {employeeID} AND LoanPaid='false' GROUP BY LoanCollection.EmployeeID";

                    DataSet dsloan = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, recoveryQuery);

                    decimal recoveredAmount = 0;
                    decimal balance = loanAmount;

                    if (dsloan.Tables[0].Rows.Count > 0)
                    {
                        recoveredAmount = Convert.ToDecimal(dsloan.Tables[0].Rows[0]["Recovery"]);
                        balance = loanAmount - recoveredAmount;
                    }

                    if (balance == 0 && loanAmount == recoveredAmount)
                    {
                        string updateLoan = $"UPDATE Loan SET LoanPaid='True' WHERE LoanID={loanID}";
                        await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateLoan);
                    }

                    SDL.Add(new SalaryModel()
                    {
                        SalaryID = loanID,
                        EmployeeID = employeeID,
                        EmployeeCode = Convert.ToInt64(empCode),
                        EmployeeName = empName,
                        LoanTaken = loanAmount,
                        LoanRecovery = recoveredAmount,
                        LoanBalance = balance,
                        Scale = dateOfLoanIssue
                    });

                    if (SDL.Count > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Data Found";
                        rp.ResponseData = SDL;
                    }
                }
                else
                {
                    rp.Message = "No active loan found.";
                }
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.Message;
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeID"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCPFLoanDetails(long employeeID, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Failed to retrieve CPF loan details!",
                ResponseData = null
            };
            #endregion

            List<SalaryModel> SDL = new List<SalaryModel>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                string cpfLoanQuery = $"SELECT CONVERT(VARCHAR, DateOCPFLoanIssue, 106) AS DateOCPFLoanIssue, CPFLoanID, CPFLoanAmount FROM CPFLoan WHERE EmployeeId={employeeID} AND LoanPaid='false'";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, cpfLoanQuery);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    string empQuery = $"SELECT EmployeeName, PhotoPath, EmployeeCode FROM Employees WHERE EmployeeId={employeeID}";
                    DataSet dsEmp = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, empQuery);

                    if (dsEmp.Tables[0].Rows.Count == 0)
                    {
                        rp.Message = "Employee not found!";
                        return rp;
                    }

                    DataRow drEmp = dsEmp.Tables[0].Rows[0];
                    string empName = drEmp["EmployeeName"].ToString();
                    string photoPath = drEmp["PhotoPath"].ToString();
                    string empCode = drEmp["EmployeeCode"].ToString();

                    DataRow drTaken = ds.Tables[0].Rows[0];
                    long cpfLoanID = Convert.ToInt64(drTaken["CPFLoanID"]);
                    decimal cpfLoanAmount = Convert.ToDecimal(drTaken["CPFLoanAmount"]);
                    string dateOfCPFLoanIssue = drTaken["DateOCPFLoanIssue"].ToString();

                    string recoveryQuery = $"SELECT SUM(CPFLoanInstallment) AS Recovery FROM CPFLoanCollection " +
                                           $"INNER JOIN CPFLoan ON CPFLoan.CPFLoanID = CPFLoanCollection.CPFLoanID " +
                                           $"WHERE CPFLoanCollection.EmployeeID = {employeeID} AND LoanPaid='false' " +
                                           $"GROUP BY CPFLoanCollection.EmployeeID";

                    DataSet dsloan = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, recoveryQuery);

                    decimal recoveredAmount = 0;
                    decimal balance = cpfLoanAmount;

                    if (dsloan.Tables[0].Rows.Count > 0)
                    {
                        recoveredAmount = Convert.ToDecimal(dsloan.Tables[0].Rows[0]["Recovery"]);
                        balance = cpfLoanAmount - recoveredAmount;
                    }

                    if (balance == 0 && cpfLoanAmount == recoveredAmount)
                    {
                        string updateLoan = $"UPDATE CPFLoan SET LoanPaid='True' WHERE CPFLoanID={cpfLoanID}";
                        await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateLoan);
                    }

                    SDL.Add(new SalaryModel()
                    {
                        SalaryID = cpfLoanID,
                        EmployeeID = employeeID,
                        EmployeeCode = Convert.ToInt64(empCode),
                        EmployeeName = empName,
                        LoanTaken = cpfLoanAmount,
                        LoanRecovery = recoveredAmount,
                        LoanBalance = balance,
                        Scale = dateOfCPFLoanIssue
                    });

                    if (SDL.Count > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "CPF Loan Data Found";
                        rp.ResponseData = SDL;
                    }
                }
                else
                {
                    rp.Message = "No active CPF loan found.";
                }
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.Message;
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCPFLoanDefaultList(string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!",
                ResponseData = null
            };
            #endregion

            List<SalaryModel> SDL = new List<SalaryModel>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "LoanDefaultListCP");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SDL.Add(new SalaryModel()
                        {
                            SalaryID = Int64.Parse(dr["LoanID"].ToString()),
                            EmployeeID = Int64.Parse(dr["EmployeeID"].ToString()),
                            EmployeeCode = Int64.Parse(dr["EmployeeCode"].ToString()),
                            EmployeeName = dr["EmpName"].ToString(),
                            LoanTaken = Convert.ToDecimal(dr["LoanAmount"].ToString()),
                            LoanRecovery = Convert.ToDecimal(dr["LoanRecoverd"].ToString()),
                            LoanBalance = Convert.ToDecimal(dr["LoanBalance"].ToString()),
                            Scale = dr["DoI"].ToString()
                        });
                    }
                }

                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = SDL;
                }

                return rp;
            }
            catch (Exception ex)
            {
                rp.Status = -1;
                rp.IsSuccess = false;
                rp.Message = "Error: " + ex.ToString();
                return rp;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetLoanDefaultList(string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!",
                ResponseData = null
            };
            #endregion

            List<SalaryModel> SDL = new List<SalaryModel>();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "LoanDefaultList");
                #endregion

                #region Process DataSet
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SDL.Add(new SalaryModel()
                        {
                            SalaryID = Int64.Parse(dr["LoanID"].ToString()),
                            EmployeeID = Int64.Parse(dr["EmployeeID"].ToString()),
                            EmployeeCode = Int64.Parse(dr["EmployeeCode"].ToString()),
                            EmployeeName = dr["EmpName"].ToString(),
                            LoanTaken = Convert.ToDecimal(dr["LoanAmount"].ToString()),
                            LoanRecovery = Convert.ToDecimal(dr["LoanRecoverd"].ToString()),
                            LoanBalance = Convert.ToDecimal(dr["LoanBalance"].ToString()),
                            Scale = dr["DoI"].ToString()
                        });
                    }
                }
                #endregion

                #region Prepare Response
                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = SDL;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                rp.Status = -1;
                rp.IsSuccess = false;
                rp.Message = "Error: " + ex.ToString();
                #endregion
            }

            return rp;
        }

    }
}
