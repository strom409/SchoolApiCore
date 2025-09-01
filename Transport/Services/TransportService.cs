using Microsoft.Data.SqlClient;
using System.Data;
using Transport.Repository;
using Transport.Repository.SQL;

namespace Transport.Services
{
    public class TransportService : ITransportService
    {
        private readonly IConfiguration _configuration;

        public TransportService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddTransport(TransportDTO transport, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Unknown error!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Validate Required Fields
                if (string.IsNullOrWhiteSpace(transport.RouteName))
                {
                    response.Status = 0;
                    response.Message = "Route name is required!";
                    return response;
                }
                #endregion
                #region Prepare Parameters
                SqlParameter[] parameters =
 {
    new SqlParameter("@RouteName", transport.RouteName ?? (object)DBNull.Value),
    new SqlParameter("@RouteStops", transport.RouteStops ?? (object)DBNull.Value),
    new SqlParameter("@VehicleNo", transport.VehicleNo ?? (object)DBNull.Value),
    new SqlParameter("@DriverName", transport.DriverName ?? (object)DBNull.Value),
    new SqlParameter("@RouteCost", transport.RouteCost ?? (object)DBNull.Value),
    new SqlParameter("@DateOfStart", transport.DateOfStart ?? (object)DBNull.Value),
    new SqlParameter("@Current_Session", transport.Current_Session ?? (object)DBNull.Value),
    new SqlParameter("@SessionID", transport.SessionID ?? (object)DBNull.Value),
    new SqlParameter("@Remarks", transport.Remarks ?? (object)DBNull.Value),
    new SqlParameter("@SeatingCapacity", transport.SeatingCapacity ?? (object)DBNull.Value),
    new SqlParameter("@InsExp", transport.InsExp ?? (object)DBNull.Value),
    new SqlParameter("@TokenExp", transport.TokenExp ?? (object)DBNull.Value),
    new SqlParameter("@PermitExp", transport.PermitExp ?? (object)DBNull.Value),
    new SqlParameter("@UserName", transport.UserName ?? (object)DBNull.Value),
    new SqlParameter("@driverPhone", transport.driverPhone ?? (object)DBNull.Value),
    new SqlParameter("@isDeleted", transport.isDeleted ?? (object)DBNull.Value),
    new SqlParameter("@pollutionExpr", transport.pollutionExpr ?? (object)DBNull.Value),
    new SqlParameter("@ftnsExpr", transport.ftnsExpr ?? (object)DBNull.Value),
    new SqlParameter("@BusType", transport.BusType ?? (object)DBNull.Value),
    new SqlParameter("@ConductorName", transport.ConducterName ?? (object)DBNull.Value),
    new SqlParameter("@ConductorPhone", transport.ConducterPhone ?? (object)DBNull.Value)
};

                #endregion

                #region Check if RouteName already exists
                string checkQuery = "SELECT COUNT(*) FROM Transport WHERE RouteName = @RouteName";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);

                int exists = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.Status = 0;
                    response.Message = "Bus Name Already Exists!";
                    return response;
                }
                #endregion

