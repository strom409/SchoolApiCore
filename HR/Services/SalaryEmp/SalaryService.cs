using Azure;
using HR.Repository;
using HR.Repository.SQL;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace HR.Services.Salary
{
    public class SalaryService : ISalaryService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployeeService _employeeService;
        public SalaryService(IConfiguration configuration, IEmployeeService employeeService)
        {
            _configuration = configuration;
            _employeeService = employeeService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeSalaryToEditOnEDID(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter sqlParam = new SqlParameter("@EDID", param);
                #endregion

                #region SQL Query
                string sql = @"
            SELECT *, 'NA' as FieldName 
            FROM EmployeeDetail 
            INNER JOIN Employees 
            ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
            WHERE EDID = @EDID 
            AND (Withdrawn = 'False' OR WithdrawnEmp = 'false')";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParam);
                #endregion

                #region Map Data
                var ed = await _employeeService.EmpData(ds, clientId);
                #endregion

                #region Check & Return Response
                if (ed.Count > 0)
                {
                    if (ed[0].EDID > 0)
                    {
                        rp.Status = 1;
                        rp.ResponseData = ed[0];
                        rp.Message = "Data Found";
                        return rp;
                    }
                    else
                    {
                        rp.IsSuccess = false;
                        rp.Status = -1;
                        rp.Message = "Error: " + ed[0].Status;
                        return rp;
                    }
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeSalaryToEditOnECode(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter sqlParam = new SqlParameter("@ECode", param);
                #endregion

                #region Get EmployeeID by EmployeeCode
                DataSet dsEmpId = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text,
                    "SELECT ISNULL(MAX(EmployeeID),0) AS EmpID FROM Employees WHERE EmployeeCode=@ECode", sqlParam);

                string employeeId = dsEmpId.Tables[0].Rows[0][0]?.ToString() ?? "0";

                if (string.IsNullOrEmpty(employeeId) || employeeId == "0")
                {
                    rp.Message = "Invalid Employee Code: " + param;
                    return rp;
                }
                #endregion

                #region Get Max EDID by EmployeeID
                string edidQuery = $"SELECT ISNULL(MAX(EDID),0) AS EDID FROM EmployeeDetail WHERE EmployeeID={employeeId}";
                DataSet dsEdid = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, edidQuery);
                string edid = dsEdid.Tables[0].Rows[0][0]?.ToString() ?? "0";
                #endregion

                #region Get Employee Salary Details
                SqlParameter sqlParamEdid = new SqlParameter("@EDID", edid);

                string sql = @"
            SELECT *, 'NA' as FieldName 
            FROM EmployeeDetail 
            INNER JOIN Employees ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
            WHERE EDID=@EDID AND (Withdrawn='False' OR WithdrawnEmp='false')";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParamEdid);
                #endregion

                #region Map Data
                var ed = await _employeeService.EmpData(ds, clientId);
                #endregion

                #region Check & Return Response
                if (ed.Count > 0)
                {
                    if (ed[0].EDID > 0)
                    {
                        rp.Status = 1;
                        rp.ResponseData = ed[0];
                        rp.Message = "Data Found";
                        return rp;
                    }
                    else
                    {
                        rp.IsSuccess = false;
                        rp.Status = -1;
                        rp.Message = "Error: " + ed[0].Status;
                        return rp;
                    }
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeSalaryToEditOnFieldName(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Split and Validate Parameters
                string[] vals = param.Split(',');
                if (vals.Length != 2)
                {
                    rp.Message = "Invalid Parameter Values of Year & Field Name";
                    return rp;
                }

                string year = vals[0];      // Year or FYear
                string fieldName = vals[1]; // Field Name
                #endregion

                #region Prepare Query and Parameter
                SqlParameter sqlParam = new SqlParameter("@Year", year);
                string sql;

                if (year.Length == 4) // On Year
                {
                    sql = $@"
                SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, *
                FROM Employees
                INNER JOIN EmployeeDetail ON Employees.EmployeeID = EmployeeDetail.EmployeeID
                WHERE Year = @Year AND (Withdrawn = 'False' OR WithdrawnEmp = 'false')
                ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                else // On FYear
                {
                    sql = $@"
                SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, *
                FROM Employees
                INNER JOIN EmployeeDetail ON Employees.EmployeeID = EmployeeDetail.EmployeeID
                WHERE FYear = @Year AND (Withdrawn = 'False' OR WithdrawnEmp = 'false')
                ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParam);
                #endregion

                #region Map Data
                List<EmployeeDetail> ed = await _employeeService.EmpData(ds, clientId);
                #endregion

                #region Check & Return Response
                if (ed.Count > 0)
                {
                    if (ed[0].EDID > 0)
                    {
                        rp.Status = 1;
                        rp.ResponseData = ed[0];
                        rp.Message = "Data Found";
                        return rp;
                    }
                    else
                    {
                        rp.IsSuccess = false;
                        rp.Status = -1;
                        rp.Message = "Error: " + ed[0].Status;
                        return rp;
                    }
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSalaryDataOnMonthFromSalaryOnDeparts(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No data found!" };
            #endregion

            try
            {
                #region Get Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Split and Validate Parameters
                string[] splitDataByDash = param.Split('-');
                if (splitDataByDash.Length != 2)
                {
                    rp.Message = "Parameter not in valid format: Year,Month-departments";
                    return rp;
                }

                string[] yearMonth = splitDataByDash[0].Split(',');
                if (yearMonth.Length != 2)
                {
                    rp.Message = "Parameter not in valid format: Year,Month-departments";
                    return rp;
                }

                string departs = splitDataByDash[1];  // No need to trim here since UI handles it
                #endregion

                #region Prepare Query and Parameters
                SqlParameter[] sqlParams = {
            new SqlParameter("@Year", yearMonth[0]),
            new SqlParameter("@Month", yearMonth[1]),
            new SqlParameter("@Departs", departs)
        };

                string query = $"SELECT * FROM Salary WHERE Year = @Year AND Month = @Month AND EDID IN ({departs}) ORDER BY EmployeeCode";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Data
                List<SalaryModel> SDL = new List<SalaryModel>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string acNo = dr["BankAccountNo"].ToString();
                        while (acNo.Length < 16)
                        {
                            acNo = "0" + acNo;
                        }

                        SDL.Add(new SalaryModel
                        {
                            SalaryID = Convert.ToInt64(dr["SalaryID"]),
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            SubDepartmentName = dr["SubDepartmentName"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Status = dr["Status"].ToString(),
                            Month = dr["Month"].ToString(),
                            Year = dr["Year"].ToString(),
                            SalaryDate = validationBLL.isDateNotNull(dr["SalaryDate"].ToString()),
                            BankAccountNo = acNo,
                            BankAccount = Convert.ToBoolean(dr["BankAccount"]),
                            BasicPay = Convert.ToDecimal(dr["BasicPay"] ?? "0"),
                            DARate = Convert.ToDecimal(dr["DARate"] ?? "0"),
                            SACAllownce = Convert.ToDecimal(dr["SACAllownce"] ?? "0"),
                            HouseRentAllownce = Convert.ToDecimal(dr["HouseRentAllownce"] ?? "0"),
                            MedicalAllownce = Convert.ToDecimal(dr["MedicalAllownce"] ?? "0"),
                            DarenessAllownce = Convert.ToDecimal(dr["DarenessAllownce"] ?? "0"),
                            TravelAllownce = Convert.ToDecimal(dr["TravelAllownce"] ?? "0"),
                            RationAllownce = Convert.ToDecimal(dr["RationAllownce"] ?? "0"),
                            AdditionslAllownce = Convert.ToDecimal(dr["AdditionslAllownce"] ?? "0"),
                            Pay = Convert.ToDecimal(dr["Pay"] ?? "0"),
                            TempBasicPay = Convert.ToDecimal(dr["TempBasicPay"] ?? "0"),
                            GrossPay = Convert.ToDecimal(dr["GrossPay"] ?? "0"),
                            NetPay = Convert.ToDecimal(dr["NetPay"] ?? "0"),
                            EmployeeCPShare = Convert.ToDecimal(dr["EmployeeCPShare"] ?? "0"),
                            EmployerCPShare = Convert.ToDecimal(dr["EmployerCPShare"] ?? "0"),
                            TotalLeavAddAmt = Convert.ToDecimal(dr["TotalLeavAddAmt"] ?? "0"),
                            TotelLeavDedAmt = Convert.ToDecimal(dr["TotelLeavDedAmt"] ?? "0"),
                            LeavesTaken = Convert.ToDecimal(dr["LeavesTaken"] ?? "0"),
                            Insurance1PercentAmt = Convert.ToDecimal(dr["Insurance1PercentAmt"] ?? "0"),
                            CPFDeduction = Convert.ToDecimal(dr["CPFDeduction"] ?? "0"),
                            TotalAllownce = Convert.ToDecimal(dr["TotalAllownce"] ?? "0"),
                            LoanDeduction = Convert.ToDecimal(dr["LoanDeduction"] ?? "0"),
                            SecurityDeduction = Convert.ToDecimal(dr["SecurityDeduction"] ?? "0"),
                            TotalDeduction = Convert.ToDecimal(dr["TotalDeduction"] ?? "0"),
                            PenaltyDeduction = Convert.ToDecimal(dr["PenaltyDeduction"] ?? "0")
                        });
                    }
                }
                #endregion

                #region Return Response
                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = SDL;
                }
                return rp;
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.Status = -1;
                rp.IsSuccess = false;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCalculatedGrossNetEtc(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No record found, please check the year!" };
            #endregion

            try
            {
                #region Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query and Parameters
                SqlParameter sqlParam = new SqlParameter("@year", param);
                string sql = @"SELECT EDID, Employees.EmployeeID, EmployeeCode, EmployeeName, BasicPay, DarenessAllownce, 
                              CPFundIntrest, Insurance1PercentRate, HouseRentAllownce, AdditionslAllownce, LeavesAvailable, 
                              SubDepartmentID 
                       FROM EmployeeDetail 
                       INNER JOIN Employees ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE Year = @year AND WithdrawnEmp = 'false' 
                       ORDER BY EmployeeCode";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParam);
                #endregion

                #region Map Data
                List<GrossPaySalary> gpL = new List<GrossPaySalary>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GrossPaySalary gp = new GrossPaySalary
                    {
                        EDID = dr["EDID"].ToString(),
                        DepartmentID = dr["SubDepartmentID"].ToString(),
                        BP = Convert.ToDecimal(dr["BasicPay"] ?? "0"),
                        DA = Convert.ToDecimal(dr["DarenessAllownce"] ?? "0")
                    };

                    gp.Pay = Math.Round(gp.BP + gp.DA);

                    gp.EPFRate = Convert.ToDecimal(dr["CPFundIntrest"] ?? "0");
                    gp.EPF = ((gp.BP + gp.DA) / 100) * gp.EPFRate;

                    gp.LI = Convert.ToDecimal(dr["Insurance1PercentRate"] ?? "0");
                    gp.LIDeduction = Math.Round(((gp.BP + gp.DA) / 100) * gp.LI);

                    gp.EPFDeduction = Math.Round(gp.EPF * 2) + gp.LIDeduction;

                    gp.HRA = Convert.ToDecimal(dr["HouseRentAllownce"] ?? "0");
                    gp.OtherAllow = Convert.ToDecimal(dr["AdditionslAllownce"] ?? "0");

                    gp.ELeave = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / 100) *
                                Convert.ToDecimal(dr["LeavesAvailable"] ?? "0");

                    gp.GrossPay = Math.Round(gp.BP + gp.DA + gp.EPFDeduction + gp.LIDeduction + gp.ELeave + gp.HRA + gp.OtherAllow);
                    gp.NetPay = Math.Round(gp.GrossPay - (gp.EPFDeduction + gp.LIDeduction));

                    gp.EmployeeCode = dr["EmployeeCode"].ToString();
                    gp.EmployeeName = dr["EmployeeName"].ToString();

                    gpL.Add(gp);
                }
                #endregion

                #region Return Response
                if (gpL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = gpL;
                }

                return rp;
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetCalculatedGrossNetEtcOnEDID(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No record found, please check EDID!" };
            #endregion

            try
            {
                #region Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query and Parameters
                SqlParameter sqlParam = new SqlParameter("@EDID", param);
                string sql = @"SELECT EDID, Employees.EmployeeID, EmployeeCode, EmployeeName, BasicPay, DarenessAllownce, 
                              CPFundIntrest, Insurance1PercentRate, HouseRentAllownce, AdditionslAllownce, LeavesAvailable 
                       FROM EmployeeDetail 
                       INNER JOIN Employees ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE EDID = @EDID AND WithdrawnEmp = 'false' 
                       ORDER BY EmployeeCode";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, sqlParam);
                #endregion

                #region Map Data
                GrossPaySalary gp = new GrossPaySalary();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    gp.EDID = dr["EDID"].ToString();
                    gp.BP = Convert.ToDecimal(dr["BasicPay"] ?? "0");
                    gp.DA = Convert.ToDecimal(dr["DarenessAllownce"] ?? "0");
                    gp.Pay = Math.Round(gp.BP + gp.DA);

                    gp.EPFRate = Convert.ToDecimal(dr["CPFundIntrest"] ?? "0");
                    gp.EPF = ((gp.BP + gp.DA) / 100) * gp.EPFRate;

                    gp.LI = Convert.ToDecimal(dr["Insurance1PercentRate"] ?? "0");
                    gp.LIDeduction = Math.Round(((gp.BP + gp.DA) / 100) * gp.LI);

                    gp.EPFDeduction = Math.Round(gp.EPF * 2) + gp.LIDeduction;

                    gp.HRA = Convert.ToDecimal(dr["HouseRentAllownce"] ?? "0");
                    gp.OtherAllow = Convert.ToDecimal(dr["AdditionslAllownce"] ?? "0");

                    gp.ELeave = (DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month) / 100) *
                                Convert.ToDecimal(dr["LeavesAvailable"] ?? "0");

                    gp.GrossPay = Math.Round(gp.BP + gp.DA + gp.EPFDeduction + gp.LIDeduction + gp.ELeave + gp.HRA + gp.OtherAllow);
                    gp.NetPay = Math.Round(gp.GrossPay - (gp.EPFDeduction + gp.LIDeduction));

                    gp.EmployeeCode = dr["EmployeeCode"].ToString();
                    gp.EmployeeName = dr["EmployeeName"].ToString();

                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = gp;
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetSalaryDataOnYearFromSalaryOnECode(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Records found!" };
            #endregion

            try
            {
                #region Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Parse Parameters
                string[] EMP = param.Split(',');
                string ECode = EMP[0];
                string Year = EMP[1];
                #endregion

                #region Prepare Query and Parameters
                SqlParameter[] sp =
                {
            new SqlParameter("@Year", Year),
            new SqlParameter("@ECode", ECode)
        };

                string qry = "SELECT * FROM Salary WHERE Year=@Year AND EmployeeCode=@ECode";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, qry, sp);
                #endregion

                #region Map Data
                List<SalaryModel> SDL = new List<SalaryModel>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SDL.Add(new SalaryModel
                        {
                            SalaryID = Convert.ToInt64(dr["SalaryID"]),
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            SubDepartmentName = dr["SubDepartmentName"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Status = dr["Status"].ToString(),
                            Month = dr["Month"].ToString(),
                            Year = dr["Year"].ToString(),
                            FYear = dr["FYear"].ToString(),
                            SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),

                            BasicPay = validationBLL.IsDecimalNotNull(dr["BasicPay"].ToString(), 0),
                            DARate = validationBLL.IsDecimalNotNull(dr["DARate"].ToString(), 0),
                            DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                            Pay = validationBLL.IsDecimalNotNull(dr["Pay"].ToString(), 0),
                            TempBasicPay = validationBLL.IsDecimalNotNull(dr["TempBasicPay"].ToString(), 0),
                            GrossPay = Convert.ToDecimal(dr["GrossPay"] ?? "0"),
                            NetPay = Convert.ToDecimal(dr["NetPay"] ?? "0"),

                            SalaryDate = validationBLL.isDateNotNull(dr["SalaryDate"].ToString()),
                            BankAccountNo = dr["BankAccountNo"].ToString(),

                            LoanAmountRefund = validationBLL.IsDecimalNotNull(dr["LoanAmountRefund"].ToString(), 0),
                            SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                            HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                            MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                            AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                            TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                            RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                            TotalAllownce = validationBLL.IsDecimalNotNull(dr["TotalAllownce"].ToString(), 0),
                            SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                            SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),

                            LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                            TotalLeavAddAmt = validationBLL.IsDecimalNotNull(dr["TotalLeavAddAmt"].ToString(), 0),
                            TotelLeavDedAmt = validationBLL.IsDecimalNotNull(dr["TotelLeavDedAmt"].ToString(), 0),

                            LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                            InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                            CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["CPFRecoveryDedAmt"].ToString(), 0),
                            Insurance1PercentAmt = validationBLL.IsDecimalNotNull(dr["Insurance1PercentAmt"].ToString(), 0),
                            CPFDeduction = validationBLL.IsDecimalNotNull(dr["CPFDeduction"].ToString(), 0),
                            EmployeeCPShare = validationBLL.IsDecimalNotNull(dr["EmployeeCPShare"].ToString(), 0),
                            EmployerCPShare = validationBLL.IsDecimalNotNull(dr["EmployerCPShare"].ToString(), 0),
                            SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                            TotalDeduction = validationBLL.IsDecimalNotNull(dr["TotalDeduction"].ToString(), 0),
                            TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                            ExcessLeaveDeduction = validationBLL.IsDecimalNotNull(dr["ExcessLeaveDeduction"].ToString(), 0),
                            WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                            CPFLoanCollection = validationBLL.IsDecimalNotNull(dr["CPFLoanCollection"].ToString(), 0),
                            PenaltyDeduction = Convert.ToDecimal(string.IsNullOrEmpty(dr["PenaltyDeduction"].ToString()) ? "0" : dr["PenaltyDeduction"].ToString()),
                        });
                    }
                }
                #endregion

                #region Final Response
                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "ok";
                    rp.ResponseData = SDL;
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
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
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No data found!" };
            #endregion

            try
            {
                #region Connection String (Dynamic by ClientId)
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "LoanDefaultList1");
                #endregion

                #region Map Data
                List<SalaryModel> SDL = new List<SalaryModel>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SDL.Add(new SalaryModel
                        {
                            SalaryID = Convert.ToInt64(dr["LoanID"]),
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmpName"].ToString(),

                            LoanTaken = Convert.ToDecimal(dr["LoanAmount"]),
                            LoanRecovery = Convert.ToDecimal(dr["LoanRecoverd"]),
                            LoanBalance = Convert.ToDecimal(dr["LoanBalance"]),
                            Scale = dr["DoI"].ToString(),
                        });
                    }
                }
                #endregion

                #region Final Response
                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = SDL;
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception Handling
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SalaryPaymentAccountStatementOnEcodeAndDates(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel { IsSuccess = true, Status = 0, Message = "No data!" };
            #endregion

            try
            {
                #region Validate Params
                string[] values = param.Split(',');
                if (values.Length != 3)
                {
                    rp.Message = "Some Params Missing!";
                    return rp;
                }
                #endregion

                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@ECode", values[0]),
            new SqlParameter("@DFrom", values[1]),
            new SqlParameter("@DTo", values[2]),
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "SalaryAccountAPI", sqlParams);
                #endregion

                #region Process Data
                List<SalaryPaymentReport> exp = new List<SalaryPaymentReport>();
                decimal bal = 0;

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (!string.IsNullOrEmpty(dr["CreditAmount"].ToString()) && dr["CreditAmount"].ToString() != "0")
                        bal += Convert.ToDecimal(dr["CreditAmount"]);
                    else
                        bal -= Convert.ToDecimal(dr["DebitAmount"]);

                    string TDate1;
                    try
                    {
                        TDate1 = Convert.ToDateTime(dr["TDate"]).ToString("dd-MM-yyyy");
                    }
                    catch
                    {
                        TDate1 = "NAr";
                    }

                    exp.Add(new SalaryPaymentReport
                    {
                        SRemarks = dr["Rem"].ToString(),
                        SDate = TDate1,
                        SalaryMethod = dr["VoucherType"].ToString(),
                        NetPay = dr["CreditAmount"].ToString(),
                        SAmount = dr["DebitAmount"].ToString(),
                        Balance = bal.ToString(),
                        SMonth = dr["SMonth"].ToString(),
                        SYear = dr["SYear"].ToString(),
                        TempBasicPay = dr["SYear"].ToString(),
                        TotalAllownce = dr["TotalAllownce"].ToString(),
                        GrossPay = dr["GrossPay"].ToString(),
                        CPFDeduction = dr["CPFDeduction"].ToString(),
                        LeavesTaken = dr["LeavesTaken"].ToString(),
                        TotelLeavDedAmt = dr["TotelLeavDedAmt"].ToString(),
                        TotalDeduction = dr["TotalDeduction"].ToString()
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

                return rp;
            }
            catch (Exception ex)
            {
                #region Exception
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAvailableNetSalaryOnMonthFromSalaryAndSalaryPaymentOnDeparts(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!"
            };
            #endregion

            try
            {
                #region Validate Params
                var splitDataByDash = param.Split('-');
                if (splitDataByDash.Length != 2)
                {
                    response.Message = "Parameter not in valid format: Year,Month-departments";
                    return response;
                }

                var yearMonth = splitDataByDash[0].Split(',');
                if (yearMonth.Length != 2)
                {
                    response.Message = "Parameter not in valid format: Year,Month-departments";
                    return response;
                }

                string year = yearMonth[0];
                string month = yearMonth[1];
                string departs = splitDataByDash[1];
                #endregion

                #region Prepare SQL Query
                string query = $"SELECT * FROM Salary WHERE Year=@Year AND Month=@Month AND EDID IN ({departs}) ORDER BY EmployeeCode";
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@Year", year),
            new SqlParameter("@Month", month),
            new SqlParameter("@Departs", departs)
        };
                #endregion

                #region Execute SQL
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                var salaries = new List<SalaryModel>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        decimal netPay = 0;

                        // Pad Account Number
                        string acNo = dr["BankAccountNo"].ToString().PadLeft(16, '0');

                        #region Get Advance Balance
                        string sql = @"SELECT ISNULL(SUM(NetPay),0) - 
                              (SELECT ISNULL(SUM(SAmount),0) 
                               FROM SalaryPayments 
                               WHERE EmployeeCodeFK=@Ecode AND IsDeleted=0) AS Adv
                               FROM Salary WHERE EmployeeCode=@Ecode";

                        SqlParameter advanceParam = new SqlParameter("@Ecode", dr["EmployeeCode"].ToString());
                        var netPayDs = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, advanceParam);

                        if (netPayDs.Tables[0].Rows.Count > 0)
                        {
                            string val = netPayDs.Tables[0].Rows[0][0].ToString();
                            if (!string.IsNullOrEmpty(val))
                                netPay = Convert.ToDecimal(val);
                        }
                        else
                        {
                            netPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["NetPay"].ToString()) ? "0" : dr["NetPay"].ToString());
                        }
                        #endregion

                        #region Map Salary Data
                        salaries.Add(new SalaryModel
                        {
                            SalaryID = Convert.ToInt64(dr["SalaryID"]),
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            SubDepartmentName = dr["SubDepartmentName"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Status = dr["Status"].ToString(),
                            Month = dr["Month"].ToString(),
                            Year = dr["Year"].ToString(),
                            SalaryDate = validationBLL.isDateNotNull(dr["SalaryDate"].ToString()),
                            BankAccountNo = acNo,
                            BankAccount = Convert.ToBoolean(dr["BankAccount"]),
                            BasicPay = Convert.ToDecimal(dr["BasicPay"] ?? 0),
                            DARate = Convert.ToDecimal(dr["DARate"] ?? 0),
                            SACAllownce = Convert.ToDecimal(dr["SACAllownce"] ?? 0),
                            HouseRentAllownce = Convert.ToDecimal(dr["HouseRentAllownce"] ?? 0),
                            MedicalAllownce = Convert.ToDecimal(dr["MedicalAllownce"] ?? 0),
                            DarenessAllownce = Convert.ToDecimal(dr["DarenessAllownce"] ?? 0),
                            TravelAllownce = Convert.ToDecimal(dr["TravelAllownce"] ?? 0),
                            RationAllownce = Convert.ToDecimal(dr["RationAllownce"] ?? 0),
                            AdditionslAllownce = Convert.ToDecimal(dr["AdditionslAllownce"] ?? 0),
                            Pay = Convert.ToDecimal(dr["Pay"] ?? 0),
                            TempBasicPay = Convert.ToDecimal(dr["TempBasicPay"] ?? 0),
                            GrossPay = Convert.ToDecimal(dr["GrossPay"] ?? 0),
                            NetPay = netPay,
                            EmployeeCPShare = Convert.ToDecimal(dr["EmployeeCPShare"] ?? 0),
                            EmployerCPShare = Convert.ToDecimal(dr["EmployerCPShare"] ?? 0),
                            TotalLeavAddAmt = Convert.ToDecimal(dr["TotalLeavAddAmt"] ?? 0),
                            TotelLeavDedAmt = Convert.ToDecimal(dr["TotelLeavDedAmt"] ?? 0),
                            LeavesTaken = Convert.ToDecimal(dr["LeavesTaken"] ?? 0),
                            Insurance1PercentAmt = Convert.ToDecimal(dr["Insurance1PercentAmt"] ?? 0),
                            CPFDeduction = Convert.ToDecimal(dr["CPFDeduction"] ?? 0),
                            TotalAllownce = Convert.ToDecimal(dr["TotalAllownce"] ?? 0),
                            LoanDeduction = Convert.ToDecimal(dr["LoanDeduction"] ?? 0),
                            SecurityDeduction = Convert.ToDecimal(dr["SecurityDeduction"] ?? 0),
                            TotalDeduction = Convert.ToDecimal(dr["TotalDeduction"] ?? 0),
                            PenaltyDeduction = Convert.ToDecimal(dr["PenaltyDeduction"] ?? 0)
                        });
                        #endregion
                    }
                }

                if (salaries.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = salaries;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetBankSalarySlipOnMonthFromSalaryAndSalaryPaymentOnDeparts(string param, string clientId)
        {
            #region Initialize Response
            var rp = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No data found!"
            };
            #endregion

            try
            {
                #region Validate Parameters
                string[] splitDataByDash = param.Split('-');
                if (splitDataByDash.Length != 2)
                {
                    rp.Message = "Parameter not in valid format: Year,Month-departments";
                    return rp;
                }

                string[] YearMonth = splitDataByDash[0].Split(',');
                if (YearMonth.Length != 2)
                {
                    rp.Message = "Parameter not in valid format: Year,Month-departments";
                    return rp;
                }

                string Departs = splitDataByDash[1];
                #endregion

                #region Database Query
                var connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);
                SqlParameter[] sp =
                {
            new SqlParameter("@Year", YearMonth[0]),
            new SqlParameter("@Month", YearMonth[1]),
            new SqlParameter("@Departs", Departs)
        };

                string qry = $"select * from Salary where Year=@Year and Month=@Month and EDID in ({Departs}) order by EmployeeCode";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, qry, sp);
                #endregion

                #region Data Mapping
                List<SalaryModel> SDL = new List<SalaryModel>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        decimal NPay = 0;
                        string AcNo = dr["BankAccountNo"].ToString().PadLeft(16, '0');

                        SqlParameter[] pp =
                        {
                    new SqlParameter("@Ecode", dr["EmployeeCode"].ToString()),
                    new SqlParameter("@Year", YearMonth[0]),
                    new SqlParameter("@Month", YearMonth[1])
                };

                        string sql = "select isnull(max(SAmount),0) from SalaryPayments where EmployeeCodeFK=@Ecode and IsDeleted=0 and SMonth=@Month and SYear=@Year";

                        DataSet dsNetPay = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, sql, pp);
                        if (dsNetPay.Tables[0].Rows.Count > 0)
                        {
                            string drNetPay = dsNetPay.Tables[0].Rows[0][0].ToString();
                            if (!string.IsNullOrEmpty(drNetPay))
                            {
                                NPay = Convert.ToDecimal(drNetPay);
                            }
                        }
                        else
                        {
                            NPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["NetPay"].ToString()) ? "0" : dr["NetPay"].ToString());
                        }

                        SalaryModel ss = new SalaryModel
                        {
                            SalaryID = Convert.ToInt64(dr["SalaryID"]),
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            SubDepartmentName = dr["SubDepartmentName"].ToString(),
                            Designation = dr["Designation"].ToString(),
                            Status = dr["Status"].ToString(),
                            Month = dr["Month"].ToString(),
                            Year = dr["Year"].ToString(),
                            SalaryDate = validationBLL.isDateNotNull(dr["SalaryDate"].ToString()),
                            BankAccountNo = AcNo,
                            BankAccount = Convert.ToBoolean(dr["BankAccount"]),
                            BasicPay = Convert.ToDecimal(dr["BasicPay"] ?? 0),
                            DARate = Convert.ToDecimal(dr["DARate"] ?? 0),
                            SACAllownce = Convert.ToDecimal(dr["SACAllownce"] ?? 0),
                            HouseRentAllownce = Convert.ToDecimal(dr["HouseRentAllownce"] ?? 0),
                            MedicalAllownce = Convert.ToDecimal(dr["MedicalAllownce"] ?? 0),
                            DarenessAllownce = Convert.ToDecimal(dr["DarenessAllownce"] ?? 0),
                            TravelAllownce = Convert.ToDecimal(dr["TravelAllownce"] ?? 0),
                            RationAllownce = Convert.ToDecimal(dr["RationAllownce"] ?? 0),
                            AdditionslAllownce = Convert.ToDecimal(dr["AdditionslAllownce"] ?? 0),
                            Pay = Convert.ToDecimal(dr["Pay"] ?? 0),
                            TempBasicPay = Convert.ToDecimal(dr["TempBasicPay"] ?? 0),
                            GrossPay = Convert.ToDecimal(dr["GrossPay"] ?? 0),
                            NetPay = NPay,
                            EmployeeCPShare = Convert.ToDecimal(dr["EmployeeCPShare"] ?? 0),
                            EmployerCPShare = Convert.ToDecimal(dr["EmployerCPShare"] ?? 0),
                            TotalLeavAddAmt = Convert.ToDecimal(dr["TotalLeavAddAmt"] ?? 0),
                            TotelLeavDedAmt = Convert.ToDecimal(dr["TotelLeavDedAmt"] ?? 0),
                            LeavesTaken = Convert.ToDecimal(dr["LeavesTaken"] ?? 0),
                            Insurance1PercentAmt = Convert.ToDecimal(dr["Insurance1PercentAmt"] ?? 0),
                            CPFDeduction = Convert.ToDecimal(dr["CPFDeduction"] ?? 0),
                            TotalAllownce = Convert.ToDecimal(dr["TotalAllownce"] ?? 0),
                            LoanDeduction = Convert.ToDecimal(dr["LoanDeduction"] ?? 0),
                            SecurityDeduction = Convert.ToDecimal(dr["SecurityDeduction"] ?? 0),
                            TotalDeduction = Convert.ToDecimal(dr["TotalDeduction"] ?? 0),
                            PenaltyDeduction = Convert.ToDecimal(dr["PenaltyDeduction"] ?? 0)
                        };

                        SDL.Add(ss);
                    }
                }

                if (SDL.Count > 0)
                {
                    rp.Status = 1;
                    rp.Message = "Data Found";
                    rp.ResponseData = SDL;
                }
                #endregion

                return rp;
            }
            catch (Exception ex)
            {
                rp.IsSuccess = false;
                rp.Status = -1;
                rp.Message = "Error: " + ex.ToString();
                return rp;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salary"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SalaryReleaseOnDepartments(SalaryModel salary, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Message = "Salary not released!", Status = 0 };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@YearIn", salary.Year),
            new SqlParameter("@MonthIn", salary.Month),
            new SqlParameter("@Days", salary.Days),
            new SqlParameter("@DateIn", salary.SalaryDate),
            new SqlParameter("@FYear", salary.FYear),
            new SqlParameter("@FMonth", salary.Month),
            new SqlParameter("@UN", salary.UserName),
            new SqlParameter("@DepartsIDS", salary.DepartmentIDS),
            new SqlParameter("@DepartmentNames", salary.SubDepartmentName)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getsalDepartmentsAPI", sqlParams);
                #endregion

                #region Process Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    int val = int.Parse(dr["val"].ToString());
                    if (val > 0)
                    {
                        response.Message = $"Salary of ( {val} ) employees released";
                        response.Status = 1;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salary"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SalaryReleaseOnEmployeeCode(SalaryModel salary, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Message = "Salary not released!", Status = 0 };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@YearIn", salary.Year),
            new SqlParameter("@MonthIn", salary.Month),
            new SqlParameter("@Days", salary.Days),
            new SqlParameter("@DateIn", salary.SalaryDate),
            new SqlParameter("@FYear", salary.FYear),
            new SqlParameter("@FMonth", salary.Month),
            new SqlParameter("@UN", salary.UserName),
            new SqlParameter("@EmCode", salary.EmployeeCode)
        };
                #endregion

                #region Execute Dataset (As per your pattern)
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "getsalECodeAPI", sqlParams);
                #endregion

                #region Process Result
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int result = Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
                    if (result > 0)
                    {
                        response.Status = 1;
                        response.Message = $"Salary Released for Month: {salary.Month}";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Exception Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSalaryDetails(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Salary not updated!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                string cpStatus = Convert.ToBoolean(emp.CPFundStatus) ? "True" : "False";
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@EDID", emp.EDID ),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@DARate", validationBLL.IsDecimalNotNull(emp.DARate.ToString(), 0)),
            new SqlParameter("@DarenessAllownce", validationBLL.IsDecimalNotNull(emp.DarenessAllownce.ToString(), 0)),
            new SqlParameter("@SACAllownce", emp.SACAllownce),
            new SqlParameter("@MedicalAllownce", emp.MedicalAllownce),
            new SqlParameter("@AdditionslAllownce", emp.AdditionslAllownce),
            new SqlParameter("@TravelAllownce", emp.TravelAllownce),
            new SqlParameter("@RationAllownce", emp.RationAllownce),
            new SqlParameter("@LoanDeduction", emp.LoanDeduction),
            new SqlParameter("@InsuranceInstallment", emp.InsuranceInstallment),
            new SqlParameter("@SecurityDeduction", emp.SecurityDeduction),
            new SqlParameter("@PenaltyDeduction", emp.PenaltyDeduction),
            new SqlParameter("@SpAllownceA", emp.SpAllownceA),
            new SqlParameter("@SpAllownceB", emp.SpAllownceB),
            new SqlParameter("@HouseRentAllownce", emp.HouseRentAllownce),
            new SqlParameter("@TransportDedAmt", emp.TransportDedAmt),
            new SqlParameter("@WelFund", emp.WelFund),
            new SqlParameter("@LeavesAvailable", emp.LeavesAvailable),
            new SqlParameter("@LeavesApplied", emp.LeavesApplied),
            new SqlParameter("@LeavesTaken", emp.LeavesTaken),
            new SqlParameter("@CPFRecoveryDedAmt", emp.CPFRecoveryDedAmt),
            new SqlParameter("@CPFLoanCollection", emp.CPFLoanCollection),
            new SqlParameter("@NPSRate", emp.NPSRate),
            new SqlParameter("@CPFPensionRate", emp.CPFPensionRate),
            new SqlParameter("@CPFundIntrest", emp.CPFundIntrest),
            new SqlParameter("@Insurance1PercentRate", emp.Insurance1PercentRate),
            new SqlParameter("@CPFundStatus", cpStatus),
            new SqlParameter("@EmpCode", emp.EmployeeCode),
            new SqlParameter("@SalaryStoped", emp.SalaryStoped),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn)
        };
                #endregion

                #region SQL Query
                string sqlQuery = @"UPDATE EmployeeDetail SET 
                                BasicPay = @BasicPay,
                                SACAllownce = @SACAllownce,
                                MedicalAllownce = @MedicalAllownce,
                                AdditionslAllownce = @AdditionslAllownce,
                                TravelAllownce = @TravelAllownce,
                                RationAllownce = @RationAllownce,
                                DARate = @DARate,
                                LoanDeduction = @LoanDeduction,
                                InsuranceInstallment = @InsuranceInstallment,
                                SecurityDeduction = @SecurityDeduction,
                                PenaltyDeduction = @PenaltyDeduction,
                                SpAllownceA = @SpAllownceA,
                                SpAllownceB = @SpAllownceB,
                                HouseRentAllownce = @HouseRentAllownce,
                                TransportDedAmt = @TransportDedAmt,
                                WelFund = @WelFund,
                                LeavesAvailable = @LeavesAvailable,
                                LeavesTaken = @LeavesTaken,
                                DarenessAllownce = @DarenessAllownce,
                                Insurance1PercentRate = @Insurance1PercentRate,
                                CPFundStatus = @CPFundStatus,
                                UpdatedBy = @UpdatedBy,
                                UpdatedOn = @UpdatedOn,
                                LeavesApplied = @LeavesApplied,
                                SalaryStoped = @SalaryStoped,
                                CPFRecoveryDedAmt = @CPFRecoveryDedAmt,
                                CPFLoanCollection = @CPFLoanCollection,
                                NPSRate = @NPSRate,
                                CPFPensionRate = @CPFPensionRate,
                                CPFundIntrest = @CPFundIntrest
                            WHERE EDID = @EDID";
                #endregion

                #region Execute Query
                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, sqlParams);
                #endregion

                #region Check Result
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Salary updated successfully";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="employeeDetails"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateSalaryDetailsOnField(List<EmployeeDetail> employeeDetails, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Salary not updated!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                int records = 0;

                #region Update Records
                foreach (var emp in employeeDetails)
                {
                    SqlParameter[] parameters =
                    {
                new SqlParameter("@EDID", emp.EDID),
                new SqlParameter("@UserName", emp.UserName),
                new SqlParameter("@UpdatedBy", emp.UpdatedBy),
                new SqlParameter("@UpdatedOn", emp.UpdatedOn),
                new SqlParameter("@FieldName", emp.FieldName),
                new SqlParameter("@FieldValue", emp.FieldValue)
            };

                    string query = $"UPDATE EmployeeDetail SET {emp.FieldName} = @FieldValue, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE EDID = @EDID";

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);
                    if (result > 0)
                    {
                        records += result;
                    }
                }
                #endregion

                #region Prepare Response
                if (records > 0)
                {
                    response.Status = 1;
                    response.Message = $"({records}) Records Updated Successfully.";
                }
                else
                {
                    response.Message = "No Records Updated!";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Response
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sal"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> DeleteSalaryOnEmployeeCode(string sal, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Salary not deleted!" };
            #endregion

            try
            {
                #region Parse Salary JSON
                SalaryModel salary = JsonConvert.DeserializeObject<SalaryModel>(sal)!;
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@EmployeeCode", salary.EmployeeCode),
            new SqlParameter("@EmployeeID", salary.EmployeeID),
            new SqlParameter("@Year", salary.Year),
            new SqlParameter("@Month", salary.Month),
            new SqlParameter("@SalaryDate", salary.SalaryDate),
            new SqlParameter("@SalaryID", salary.SalaryID)
        };
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "deleteSalAPI", parameters);
                #endregion

                #region Response Mapping
                if (result > 0)
                {
                    response.Status = 1;
                    response.Message = "Salary Deleted Successfully";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Response
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salaries"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> DeleteSalaryOnDepartments(List<SalaryModel> salaries, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Salary not deleted!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Delete Salaries
                int deletedCount = 0;
                foreach (var salary in salaries)
                {
                    long salaryId =await getMaxSalaryID(salary,clientId); // Your existing method

                    SqlParameter[] parameters =
                    {
                new SqlParameter("@EmployeeCode", salary.EmployeeCode),
                new SqlParameter("@EmployeeID", salary.EmployeeID),
                new SqlParameter("@Year", salary.Year),
                new SqlParameter("@Month", salary.Month),
                new SqlParameter("@SalaryDate", salary.SalaryDate),
                new SqlParameter("@SalaryID", salaryId)
            };

                    int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.StoredProcedure, "deleteSal", parameters);
                    deletedCount += result;
                }
                #endregion

                #region Response Mapping
                if (deletedCount > 0)
                {
                    response.Status = 1;
                    response.Message = $"({deletedCount}) Records deleted";
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Response
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salary"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<long> getMaxSalaryID(SalaryModel salary, string clientId)
        {
            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            #region SQL Parameters
            SqlParameter[] parameters =
            {
        new SqlParameter("@EmployeeCode", salary.EmployeeCode),
        new SqlParameter("@EmployeeID", salary.EmployeeID),
        new SqlParameter("@Year", salary.Year),
        new SqlParameter("@Month", salary.Month),
        new SqlParameter("@SalaryDate", salary.SalaryDate),
        new SqlParameter("@SalaryID", salary.SalaryID)
    };
            #endregion

            #region Execute Dataset Query
            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                connectionString,
                CommandType.Text,
                "SELECT ISNULL(MAX(SalaryID), 0) AS SalaryID FROM Salary WHERE EmployeeCode = @EmployeeCode AND Year = @Year AND Month = @Month",
                parameters
            );
            #endregion

            #region Extract Scalar Value
            long salaryId = 0;
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                salaryId = Convert.ToInt64(ds.Tables[0].Rows[0]["SalaryID"]);
            }
            return salaryId;
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salJson"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddNewLoan(string salJson, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Loan/Advance section transaction failed!"
            };
            #endregion

            try
            {
                #region Deserialize Salary Object
                var salary = JsonConvert.DeserializeObject<SalaryModel>(salJson);
                if (salary == null)
                {
                    response.Message = "Invalid Salary Data!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check Existing Loan
                SqlParameter[] sqlParam1 = {
            new SqlParameter("@LoanAmount", salary.LoanBalance),
            new SqlParameter("@EmployeeCode", salary.EmployeeCode),
            new SqlParameter("@EmployeeID", salary.EmployeeID),
            new SqlParameter("@Year", salary.Year),
            new SqlParameter("@Month", salary.Month),
            new SqlParameter("@DateOfLoanIssue", salary.SalaryDate)
        };

                string checkLoanSql = "SELECT COUNT(LoanID) FROM Loan WHERE EmployeeCode=@EmployeeCode AND LoanPaid=0";

                DataSet dsCheck = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkLoanSql, sqlParam1);
                int loanCount = 0;
                if (dsCheck.Tables.Count > 0 && dsCheck.Tables[0].Rows.Count > 0)
                {
                    loanCount = Convert.ToInt32(dsCheck.Tables[0].Rows[0][0]);
                }

                if (loanCount > 0)
                {
                    response.Message = "Loan already issued";
                    return response;
                }
                #endregion

                #region Issue New Loan
                string insertLoanSql = @"
            INSERT INTO Loan (LoanAmount, EmployeeID, EmployeeCode, DateOfLoanIssue, Year, Month, LoanPaid)
            VALUES (@LoanAmount, @EmployeeID, @EmployeeCode, @DateOfLoanIssue, @Year, @Month, 'false');
            SELECT MAX(LoanID) FROM Loan WHERE EmployeeCode=@EmployeeCode AND LoanPaid='false';
        ";

                DataSet dsLoan = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, insertLoanSql, sqlParam1);
                string loanId = dsLoan.Tables[0].Rows[0][0]?.ToString();

                if (!string.IsNullOrEmpty(loanId))
                {
                    SqlParameter[] param = {
                new SqlParameter("@LoanID", loanId),
                new SqlParameter("@EmployeeIDIN", salary.EmployeeID),
                new SqlParameter("@YearIN", salary.Year),
                new SqlParameter("@MonthIN", salary.Month),
                new SqlParameter("@DateOfInstallment", salary.SalaryDate),
            };

                    string insertInstallmentSql = @"
                INSERT INTO LoanCollection (LoanInstallment, LoanID, EmployeeID, Year, Month, DateOfInstallment, Type)
                VALUES (0, @LoanID, @EmployeeIDIN, @YearIN, @MonthIN, @DateOfInstallment, 1);
            ";

                    int rt = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertInstallmentSql, param);
                    if (rt > 0)
                    {
                        response.Status = 1;
                        response.Message = "Loan issued successfully";
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }


        public Task<ResponseModel> GetEmployeeSalaryToEdit(string eCode, string clientId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="salary"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetDemoSalaryOnDepartments(SalaryModel salary, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Salary not released!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@YearIn", salary.Year),
            new SqlParameter("@MonthIn", salary.Month),
            new SqlParameter("@Days", salary.Days),
            new SqlParameter("@DateIn", salary.SalaryDate),
            new SqlParameter("@FYear", salary.FYear),
            new SqlParameter("@FMonth", salary.Month),
            new SqlParameter("@UN", salary.UserName),
            new SqlParameter("@DepartsIDS", salary.DepartmentIDS),
            new SqlParameter("@DepartmentNames", salary.SubDepartmentName)
        };
                #endregion

                #region Execute Stored Procedure
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, "SalaryStatementAPI", sqlParams);
                #endregion

                #region Process Dataset
                List<SalaryModel> salaries = new List<SalaryModel>();

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SalaryModel s = new SalaryModel
                        {
                            EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            Month = dr["Month"].ToString(),
                            Year = dr["Year"].ToString(),
                            BasicPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["BasicPay"]?.ToString()) ? "0" : dr["BasicPay"]),
                            DARate = Convert.ToDecimal(string.IsNullOrEmpty(dr["DARate"]?.ToString()) ? "0" : dr["DARate"]),
                            SACAllownce = Convert.ToDecimal(dr["SACAllownce"]),
                            HouseRentAllownce = Convert.ToDecimal(dr["HouseRentAllownce"]),
                            MedicalAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["MedicalAllownce"]?.ToString()) ? "0" : dr["MedicalAllownce"]),
                            DarenessAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["DarenessAllownce"]?.ToString()) ? "0" : dr["DarenessAllownce"]),
                            TravelAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["TravelAllownce"]?.ToString()) ? "0" : dr["TravelAllownce"]),
                            RationAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["RationAllownce"]?.ToString()) ? "0" : dr["RationAllownce"]),
                            AdditionslAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["AdditionslAllownce"]?.ToString()) ? "0" : dr["AdditionslAllownce"]),
                            Pay = Convert.ToDecimal(string.IsNullOrEmpty(dr["Pay"]?.ToString()) ? "0" : dr["Pay"]),
                            TempBasicPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["TempBasicPay"]?.ToString()) ? "0" : dr["TempBasicPay"]),
                            GrossPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["GrossPay"]?.ToString()) ? "0" : dr["GrossPay"]),
                            NetPay = Convert.ToDecimal(string.IsNullOrEmpty(dr["NetPay"]?.ToString()) ? "0" : dr["NetPay"]),
                            EmployeeCPShare = Convert.ToDecimal(string.IsNullOrEmpty(dr["EmployeeCPShare"]?.ToString()) ? "0" : dr["EmployeeCPShare"]),
                            EmployerCPShare = Convert.ToDecimal(string.IsNullOrEmpty(dr["EmployerCPShare"]?.ToString()) ? "0" : dr["EmployerCPShare"]),
                            TotalLeavAddAmt = Convert.ToDecimal(string.IsNullOrEmpty(dr["TotalLeavAddAmt"]?.ToString()) ? "0" : dr["TotalLeavAddAmt"]),
                            TotelLeavDedAmt = Convert.ToDecimal(string.IsNullOrEmpty(dr["TotelLeavDedAmt"]?.ToString()) ? "0" : dr["TotelLeavDedAmt"]),
                            Insurance1PercentAmt = Convert.ToDecimal(string.IsNullOrEmpty(dr["Insurance1PercentAmt"]?.ToString()) ? "0" : dr["Insurance1PercentAmt"]),
                            CPFDeduction = Convert.ToDecimal(string.IsNullOrEmpty(dr["CPFDeduction"]?.ToString()) ? "0" : dr["CPFDeduction"]),
                            TotalAllownce = Convert.ToDecimal(string.IsNullOrEmpty(dr["TotalAllownce"]?.ToString()) ? "0" : dr["TotalAllownce"]),
                            TotalDeduction = Convert.ToDecimal(string.IsNullOrEmpty(dr["TotalDeduction"]?.ToString()) ? "0" : dr["TotalDeduction"])
                        };
                        salaries.Add(s);
                    }
                }
                #endregion

                #region Prepare Response
                if (salaries.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = salaries;
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }

    }
}

