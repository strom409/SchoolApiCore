using Microsoft.Data.SqlClient;
using System.Collections;
using System.Data;

namespace Transport.Repository.SQL
{
    public class SQLHelperCore
    {
        public static string Connect { get; }
        public static string Connect2 { get; }


        #region private utility methods & constructors

        // Since this class provides only static methods, make the default constructor private to prevent 
        // instances from being created with "new SqlHelper()"
        static SQLHelperCore()
        {
            var configuration = new ConfigurationBuilder()
                   .SetBasePath(AppContext.BaseDirectory) // Ensure it looks in the application's directory
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
                   .Build();

            Connect = configuration.GetConnectionString("Constr"); // Access the connection string
        }

        /// <summary>
        /// This method is used to attach an array of SqlParameters to a SqlCommand.
        /// </summary>
        /// <param name="command">The command to which the parameters will be added</param>
        /// <param name="commandParameters">An array of SqlParameters to be added to command</param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (commandParameters != null)
            {
                foreach (var p in commandParameters.Where(p => p != null))
                {
                    if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    command.Parameters.Add(p);
                }
            }
        }

        /// <summary>
        /// Assigns DataRow column values to an array of SqlParameters.
        /// </summary>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if (commandParameters == null || dataRow == null) return;

            for (int i = 0; i < commandParameters.Length; i++)
            {
                var commandParameter = commandParameters[i];
                if (string.IsNullOrEmpty(commandParameter.ParameterName) || commandParameter.ParameterName.Length <= 1)
                {
                    throw new Exception($"Invalid parameter name at index {i}: '{commandParameter.ParameterName}'");
                }

                string columnName = commandParameter.ParameterName.Substring(1);
                if (dataRow.Table.Columns.Contains(columnName))
                {
                    commandParameter.Value = dataRow[columnName];
                }
            }
        }

        /// <summary>
        /// Assigns an array of values to an array of SqlParameters.
        /// </summary>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if (commandParameters == null || parameterValues == null) return;

            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Parameter Value count.");
            }

            for (int i = 0; i < commandParameters.Length; i++)
            {
                commandParameters[i].Value = parameterValues[i] switch
                {
                    IDbDataParameter paramInstance when paramInstance.Value == null => DBNull.Value,
                    null => DBNull.Value,
                    _ => parameterValues[i]
                };
            }
        }

        /// <summary>
        /// Asynchronously opens a connection, assigns a transaction, command type, and parameters to the provided command.
        /// </summary>
        private static async Task PrepareCommandAsync(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (string.IsNullOrEmpty(commandText)) throw new ArgumentNullException(nameof(commandText));

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            command.Connection = connection;
            command.CommandText = commandText;
            command.Transaction = transaction ?? throw new ArgumentException("The transaction was rolled back or committed. Please provide an open transaction.", nameof(transaction));
            command.CommandType = commandType;

            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }
        #endregion private utility methods & constructors

        #region ExecuteNonQuery (Async)

        public static async Task<int> ExecuteNonQueryAsync(string connectionString, CommandType commandType, string commandText)
        {
            return await ExecuteNonQueryAsync(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static async Task<int> ExecuteNonQueryAsync(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            await using SqlConnection connection = new SqlConnection(connectionString);
            await connection.OpenAsync(); // ✅ Use OpenAsync() for non-blocking connection

            return await ExecuteNonQueryAsync(connection, commandType, commandText, commandParameters);
        }

        public static async Task<int> ExecuteNonQueryAsync(SqlConnection connection, CommandType commandType, string commandText)
        {
            return await ExecuteNonQueryAsync(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static async Task<int> ExecuteNonQueryAsync(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            await using SqlCommand cmd = new SqlCommand(commandText, connection)
            {
                CommandType = commandType
            };

            if (commandParameters != null)
            {
                foreach (var param in commandParameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value });
                }
            }

            return await cmd.ExecuteNonQueryAsync(); // ✅ Use ExecuteNonQueryAsync()
        }

        public static async Task<int> ExecuteNonQueryAsync(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null || transaction.Connection == null)
                throw new ArgumentException("Transaction is null or has been committed/rolled back.", nameof(transaction));

            await using SqlCommand cmd = new SqlCommand(commandText, transaction.Connection, transaction)
            {
                CommandType = commandType
            };

            if (commandParameters != null)
            {
                foreach (var param in commandParameters)
                {
                    cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.SqlDbType) { Value = param.Value });
                }
            }

            return await cmd.ExecuteNonQueryAsync(); // ✅ Use ExecuteNonQueryAsync()
        }

        #endregion ExecuteNonQuery (Async)


        #region ExecuteDataset
        public static async Task<DataSet> ExecuteDatasetAsync(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await ExecuteDatasetAsync(connection, commandType, commandText, commandParameters);
            }
        }

        public static async Task<DataSet> ExecuteDatasetAsync(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            using (SqlCommand cmd = new SqlCommand(commandText, connection))
            {
                cmd.CommandType = commandType;
                if (commandParameters != null)
                    cmd.Parameters.AddRange(commandParameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    await Task.Run(() => da.Fill(ds)); // Using Task.Run to keep Fill operation async-compatible
                    return ds;
                }
            }
        }

        public static async Task<DataSet> ExecuteDatasetAsync(string connectionString, string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return await ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return await ExecuteDatasetAsync(connectionString, CommandType.StoredProcedure, spName);
            }
        }
        public static async Task<DataSet> ExecuteDatasetAsync(SqlConnection connection, CommandType commandType, string commandText)
        {
            return await ExecuteDatasetAsync(connection, commandType, commandText, (SqlParameter[])null);
        }


        public static async Task<DataSet> ExecuteDatasetAsync(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return await ExecuteDatasetAsync(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static async Task<DataSet> ExecuteDatasetAsync(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null || transaction.Connection == null) throw new ArgumentException("The transaction is not valid or has been committed/rolled back.", nameof(transaction));

            using (SqlCommand cmd = new SqlCommand())
            {
                bool mustCloseConnection;
                PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    await Task.Run(() => da.Fill(ds));
                    cmd.Parameters.Clear();
                    return ds;
                }
            }
        }

        public static async Task<DataSet> ExecuteDatasetAsync(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null || transaction.Connection == null) throw new ArgumentException("The transaction is not valid or has been committed/rolled back.", nameof(transaction));
            if (string.IsNullOrEmpty(spName)) throw new ArgumentNullException(nameof(spName));

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return await ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return await ExecuteDatasetAsync(transaction, CommandType.StoredProcedure, spName);
            }
        }


        public sealed class SqlHelperParameterCache
        {
            #region private methods, variables, and constructors

            //Since this class provides only static methods, make the default constructor private to prevent 
            //instances from being created with "new SqlHelperParameterCache()"
            private SqlHelperParameterCache() { }

            private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

            /// <summary>
            /// Resolve at run time the appropriate set of SqlParameters for a stored procedure
            /// </summary>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">Whether or not to include their return value parameter</param>
            /// <returns>The parameter array discovered.</returns>
            private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;

                connection.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
                connection.Close();

                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }

                SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];

                cmd.Parameters.CopyTo(discoveredParameters, 0);

                // Init the parameters with a DBNull value
                foreach (SqlParameter discoveredParameter in discoveredParameters)
                {
                    discoveredParameter.Value = DBNull.Value;
                }
                return discoveredParameters;
            }

            /// <summary>
            /// Deep copy of cached SqlParameter array
            /// </summary>
            /// <param name="originalParameters"></param>
            /// <returns></returns>
            private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
            {
                SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length];

                for (int i = 0, j = originalParameters.Length; i < j; i++)
                {
                    clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
                }

                return clonedParameters;
            }

            #endregion private methods, variables, and constructors

            #region caching functions

            /// <summary>
            /// Add parameter array to the cache
            /// </summary>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="commandText">The stored procedure name or T-SQL command</param>
            /// <param name="commandParameters">An array of SqlParamters to be cached</param>
            public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

                string hashKey = connectionString + ":" + commandText;

                paramCache[hashKey] = commandParameters;
            }

            /// <summary>
            /// Retrieve a parameter array from the cache
            /// </summary>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="commandText">The stored procedure name or T-SQL command</param>
            /// <returns>An array of SqlParamters</returns>
            public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

                string hashKey = connectionString + ":" + commandText;

                SqlParameter[] cachedParameters = paramCache[hashKey] as SqlParameter[];
                if (cachedParameters == null)
                {
                    return null;
                }
                else
                {
                    return CloneParameters(cachedParameters);
                }
            }

            #endregion caching functions

            #region Parameter Discovery Functions

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
            {
                return GetSpParameterSet(connectionString, spName, false);
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connectionString">A valid connection string for a SqlConnection</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
            {
                if (connectionString == null || connectionString.Length == 0) throw new ArgumentNullException("connectionString");
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
                }
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
            {
                return GetSpParameterSet(connection, spName, false);
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <remarks>
            /// This method will query the database for this information, and then store it in a cache for future requests.
            /// </remarks>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                using (SqlConnection clonedConnection = (SqlConnection)((ICloneable)connection).Clone())
                {
                    return GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
                }
            }

            /// <summary>
            /// Retrieves the set of SqlParameters appropriate for the stored procedure
            /// </summary>
            /// <param name="connection">A valid SqlConnection object</param>
            /// <param name="spName">The name of the stored procedure</param>
            /// <param name="includeReturnValueParameter">A bool value indicating whether the return value parameter should be included in the results</param>
            /// <returns>An array of SqlParameters</returns>
            private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                if (connection == null) throw new ArgumentNullException("connection");
                if (spName == null || spName.Length == 0) throw new ArgumentNullException("spName");

                string hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");

                SqlParameter[] cachedParameters;

                cachedParameters = paramCache[hashKey] as SqlParameter[];
                if (cachedParameters == null)
                {
                    SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                    paramCache[hashKey] = spParameters;
                    cachedParameters = spParameters;
                }

                return CloneParameters(cachedParameters);
            }

            #endregion Parameter Discovery Functions
        }

        #endregion
    }
}