                #region Insert Query
                string insertQuery = @"
            INSERT INTO Transport (
                RouteName, RouteStops, VehicleNo, DriverName, RouteCost,
                DateOfStart, Current_Session, SessionID, Remarks, SeatingCapacity,
                InsExp, TokenExp, PermitExp, UserName, driverPhone, isDeleted,
                pollutionExpr, ftnsExpr, ConductorName, conductorphone
            )
            VALUES (
                @RouteName, @RouteStops, @VehicleNo, @DriverName, 0,
                @DateOfStart, @Current_Session, 0, @Remarks, @SeatingCapacity,
                @InsExp, @TokenExp, @PermitExp, @UserName, @driverPhone, 'false',
                @pollutionExpr, @ftnsExpr, @ConductorName, @conductorphone
            )
        ";

                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);
                #endregion

                #region Final Response
                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Added Successfully!";
                }
                else
                {
                    response.Status = 0;
                    response.Message = "Bus Not Added!";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddBusStops(TransportDTO transport, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "Unknown error!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@RouteID", transport.RouteID),
            new SqlParameter("@BusStopName", transport.BusStopName),
            new SqlParameter("@busrate", transport.BusRate),
            new SqlParameter("@Current_Session", transport.Current_Session)
        };
                #endregion

                #region Check If Bus Stop Exists
                string checkQuery = @"SELECT COUNT(*) FROM BusStops 
                              WHERE RouteID = @RouteID AND BusStop = @BusStopName";
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);

                int exists = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.Status = 0;
                    response.Message = "Bus Stop Already Exists!";
                    return response;
                }
                #endregion

                #region Insert Query
                string insertQuery = @"INSERT INTO BusStops (BusStop, BusRate, RouteID, Removed, Current_Session) 
                               VALUES (@BusStopName, @busrate, @RouteID, 0, @Current_Session)";
                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, insertQuery, parameters);
                #endregion

                #region Final Response
                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Stop Added Successfully";
                }
                else
                {
                    response.Status = 0;
                    response.Message = "Stop Not Added";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateTransport(TransportDTO transport, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Check for Existing Route Name
                string checkQuery = @"SELECT COUNT(*) 
                              FROM Transport 
                              WHERE RouteName = @RouteName AND RouteID != @RouteID AND isPrimary = 1  and current_session=@CurrentSesion";

                SqlParameter[] checkParams =
                {
            new SqlParameter("@RouteName", transport.RouteName),
            new SqlParameter("@RouteID", transport.RouteID),
            new SqlParameter("@CurrentSesion", transport.Current_Session),
        };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, checkParams);
                int exists = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.Message = "Bus Name Already Exists!";
                    return response;
                }
                #endregion

                #region SQL Parameters for Update
                SqlParameter[] parameters =
                {
            new SqlParameter("@RouteID", transport.RouteID),
            new SqlParameter("@RouteName", transport.RouteName),
          //  new SqlParameter("@RouteStops", transport.RouteStops),
            new SqlParameter("@VehicleNo", transport.VehicleNo),
            new SqlParameter("@DriverName", transport.DriverName),
            new SqlParameter("@DateOfStart", transport.DateOfStart),
            new SqlParameter("@Current_Session", transport.Current_Session),
            new SqlParameter("@Remarks", transport.Remarks),
            new SqlParameter("@SeatingCapacity", transport.SeatingCapacity),
            new SqlParameter("@InsExp", transport.InsExp),
            new SqlParameter("@TokenExp", transport.TokenExp),
            new SqlParameter("@PermitExp", transport.PermitExp),
            new SqlParameter("@UserName", transport.UserName),
           // new SqlParameter("@driverPhone", transport.driverPhone),
          //  new SqlParameter("@isDeleted", transport.isDeleted ?? (object)DBNull.Value),
            new SqlParameter("@pollutionExpr", transport.pollutionExpr),
            new SqlParameter("@ftnsExpr", transport.ftnsExpr),
            //new SqlParameter("@BusType", transport.BusType),
            new SqlParameter("@ConductorName", transport.ConducterName ?? (object)DBNull.Value),
            new SqlParameter("@ConductorPhone", transport.ConducterPhone ?? (object)DBNull.Value)
        };
                #endregion

                #region Update Query
                string updateQuery = @"UPDATE Transport 
                               SET RouteName = @RouteName,
                                   VehicleNo = @VehicleNo,
                                   DriverName = @DriverName,
                                   DateOfStart = @DateOfStart,
                                   Remarks = @Remarks,
                                   SeatingCapacity = @SeatingCapacity,
                                   InsExp = @InsExp,
                                   TokenExp = @TokenExp,
                                   PermitExp = @PermitExp,
                                   UserName = @UserName,
                                   pollutionExpr = @pollutionExpr,
                                   ftnsExpr = @ftnsExpr,
                                   ConductorName = @ConductorName,
                                   Current_Session = @Current_Session,
                                   ConductorPhone = @ConductorPhone
                               WHERE RouteID = @RouteID";

                int affectedRows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Final Response
                if (affectedRows > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully";
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


        //public async Task<ResponseModel> UpdateTransport(TransportDTO transport, string clientId)
        //{
        //    #region Initialize Response
        //    var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
        //    #endregion

        //    try
        //    {
        //        #region Get Connection String
        //        var connectionStringHelper = new ConnectionStringHelper(_configuration);
        //        string connectionString = connectionStringHelper.GetConnectionString(clientId);
        //        #endregion

        //        #region SQL Parameters
        //        SqlParameter[] parameters =
        //        {
        //     new SqlParameter("@RouteID",transport.RouteID),
        //          new SqlParameter("@RouteName",transport.RouteName),
        //          new SqlParameter("@RouteStops",transport.RouteStops),
        //          new SqlParameter("@VehicleNo",transport.VehicleNo),
        //          new SqlParameter("@DriverName",transport.DriverName),
        //         // new SqlParameter("@RouteCost",tr.RouteCost),
        //          new SqlParameter("@DateOfStart",transport.DateOfStart),
        //          new SqlParameter("@Current_Session",transport.Current_Session),
        //         // new SqlParameter("@SessionID",transport.SessionID),
        //          new SqlParameter("@Remarks",transport.Remarks),
        //          new SqlParameter("@SeatingCapacity",transport.SeatingCapacity),
        //          new SqlParameter("@InsExp",transport.InsExp),
        //          new SqlParameter("@TokenExp",transport.TokenExp),
        //          new SqlParameter("@PermitExp",transport.PermitExp),
        //          new SqlParameter("@UserName",transport.UserName),
        //          new SqlParameter("@driverPhone",transport.driverPhone),
        //          new SqlParameter("@isDeleted",transport.isDeleted),
        //          new SqlParameter("@pollutionExpr",transport.pollutionExpr),
        //          new SqlParameter("@ftnsExpr",transport.ftnsExpr),
        //          new SqlParameter("@BusType",transport.BusType),
        //          new SqlParameter("@ConductorName",transport.ConducterName),
        //          new SqlParameter("@ConductorPhone",transport.ConducterPhone),
        //};
        //        #endregion

        //        #region Check for Existing Route Name
        //        string checkQuery = @"SELECT COUNT(*) 
        //                      FROM Transport 
        //                      WHERE RouteName = @RouteName AND RouteID != @RouteID AND isPrimary = 1";
        //        SqlParameter[] checkParams =
        //{
        //    new SqlParameter("@RouteName", transport.RouteName),
        //    new SqlParameter("@RouteID", transport.RouteID)
        //};

        //        DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
        //        int exists = 0;
        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //            exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

        //        if (exists > 0)
        //        {
        //            response.Message = "Bus Name Already Exists!";
        //            return response;
        //        }
        //        #endregion

        //        #region Update Query
        //       string updateQuery = "update transport set RouteName=@RouteName,RouteStops=@RouteStops,VehicleNo=@VehicleNo,DriverName=@DriverName,DateOfStart=@DateOfStart,Remarks=@Remarks,SeatingCapacity=@SeatingCapacity,InsExp=@InsExp,TokenExp=@TokenExp,PermitExp=@PermitExp,UserName=@UserName, driverPhone=@driverPhone,pollutionExpr=@pollutionExpr,ftnsExpr=@ftnsExpr,ConductorName=@ConducterName, Current_Session = @Current_Session, isDeleted = @isDeleted ,ConductorPhone=@ConducterPhone where RouteID=@RouteID ";

        //        int affectedRows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
        //        #endregion

        //        #region Final Response
        //        if (affectedRows > 0)
        //        {
        //            response.Status = 1;
        //            response.Message = "Updated Successfully";
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.Status = -1;
        //        response.Message = "Error: " + ex.Message;
        //    }

        //    return response;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateBusStops(TransportDTO transport, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@BusStopID", transport.BusStopID),
            new SqlParameter("@BusStopName", transport.BusStopName),
            new SqlParameter("@BusRate", transport.BusRate),
            new SqlParameter("@RouteID", transport.RouteID)
        };
                #endregion

                #region Check for Existing Stop
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM BusStops 
            WHERE BusStop = @BusStopName 
              AND RouteID = @RouteID 
              AND BusStopID != @BusStopID";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
                int exists = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.Message = "Stop Name Already Exists!";
                    return response;
                }
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE BusStops 
            SET BusStop = @BusStopName, BusRate = @BusRate 
            WHERE BusStopID = @BusStopID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Final Response
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateBusStopsLatLong(TransportDTO transport, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@BusStopID", transport.BusStopID),
            new SqlParameter("@BusStopName", transport.BusStopName),
            new SqlParameter("@BusRate", transport.BusRate),
            new SqlParameter("@RouteID", transport.RouteID),
            new SqlParameter("@Latitude", transport.Latitude),
            new SqlParameter("@Longitude", transport.Longitude)
        };
                #endregion

                #region Check for Existing Stop
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM BusStops 
            WHERE BusStop = @BusStopName 
              AND RouteID = @RouteID 
              AND BusStopID != @BusStopID";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, parameters);
                int exists = 0;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    exists = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (exists > 0)
                {
                    response.Message = "Stop Name Already Exists!";
                    return response;
                }
                #endregion

                #region Update Query
                string updateQuery = @"
            UPDATE BusStops 
            SET BusStop = @BusStopName, 
                BusRate = @BusRate, 
                Longitude = @Longitude, 
                Latitude = @Latitude 
            WHERE BusStopID = @BusStopID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Final Response
                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbr"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateBusStopRates(StudentBusReportDTO sbr, string clientId)
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
                #region Validate
                if (string.IsNullOrEmpty(sbr.StudentInfoID) || string.IsNullOrEmpty(sbr.RouteID) || string.IsNullOrEmpty(sbr.BusStopID))
                {
                    response.Message = "Missing RouteID, BusStopID or StudentInfoID";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters & Query
                SqlParameter[] parameters =
                {
            new SqlParameter("@SID", sbr.SID),
            new SqlParameter("@StudentInfoID", sbr.StudentInfoID),
            new SqlParameter("@AdminNo", sbr.AdminNo),
            new SqlParameter("@RouteID", sbr.RouteID),
            new SqlParameter("@BusStopID", sbr.BusStopID),
            new SqlParameter("@UserName", sbr.UserName)
        };

                string query = @"UPDATE StudentInfo 
                         SET RouteID = @RouteID, BusStopID = @BusStopID 
                         WHERE StudentInfoID = @StudentInfoID";
                #endregion

                #region Execute
                int rows = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Finalize Response
                if (rows > 0)
                {
                    response.Status = 1;
                    response.Message = "Record Updated Successfully";
                }
                else
                {
                    response.Status = 0;
                    response.IsSuccess = false;
                    response.Message = "No records were updated.";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transport"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> updateroute(TransportDTO transport, string clientId)
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
                #region Validate Inputs
                if (string.IsNullOrEmpty(transport.RouteID) || string.IsNullOrEmpty(transport.BusStopID))
                {
                    response.Message = "Route ID or Stop ID is null!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters & Query
                SqlParameter[] parameters =
                {
            new SqlParameter("@StudentinfoID", transport.StudentinfoID),
            new SqlParameter("@RouteID", transport.RouteID),
            new SqlParameter("@BusStopID", transport.BusStopID)
        };

                string updateQuery = @"
            UPDATE StudentInfo 
            SET RouteID = @RouteID, BusStopID = @BusStopID 
            WHERE StudentInfoID = @StudentinfoID";
                #endregion

                #region Execute Update
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);
                #endregion

                #region Finalize Response
                if (result > 0)
                {
                    response.Status = 1;
                    response.Message = "Updated Successfully";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sbr"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdateStudentRouteAndBusStop(StudentBusReportDTO sbr, string clientId)
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
                #region Validate Inputs
                if (string.IsNullOrEmpty(sbr.StudentInfoID) || string.IsNullOrEmpty(sbr.RouteID) || string.IsNullOrEmpty(sbr.BusStopID))
                {
                    response.Message = "Missing StudentInfoID, RouteID, or BusStopID.";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@SID", sbr.SID ?? string.Empty),
            new SqlParameter("@StudentInfoID", sbr.StudentInfoID),
            new SqlParameter("@AdminNo", sbr.AdminNo ?? string.Empty),
            new SqlParameter("@RouteID", sbr.RouteID),
            new SqlParameter("@BusStopID", sbr.BusStopID),
            new SqlParameter("@UserName", sbr.UserName ?? "system")
        };

                string query = @"UPDATE StudentInfo 
                         SET RouteID = @RouteID, BusStopID = @BusStopID 
                         WHERE StudentInfoID = @StudentInfoID";
                #endregion

                #region Execute Query
                int result = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Final Response
                if (result > 0)
                {
                    response.Status = 1;
                    response.Message = "Record Updated Successfully";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Status = 0;
                    response.Message = "No records were updated";
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bs"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> aupdateBusStop(BusStop bs, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Bus Stop Not Updated!"
            };

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Parameters
                SqlParameter[] parameters =
                {
            new SqlParameter("@BusStopID", bs.BusStopID),
            new SqlParameter("@BusStop", bs.BusStopName),
            new SqlParameter("@RouteID", bs.RouteID),
            new SqlParameter("@BusRate", bs.BusRate),
            new SqlParameter("@Current_Session", bs.Current_Session),
            new SqlParameter("@Distance", bs.Distance),
            new SqlParameter("@latitude", bs.latitude),
            new SqlParameter("@longitude", bs.longitude),
            new SqlParameter("@busseq", bs.busseq),
            new SqlParameter("@Removed", bs.Removed)
        };
                #endregion

                #region Check for Duplicate Using ExecuteDatasetAsync
                string duplicateQuery = @"SELECT COUNT(*) 
                                  FROM BusStops 
                                  WHERE BusStop = @BusStop 
                                    AND RouteID = @RouteID 
                                    AND BusStopID != @BusStopID";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, duplicateQuery, parameters);
                int duplicateCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

                if (duplicateCount > 0)
                {
                    response.Message = "Duplicate Bus Stop not allowed";
                    return response;
                }
                #endregion

                #region Update Query
                string updateQuery = @"UPDATE BusStops 
                               SET BusStop = @BusStop, 
                                   BusRate = @BusRate, 
                                   Distance = @Distance, 
                                   latitude = @latitude, 
                                   longitude = @longitude, 
                                   busseq = @busseq, 
                                   Removed = @Removed 
                               WHERE BusStopID = @BusStopID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, parameters);

                if (rowsAffected > 0)
                {
                    response.Status = 1;
                    response.Message = "Bus Stop Updated Successfully";
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error occurred while updating Bus Stop.";
                response.Error = ex.Message;
                return response;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportListOnSession(string session, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            #region SQL Query and Parameters
            string query = "SELECT * FROM Transport WHERE Current_Session = @sess";
            SqlParameter param = new SqlParameter("@sess", string.IsNullOrEmpty(session) ? "0" : session);
            var transportList = new List<Transport>();
            #endregion

            try
            {
                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Results
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        transportList.Add(new Transport
                        {
                            RouteID = dr["RouteID"]?.ToString(),
                            RouteName = dr["RouteName"]?.ToString(),
                            RouteStops = dr["RouteStops"]?.ToString(),
                            VehicleNo = dr["VehicleNo"]?.ToString(),
                            DriverName = dr["DriverName"]?.ToString(),
                            RouteCost = dr["RouteCost"]?.ToString(),

                            // Format date only
                            DateOfStart = dr["DateOfStart"] == DBNull.Value ? null : Convert.ToDateTime(dr["DateOfStart"]).ToString("dd-MM-yyyy"),
                            Current_Session = dr["Current_Session"]?.ToString(),
                            SessionID = string.IsNullOrEmpty(dr["SessionID"]?.ToString()) ? "0" : dr["SessionID"].ToString(),
                            Remarks = dr["Remarks"]?.ToString(),
                            SeatingCapacity = string.IsNullOrEmpty(dr["SeatingCapacity"]?.ToString()) ? "0" : dr["SeatingCapacity"].ToString(),

                            InsExp = dr["InsExp"] == DBNull.Value ? null : Convert.ToDateTime(dr["InsExp"]).ToString("dd-MM-yyyy"),
                            TokenExp = dr["TokenExp"] == DBNull.Value ? null : Convert.ToDateTime(dr["TokenExp"]).ToString("dd-MM-yyyy"),
                            PermitExp = dr["PermitExp"] == DBNull.Value ? null : Convert.ToDateTime(dr["PermitExp"]).ToString("dd-MM-yyyy"),
                            UserName = dr["UserName"]?.ToString(),
                            pollutionExpr = dr["pollutionExpr"] == DBNull.Value ? null : Convert.ToDateTime(dr["pollutionExpr"]).ToString("dd-MM-yyyy"),
                            ftnsExpr = dr["ftnsExpr"] == DBNull.Value ? null : Convert.ToDateTime(dr["ftnsExpr"]).ToString("dd-MM-yyyy"),

                            driverPhone = dr["driverPhone"]?.ToString(),
                            ConducterName = dr["ConductorName"]?.ToString(),
                            ConducterPhone = dr["ConductorPhone"]?.ToString()
                        });
                    }


                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                response.Error = ex.ToString();
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportList(string routeId, string clientId)
        {
            var response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found!" };

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            #region Validate Input
            if (string.IsNullOrEmpty(routeId))
            {
                response.Message = "Route ID is null!";
                return response;
            }
            #endregion

            #region SQL Query & Parameters
            string query = @"
                SELECT studentinfoid, photopath, Studentname, rollno, PhoneNo, PerminantAddress, 
                       classid, sectionid, busstopid, routeid 
                FROM students 
                INNER JOIN studentinfo ON students.studentid = studentinfo.studentid 
                WHERE studentinfo.RouteID = @routeid";

            SqlParameter param = new SqlParameter("@routeid", routeId);
            List<Transport> transportList = new List<Transport>();
            #endregion

            try
            {
                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Process Data
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        transportList.Add(new Transport
                        {
                            StudentName = dr["StudentName"].ToString(),
                            Rollno = dr["Rollno"].ToString(),
                            PhoneNo = dr["PhoneNo"].ToString(),
                            PerminantAddress = dr["PerminantAddress"].ToString(),
                            Classname = await GetClassName(dr["classid"].ToString(), connectionString),
                            SectionName = await GetSectionName(dr["sectionid"].ToString(), connectionString),
                            BusRate = await GetStopRateOnly(dr["busstopid"].ToString(), connectionString),
                            BusStopName = await GetBusStopName(dr["busstopid"].ToString(), connectionString),
                            RouteID = dr["routeid"].ToString(),
                            BusStopID = dr["busstopid"].ToString(),
                            Photopath = dr["photopath"].ToString(),
                            StudentinfoID = dr["studentinfoid"].ToString()

                        });
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
                }
                #endregion

                return response;
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Error = ex.Message;
                response.Message = "Exception while fetching transport list.";
                return response;
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportListRateFromInfo(string routeId, string clientId)
        {
            #region Initialize
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Validation
            if (string.IsNullOrEmpty(routeId))
            {
                response.Message = "Route ID is null!";
                return response;
            }
            #endregion

            #region Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            #region Prepare Query
            string query = @"
                SELECT Studentname, rollno, PhoneNo, PerminantAddress, classid, sectionid, busstopid, busfee
                FROM students 
                INNER JOIN studentinfo ON students.studentid = studentinfo.studentid 
                WHERE StudentInfo.RouteID = @routeid  AND (isdischarged = 0 OR discharged = 'False') 
                ORDER BY classid";

            SqlParameter param = new SqlParameter("@routeid", routeId);
            #endregion

            #region Execute Query
            List<Transport> transportList = new List<Transport>();

            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var transport = new Transport
                        {
                            StudentName = dr["StudentName"].ToString(),
                            Rollno = dr["Rollno"].ToString(),
                            PhoneNo = dr["PhoneNo"].ToString(),
                            PerminantAddress = dr["PerminantAddress"].ToString(),
                            Classname = await GetClassName(dr["classid"].ToString(), connectionString),
                            SectionName = await GetSectionName(dr["sectionid"].ToString(), connectionString),
                            BusRate = dr["busfee"].ToString(),
                            BusStopName = await GetBusStopName(dr["busstopid"].ToString(), connectionString)
                        };

                        transportList.Add(transport);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
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
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportListWithBusRate(string routeId, string clientId)
        {
            #region Initialize
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };
            #endregion

            #region Validation
            if (string.IsNullOrEmpty(routeId))
            {
                response.Message = "Route ID is null!";
                return response;
            }
            #endregion

            #region Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            #region Query Setup
            string query = @"
        SELECT Studentname, rollno, PhoneNo, PerminantAddress, classid, sectionid, busstopid, busfee 
        FROM students 
        INNER JOIN studentinfo ON students.studentid = studentinfo.studentid 
        WHERE studentinfo.RouteID = @routeid 
        AND (isdischarged = 0 OR discharged = 'False') 
        ORDER BY classid";

            SqlParameter param = new SqlParameter("@routeid", routeId);
            #endregion

            #region Execute Query
            List<Transport> transportList = new List<Transport>();

            try
            {
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        var transport = new Transport
                        {
                            StudentName = dr["StudentName"].ToString(),
                            Rollno = dr["Rollno"].ToString(),
                            PhoneNo = dr["PhoneNo"].ToString(),
                            PerminantAddress = dr["PerminantAddress"].ToString(),
                            Classname = await GetClassName(dr["classid"].ToString(), connectionString),
                            SectionName = await GetSectionName(dr["sectionid"].ToString(), connectionString),
                            BusStopName = await GetBusStopName(dr["busstopid"].ToString(), connectionString),
                            BusRate = await GetBusStopRate(dr["busstopid"].ToString(), connectionString)
                        };

                        transportList.Add(transport);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
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
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportByRouteId(string routeId, string clientId)
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
                if (string.IsNullOrEmpty(routeId))
                {
                    response.Message = "Route ID is null!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Setup
                string query = "SELECT * FROM Transport WHERE RouteID = @RouteID";
                SqlParameter[] parameters = new[]
                {
            new SqlParameter("@RouteID", routeId)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Result
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    var transport = new Transport
                    {
                        RouteID = dr["RouteID"]?.ToString(),
                        RouteName = dr["RouteName"]?.ToString(),
                        RouteStops = dr["RouteStops"]?.ToString(),
                        VehicleNo = dr["VehicleNo"]?.ToString(),
                        DriverName = dr["DriverName"]?.ToString(),
                        RouteCost = dr["RouteCost"]?.ToString(),

                        DateOfStart = dr["DateOfStart"] != DBNull.Value
         ? Convert.ToDateTime(dr["DateOfStart"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        Current_Session = dr["Current_Session"]?.ToString(),
                        SessionID = string.IsNullOrEmpty(dr["SessionID"]?.ToString())
         ? "0"
         : dr["SessionID"].ToString(),
                        Remarks = dr["Remarks"]?.ToString(),

                        SeatingCapacity = string.IsNullOrEmpty(dr["SeatingCapacity"]?.ToString())
         ? "0"
         : dr["SeatingCapacity"].ToString(),

                        InsExp = dr["InsExp"] != DBNull.Value
         ? Convert.ToDateTime(dr["InsExp"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        TokenExp = dr["TokenExp"] != DBNull.Value
         ? Convert.ToDateTime(dr["TokenExp"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        PermitExp = dr["PermitExp"] != DBNull.Value
         ? Convert.ToDateTime(dr["PermitExp"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        UserName = dr["UserName"]?.ToString(),

                        pollutionExpr = dr["pollutionExpr"] != DBNull.Value
         ? Convert.ToDateTime(dr["pollutionExpr"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        driverPhone = dr["driverPhone"]?.ToString(),

                        ftnsExpr = dr["ftnsExpr"] != DBNull.Value
         ? Convert.ToDateTime(dr["ftnsExpr"]).ToString("yyyy-MM-dd")
         : string.Empty,

                        //BusType = string.IsNullOrEmpty(dr["BusType"]?.ToString()) ? "0" : dr["BusType"].ToString(),

                        ConducterName = dr["ConductorName"]?.ToString(),
                        ConducterPhone = dr["conductorphone"]?.ToString()
                    };


                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transport;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
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
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentRouteDetails(string param, string clientId)
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
                #region Parse Input
                string[] parts = param.Split(',');

                if (parts.Length != 3 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]) || string.IsNullOrEmpty(parts[2]))
                {
                    response.Message = "Invalid parameter: classid, sectionid, session";
                    return response;
                }

                // Convert classId and sectionId to int (important!)
                int classId = Convert.ToInt32(parts[0]);
                int sectionId = Convert.ToInt32(parts[1]);
                string sessionId = parts[2];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Parameters
                string query = @"
            SELECT Students.studentid AS studentid, RollNo, studentname, StudentInfoID,
                   StudentInfo.RouteID AS rid, StudentInfo.BusStopId AS stopid,
                   Students.AdmissionNo, phoneno, presentAddress, perminantAddress,
                   classid, sectionid
            FROM StudentInfo
            INNER JOIN Students ON Students.studentid = StudentInfo.studentid
            WHERE ClassID = @classid AND SectionID = @sectionID
              AND (isdischarged = 0 OR discharged = 'False')
              AND StudentInfo.Current_Session = @session
            ORDER BY rollno";

                SqlParameter[] sqlParams = new[]
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@sectionID", sectionId),
            new SqlParameter("@session", sessionId)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Results
                List<Transport> transportList = new List<Transport>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string stopId = dr["stopid"].ToString();
                    string routeId = dr["rid"].ToString();

                    transportList.Add(new Transport
                    {
                        Rollno = dr["RollNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        RouteID = routeId,
                        BusStopID = stopId,
                        StudentinfoID = dr["StudentInfoID"].ToString(),
                        PerminantAddress = dr["perminantAddress"].ToString(),
                        PresentAddress = dr["presentAddress"].ToString(),
                        PhoneNo = dr["phoneno"].ToString(),

                        BusRate = await GetStopRateOnly(stopId, connectionString),
                        RouteName = await GetRouteName(routeId, connectionString),
                        BusStopName = await GetStopName(stopId, connectionString),
                        Classname = await GetClassName(classId.ToString(), connectionString),
                        SectionName = await GetSectionName(sectionId.ToString(), connectionString)
                    });
                }

                if (transportList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
                #endregion
            }

            return response;
        }
        //public async Task<ResponseModel> GetStudentRouteDetails(string param, string clientId)
        //{
        //    #region Initialize Response
        //    var response = new ResponseModel
        //    {
        //        IsSuccess = true,
        //        Status = 0,
        //        Message = "No Data Found!"
        //    };
        //    #endregion

        //    try
        //    {
        //        #region Parse Input
        //        string[] parts = param.Split(',');

        //        if (parts.Length != 3 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]) || string.IsNullOrEmpty(parts[2]))
        //        {
        //            response.Message = "Invalid parameter: classid, sectionid, session";
        //            return response;
        //        }

        //        string classId = parts[0];
        //        string sectionId = parts[1];
        //        string sessionId = parts[2];
        //        #endregion

        //        #region Get Connection String
        //        var connectionStringHelper = new ConnectionStringHelper(_configuration);
        //        string connectionString = connectionStringHelper.GetConnectionString(clientId);
        //        #endregion

        //        #region SQL Query & Parameters
        //        string query = @"
        //    SELECT Students.studentid AS studentid, RollNo, studentname, StudentInfoID,
        //           StudentInfo.RouteID AS rid, StudentInfo.BusStopId AS stopid,
        //           Students.AdmissionNo, phoneno, presentAddress, perminantAddress,
        //           classid, sectionid
        //    FROM StudentInfo
        //    INNER JOIN Students ON Students.studentid = StudentInfo.studentid
        //    WHERE ClassID = @classid AND SectionID = @sectionID
        //      AND (isdischarged = 0 OR discharged = 'False')
        //      AND StudentInfo.Current_Session = @session
        //    ORDER BY rollno";

        //        SqlParameter[] sqlParams = new[]
        //        {
        //    new SqlParameter("@classid", classId),
        //    new SqlParameter("@sectionID", sectionId),
        //    new SqlParameter("@session", sessionId)
        //};
        //        #endregion

        //        #region Execute Query
        //        DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
        //        #endregion

        //        #region Map Results
        //        List<Transport> transportList = new List<Transport>();

        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            string stopId = dr["stopid"].ToString();
        //            string routeId = dr["rid"].ToString();

        //            transportList.Add(new Transport
        //            {
        //                Rollno = dr["RollNo"].ToString(),
        //                StudentName = dr["StudentName"].ToString(),
        //                RouteID = routeId,
        //                BusStopID = stopId,
        //                StudentinfoID = dr["StudentInfoID"].ToString(),
        //                PerminantAddress = dr["perminantAddress"].ToString(),
        //                PresentAddress = dr["presentAddress"].ToString(),
        //                PhoneNo = dr["phoneno"].ToString(),

        //                BusRate = await GetStopRateOnly(stopId, connectionString),
        //                RouteName = await GetRouteName(routeId, connectionString),
        //                BusStopName = await GetStopName(stopId, connectionString),
        //                Classname = await GetClassName(classId, connectionString),
        //                SectionName = await GetSectionName(sectionId, connectionString)
        //            });
        //        }

        //        if (transportList.Count > 0)
        //        {
        //            response.Status = 1;
        //            response.Message = "ok";
        //            response.ResponseData = transportList;
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Error Handling
        //        response.IsSuccess = false;
        //        response.Status = -1;
        //        response.Message = "Error: " + ex.Message;
        //        #endregion
        //    }

        //    return response;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopName"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStopListByName(string stopName, string clientId)
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
                #region Validate Parameter
                if (string.IsNullOrEmpty(stopName))
                {
                    response.Message = "Stop Name is null!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Parameters
                string query = @"
            SELECT BusStopID, RouteName, BusStop, BusRate
            FROM Transport
            INNER JOIN BusStops ON Transport.RouteID = BusStops.RouteID
            WHERE BusStop = @StopName AND IsPrimary = 1";

                SqlParameter sqlParam = new SqlParameter("@StopName", stopName);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParam);
                #endregion

                #region Map Result
                List<Transport> stopList = new List<Transport>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    stopList.Add(new Transport
                    {
                        BusStopID = dr["BusStopID"].ToString(),
                        RouteName = dr["RouteName"].ToString(),
                        BusStopName = dr["BusStop"].ToString(),
                        BusRate = dr["BusRate"].ToString()
                    });
                }

                if (stopList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = stopList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
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
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAllStops(string clientId)
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
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT BusStopID, BusStop, BusRate
            FROM BusStops
            INNER JOIN Transport ON Transport.RouteID = BusStops.RouteID
            WHERE IsPrimary = 1
            ORDER BY BusStop";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Map Results
                List<Transport> stopList = new List<Transport>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    stopList.Add(new Transport
                    {
                        BusStopID = dr["BusStopID"].ToString(),
                        BusStopName = dr["BusStop"].ToString(),
                        BusRate = dr["BusRate"].ToString()
                    });
                }

                if (stopList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = stopList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
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
        /// <param name="userId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetClassIdsAssigned(string userId, string clientId)
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
                if (string.IsNullOrEmpty(userId))
                {
                    response.Message = "User ID is null!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Params
                string query = "SELECT ClassIDS FROM Users WHERE UserID = @userid";
                SqlParameter param = new SqlParameter("@userid", userId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Results
                List<Transport> classList = new List<Transport>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    classList.Add(new Transport
                    {
                        Classids = dr["ClassIDS"].ToString()
                    });
                }

                if (classList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = classList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
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
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetAssignedSections(string param, string clientId)
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
                    response.Message = "Input is null!";
                    return response;
                }

                var parts = param.Split(',');
                if (parts.Length != 2)
                {
                    response.Message = "Invalid parameter format. Expected: classId,empCode";
                    return response;
                }

                string classId = parts[0];
                string empCode = parts[1];
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Params
                string query = @"SELECT SectionID FROM Sections WHERE ClassID = @classid AND EmpCode = @ecode";
                SqlParameter[] sqlParams =
                {
            new SqlParameter("@classid", classId),
            new SqlParameter("@ecode", empCode)
        };
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                #endregion

                #region Map Result
                List<Transport> sectionList = new List<Transport>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sectionList.Add(new Transport
                    {
                        SectionID = dr["SectionID"].ToString()
                    });
                }

                if (sectionList.Count > 0)
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = sectionList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
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
        /// <param name="sectionId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentBusReportListOnSectionID(string sectionId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found"
            };
            #endregion

            try
            {
                #region Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Query Setup
                SqlParameter param = new SqlParameter("@SecID", sectionId);
                string query = @"
            SELECT Students.StudentID, StudentInfoID, Students.AdmissionNo, StudentName, RollNo, PhoneNo, 
                   SectionName, PresentAddress, Classes.ClassName, RouteID, BusStopID
            FROM Students
            INNER JOIN StudentInfo ON StudentInfo.StudentId = Students.StudentID
            INNER JOIN Sections ON Sections.SectionID = StudentInfo.SectionID
            INNER JOIN Classes ON Classes.ClassId = Sections.ClassId
            WHERE StudentInfo.SectionID = @SecID";
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                var studentList = new List<StudentBusReport>();
                #endregion

                #region Map Results
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string routeId = dr["RouteID"]?.ToString() ?? "0";
                        string stopId = dr["BusStopID"]?.ToString() ?? "0";

                        var transport = await GetTransportOnID(routeId, connectionString);
                        var busStop = await GetBusStopOnID(stopId, connectionString);

                        studentList.Add(new StudentBusReport
                        {
                            SID = dr["StudentID"].ToString(),
                            StudentInfoID = dr["StudentInfoID"].ToString(),
                            AdminNo = dr["AdmissionNo"].ToString(),
                            StudentName = dr["StudentName"].ToString(),
                            RollNo = dr["RollNo"].ToString(),
                            SesctionName = dr["SectionName"].ToString(),
                            ClassName = dr["ClassName"].ToString(),
                            RouteID = routeId,
                            BusStopID = stopId,
                            RouteName = transport?.RouteName ?? "Not Assigned",
                            BusStopName = busStop?.BusStopName ?? "Not Assigned",
                            PhoneNo = dr["PhoneNo"].ToString(),
                            PresentAddress = dr["PresentAddress"].ToString(),
                            BusRate = busStop?.BusRate.ToString() ?? "0"
                        });
                    }

                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentList;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentListOnRouteID(string routeId, string clientId)
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
                if (string.IsNullOrEmpty(routeId))
                {
                    response.Message = "Route ID is required.";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = @"
            SELECT Students.StudentID, StudentInfoID, Students.AdmissionNo, StudentName, RollNo, PhoneNo,
                   SectionName, PresentAddress, Classes.ClassName, RouteID, BusStopID
            FROM Students
            INNER JOIN StudentInfo ON StudentInfo.StudentId = Students.StudentID
            INNER JOIN Sections ON Sections.SectionID = StudentInfo.SectionID
            INNER JOIN Classes ON Classes.ClassId = Sections.ClassId
            WHERE StudentInfo.RouteID = @RID";

                SqlParameter param = new SqlParameter("@RID", routeId);
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Results
                var studentList = new List<StudentBusReport>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    var routeIdVal = dr["RouteID"].ToString();
                    var stopIdVal = dr["BusStopID"].ToString();

                    // You can optionally make these parallel for performance using Task.WhenAll
                    var transport = await GetTransportOnID(routeIdVal, connectionString);
                    var stop = await GetBusStopOnID(stopIdVal, connectionString);

                    studentList.Add(new StudentBusReport
                    {
                        SID = dr["StudentID"].ToString(),
                        StudentInfoID = dr["StudentInfoID"].ToString(),
                        AdminNo = dr["AdmissionNo"].ToString(),
                        StudentName = dr["StudentName"].ToString(),
                        RollNo = dr["RollNo"].ToString(),
                        SesctionName = dr["SectionName"].ToString(),
                        ClassName = dr["ClassName"].ToString(),
                        RouteID = routeIdVal,
                        BusStopID = stopIdVal,
                        BusStopName = stop.BusNo,
                        PhoneNo = dr["PhoneNo"].ToString(),
                        PresentAddress = dr["PresentAddress"].ToString(),
                        RouteName = transport.RouteName,
                        BusRate = stop.BusRate.ToString()
                    });
                }

                if (studentList.Any())
                {
                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = studentList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
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
        /// <param name="classId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentBusRateClasswise(string classId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found!"
            };

            try
            {
                #region Validate Parameters
                if (string.IsNullOrEmpty(classId))
                {
                    response.Message = "Class ID is required!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                var connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query & Params
                string query = @"
            SELECT Students.studentid AS studentid, RollNo, studentname, StudentInfoID,
                   StudentInfo.RouteID AS rid, StudentInfo.BusStopId AS stopid,
                   Students.AdmissionNo, phoneno, presentAddress, perminantAddress,
                   classid, sectionid, busfee
            FROM StudentInfo
            INNER JOIN Students ON Students.studentid = StudentInfo.studentid
            WHERE ClassID = @classid AND isdischarged = 0
            ORDER BY sectionid";

                SqlParameter[] parameters = { new SqlParameter("@classid", classId) };
                #endregion

                #region Execute Query
                var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, parameters);
                #endregion

                #region Map Results
                List<Transport> transportList = new List<Transport>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string routeId = dr["rid"].ToString();
                        string stopId = dr["stopid"].ToString();

                        var transport = new Transport
                        {
                            Rollno = dr["Rollno"].ToString(),
                            StudentName = dr["StudentName"].ToString(),
                            RouteID = routeId,
                            BusStopID = stopId,
                            StudentinfoID = dr["StudentInfoID"].ToString(),
                            PerminantAddress = dr["perminantAddress"].ToString(),
                            PresentAddress = dr["presentAddress"].ToString(),
                            PhoneNo = dr["phoneno"].ToString(),
                            BusRate = dr["busfee"].ToString(),
                            RouteName = await GetRouteName(routeId, clientId),
                            BusStopName = await GetStopName(stopId, clientId),
                            Classname = await GetClassName(dr["classid"].ToString(), connectionString),
                            SectionName = await GetSectionName(dr["sectionid"].ToString(), connectionString)
                        };

                        transportList.Add(transport);
                    }

                    response.Status = 1;
                    response.Message = "ok";
                    response.ResponseData = transportList;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStudentBusRate(string param, string clientId)
        {
            ResponseModel response = new ResponseModel { IsSuccess = true, Status = 0, Message = "No Data Found" };
            List<TransportDTO> result = new List<TransportDTO>();

            try
            {
                // #region Parse Params
                var parts = param.Split(',');
                if (parts.Length != 2 || string.IsNullOrEmpty(parts[0]) || string.IsNullOrEmpty(parts[1]))
                {
                    response.Message = "Invalid parameters: classid, sectionid";
                    return response;
                }
                string classId = parts[0], sectionId = parts[1];
                // #endregion

                // #region Connection
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                var connectionString = connectionStringHelper.GetConnectionString(clientId);
                // #endregion

                // #region Query
                string query = @"SELECT Students.studentid AS studentid, RollNo, studentname, StudentInfoID, 
                            StudentInfo.RouteID AS rid, StudentInfo.BusStopId AS stopid, Students.AdmissionNo,
                            phoneno, presentAddress, perminantAddress, classid, sectionid, busfee
                            FROM StudentInfo 
                            INNER JOIN students ON students.studentid = StudentInfo.studentid 
                            WHERE ClassID = @classid AND SectionID = @sectionID AND isdischarged = 0 
                            ORDER BY rollno";

                SqlParameter[] sqlParams = {
                new SqlParameter("@classid", classId),
                new SqlParameter("@sectionID", sectionId)
            };

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, sqlParams);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        result.Add(new TransportDTO
                        {
                            Rollno = dr["Rollno"].ToString(),
                            StudentName = dr["StudentName"].ToString(),
                            RouteID = dr["rid"].ToString(),
                            BusStopID = dr["stopid"].ToString(),
                            StudentinfoID = dr["StudentInfoID"].ToString(),
                            PerminantAddress = dr["perminantAddress"].ToString(),
                            PresentAddress = dr["presentAddress"].ToString(),
                            PhoneNo = dr["phoneno"].ToString(),
                            BusRate = dr["busfee"].ToString(),
                            RouteName = await GetRouteName(dr["rid"].ToString(), connectionString),
                            BusStopName = await GetStopName(dr["stopid"].ToString(), connectionString),
                            Classname = await GetClassName(dr["classid"].ToString(), connectionString),
                            SectionName = await GetSectionName(dr["sectionid"].ToString(), connectionString)
                        });
                    }

                    response.Status = 1;
                    response.Message = "OK";
                    response.ResponseData = result;
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error: " + ex.Message;
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetTransportNameById(string routeId, string clientId)
        {
            var response = new ResponseModel();

            #region Validate Input
            if (string.IsNullOrEmpty(routeId))
            {
                response.IsSuccess = false;
                response.Message = "Invalid Route ID";
                response.ResponseData = "NA";
                return response;
            }
            #endregion

            #region Get Connection String
            var connectionStringHelper = new ConnectionStringHelper(_configuration);
            string connectionString = connectionStringHelper.GetConnectionString(clientId);
            #endregion

            try
            {
                #region Prepare Query and Parameters
                string query = "SELECT RouteName FROM Transport WHERE RouteID = @routeId";
                SqlParameter param = new SqlParameter("@routeId", routeId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string transportName = ds.Tables[0].Rows[0]["RouteName"].ToString();

                    response.IsSuccess = true;
                    response.Message = "Route name retrieved successfully";
                    response.ResponseData = transportName;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Route not found";
                    response.ResponseData = "NA";
                }
                #endregion
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
                response.ResponseData = "NA";
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> GetStopListWithLatLong(string routeId, string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Validate Input
                if (string.IsNullOrEmpty(routeId))
                {
                    response.Message = "Route ID is required!";
                    return response;
                }
                #endregion

                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region Prepare Query
                string query = "SELECT * FROM BusStops WHERE RouteID = @RouteID";
                SqlParameter param = new SqlParameter("@RouteID", routeId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                #endregion

                #region Map Results
                List<Transport> stopList = new List<Transport>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        stopList.Add(new Transport
                        {
                            BusStopID = dr["BusStopID"].ToString(),
                            BusStopName = dr["BusStop"].ToString(),
                            BusRate = dr["BusRate"].ToString(),
                            RouteID = dr["RouteID"].ToString(),
                            Current_Session = dr["Current_Session"].ToString(),
                            Distance = dr["Distance"].ToString(),
                            Latitude = dr["Latitude"].ToString(),
                            Longitude = dr["Longitude"].ToString()
                        });
                    }

                    response.Status = 1;
                    response.Message = "Success";
                    response.ResponseData = stopList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error
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
        /// <param name="classId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetClassName(string classId, string connectionString)
        {
            #region Validate
            if (string.IsNullOrEmpty(classId))
                return "Null";
            #endregion

            #region Query
            string query = "SELECT classname FROM classes WHERE classid = @classid";
            SqlParameter param = new SqlParameter("@classid", classId);
            #endregion

            #region Execute
            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["classname"].ToString() : "Null";
            #endregion
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetSectionName(string sectionId, string connectionString)
        {
            if (string.IsNullOrEmpty(sectionId))
                return "Null";

            string query = "SELECT sectionname FROM sections WHERE sectionid = @sectionid";
            SqlParameter param = new SqlParameter("@sectionid", sectionId);

            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["sectionname"].ToString() : "Null";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busStopId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetBusStopName(string busStopId, string connectionString)
        {
            if (string.IsNullOrEmpty(busStopId))
                return "0";

            string query = "SELECT busstop FROM busstops WHERE busstopid = @busstopid";
            SqlParameter param = new SqlParameter("@busstopid", busStopId);

            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["busstop"].ToString() : "0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busStopId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetStopRateOnly(string busStopId, string connectionString)
        {
            if (string.IsNullOrEmpty(busStopId))
                return "0";

            string query = "SELECT busrate FROM busstops WHERE busstopid = @busstopid";
            SqlParameter param = new SqlParameter("@busstopid", busStopId);

            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            return ds.Tables[0].Rows.Count > 0 ? ds.Tables[0].Rows[0]["busrate"].ToString() : "0";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busStopId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetBusStopRate(string busStopId, string connectionString)
        {
            if (string.IsNullOrEmpty(busStopId))
                return "0";

            string result = "0";

            string query = "SELECT busrate FROM busstops WHERE busstopid = @id";
            // SqlParameter param = new SqlParameter("@id", Convert.ToUInt64(busStopId));
            SqlParameter param = new SqlParameter("@id", SqlDbType.BigInt)
            {
                Value = Convert.ToInt64(busStopId)
            };
            DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);

            if (ds.Tables[0].Rows.Count > 0)
            {
                result = ds.Tables[0].Rows[0]["busrate"].ToString();
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetRouteName(string routeId, string connectionString)
        {
            #region Validate Input
            if (string.IsNullOrEmpty(routeId))
            {
                return "Not Assigned";
            }
            #endregion

            try
            {
                #region SQL Query
                if (!Int64.TryParse(routeId, out long routeNumericId))
                {
                    return "Invalid Route ID";
                }

                string query = "SELECT RouteName FROM Transport WHERE RouteID = @routeId";
                SqlParameter param = new SqlParameter("@routeId", routeNumericId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0]["RouteName"].ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "Not Assigned";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<string> GetStopName(string stopId, string connectionString)
        {
            #region Input Validation
            if (string.IsNullOrEmpty(stopId))
            {
                return "Not Assigned";
            }
            #endregion

            try
            {
                #region SQL Query Setup
                if (!Int64.TryParse(stopId, out long stopNumericId))
                {
                    return "Invalid Stop ID";
                }

                string query = "SELECT BusStop FROM BusStops WHERE BusStopID = @stopId";
                SqlParameter param = new SqlParameter("@stopId", stopNumericId);
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    return ds.Tables[0].Rows[0]["BusStop"].ToString();
                }
                #endregion
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return "Not Assigned";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<Transport> GetTransportOnID(string routeId, string connectionString)
        {
            if (string.IsNullOrEmpty(routeId)) return null;

            string query = "SELECT * FROM Transport WHERE RouteID = @routeId";
            SqlParameter param = new SqlParameter("@routeId", routeId);

            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                return new Transport
                {
                    RouteID = dr["RouteID"].ToString(),
                    RouteName = dr["RouteName"].ToString(),
                    VehicleNo = dr["VehicleNo"].ToString(),
                    DriverName = dr["DriverName"].ToString(),
                    ConducterName = dr["ConductorName"].ToString(),
                    ConducterPhone = dr["conductorphone"].ToString()
                    // Add more as needed
                };
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopId"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private async Task<BusStop> GetBusStopOnID(string stopId, string connectionString)
        {
            if (string.IsNullOrEmpty(stopId)) return null;

            string query = "SELECT * FROM BusStops WHERE Removed = 0 AND BusStopID = @BusStopID";
            SqlParameter param = new SqlParameter("@BusStopID", stopId);

            var ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query, param);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var dr = ds.Tables[0].Rows[0];
                return new BusStop
                {
                    BusStopID = Convert.ToInt64(dr["BusStopID"]),
                    BusStopName = dr["BusStop"].ToString(),
                    BusRate = Convert.ToDecimal(dr["BusRate"]),
                    RouteID = Convert.ToInt64(dr["RouteID"]),
                    BusNo = dr["RouteID"].ToString(), // Replace with proper lookup if needed
                    Current_Session = dr["Current_Session"].ToString()
                };
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public async Task<ResponseModel> getTransportList(string clientId)
        {
            #region Initialize Response
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "No Data Found",
                ResponseData = null
            };
            #endregion

            try
            {
                #region Get Connection String
                var connectionStringHelper = new ConnectionStringHelper(_configuration);
                string connectionString = connectionStringHelper.GetConnectionString(clientId);
                #endregion

                #region SQL Query
                string query = "SELECT * FROM Transport";
                #endregion

                #region Execute Query
                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, query);
                #endregion

                #region Map Results
                List<Transport> transportList = new List<Transport>();

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        transportList.Add(new Transport
                        {
                            RouteID = dr["RouteID"].ToString(),
                            RouteName = dr["RouteName"].ToString(),
                            RouteStops = dr["RouteStops"].ToString(),
                            VehicleNo = dr["VehicleNo"].ToString(),
                            DriverName = dr["DriverName"].ToString(),
                            RouteCost = dr["RouteCost"].ToString(),
                            DateOfStart = dr["DateOfStart"] != DBNull.Value ? Convert.ToDateTime(dr["DateOfStart"]).ToString("dd/MM/yyyy") : string.Empty,

                            Current_Session = dr["Current_Session"].ToString(),
                            SessionID = dr["SessionID"].ToString(),
                            Remarks = dr["Remarks"].ToString(),
                            SeatingCapacity = string.IsNullOrEmpty(dr["SeatingCapacity"].ToString()) ? "0" : dr["SeatingCapacity"].ToString(),
                            ftnsExpr = dr["ftnsExpr"] != DBNull.Value ? Convert.ToDateTime(dr["ftnsExpr"]).ToString("dd/MM/yyyy") : string.Empty,
                            // ftnsExpr = dr["ftnsExpr"].ToString()
                            // Uncomment if needed:
                            // InsExp = dr["InsExp"].ToString(),
                            // TokenExp = dr["TokenExp"].ToString(),
                            // PermitExp = dr["PermitExp"].ToString(),
                            // UserName = dr["UserName"].ToString(),
                            // isDeleted = Convert.ToBoolean(dr["isDeleted"]),
                            // pollutionExpr = dr["pollutionExpr"].ToString(),
                            // driverPhone = dr["driverPhone"].ToString(),
                            // BusType = dr["BusType"].ToString(),
                            // ConducterName = dr["ConducterName"].ToString(),
                            // ConducterPhone = dr["ConducterPhone"].ToString(),
                        });
                    }

                    response.Status = 1;
                    response.Message = "Success";
                    response.ResponseData = transportList;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Error Handling
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
        /// <param name="routeId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> deleteTransport(string routeId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid request"
            };

            string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);

            try
            {
                int val = 0;
                SqlParameter para = new SqlParameter("@TID", routeId);

                string checkQuery = @"
            SELECT COUNT(*) AS val FROM BusStops WHERE RouteID = @TID;
            SELECT COUNT(*) AS val FROM StudentInfo WHERE RouteID = @TID;";

                DataSet ds = await SQLHelperCore.ExecuteDatasetAsync(connectionString, CommandType.Text, checkQuery, para);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    val += Convert.ToInt32(ds.Tables[0].Rows[0]["val"]);

                if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    val += Convert.ToInt32(ds.Tables[1].Rows[0]["val"]);

                if (val > 0)
                {
                    response.Message = "This record can't be deleted as it is used in other files!";
                    return response;
                }

                // Safe to delete
                SqlParameter deleteParam = new SqlParameter("@TID", routeId);
                string updateQuery = "UPDATE Transport SET isDeleted = 1 WHERE RouteID = @TID";

                int deleted = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, updateQuery, deleteParam);

                if (deleted > 0)
                {
                    response.Status = 1;
                    response.Message = "Bus Deleted Successfully";
                }
                else
                {
                    response.Message = "Bus not deleted or doesn't exist";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Error while deleting transport";
                response.Error = ex.Message;
            }

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="busStopId"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>

        public async Task<ResponseModel> DeleteBusStop(string busStopId, string clientId)
        {
            var response = new ResponseModel
            {
                IsSuccess = true,
                Status = 0,
                Message = "Invalid request"
            };

            try
            {
                if (string.IsNullOrEmpty(busStopId))
                {
                    response.Message = "BusStopID is required.";
                    return response;
                }

                string connectionString = new ConnectionStringHelper(_configuration).GetConnectionString(clientId);

                SqlParameter para = new SqlParameter("@BusStopID", busStopId);
                string query = "UPDATE BusStops SET Removed = 1 WHERE BusStopID = @BusStopID";

                int rowsAffected = await SQLHelperCore.ExecuteNonQueryAsync(connectionString, CommandType.Text, query, para);

                if (rowsAffected > 0)
                {
                    response.IsSuccess = true;
                    response.Status = 1;
                    response.Message = "Bus Stop marked as deleted successfully.";
                }
                else
                {
                    response.Message = "Bus Stop not found or already removed.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Status = -1;
                response.Message = "Exception occurred while deleting bus stop.";
                response.Error = ex.Message;
            }

            return response;
        }

    }
}


