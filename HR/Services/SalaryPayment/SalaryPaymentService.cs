using Azure;
using HR.Repository;
using HR.Repository.SQL;
using HR.Services.SalaryPayment.EasioCore;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services.SalaryPayment
{
    public class SalaryPaymentService : ISalaryPaymentService
    {
         private  IConfiguration _configuration;
        public SalaryPaymentService(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payments"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddSalaryPayment(List<SalaryPayment> payments, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Message = "Payment not released!",
                Status = 0,
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Get Salary Ledger ID
                string sqlLedger = "SELECT ISNULL(MAX(LedgerID),0) AS LID FROM Ledgers WHERE LedgerName='Salary' AND SubAccountID IN (2,3) AND LedgerDeleted='false'";
                var ledgerDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sqlLedger);
               // var ledgerDs = await SQLHelperCore.ExecuteDatasetAsync(SQLHelperCore.Connect, CommandType.Text, sqlLedger);
                string expLID = ledgerDs.Tables[0].Rows[0][0].ToString();

                if (string.IsNullOrEmpty(expLID) || int.Parse(expLID) == 0)
                {
                    rp.Message = "Salary Ledger not created";
                    return rp;
                }
                #endregion

                int successCount = 0;

                foreach (SalaryPayment SR in payments)
                {
                    #region Add Entry In Expenses & Expenses Payment Table
                    ExpensesData ED = new ExpensesData
                    {
                        ExpensesDate = SR.SDate,
                        Rate = SR.SAmount,
                        Amount = SR.SAmount,
                        Qnty = "1",
                        CashLID = "1", // Not Required
                        ExpenseLID = expLID, // Required
                        CreditorLID = "1", // Not Required
                        Remarks = SR.SRemarks,
                        UserName = SR.UpdatedBy,
                        ExpenseType = "1",
                        PaymentType = "1",
                        PaymentMode = SR.SalaryMethod,
                        FYear = SR.SYear,
                        SYear = SR.SMonth
                    };

                    ResponseModel expResponse =  await AddNewExpensePayment(ED,clientId);
                    SR.PIDFK = expResponse.ResponseData?.ToString();
                    #endregion

                    if (expResponse.Status == 1 && !string.IsNullOrEmpty(SR.PIDFK))
                    {
                        #region Insert Salary Payment if not duplicate
                        SqlParameter[] sqlParams =
                        {
                    new SqlParameter("@EmployeeCodeFK", SR.EmployeeCodeFK),
                    new SqlParameter("@SAmount", SR.SAmount),
                    new SqlParameter("@SYear", SR.SYear),
                    new SqlParameter("@SMonth", SR.SMonth),
                    new SqlParameter("@MonthNo", SR.MonthNo),
                    new SqlParameter("@SDate", SR.SDate),
                    new SqlParameter("@UpdatedBy", SR.UpdatedBy),
                    new SqlParameter("@SalaryIDFK", SR.SalaryIDFK),
                    new SqlParameter("@IsDeleted", "0"),
                    new SqlParameter("@SRemarks", SR.SRemarks),
                    new SqlParameter("@PIDFK", SR.PIDFK),
                    new SqlParameter("@SalaryMethod", SR.SalaryMethod)
                };

                        string dupEntry = "SELECT COUNT(SPID) AS DupCount FROM SalaryPayments WHERE EmployeeCodeFK=@EmployeeCodeFK AND SAmount=@SAmount AND SDate=@SDate AND SRemarks=@SRemarks";

                        DataSet dupDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, dupEntry, sqlParams);
                        int dupCount = 0;

                        if (dupDs.Tables.Count > 0 && dupDs.Tables[0].Rows.Count > 0)
                        {
                            dupCount = Convert.ToInt32(dupDs.Tables[0].Rows[0]["DupCount"]);
                        }


                        if (dupCount == 0)
                        {
                            string insertSql = "INSERT INTO SalaryPayments (EmployeeCodeFK, SAmount, SYear, SMonth, MonthNo, SDate, UpdatedBy, SalaryIDFK, IsDeleted, SRemarks, PIDFK) " +
                                               "VALUES (@EmployeeCodeFK, @SAmount, @SYear, @SMonth, @MonthNo, @SDate, @UpdatedBy, @SalaryIDFK, @IsDeleted, @SRemarks, @PIDFK)";

                            int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertSql, sqlParams);
                            if (result > 0)
                            {
                                successCount++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        rp.Message = "Technical Error In Expenses Transaction!";
                        return rp;
                    }
                }

                #region Final Response
                if (successCount > 0)
                {
                    rp.Status = 1;
                    rp.Message = $"Salary of ({successCount}) employees released";
                }
                else
                {
                    rp.Message = "No Transaction!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.Message;

                var errorLog = new Repository.Error.ErrorLog
                {
                    Title = "Salary Add Payment",
                    PageName = "Salary Payment BLL",
                    Error = ex.ToString(),
                   
                };

                Repository.Error.ErrorBLL.CreateErrorLog(errorLog);
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSalaryPaymentStatementOnMonthAndYear(string? param, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion
                #region Validate Param
                if (string.IsNullOrEmpty(param))
                {
                    rp.Message = "Parameter is required.";
                    return rp;
                }

                List<SalaryPayment> exp = new List<SalaryPayment>();
                string[] data = param.Split(',');
                if (data.Length != 2)
                {
                    rp.Message = "Invalid Parameters: [SMonth,SYear]";
                    return rp;
                }
                #endregion

                #region SQL Query & Execution
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@SMonth", data[0]),
            new SqlParameter("@SYear", data[1])
        };

                string sql = "SELECT * FROM SalaryPayments WHERE SMonth=@SMonth AND SYear=@SYear AND IsDeleted=0";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParams);
                #endregion

                #region Process Data
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    exp.Add(new SalaryPayment
                    {
                        SPID = dr["SPID"].ToString(),
                        EmployeeCodeFK = dr["EmployeeCodeFK"].ToString(),
                        PIDFK = dr["PIDFK"].ToString(),
                        SAmount = dr["SAmount"].ToString(),
                        SalaryIDFK = dr["SalaryIDFK"].ToString(),
                        SMonth = dr["SMonth"].ToString(),
                        SYear = dr["SYear"].ToString(),
                        SalaryMethod = dr["SalaryMethod"].ToString(),
                        UpdatedBy = dr["UpdatedBy"].ToString(),
                        SRemarks = dr["SRemarks"].ToString(),
                        SDate = Convert.ToDateTime(dr["SDate"].ToString()).ToString("dd-MM-yyyy")
                    });
                }
                #endregion

                #region Final Response
                if (exp.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = exp;
                }
                else
                {
                    rp.Message = "No Data Found";
                }
                #endregion
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
            }

            return await Task.FromResult(rp);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payments"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteSalaryPayment(List<SalaryPayment> payments, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Message = "No Record updated!",
                Status = 0,
                ResponseData = null
            };
            #endregion

            try
            {

                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                int x = 0;

                foreach (SalaryPayment SR in payments)
                {
                    #region Validate Parameters
                    if (string.IsNullOrEmpty(SR.SPID) || string.IsNullOrEmpty(SR.UpdatedBy) ||
                        string.IsNullOrEmpty(SR.SRemarks) || string.IsNullOrEmpty(SR.PIDFK))
                    {
                        rp.Message = "Params missing: SPID,UpdatedBy,SRemarks,PIDFK";
                        continue;
                    }
                    #endregion

                    #region Delete Entry from Expenses & ExpensesPayment
                    string epQuery = $"SELECT ISNULL(MAX(EPID),0) AS EPID FROM ExpensesPayment WHERE EIDFK={SR.PIDFK}";
                    var epDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, epQuery);
                    string EPID = epDs.Tables[0].Rows[0][0].ToString();

                    ExpensesData expData = new ExpensesData
                    {
                        EID = SR.PIDFK,
                        EPID = EPID,
                        Remarks = SR.SRemarks,
                        DeletedBy = SR.UpdatedBy,
                        DeletedOn = DateTime.Now.ToString("MM-dd-yyyy")
                    };

                    await DeleteExpensePayment(expData, clientId);
                    #endregion

                    #region Update SalaryPayment Record
                    SqlParameter[] sqlParams =
                    {
                new SqlParameter("@SPID", SR.SPID),
                new SqlParameter("@UpdatedBy", SR.UpdatedBy),
                new SqlParameter("@SRemarks", SR.SRemarks),
                new SqlParameter("@PIDFK", SR.PIDFK)
            };

                    string updateSql = "UPDATE SalaryPayments SET IsDeleted=1, UpdatedBy=@UpdatedBy, SRemarks=@SRemarks WHERE SPID=@SPID";
                    int affectedRows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateSql, sqlParams);

                    if (affectedRows > 0)
                    {
                        x++;
                    }
                    #endregion
                }

                #region Final Response
                if (x > 0)
                {
                    rp.Status = 1;
                    rp.Message = $"Salary of ({x}) employees deleted!";
                }
                else
                {
                    rp.Message = "No Transaction!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();

                var errorLog = new Repository.Error.ErrorLog
                {
                    Title = "Salary Deleted Payment",
                    PageName = "Salary Payment BLL",
                    Error = ex.ToString(),
                };

                Repository.Error.ErrorBLL.CreateErrorLog(errorLog);
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IN"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteExpensePayment(ExpensesData IN, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Payment Voucher Not Deleted!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Validate Parameters
                if (string.IsNullOrEmpty(IN.EID) || string.IsNullOrEmpty(IN.EPID) ||
                    string.IsNullOrEmpty(IN.DeletedBy) || string.IsNullOrEmpty(IN.DeletedOn))
                {
                    rp.Message = "Some params missing!";
                    return rp;
                }
                #endregion

                #region SQL Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@EID", IN.EID),
            new SqlParameter("@EPID", IN.EPID),
            new SqlParameter("@Remarks", IN.Remarks),
            //new SqlParameter("@UserName", IN.UserName),
            new SqlParameter("@DeletedBy", IN.DeletedBy),
            new SqlParameter("@DeletedOn", IN.DeletedOn)
        };
                #endregion

                #region Mark ExpensesPayment as Deleted
                string updateExpensePaymentSql = "UPDATE ExpensesPayment SET IsDeleted='true', Remarks=@Remarks, DeletedBy=@DeletedBy, DeletedOn=@DeletedOn WHERE EPID=@EPID";
                int expensePaymentResult = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateExpensePaymentSql, param);
                #endregion

                if (expensePaymentResult > 0)
                {
                    #region Mark Expenses as Deleted
                    string updateExpenseSql = "UPDATE Expenses SET IsDeleted='true', DeletedBy=@DeletedBy, DeletedOn=@DeletedOn, Remarks=@Remarks WHERE EID=@EID";
                    int expenseResult = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateExpenseSql, param);
                    #endregion

                    if (expenseResult > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Payment Voucher Deleted Successfully";
                    }
                    else
                    {
                        rp.Message = "Payment Voucher not deleted properly!";
                    }
                }
                else
                {
                    rp.Message = "Payment Voucher modification failed!";
                }
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
            }

            return rp;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IN"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddNewExpensePayment(ExpensesData IN, string clientId)
        {
            #region Initialize Response
            ResponseModel rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Payment Voucher Not Created!",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Validate Parameters
                if (string.IsNullOrEmpty(IN.ExpenseLID) || string.IsNullOrEmpty(IN.CashLID) ||
                    string.IsNullOrEmpty(IN.ExpensesDate) || string.IsNullOrEmpty(IN.Amount))
                {
                    rp.Message = "Some params missing!";
                    return rp;
                }
                #endregion

                #region Get Financial Year
                string currentFYear = "2023-24"; // Replace with actual financial year logic if needed
                if (currentFYear.Length != 7)
                {
                    rp.Message = "Invalid Financial Year/ Date";
                    return rp;
                }
                IN.FYear = currentFYear;
                #endregion

                #region Get Next PaymentNo
                string rNoQuery = $"SELECT ISNULL(MAX(PaymentNo),0) AS PaymentNo FROM ExpensesPayment WHERE FYear='{IN.FYear}' AND IsDeleted='false'";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, rNoQuery);
                long recNo = Convert.ToInt64(ds.Tables[0].Rows[0][0]) + 1;
                #endregion

                #region Prepare Parameters
                SqlParameter[] param =
                {
            new SqlParameter("@Rate", IN.Rate),
            new SqlParameter("@Amount", IN.Amount),
            new SqlParameter("@Qnty", IN.Qnty),
            new SqlParameter("@ExpensesDate", IN.ExpensesDate),
            new SqlParameter("@ExpenseLID", IN.ExpenseLID),
            new SqlParameter("@CashLID", IN.CashLID),
            new SqlParameter("@CreditorLID", IN.CreditorLID),
            new SqlParameter("@Remarks", IN.Remarks),
            new SqlParameter("@UserName", IN.UserName),
            new SqlParameter("@FYear", IN.FYear),
            new SqlParameter("@SYear", IN.SYear),
            new SqlParameter("@SMonth", (object)IN.SMonth ?? DBNull.Value),

           // new SqlParameter("@SMonth", IN.SMonth),
           // new SqlParameter("@UpdatedBy", IN.UpdatedBy),
           // new SqlParameter("@UpDatedOn", IN.UpdatedOn),
            new SqlParameter("@ExpenseType", IN.ExpenseType),
            new SqlParameter("@PaymentType", IN.PaymentType),
            new SqlParameter("@PaymentMode", IN.PaymentMode),
            new SqlParameter("@PaymentNo", recNo)
        };
                #endregion

                #region Insert into Expenses Table
                string expenseSql = "INSERT INTO Expenses ([ExpensesDate],[Rate],[Qnty],[Amount],[CashLID],[ExpenseLID],[FYear],[Remarks],[SYear],[SMonth],[IsDeleted],[UserName],[CreditorLID],[ExpenseType]) " +
                                    "VALUES (@ExpensesDate ,@Rate, @Qnty,@Amount,@CashLID,@ExpenseLID,@FYear,@Remarks,@SYear,@SMonth,'false', @UserName,@CreditorLID,@ExpenseType); " +
                                    "SELECT MAX(EID) AS MaxEID FROM Expenses WHERE ExpenseLID=@ExpenseLID AND ExpensesDate=@ExpensesDate AND UserName=@UserName";

                DataSet expenseDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, expenseSql, param);
                long IIDFK = Convert.ToInt64(expenseDs.Tables[0].Rows[0]["MaxEID"]);
                #endregion

                if (IIDFK > 0)
                {
                    #region Insert into ExpensesPayment Table
                    string paymentSql = "INSERT INTO ExpensesPayment ([PaymentDate],[CashOut],[LIDFrom],[LIDTo],[FYear],[Remarks],[SYear],[SMonth],[IsDeleted],[IsCancelled],[PaymentNo],[EIDFK],[UserName],[PaymentMode],[PaymentType]) " +
                                        "VALUES (@ExpensesDate,@Amount,@CashLID,@ExpenseLID,@FYear,@Remarks,@SYear,@SMonth,'false','false',@PaymentNo," + IIDFK + ",@UserName,@PaymentMode,@PaymentType)";

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, paymentSql, param);

                    if (result > 0)
                    {
                        rp.Status = 1;
                        rp.Message = "Payment Voucher added successfully";
                        rp.ResponseData = IIDFK.ToString();
                    }
                    else
                    {
                        rp.Message = "Payment Voucher not created properly!";
                    }
                    #endregion
                }
                else
                {
                    rp.Message = "Payment Voucher creation action failed!";
                }
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
            }

            return rp;
        }

    }
}
