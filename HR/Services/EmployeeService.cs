using HR.Repository;
using HR.Repository.SQL;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HR.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IConfiguration _configuration;
        public EmployeeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> AddNewEmployee(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Added!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@SessionOfJoin", ""),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@LastCode", emp.EmployeeCode),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", emp.BankAccount),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", emp.RouteID),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", emp.IsBEd),
            new SqlParameter("@QIDFK", emp.QidFk),
            new SqlParameter("@otherqual", emp.Qualification),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", emp.m_status),
            new SqlParameter("@BasicPay", emp.BasicPay),
            new SqlParameter("@ESIDFK", emp.ESIDFK),
            new SqlParameter("@IsTeacher", emp.IsTeacher),
            new SqlParameter("@PANCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@BankName", emp.BankName),
            new SqlParameter("@BankBranch", emp.BankBranch),
            new SqlParameter("@IFSCCode", emp.IFSCCode),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@NPSRate", emp.NPSRate),
            new SqlParameter("@NPSNo", emp.NPSNo)
        };
                #endregion

                #region Execute Stored Procedure
                int result = await SQLHelperCore.ExecuteNonQueryAsync(
                    connectionString, CommandType.StoredProcedure, "[AddEmpDetailsnewAPI]", sqlParam
                );
                #endregion

                #region Set Response
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Employee Added Successfully";
                }
                else
                {
                    response.Message = "Employee Code Already Exists!";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "AddNewEmployee", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> UpdateEmployee(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EDID", emp.EDID),
            new SqlParameter("@EmployeeCode", emp.EmployeeCode),
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("@Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@DateOfPermanent", emp.DateOfPermanent),
            new SqlParameter("@SessionOfJoin", emp.SessionOfJoin),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", validationBLL.IsBoolNotNull(emp.BankAccount.ToString(), false)),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", validationBLL.IsNumber(emp.RouteID.ToString(), 0)),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", validationBLL.IsNumber(emp.IsBEd.ToString(), 0)),
            new SqlParameter("@QIDFK", validationBLL.IsNumber(emp.QidFk.ToString(), 0)),
            new SqlParameter("@otherqual", emp.OtherQual),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", validationBLL.IsNumber(emp.m_status.ToString(), 0)),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@ESIDFK", validationBLL.IsNumber(emp.ESIDFK.ToString(), 0)),
            new SqlParameter("@IsTeacher", validationBLL.IsNumber(emp.IsTeacher.ToString(), 0)),
            new SqlParameter("@PanCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@NPSNo", emp.NPSNo),
            new SqlParameter("@BankBranch", emp.BankBranch),
            new SqlParameter("@IFSCCode", emp.IFSCCode),
            new SqlParameter("@BankName", emp.BankName),
            new SqlParameter("@NPSRate", emp.NPSRate)
        };
                #endregion

                #region Prepare Queries
                string sqlEcode = @"
            UPDATE Employees 
            SET EmployeeName = @EmployeeName, 
                DOB = @DOB, 
                Gender = @Gender, 
                Address = @Address, 
                City = @City, 
                PinNo = @PinNo, 
                Qualification = @Qualification, 
                DOJ = @DOJ, 
                SessionOfJoin = @SessionOfJoin, 
                FatherName = @FatherName, 
                SpouseName = @SpouseName, 
                E_Mail = @E_Mail, 
                AdhaarNo = @AdhaarNo, 
                m_status = @m_status, 
                PanCard = @PanCard, 
                NPSNo = @NPSNo, 
                OtherQual = @otherqual, 
                PhoneNo = @PhoneNo, 
                DateOfPermanent = @DateOfPermanent 
            WHERE EmployeeCode = @EmployeeCode";

                string sqlEDID = @"
            UPDATE EmployeeDetail 
            SET Current_Session = @Current_Session, 
                Status = @Status, 
                DesignationID = @DesignationID, 
                SubDepartmentID = @SubDepartmentID, 
                Scale = @Scale, 
                BankAccount = @BankAccount, 
                BankAccountNo = @BankAccountNo, 
                CPFundAccountNo = @CPFundAccountNo, 
                InsurancePolicyNo = @InsurancePolicyNo, 
                IsBed = @IsBed, 
                QIDFK = @QIDFK, 
                ESIDFK = @ESIDFK, 
                IsTeacher = @IsTeacher, 
                UpdatedBy = @UpdatedBy, 
                UpdatedOn = @UpdatedOn, 
                Remarks = @Remarks, 
                BankBranch = @BankBranch, 
                IFSCCode = @IFSCCode, 
                BankName = @BankName, 
                NPSRate = @NPSRate 
            WHERE EDID = @EDID";
                #endregion

                #region Execute Queries
                int result = 0;
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEcode, sqlParam);
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEDID, sqlParam);
                #endregion

                #region Set Response
                if (result > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Employee Updated Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "UpdateEmployee", ex.ToString());
                return response;
                #endregion
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateMultipleEmployee(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EDID", emp.EDID),
            new SqlParameter("@EmployeeCode", emp.EmployeeCode),
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("@Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@SessionOfJoin", emp.SessionOfJoin),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", validationBLL.IsBoolNotNull(emp.BankAccount.ToString(), false)),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", validationBLL.IsNumber(emp.RouteID.ToString(), 0)),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", validationBLL.IsNumber(emp.IsBEd.ToString(), 0)),
            new SqlParameter("@QIDFK", validationBLL.IsNumber(emp.QidFk.ToString(), 0)),
            new SqlParameter("@otherqual", ""),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", validationBLL.IsNumber(emp.m_status.ToString(), 0)),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@ESIDFK", validationBLL.IsNumber(emp.ESIDFK.ToString(), 0)),
            new SqlParameter("@IsTeacher", validationBLL.IsNumber(emp.IsTeacher.ToString(), 0)),
            new SqlParameter("@PanCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@FieldName", emp.FieldName),
            new SqlParameter("@FieldValue", emp.FieldValue)
        };
                #endregion

                #region Prepare Query (Dynamic Field Update)
                string sqlEcode = $"UPDATE Employees SET {emp.FieldName} = @FieldValue WHERE EmployeeCode = @EmployeeCode";
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEcode, sqlParam);
                #endregion

                #region Handle Result
                if (result > 0)
                {
                    response.Status = 1;
                    response.Message = "Employee Details Updated";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "UpdateMultipleEmployee", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseModel> UpdateEmployeeMonthlyAttendance(EmployeeDetail value, string clientId)
        {
            #region UpdateEmployeeMonthlyAttendance

            ResponseModel response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Attendance Not Updated!"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                SqlParameter[] sqlParam = {
            new SqlParameter("@EDID", value.EDID),
            new SqlParameter("@EcodeFK", value.EmployeeCode),
            new SqlParameter("@Syear", value.Year),
            new SqlParameter("@Mname", value.Month),
            new SqlParameter("@Mid", value.SessionID),
            new SqlParameter("@Lavlb", value.LeavesAvailable),
            new SqlParameter("@Ltaken", value.LeavesTaken),
            new SqlParameter("@UserName", value.UserName),
            new SqlParameter("@Upadatedon", value.UpdatedOn)
        };

                int rt = 0;

                string sql = "INSERT INTO LeaveMonth([EcodeFK],[Mname],[Mid],[Syear],[Username],[Upadatedby],[Upadatedon],[Ltaken],[Lavlb]) " +
                             "VALUES (@EcodeFK,@Mname,@Mid,@Syear,@Username,@Username,@Upadatedon,@Ltaken,@Lavlb)";

                if (value.EDID != 0)
                {
                    sql = "UPDATE LeaveMonth SET Ltaken=@Ltaken, Lavlb=@Lavlb, Upadatedby=@Username, Upadatedon=@Upadatedon WHERE Lmid=@EDID";
                }

                rt = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sql, sqlParam); // ✅ await

                if (rt > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Employee Attendance Updated Successfully";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                return response;
            }

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> WithdrawEmployee(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Withdrawn"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EDID", emp.EDID),
            new SqlParameter("@EmployeeCode", emp.EmployeeCode),
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("@Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@DateOfPermanent", emp.DateOfPermanent),
            new SqlParameter("@SessionOfJoin", emp.SessionOfJoin),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", validationBLL.IsBoolNotNull(emp.BankAccount.ToString(), false)),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", validationBLL.IsNumber(emp.RouteID.ToString(), 0)),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", validationBLL.IsNumber(emp.IsBEd.ToString(), 0)),
            new SqlParameter("@QIDFK", validationBLL.IsNumber(emp.QidFk.ToString(), 0)),
            new SqlParameter("@otherqual", emp.OtherQual),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", validationBLL.IsNumber(emp.m_status.ToString(), 0)),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@ESIDFK", validationBLL.IsNumber(emp.ESIDFK.ToString(), 0)),
            new SqlParameter("@IsTeacher", validationBLL.IsNumber(emp.IsTeacher.ToString(), 0)),
            new SqlParameter("@PanCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@DateOfWithdraw", emp.DateOfWithdraw),
            new SqlParameter("@WithdrawRemarks", emp.WithdrawRemarks),
            new SqlParameter("@NPSNo", emp.NPSNo),
            new SqlParameter("@BankBranch", emp.BankBranch),
            new SqlParameter("@IFSCCode", emp.IFSCCode),
            new SqlParameter("@BankName", emp.BankName),
            new SqlParameter("@NPSRate  ", emp.NPSRate)
        };
                #endregion

                #region Prepare Queries
                string sqlEcode = "UPDATE Employees SET Withdrawn='True', DOW=@DateOfWithdraw WHERE EmployeeCode=@EmployeeCode";
                string sqlEDID = "UPDATE EmployeeDetail SET UpdatedBy=@UpdatedBy, UpdatedOn=@UpdatedOn, Remarks=@Remarks, " +
                                 "WithdrawnEmp='True', WithdrawRemarks=@WithdrawRemarks, DateOfWithdraw=@DateOfWithdraw WHERE EDID=@EDID";
                #endregion

                #region Execute Queries
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEcode, sqlParam);
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEDID, sqlParam);
                #endregion

                #region Handle Result
                if (result > 0)
                {
                    response.Status = 0;
                    response.Message = "Employee Withdrawn partially!";
                    if (result == 2)
                    {
                        response.Status = 1;
                        response.Message = "Employee Withdrawn Successfully!";
                    }
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "WithdrawEmployee", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> RejoinEmployee(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Rejoined!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EDID", emp.EDID),
            new SqlParameter("@EmployeeID", emp.EmployeeID),
            new SqlParameter("@EmployeeCode", emp.EmployeeCode),
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("@Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@DateOfPermanent", emp.DateOfPermanent),
            new SqlParameter("@SessionOfJoin", emp.SessionOfJoin),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", validationBLL.IsBoolNotNull(emp.BankAccount.ToString(), false)),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", validationBLL.IsNumber(emp.RouteID.ToString(), 0)),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", validationBLL.IsNumber(emp.IsBEd.ToString(), 0)),
            new SqlParameter("@QIDFK", validationBLL.IsNumber(emp.QidFk.ToString(), 0)),
            new SqlParameter("@otherqual", emp.OtherQual),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", validationBLL.IsNumber(emp.m_status.ToString(), 0)),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@ESIDFK", validationBLL.IsNumber(emp.ESIDFK.ToString(), 0)),
            new SqlParameter("@IsTeacher", validationBLL.IsNumber(emp.IsTeacher.ToString(), 0)),
            new SqlParameter("@PanCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@NPSNo", emp.NPSNo),
            new SqlParameter("@BankBranch", emp.BankBranch),
            new SqlParameter("@IFSCCode", emp.IFSCCode),
            new SqlParameter("@BankName", emp.BankName),
            new SqlParameter("@NPSRate  ", emp.NPSRate)
        };
                #endregion

                #region Prepare Queries
                string sqlEcode = "UPDATE Employees SET EmployeeName=@EmployeeName, DOB=@DOB, Gender=@Gender, Address=@Address, City=@City, " +
                                  "PinNo=@PinNo, Qualification=@Qualification, DOJ=@DOJ, FatherName=@FatherName, SpouseName=@SpouseName, " +
                                  "E_Mail=@E_Mail, AdhaarNo=@AdhaarNo, m_status=@m_status, PanCard=@PanCard, NPSNo=@NPSNo, OtherQual=@otherqual, " +
                                  "PhoneNo=@PhoneNo, DateOfPermanent=@DateOfPermanent, Withdrawn='false' WHERE EmployeeCode=@EmployeeCode";

                string sqlEDID = "UPDATE EmployeeDetail SET Current_Session=@Current_Session, Status=@Status, DesignationID=@DesignationID, " +
                                 "SubDepartmentID=@SubDepartmentID, Scale=@Scale, BankAccount=@BankAccount, BankAccountNo=@BankAccountNo, " +
                                 "CPFundAccountNo=@CPFundAccountNo, InsurancePolicyNo=@InsurancePolicyNo, IsBed=@IsBed, QIDFK=@QIDFK, ESIDFK=@ESIDFK, " +
                                 "IsTeacher=@IsTeacher, UpdatedBy=@UpdatedBy, UpdatedOn=@UpdatedOn, Remarks=@Remarks, BankBranch=@BankBranch, " +
                                 "IFSCCode=@IFSCCode, BankName=@BankName, NPSRate=@NPSRate, WithdrawnEmp='false' WHERE EDID=@EDID";

                // Special Case: Insert New Row if Year != FYear
                if (emp.Year != emp.FYear)
                {
                    sqlEDID = "INSERT INTO EmployeeDetail (EmployeeID, Current_Session, SessionID, Status, DesignationID, SubDepartmentID, Scale, " +
                              "BankAccount, BankAccountNo, CPFundAccountNo, InsurancePolicyNo, Grade, RouteID, Remarks, LeavesAvailable, LeavesTaken, " +
                              "CPFundCollection, SecurityFundcollection, LoanBalance, DarenessAllownce, SACAllownce, HouseRentAllownce, MedicalAllownce, " +
                              "AdditionslAllownce, TravelAllownce, Increment, InsuranceAmount, RationAllownce, DARate, EmployeeCPShare, EmployerCPShare, " +
                              "LoanDeduction, InsuranceInstallment, CPFundIntrest, PenaltyDeduction, Insurance1PercentRate, SpAllownceA, SpAllownceB, " +
                              "CPFRecoveryDedAmt, CPFLoanTaken, CPFLoanCollection, Insurance1PercentAmt, CPFDeduction, SecurityDeduction, LeavesApplied, " +
                              "Year, SalaryStoped, ExcessLeaves, WorkingDays, TransportDedAmt, CPFundStatus, FYear, WithdrawnEmp, Qidfk, isbed, BasicPay, " +
                              "ESIDFK, IsTeacher, UpdatedBy, BankName, BankBranch, IFSCCode, UpdatedOn, NPSRate, UserName) " +
                              "VALUES (@EmployeeID, @Current_Session, @SessionID, @Status, @DesignationID, @SubDepartmentID, @Scale, @BankAccount, " +
                              "@BankAccountNo, @CPFundAccountNo, @InsurancePolicyNo, @Grade, @RouteID, @Remarks, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, " +
                              "0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'True', @FYear, 'False', 0, 0, 0, 'False', 0, 0, @QIDFK, @IsBed, 0, @ESIDFK, " +
                              "@IsTeacher, @UpdatedBy, @BankName, @BankBranch, @IFSCCode, @UpdatedOn, @NPSRate, @UserName)";
                }
                #endregion

                #region Execute Queries
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEcode, sqlParam);
                result += await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlEDID, sqlParam);
                #endregion

                #region Handle Result
                if (result > 0)
                {
                    response.Status = 0;
                    response.Message = "Employee Rejoined Partially!";
                    if (result == 2)
                    {
                        response.Status = 1;
                        response.Message = "Employee Rejoined Successfully!";
                    }
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "RejoinEmployee", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateEmployeeDetailField(EmployeeDetail emp, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Employee Not Updated!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare SQL Parameters
                SqlParameter[] sqlParam =
                {
            new SqlParameter("@EDID", emp.EDID),
            new SqlParameter("@EmployeeCode", emp.EmployeeCode),
            new SqlParameter("@EmployeeName", emp.EmployeeName),
            new SqlParameter("@DOB", emp.DOB),
            new SqlParameter("@Gender", emp.Gender),
            new SqlParameter("@Address", emp.Address),
            new SqlParameter("@City", emp.City),
            new SqlParameter("@PinNo", emp.PinNo),
            new SqlParameter("@PhoneNo", emp.PhoneNo),
            new SqlParameter("@Qualification", emp.Qualification),
            new SqlParameter("@DOJ", emp.DOJ),
            new SqlParameter("@SessionOfJoin", emp.SessionOfJoin),
            new SqlParameter("@PhotoPath", emp.PhotoPath),
            new SqlParameter("@FatherName", emp.FatherName),
            new SqlParameter("@Current_Session", emp.Current_Session),
            new SqlParameter("@SessionID", ""),
            new SqlParameter("@Status", emp.Status),
            new SqlParameter("@DesignationID", emp.DesignationID),
            new SqlParameter("@SubDepartmentID", emp.SubDepartmentID),
            new SqlParameter("@Scale", emp.Scale),
            new SqlParameter("@BankAccount", validationBLL.IsBoolNotNull(emp.BankAccount.ToString(), false)),
            new SqlParameter("@BankAccountNo", emp.BankAccountNo),
            new SqlParameter("@CPFundAccountNo", emp.CPFundAccountNo),
            new SqlParameter("@InsurancePolicyNo", emp.InsurancePolicyNo),
            new SqlParameter("@Grade", emp.Grade),
            new SqlParameter("@RouteID", validationBLL.IsNumber(emp.RouteID.ToString(), 0)),
            new SqlParameter("@Remarks", emp.Remarks),
            new SqlParameter("@Year", emp.Year),
            new SqlParameter("@FYear", emp.FYear),
            new SqlParameter("@UserName", emp.UserName),
            new SqlParameter("@IsBed", validationBLL.IsNumber(emp.IsBEd.ToString(), 0)),
            new SqlParameter("@QIDFK", validationBLL.IsNumber(emp.QidFk.ToString(), 0)),
            new SqlParameter("@otherqual", ""),
            new SqlParameter("@SpouseName", emp.SpouseName),
            new SqlParameter("@E_Mail", emp.E_Mail),
            new SqlParameter("@AdhaarNo", emp.AdhaarNo),
            new SqlParameter("@m_status", validationBLL.IsNumber(emp.m_status.ToString(), 0)),
            new SqlParameter("@BasicPay", validationBLL.IsDecimalNotNull(emp.BasicPay.ToString(), 0)),
            new SqlParameter("@ESIDFK", validationBLL.IsNumber(emp.ESIDFK.ToString(), 0)),
            new SqlParameter("@IsTeacher", validationBLL.IsNumber(emp.IsTeacher.ToString(), 0)),
            new SqlParameter("@PanCard", emp.PANCard),
            new SqlParameter("@UpdatedBy", emp.UpdatedBy),
            new SqlParameter("@UpdatedOn", emp.UpdatedOn),
            new SqlParameter("@FieldName", emp.FieldName),
            new SqlParameter("@FieldValue", emp.FieldValue)
        };
                #endregion

                #region Prepare Query
                string sqlQuery = $"UPDATE EmployeeDetail SET {emp.FieldName} = @FieldValue WHERE EDID = @EDID";
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, sqlQuery, sqlParam);
                #endregion

                #region Handle Result
                if (result > 0)
                {
                    response.Status = 1;
                    response.Message = "Employee Details Updated";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "UpdateEmployeeDetailField", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="empCode"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeeByCode(string empCode, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found, Please check Emp Code!"
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                string query = @"
            SELECT TOP 1 EmployeeDetail.EDID, *, 'NA' AS FieldName 
            FROM Employees  
            INNER JOIN EmployeeDetail 
            ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
            WHERE EmployeeCode = @Code  
            ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";

                SqlParameter[] parameters =
                {
            new SqlParameter("@Code", empCode)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    query,
                    parameters);
                #endregion

                #region Map Data
                List<EmployeeDetail> empList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (empList.Count > 0)
                {
                    var emp = empList.FirstOrDefault();
                    if (emp.EDID == 0)
                    {
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Error: " + emp.Status;
                        response.ResponseData = null;
                        return response;
                    }

                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = empList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeeByCode", ex.ToString());
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="year"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllEmployeesByYear(string year, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Initialize List
            List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
            #endregion

            try
            {
                #region Validate Year
                if (year.Length != 4 && year.Length != 7) // If invalid value
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid Year or Fyear";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                SqlParameter param = new SqlParameter("@Year", year);
                string query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                  FROM Employees  
                  INNER JOIN EmployeeDetail 
                  ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                  WHERE Year = @Year 
                  ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";

                if (year.Length == 7) // If Search On FYear
                {
                    query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
               FROM Employees  
               INNER JOIN EmployeeDetail 
               ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
               WHERE FYear = @Year 
               ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Data
                employeeList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    EmployeeDetail firstEmp = employeeList.FirstOrDefault();
                    if (firstEmp.EDID == 0)
                    {
                        // Incase Cached error in EmpData Func
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Error: " + firstEmp.Status;
                        response.ResponseData = null;
                        return response;
                    }

                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetAllEmployeesByYear", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesBySubDept(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Initialize List
            List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
            #endregion

            try
            {
                #region Validate Parameters
                string[] data = param.Split(',');
                if (data.Length != 2)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Some parameter missing!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@DID", data[0]), // SubDepartmentID
            new SqlParameter("@Year", data[1]) // Year or FYear
        };

                string query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                  FROM Employees  
                  INNER JOIN EmployeeDetail 
                  ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                  WHERE SubDepartmentID = @DID AND Year = @Year 
                  AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                  ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";

                if (data[1].Length == 7) // If Search On FYear
                {
                    query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                FROM Employees  
                INNER JOIN EmployeeDetail 
                ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                WHERE SubDepartmentID = @DID AND FYear = @Year 
                AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Data
                employeeList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    EmployeeDetail firstEmp = employeeList.FirstOrDefault();
                    if (firstEmp.EDID == 0)
                    {
                        // In case cached error in EmpData Func
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Error: " + firstEmp.Status;
                        response.ResponseData = null;
                        return response;
                    }

                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesBySubDept", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByDesignation(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Initialize List
            List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
            #endregion

            try
            {
                #region Validate Parameters
                string[] data = param.Split(',');
                if (data.Length != 2)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Some parameter missing!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@DID", data[0]), // DesignationID
            new SqlParameter("@Year", data[1]) // Year or FYear
        };

                string query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                  FROM Employees  
                  INNER JOIN EmployeeDetail 
                  ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                  WHERE DesignationID = @DID AND Year = @Year 
                  AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                  ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                // change logic use mistakenly searching by SubDepartmentID in designation method.fixed that and kept DesignationID in both cases, 

                if (data[1].Length == 7) // If Search On FYear
                {
                    query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName  
                FROM Employees  
                INNER JOIN EmployeeDetail 
                ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                WHERE DesignationID = @DID AND FYear = @Year 
                AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Data
                employeeList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    EmployeeDetail firstEmp = employeeList.FirstOrDefault();
                    if (firstEmp.EDID == 0)
                    {
                        // In case cached error in EmpData Func
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Error: " + firstEmp.Status;
                        response.ResponseData = null;
                        return response;
                    }

                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesByDesignation", ex.ToString());
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByStatus(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Variable Declaration
            List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
            #endregion

            try
            {
                #region Validate Input
                string[] data = param.Split(',');
                if (data.Length != 2)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Some parameter missing!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@Status", data[0]),  // Status
            new SqlParameter("@Year", data[1])     // Year or FYear
        };

                string query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                         FROM Employees  
                         INNER JOIN EmployeeDetail 
                         ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                         WHERE Status = @Status 
                         AND Year = @Year 
                         AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                         ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";

                if (data[1].Length == 7)  // If Search On FYear
                {
                    query = @"SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName 
                      FROM Employees  
                      INNER JOIN EmployeeDetail 
                      ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                      WHERE Status = @Status 
                      AND FYear = @Year 
                      AND (Withdrawn = 'false' OR WithdrawnEmp = 'false') 
                      ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Data
                employeeList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    var firstEmp = employeeList.FirstOrDefault();
                    if (firstEmp.EDID == 0)
                    {
                        response.IsSuccess = false;
                        response.Status = -1;
                        response.Message = "Error: " + firstEmp.Status;
                        response.ResponseData = null;
                        return response;
                    }

                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesByStatus", ex.ToString());
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByName(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found, Please check Emp Name!"
            };
            #endregion

            List<EmployeeDetail> eL = new List<EmployeeDetail>();

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(param) || param.Length < 3)
                    return response;
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Query
                SqlParameter nameParam = new SqlParameter("@name", param);
                string qr = @"select EmployeeDetail.EDID,
                             (select convert(varchar, DOB, 105)) as DOBDate,
                             (select convert(varchar, DOJ, 105)) as DOJDate,
                             (select convert(varchar, DOW, 105)) as DOWDate,
                             (select convert(varchar, DateOfWithdraw, 105)) as DOWithDate,
                             (select convert(varchar, UpdatedOn, 105)) as UpdatedDate,
                             (select convert(varchar, DateofIncrement, 105)) as DOIncDate,
                             (select convert(varchar, DateOfPermanent, 105)) as DOPDate, *
                      from Employees
                      inner join EmployeeDetail on Employees.EmployeeID = EmployeeDetail.EmployeeID
                      where EmployeeName like '%' + @name + '%'
                      order by Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID desc";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, qr, nameParam);
                #endregion

                #region Map Data
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var exists = eL.Any(emp => emp.EmployeeCode == Int64.Parse(dr["EmployeeCode"].ToString()));
                        if (!exists)
                        {
                            var designation = await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId);
                            var department = await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId);

                            eL.Add(new EmployeeDetail()
                            {
                                EDID = Convert.ToInt64(dr["EDID"].ToString()),
                                EmployeeCode = Int64.Parse(dr["EmployeeCode"].ToString()),
                                EmployeeID = Int64.Parse(dr["EmployeeID"].ToString()),
                                EmployeeName = dr["EmployeeName"].ToString(),
                                FatherName = dr["FatherName"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Address = dr["Address"].ToString(),
                                E_Mail = dr["E_Mail"].ToString(),
                                City = dr["City"].ToString(),
                                DesignationName = designation.Designation,
                                DesignationID = Int64.Parse(dr["DesignationID"].ToString()),
                                DepartmentName = department.SubDepartmentName,
                                SubDepartmentID = Int64.Parse(dr["SubDepartmentID"].ToString()),
                                Status = dr["Status"].ToString(),
                                Grade = dr["Grade"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                AdhaarNo = dr["AdhaarNo"].ToString(),
                                PinNo = dr["PinNo"].ToString(),
                                BankAccountNo = dr["BankAccountNo"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                BankBranch = dr["BankBranch"].ToString(),
                                InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                                CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                                IFSCCode = dr["IFSCCode"].ToString(),
                                PhotoPath = dr["PhotoPath"].ToString(),
                                Qualification = dr["Qualification"].ToString(),
                                OtherQual = dr["OtherQual"].ToString(),
                                SpouseName = dr["SpouseName"].ToString(),
                                PANCard = dr["PANCard"].ToString(),
                                SessionOfJoin = dr["SessionOfJoin"].ToString(),
                                Current_Session = dr["Current_Session"].ToString(),
                                NPSNo = dr["NPSNo"].ToString(),

                                DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                                DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                                DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                                DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                                UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                                UpdatedBy = dr["UpdatedBy"].ToString(),
                                DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                                WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                                LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), 0).ToString(),
                                m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                                QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                                ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                                IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                                FYear = dr["FYear"].ToString(),
                                Year = dr["Year"].ToString(),
                                Scale = dr["Scale"].ToString(),
                                Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                                WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),
                                BasicPay = Convert.ToDecimal(dr["BasicPay"].ToString()),
                                DARate = Convert.ToDecimal(dr["DARate"].ToString()),
                                DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                                SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                                SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                                MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                                AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                                TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                                RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                                HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                                SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                                SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                                InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                                Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                                CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                                CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                                CPFundStatus = validationBLL.IsBoolNotNull(dr["CPFundStatus"].ToString(), false),
                                BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                                SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                                PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                                TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                                WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                                LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                                CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                                LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                                LeavesAvailable = validationBLL.IsDecimalNotNull(dr["LeavesAvailable"].ToString(), 0),
                                LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                ExcessLeaves = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0)
                            });
                        }
                    }
                }
                #endregion

                #region Set Response
                if (eL.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = eL;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByField(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Variable Declaration
            List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
            #endregion

            try
            {
                #region Validate Input
                string[] vals = param.Split(',');
                if (vals.Length != 4)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid Parameters: Year, FieldName, StatusDepartDesig, Type";
                    return response;
                }

                string year = vals[0];
                string fieldName = vals[1];
                string statusDepartDesig = vals[2];
                string type = vals[3];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query & Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@Year", year),
            new SqlParameter("@FieldName", fieldName),
            new SqlParameter("@StatusDepartDesig", statusDepartDesig)
        };

                string query = "";

                if (year.Length == 4 && type == "1") // Year & Status
                {
                    query = $@"SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, * 
                       FROM Employees 
                       INNER JOIN EmployeeDetail 
                       ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE Year = @Year 
                       AND (Withdrawn = 'False' OR WithdrawnEmp = 'False') 
                       AND Status = @StatusDepartDesig 
                       ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                else if (year.Length == 4 && type == "2") // Year & SubDepartmentID
                {
                    query = $@"SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, * 
                       FROM Employees 
                       INNER JOIN EmployeeDetail 
                       ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE Year = @Year 
                       AND (Withdrawn = 'False' OR WithdrawnEmp = 'False') 
                       AND SubDepartmentID = @StatusDepartDesig 
                       ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                else if (year.Length == 4 && type == "3") // Year & DesignationID
                {
                    query = $@"SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, * 
                       FROM Employees 
                       INNER JOIN EmployeeDetail 
                       ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE Year = @Year 
                       AND (Withdrawn = 'False' OR WithdrawnEmp = 'False') 
                       AND DesignationID = @StatusDepartDesig 
                       ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                else // FYear
                {
                    query = $@"SELECT EmployeeDetail.EDID, {fieldName} AS FieldName, * 
                       FROM Employees 
                       INNER JOIN EmployeeDetail 
                       ON Employees.EmployeeID = EmployeeDetail.EmployeeID 
                       WHERE FYear = @Year 
                       AND (Withdrawn = 'False' OR WithdrawnEmp = 'False') 
                       ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Data
                employeeList = await EmpData(ds, clientId);
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesByField", ex.ToString());
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByMobile(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found, Please check Phone No.!"
            };
            #endregion

            #region Validate Input
            if (string.IsNullOrEmpty(param) || param.Length < 3)
            {
                return response;
            }
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                SqlParameter eName = new SqlParameter("@name", param);
                string query = @"
            SELECT EmployeeDetail.EDID,
                   (SELECT CONVERT(VARCHAR, DOB, 105)) AS DOBDate,
                   (SELECT CONVERT(VARCHAR, DOJ, 105)) AS DOJDate,
                   (SELECT CONVERT(VARCHAR, DOW, 105)) AS DOWDate,
                   (SELECT CONVERT(VARCHAR, DateOfWithdraw, 105)) AS DOWithDate,
                   (SELECT CONVERT(VARCHAR, UpdatedOn, 105)) AS UpdatedDate,
                   (SELECT CONVERT(VARCHAR, DateofIncrement, 105)) AS DOIncDate,
                   (SELECT CONVERT(VARCHAR, DateOfPermanent, 105)) AS DOPDate,
                   *
            FROM Employees
            INNER JOIN EmployeeDetail 
                ON Employees.EmployeeID = EmployeeDetail.EmployeeID
            WHERE PhoneNo LIKE @name + '%'
            ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, eName);
                #endregion

                #region Map Data
                List<EmployeeDetail> employeeList = new List<EmployeeDetail>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        long empCode = Convert.ToInt64(dr["EmployeeCode"]);
                        if (!employeeList.Any(x => x.EmployeeCode == empCode))
                        {
                            //  long designationId = Convert.ToInt64(dr["DesignationID"]);
                            // long subDepartmentId = Convert.ToInt64(dr["SubDepartmentID"]);

                            //  var designation = await getDesignationOnID(designationId, clientId);
                            //  var department = await getDepartmentOnID(subDepartmentId, clientId);

                            employeeList.Add(new EmployeeDetail
                            {
                                EDID = Convert.ToInt64(dr["EDID"]),
                                EmployeeCode = empCode,
                                EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                                EmployeeName = dr["EmployeeName"].ToString(),
                                FatherName = dr["FatherName"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Address = dr["Address"].ToString(),
                                E_Mail = dr["E_Mail"].ToString(),
                                City = dr["City"].ToString(),

                                DesignationName = (await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId)).Designation,
                                DesignationID = Int64.Parse(dr["DesignationID"].ToString()),

                                DepartmentName = (await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId)).SubDepartmentName,
                                SubDepartmentID = Int64.Parse(dr["SubDepartmentID"].ToString()),

                                Status = dr["Status"].ToString(),
                                Grade = dr["Grade"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                AdhaarNo = dr["AdhaarNo"].ToString(),
                                PinNo = dr["PinNo"].ToString(),
                                BankAccountNo = dr["BankAccountNo"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                BankBranch = dr["BankBranch"].ToString(),
                                InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                                CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                                IFSCCode = dr["IFSCCode"].ToString(),
                                PhotoPath = dr["PhotoPath"].ToString(),
                                Qualification = dr["Qualification"].ToString(),
                                OtherQual = dr["OtherQual"].ToString(),
                                SpouseName = dr["SpouseName"].ToString(),
                                PANCard = dr["PANCard"].ToString(),
                                SessionOfJoin = dr["SessionOfJoin"].ToString(),
                                Current_Session = dr["Current_Session"].ToString(),
                                NPSNo = dr["NPSNo"].ToString(),

                                DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                                DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                                DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                                DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                                UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                                UpdatedBy = dr["UpdatedBy"].ToString(),
                                DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                                WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                                LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), 0).ToString(),
                                m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                                QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                                ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                                IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                                FYear = dr["FYear"].ToString(),
                                Year = dr["Year"].ToString(),
                                Scale = dr["Scale"].ToString(),
                                Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                                WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),

                                BasicPay = Convert.ToDecimal(dr["BasicPay"]),
                                DARate = Convert.ToDecimal(dr["DARate"]),
                                DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                                SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                                SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                                MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                                AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                                TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                                RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                                HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                                SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                                SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                                InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                                Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                                CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                                CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                                CPFundStatus = validationBLL.IsBoolNotNull(dr["CPFundStatus"].ToString(), false),
                                BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                                SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                                PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                                TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                                WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                                LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                                CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                                LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                                LeavesAvailable = validationBLL.IsDecimalNotNull(dr["LeavesAvailable"].ToString(), 0),
                                LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                ExcessLeaves = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0)
                            });
                        }
                    }
                }
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesByMobile", ex.ToString());
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByParentage(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found, Please check Parentage!"
            };
            #endregion

            #region Validate Input
            if (string.IsNullOrEmpty(param) || param.Length < 3)
            {
                return response;
            }
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                SqlParameter eName = new SqlParameter("@name", param);
                string query = @"
SELECT EmployeeDetail.EDID,
       (SELECT CONVERT(VARCHAR, DOB, 105)) AS DOBDate,
       (SELECT CONVERT(VARCHAR, DOJ, 105)) AS DOJDate,
       (SELECT CONVERT(VARCHAR, DOW, 105)) AS DOWDate,
       (SELECT CONVERT(VARCHAR, DateOfWithdraw, 105)) AS DOWithDate,
       (SELECT CONVERT(VARCHAR, UpdatedOn, 105)) AS UpdatedDate,
       (SELECT CONVERT(VARCHAR, DateofIncrement, 105)) AS DOIncDate,
       (SELECT CONVERT(VARCHAR, DateOfPermanent, 105)) AS DOPDate,
       *
FROM Employees 
INNER JOIN EmployeeDetail 
ON Employees.EmployeeID = EmployeeDetail.EmployeeID
WHERE FatherName LIKE @name + '%'
ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, eName);
                #endregion

                #region Map Data
                List<EmployeeDetail> employeeList = new List<EmployeeDetail>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var li = (from ord in employeeList where ord.EmployeeCode == Convert.ToInt64(dr["EmployeeCode"]) select ord);
                        if (!li.Any())
                        {
                            //   var designation = await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId);
                            //   var department = await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId);

                            employeeList.Add(new EmployeeDetail
                            {
                                EDID = Convert.ToInt64(dr["EDID"]),
                                EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                                EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                                EmployeeName = dr["EmployeeName"].ToString(),
                                FatherName = dr["FatherName"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Address = dr["Address"].ToString(),
                                E_Mail = dr["E_Mail"].ToString(),
                                City = dr["City"].ToString(),
                                DesignationName = (await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId)).Designation,
                                DesignationID = Int64.Parse(dr["DesignationID"].ToString()),

                                DepartmentName = (await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId)).SubDepartmentName,
                                SubDepartmentID = Int64.Parse(dr["SubDepartmentID"].ToString()),
                                Status = dr["Status"].ToString(),
                                Grade = dr["Grade"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                AdhaarNo = dr["AdhaarNo"].ToString(),
                                PinNo = dr["PinNo"].ToString(),
                                BankAccountNo = dr["BankAccountNo"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                BankBranch = dr["BankBranch"].ToString(),
                                InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                                CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                                IFSCCode = dr["IFSCCode"].ToString(),
                                PhotoPath = dr["PhotoPath"].ToString(),
                                Qualification = dr["Qualification"].ToString(),
                                OtherQual = dr["OtherQual"].ToString(),
                                SpouseName = dr["SpouseName"].ToString(),
                                PANCard = dr["PANCard"].ToString(),
                                SessionOfJoin = dr["SessionOfJoin"].ToString(),
                                Current_Session = dr["Current_Session"].ToString(),
                                NPSNo = dr["NPSNo"].ToString(),

                                DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                                DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                                DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                                DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                                UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                                UpdatedBy = dr["UpdatedBy"].ToString(),
                                DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                                WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                                LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), 0).ToString(),
                                m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                                QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                                ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                                IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                                FYear = dr["FYear"].ToString(),
                                Year = dr["Year"].ToString(),
                                Scale = dr["Scale"].ToString(),
                                Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                                WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),
                                BasicPay = Convert.ToDecimal(dr["BasicPay"]),
                                DARate = Convert.ToDecimal(dr["DARate"]),
                                DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                                SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                                SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                                MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                                AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                                TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                                RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                                HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                                SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                                SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                                InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                                Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                                CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                                CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                                CPFundStatus = validationBLL.IsBoolNotNull(dr["CPFundStatus"].ToString(), false),
                                BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                                SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                                PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                                TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                                WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                                LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                                CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                                LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                                LeavesAvailable = validationBLL.IsDecimalNotNull(dr["LeavesAvailable"].ToString(), 0),
                                LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                ExcessLeaves = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0)
                            });
                        }
                    }
                }
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                response.ResponseData = null;
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesByAddress(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found, Please check Address!"
            };
            #endregion

            #region Validate Input
            if (string.IsNullOrEmpty(param) || param.Length < 3)
            {
                return response;
            }
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                SqlParameter eName = new SqlParameter("@name", param);
                string query = @"
SELECT EmployeeDetail.EDID,
       (SELECT CONVERT(VARCHAR, DOB, 105)) AS DOBDate,
       (SELECT CONVERT(VARCHAR, DOJ, 105)) AS DOJDate,
       (SELECT CONVERT(VARCHAR, DOW, 105)) AS DOWDate,
       (SELECT CONVERT(VARCHAR, DateOfWithdraw, 105)) AS DOWithDate,
       (SELECT CONVERT(VARCHAR, UpdatedOn, 105)) AS UpdatedDate,
       (SELECT CONVERT(VARCHAR, DateofIncrement, 105)) AS DOIncDate,
       (SELECT CONVERT(VARCHAR, DateOfPermanent, 105)) AS DOPDate,
       *
FROM Employees 
INNER JOIN EmployeeDetail 
ON Employees.EmployeeID = EmployeeDetail.EmployeeID
WHERE Address LIKE @name + '%'
ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, eName);
                #endregion

                #region Map Data
                List<EmployeeDetail> employeeList = new List<EmployeeDetail>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var li = from ord in employeeList
                                 where ord.EmployeeCode == Convert.ToInt64(dr["EmployeeCode"])
                                 select ord;
                        if (!li.Any())
                        {
                            //   var designation = await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId);
                            //  var department = await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId);
                            employeeList.Add(new EmployeeDetail
                            {
                                EDID = Convert.ToInt64(dr["EDID"]),
                                EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                                EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                                EmployeeName = dr["EmployeeName"].ToString(),
                                FatherName = dr["FatherName"].ToString(),
                                PhoneNo = dr["PhoneNo"].ToString(),
                                Address = dr["Address"].ToString(),
                                E_Mail = dr["E_Mail"].ToString(),
                                City = dr["City"].ToString(),
                                DesignationName = (await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId)).Designation,
                                DesignationID = Int64.Parse(dr["DesignationID"].ToString()),

                                DepartmentName = (await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId)).SubDepartmentName,
                                SubDepartmentID = Int64.Parse(dr["SubDepartmentID"].ToString()),
                                Status = dr["Status"].ToString(),
                                Grade = dr["Grade"].ToString(),
                                Remarks = dr["Remarks"].ToString(),
                                AdhaarNo = dr["AdhaarNo"].ToString(),
                                PinNo = dr["PinNo"].ToString(),
                                BankAccountNo = dr["BankAccountNo"].ToString(),
                                BankName = dr["BankName"].ToString(),
                                BankBranch = dr["BankBranch"].ToString(),
                                InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                                CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                                IFSCCode = dr["IFSCCode"].ToString(),
                                PhotoPath = dr["PhotoPath"].ToString(),
                                Qualification = dr["Qualification"].ToString(),
                                OtherQual = dr["OtherQual"].ToString(),
                                SpouseName = dr["SpouseName"].ToString(),
                                PANCard = dr["PANCard"].ToString(),
                                SessionOfJoin = dr["SessionOfJoin"].ToString(),
                                Current_Session = dr["Current_Session"].ToString(),
                                NPSNo = dr["NPSNo"].ToString(),

                                DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                                DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                                DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                                DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                                UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                                UpdatedBy = dr["UpdatedBy"].ToString(),
                                DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                                WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                                Gender = dr["Gender"].ToString(),
                                IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                                LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), 0).ToString(),
                                m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                                QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                                ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                                IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                                FYear = dr["FYear"].ToString(),
                                Year = dr["Year"].ToString(),
                                Scale = dr["Scale"].ToString(),
                                Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                                WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),
                                BasicPay = Convert.ToDecimal(dr["BasicPay"]),
                                DARate = Convert.ToDecimal(dr["DARate"]),
                                DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                                SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                                SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                                MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                                AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                                TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                                RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                                HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                                SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                                SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                                InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                                Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                                CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                                CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                                CPFundStatus = validationBLL.IsBoolNotNull(dr["CPFundStatus"].ToString(), false),
                                BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                                SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                                PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                                TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                                WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                                LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                                CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                                LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                                LeavesAvailable = validationBLL.IsDecimalNotNull(dr["LeavesAvailable"].ToString(), 0),
                                LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                ExcessLeaves = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                                WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0)
                            });
                        }
                    }
                }
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.Status = -1;
                response.Message = "Error: " + ex.ToString();
                response.ResponseData = null;
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetEmployeesForAttendanceUpdate(string param, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(param))
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Invalid Input!";
                    return response;
                }

                string[] data = param.Split(',');
                if (data.Length != 4)
                {
                    response.IsSuccess = false;
                    response.Status = -1;
                    response.Message = "Some parameter missing!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                SqlParameter[] sqlParams = {
            new SqlParameter("@DID", data[0]),
            new SqlParameter("@Year", data[1]),
            new SqlParameter("@MName", data[2]),
            new SqlParameter("@MNo", data[3])
        };

                string query = @"
            SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName
            FROM Employees
            INNER JOIN EmployeeDetail ON Employees.EmployeeID = EmployeeDetail.EmployeeID
            WHERE SubDepartmentID = @DID 
              AND Year = @Year 
              AND (Withdrawn = 'false' OR WithdrawnEmp = 'false')
            ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";

                if (data[1].Length == 7)
                {
                    query = @"
                SELECT EmployeeDetail.EDID, *, 'NA' AS FieldName
                FROM Employees
                INNER JOIN EmployeeDetail ON Employees.EmployeeID = EmployeeDetail.EmployeeID
                WHERE SubDepartmentID = @DID 
                  AND FYear = @Year 
                  AND (Withdrawn = 'false' OR WithdrawnEmp = 'false')
                ORDER BY Employees.EmployeeCode, EmployeeName, EmployeeDetail.EDID DESC";
                }
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Data
                List<EmployeeDetail> employeeList = new List<EmployeeDetail>();
                LeaveMonth lm = new LeaveMonth();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lm.Syear = data[1];
                        lm.Mid = data[3];
                        lm.EcodeFK = dr["EmployeeCode"].ToString();
                        lm = await getLeaveData(lm, clientId); // You can wrap this in Task.Run if needed

                        employeeList.Add(new EmployeeDetail
                        {
                            EDID = Convert.ToInt64(lm.Lmid),
                            EmployeeCode = Int64.Parse(dr["EmployeeCode"].ToString()),
                            EmployeeID = Int64.Parse(dr["EmployeeID"].ToString()),
                            EmployeeName = dr["EmployeeName"].ToString(),
                            FatherName = dr["FatherName"].ToString(),
                            PhoneNo = dr["PhoneNo"].ToString(),
                            Address = dr["Address"].ToString(),
                            E_Mail = dr["E_Mail"].ToString(),
                            City = dr["City"].ToString(),

                            DesignationName = (await getDesignationOnID(Int64.Parse(dr["DesignationID"].ToString()), clientId)).Designation,
                            DesignationID = Int64.Parse(dr["DesignationID"].ToString()),

                            DepartmentName = (await getDepartmentOnID(Int64.Parse(dr["SubDepartmentID"].ToString()), clientId)).SubDepartmentName,
                            SubDepartmentID = Int64.Parse(dr["SubDepartmentID"].ToString()),
                            Status = dr["Status"].ToString(),
                            Grade = dr["Grade"].ToString(),
                            Remarks = dr["Remarks"].ToString(),
                            AdhaarNo = dr["AdhaarNo"].ToString(),
                            PinNo = dr["PinNo"].ToString(),
                            BankAccountNo = dr["BankAccountNo"].ToString(),
                            BankName = dr["BankName"].ToString(),
                            BankBranch = dr["BankBranch"].ToString(),


                            InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                            CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                            IFSCCode = dr["IFSCCode"].ToString(),
                            PhotoPath = dr["PhotoPath"].ToString(),
                            Qualification = dr["Qualification"].ToString(),
                            OtherQual = dr["OtherQual"].ToString(),
                            SpouseName = dr["SpouseName"].ToString(),
                            PANCard = dr["PANCard"].ToString(),
                            SessionOfJoin = dr["SessionOfJoin"].ToString(),
                            Current_Session = dr["Current_Session"].ToString(),
                            NPSNo = dr["NPSNo"].ToString(),





                            // Date Fields

                            DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                            // DOB = Conv. dr["DOB"].ToString(),
                            DOBString = Convert.ToDateTime(dr["DOB"].ToString()).ToString("dd-MM-yyyy"),
                            DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                            DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                            DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                            UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                            UpdatedBy = dr["UpdatedBy"].ToString(),
                            DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                            WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                            LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), long.Parse("0")).ToString(), // Check for Null Value
                            m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                            QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                            ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                            IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                            FYear = dr["FYear"].ToString(),
                            Year = dr["Year"].ToString(),
                            Scale = dr["Scale"].ToString(),
                            Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                            WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),
                            //// Salary  details
                            BasicPay = Convert.ToDecimal(dr["BasicPay"].ToString()),
                            DARate = Convert.ToDecimal(dr["DARate"].ToString()),
                            DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                            SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                            SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                            MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                            AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                            TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                            RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                            HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                            SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                            SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                            InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                            Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                            CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                            CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                            CPFundStatus = validationBLL.IsBoolNotNull(dr["CPFundStatus"].ToString(), false),
                            BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                            SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                            PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                            TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                            WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                            LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                            CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                            LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                            LeavesAvailable = validationBLL.IsDecimalNotNull(lm.Lavlb, 0),
                            LeavesTaken = validationBLL.IsDecimalNotNull(lm.Ltaken, 0),
                            ExcessLeaves = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                            WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0),
                            NPSRate = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                            FieldName = dr["FieldName"].ToString()
                        });
                    }
                }
                #endregion

                #region Handle Result
                if (employeeList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "Data Found";
                    response.ResponseData = employeeList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Handle Exception
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.ResponseData = null;

                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeService", "GetEmployeesForAttendanceUpdate", ex.ToString());
                #endregion

                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public Task<ResponseModel> GetEmployeeTableFields(string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found",
                ResponseData = new List<EmployeeTableFields>()
            };

            List<EmployeeTableFields> fields = new List<EmployeeTableFields>
    {
        new EmployeeTableFields { FieldName = "EmployeeName", DisplayName = "Employee Name", FieldType = "string" },
        new EmployeeTableFields { FieldName = "Address", DisplayName = "Employee Address", FieldType = "string" },
        new EmployeeTableFields { FieldName = "Parentage", DisplayName = "Parentage", FieldType = "string" }
    };

            if (fields.Count > 0)
            {
                response.Status = 1;
                response.Message = "";
                response.ResponseData = fields;
            }

            return Task.FromResult(response);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<List<EmployeeDetail>> EmpData(DataSet ds, string clientId)
        {
            var employeeList = new List<EmployeeDetail>();

            try
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        long designationId = dr["DesignationID"] != DBNull.Value ? Convert.ToInt64(dr["DesignationID"]) : 0;
                        long subDepartmentId = dr["SubDepartmentID"] != DBNull.Value ? Convert.ToInt64(dr["SubDepartmentID"]) : 0;
                        // Fetch Designation & Department (async per row - optional: can optimize by caching if needed)
                        var designation = await getDesignationOnID(designationId, clientId);
                        var department = await getDepartmentOnID(subDepartmentId, clientId);

                        var employee = new EmployeeDetail
                        {
                            // EDID = Convert.ToInt64(dr["EDID"]),
                            EmployeeCode = dr["EmployeeCode"] != DBNull.Value ? Convert.ToInt64(dr["EmployeeCode"]) : 0,
                            EmployeeID = dr["EmployeeID"] != DBNull.Value ? Convert.ToInt64(dr["EmployeeID"]) : 0,
                           // EmployeeCode = Convert.ToInt64(dr["EmployeeCode"]),
                           // EmployeeID = Convert.ToInt64(dr["EmployeeID"]),
                            EmployeeName = dr["EmployeeName"].ToString() ?? string.Empty,
                            FatherName = dr["FatherName"].ToString() ?? string.Empty,
                            PhoneNo = dr["PhoneNo"].ToString() ?? string.Empty,
                            Address = dr["Address"].ToString() ?? string.Empty,
                            E_Mail = dr["E_Mail"].ToString() ?? string.Empty,
                            City = dr["City"].ToString() ?? string.Empty,


                            EDID = dr["EDID"] != DBNull.Value ? Convert.ToInt64(dr["EDID"]) : 0,
                            BasicPay = dr["BasicPay"] != DBNull.Value ? Convert.ToDecimal(dr["BasicPay"]) : 0M,
                            DOBString = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("dd-MM-yyyy") : null,



                            DesignationID = designationId,
                            DesignationName = designation?.Designation ?? string.Empty,

                            SubDepartmentID = subDepartmentId,
                            DepartmentName = department?.SubDepartmentName ?? string.Empty,

                            Status = dr["Status"].ToString(),
                            Grade = dr["Grade"].ToString(),
                            Remarks = dr["Remarks"].ToString(),
                            AdhaarNo = dr["AdhaarNo"].ToString(),
                            PinNo = dr["PinNo"].ToString(),
                            BankAccountNo = dr["BankAccountNo"].ToString(),
                            BankName = dr["BankName"].ToString(),
                            BankBranch = dr["BankBranch"].ToString(),
                            InsurancePolicyNo = dr["InsurancePolicyNo"].ToString(),
                            CPFundAccountNo = validationBLL.IsNumber(dr["CPFundAccountNo"].ToString(), 0),
                            IFSCCode = dr["IFSCCode"].ToString(),
                            PhotoPath = dr["PhotoPath"].ToString(),
                            Qualification = dr["Qualification"].ToString(),
                            OtherQual = dr["OtherQual"].ToString(),
                            SpouseName = dr["SpouseName"].ToString(),
                            PANCard = dr["PANCard"].ToString(),
                            SessionOfJoin = dr["SessionOfJoin"].ToString(),
                            Current_Session = dr["Current_Session"].ToString(),
                            NPSNo = dr["NPSNo"].ToString(),
                            DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),



                            // DOB = validationBLL.isDateNotNull(dr["DOB"].ToString()),
                            //DOBString = Convert.ToDateTime(dr["DOB"]).ToString("dd-MM-yyyy"),
                            DOJ = validationBLL.isDateNotNull(dr["DOJ"].ToString()),
                            DateOfPermanent = validationBLL.isDateNotNull(dr["DateOfPermanent"].ToString()),
                            DateOfWithdraw = validationBLL.isDateNotNull(dr["DateOfWithdraw"].ToString()),
                            UpdatedOn = validationBLL.isDateNotNull(dr["UpdatedOn"].ToString()),
                            UpdatedBy = dr["UpdatedBy"].ToString(),
                            DOW = validationBLL.isDateNotNull(dr["DOW"].ToString()),
                            WithdrawRemarks = dr["WithdrawRemarks"].ToString(),
                            Gender = dr["Gender"].ToString(),
                            IsBEd = validationBLL.IsNumber(dr["IsBEd"].ToString(), 0),
                            LIDFK = validationBLL.IsNumber(dr["LIDFK"].ToString(), 0).ToString(),
                            m_status = validationBLL.IsNumber(dr["m_status"].ToString(), 0),
                            QidFk = validationBLL.IsNumber(dr["QidFk"].ToString(), 0),
                            ESIDFK = validationBLL.IsNumber(dr["ESIDFK"].ToString(), 0),
                            IsTeacher = validationBLL.IsNumber(dr["IsTeacher"].ToString(), 0),
                            FYear = dr["FYear"].ToString(),
                            Year = dr["Year"].ToString(),
                            Scale = dr["Scale"].ToString(),
                            Withdrawn = validationBLL.IsBoolNotNull(dr["Withdrawn"].ToString(), false),
                            WithdrawnEmp = validationBLL.IsBoolNotNull(dr["WithdrawnEmp"].ToString(), false),

                            // Salary Details (same as your old code)
                            //BasicPay = Convert.ToDecimal(dr["BasicPay"]),
                            DARate = Convert.ToDecimal(dr["DARate"]),
                            DarenessAllownce = validationBLL.IsDecimalNotNull(dr["DarenessAllownce"].ToString(), 0),
                            SalaryStoped = validationBLL.IsBoolNotNull(dr["SalaryStoped"].ToString(), false),
                            SACAllownce = validationBLL.IsDecimalNotNull(dr["SACAllownce"].ToString(), 0),
                            MedicalAllownce = validationBLL.IsDecimalNotNull(dr["MedicalAllownce"].ToString(), 0),
                            AdditionslAllownce = validationBLL.IsDecimalNotNull(dr["AdditionslAllownce"].ToString(), 0),
                            TravelAllownce = validationBLL.IsDecimalNotNull(dr["TravelAllownce"].ToString(), 0),
                            RationAllownce = validationBLL.IsDecimalNotNull(dr["RationAllownce"].ToString(), 0),
                            HouseRentAllownce = validationBLL.IsDecimalNotNull(dr["HouseRentAllownce"].ToString(), 0),
                            SpAllownceA = validationBLL.IsDecimalNotNull(dr["SpAllownceA"].ToString(), 0),
                            SpAllownceB = validationBLL.IsDecimalNotNull(dr["SpAllownceB"].ToString(), 0),
                            InsuranceInstallment = validationBLL.IsDecimalNotNull(dr["InsuranceInstallment"].ToString(), 0),
                            NPSRate = validationBLL.IsDecimalNotNull(dr["NPSRate"].ToString(), 0),
                            CPFundIntrest = validationBLL.IsDecimalNotNull(dr["CPFundIntrest"].ToString(), 0),
                            CPFPensionRate = validationBLL.IsDecimalNotNull(dr["CPFPensionRate"].ToString(), 0),
                            Insurance1PercentRate = validationBLL.IsDecimalNotNull(dr["Insurance1PercentRate"].ToString(), 0),
                            CPFundStatus = string.IsNullOrEmpty(dr["CPFundStatus"].ToString()) ? false : Convert.ToBoolean(dr["CPFundStatus"]),
                            BankAccount = validationBLL.IsBoolNotNull(dr["BankAccount"].ToString(), false),
                            SecurityDeduction = validationBLL.IsDecimalNotNull(dr["SecurityDeduction"].ToString(), 0),
                            PenaltyDeduction = validationBLL.IsDecimalNotNull(dr["PenaltyDeduction"].ToString(), 0),
                            TransportDedAmt = validationBLL.IsDecimalNotNull(dr["TransportDedAmt"].ToString(), 0),
                            WelFund = validationBLL.IsDecimalNotNull(dr["WelFund"].ToString(), 0),
                            LoanDeduction = validationBLL.IsDecimalNotNull(dr["LoanDeduction"].ToString(), 0),
                            CPFRecoveryDedAmt = validationBLL.IsDecimalNotNull(dr["cpfRecoveryDedAmt"].ToString(), 0),
                            LeavesApplied = validationBLL.IsBoolNotNull(dr["LeavesApplied"].ToString(), true),
                            LeavesAvailable = validationBLL.IsDecimalNotNull(dr["LeavesAvailable"].ToString(), 0),
                            LeavesTaken = validationBLL.IsDecimalNotNull(dr["LeavesTaken"].ToString(), 0),
                            ExcessLeaves = validationBLL.IsDecimalNotNull(dr["ExcessLeaves"].ToString(), 0),
                            WorkingDays = validationBLL.IsNumber(dr["WorkingDays"].ToString(), 0),
                            FieldName = dr["FieldName"].ToString()
                        };

                        employeeList.Add(employee);
                    }
                }

                return employeeList;
            }
            catch (Exception ex)
            {
                Repository.Error.ErrorBLL.CreateErrorLog("EmployeeDataMapper", "MapEmployeeDetailsAsync", ex.ToString());

                employeeList.Add(new EmployeeDetail
                {
                    EDID = 0,
                    Status = "Error: " + ex.Message
                });

                return employeeList;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<Designations> getDesignationOnID(long id, string clientId)
        {
            Designations designation = new Designations();

            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            string query = "SELECT * FROM Designations WHERE DesignationID = @DID";

            SqlParameter[] parameters =
            {
            new SqlParameter("@DID", id)
        };

            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                designation.Designation = dr["Designation"].ToString();
                designation.DesignationID = Convert.ToInt64(dr["DesignationID"]);
            }

            return designation;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<SubDepartment> getDepartmentOnID(long id, string clientId)
        {
            SubDepartment subDepartment = new SubDepartment();

            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);

            string query = "SELECT * FROM SubDepartments WHERE SubDepartmentID = @DID";

            SqlParameter[] parameters =
            {
            new SqlParameter("@DID", id)
        };

            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);

            if (ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                subDepartment.SubDepartmentName = dr["SubDepartmentName"].ToString();
                subDepartment.SubDepartmentID = Convert.ToInt64(dr["SubDepartmentID"]);
            }

            return subDepartment;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lm"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<LeaveMonth> getLeaveData(LeaveMonth lm, string clientId)
        {
            LeaveMonth lmData = new LeaveMonth();

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                SqlParameter[] param =
                {
            new SqlParameter("@Lmid", lm.Lmid),
            new SqlParameter("@EcodeFK", lm.EcodeFK),
            new SqlParameter("@Mid", lm.Mid),
            new SqlParameter("@Mname", lm.Mname),
            new SqlParameter("@Syear", lm.Syear),
            new SqlParameter("@Username", lm.Username),
            new SqlParameter("@Upadatedon", lm.Upadatedon),
            new SqlParameter("@Ltaken", lm.Ltaken),
            new SqlParameter("@Lavlb", lm.Lavlb)
        };

                string query = "SELECT * FROM LeaveMonth WHERE Mid = @Mid AND EcodeFK = @EcodeFK AND Syear = @Syear";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Data
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];

                    lmData.Lmid = dr["Lmid"].ToString();
                    lmData.EcodeFK = dr["EcodeFK"].ToString();
                    lmData.Mid = dr["Mid"].ToString();
                    lmData.Syear = dr["Syear"].ToString();
                    lmData.Ltaken = dr["Ltaken"].ToString();
                    lmData.Lavlb = dr["Lavlb"].ToString();
                    lmData.Username = dr["Username"].ToString();
                    lmData.Upadatedby = dr["Upadatedby"].ToString();
                }
                else
                {
                    lmData.Lmid = "-1";
                    lmData.EcodeFK = "0";
                }
                #endregion

                return lmData;
            }
            catch (Exception ex)
            {
                // Optional: You can log error if needed
                lmData.Lmid = "-1";
                lmData.EcodeFK = "0";
                return lmData;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetNextEmployeeCode(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No employee code found.",
                ResponseData = null
            };
            #endregion

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            try
            {
                #region Execute SQL to Get Max EmployeeCode
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(
                    connectionString,
                    CommandType.Text,
                    "SELECT ISNULL(MAX(EmployeeCode), 0) AS EmployeeCode FROM Employees"
                );
                #endregion

                #region Extract and Increment Code
                long empCode = 0;

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    object result = ds.Tables[0].Rows[0]["EmployeeCode"];
                    empCode = result != DBNull.Value ? Convert.ToInt64(result) : 0;
                }

                empCode += 1;
                #endregion

                #region Set Success Response
                response.ResponseData = empCode.ToString(); // use ToString("D5") for padding
                response.Status = 1;
                response.Message = "Success";
                #endregion
            }
            catch (Exception ex)
            {
                #region Set Error Response
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = ex.Message;
                #endregion
            }

            return response;
        }



    }
}
