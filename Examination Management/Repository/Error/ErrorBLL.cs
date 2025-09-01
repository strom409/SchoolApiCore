

using Microsoft.Data.SqlClient;

namespace Examination_Management.Repository.Error
{
    public class ErrorBLL
    {
        private static IServiceProvider? _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        // ✅ Constructor for DI-based instance methods
        public ErrorBLL(IWebHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }

        public object SqlHelper { get; private set; }

        // ✅ Configure DI container (Call this in Program.cs)
        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // ✅ Static method for error logging
        public static void CreateErrorLog(string title, string pageName, string error)
        {
            if (_serviceProvider == null)
                throw new Exception("ErrorBLL is not configured. Call Configure() in Program.cs");

            using (var scope = _serviceProvider.CreateScope())
            {
                var errorBLL = scope.ServiceProvider.GetRequiredService<ErrorBLL>();

                var log = new ErrorLog
                {
                    Title = title,
                    PageName = pageName,
                    Error = error
                };

                errorBLL.LogError(log);
            }
        }

        // ✅ Overloaded method to accept an ErrorLog object
        public static void CreateErrorLog(ErrorLog errorLog)
        {
            if (_serviceProvider == null)
                throw new Exception("ErrorBLL is not configured. Call Configure() in Program.cs");

            using (var scope = _serviceProvider.CreateScope())
            {
                var errorBLL = scope.ServiceProvider.GetRequiredService<ErrorBLL>();
                errorBLL.LogError(errorLog);
            }
        }

        // ✅ Instance method for logging (called internally)
        public string LogError(ErrorLog el)
        {
            try
            {
                // Define log directory inside the application root
                string logPath = Path.Combine(_env.ContentRootPath, "ErrorLog");
                if (!Directory.Exists(logPath))
                    Directory.CreateDirectory(logPath);

                // Create Unique File Name
                string fileName = $"{DateTime.Now:ddMMyy_hhmmss}_{el.Title.Replace(' ', '_').Trim()}.txt";
                string filePath = Path.Combine(logPath, fileName);

                // Save error to text file
                File.WriteAllText(filePath, el.Error);

                // Create the SQL connection
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("YourConnectionStringName"))) // Update with your connection string name
                {
                    connection.Open();

                    // Prepare the SQL query
                    string sql = "INSERT INTO ErrorLogs (Title, PageName, Error, EDate, ETime, FilePath, Module) " +
                                 "VALUES (@Title, @PageName, @Error, @EDate, @ETime, @FilePath, @Module)";

                    // Create the command
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Title", el.Title);
                        command.Parameters.AddWithValue("@PageName", el.PageName);
                        command.Parameters.AddWithValue("@Error", el.Error);
                        command.Parameters.AddWithValue("@EDate", DateTime.Now.ToString("MM-dd-yyyy"));
                        command.Parameters.AddWithValue("@ETime", DateTime.Now.ToString("h:mm tt"));
                        command.Parameters.AddWithValue("@FilePath", fileName);
                        command.Parameters.AddWithValue("@Module", el.Module);

                        // Execute the command
                        int result = command.ExecuteNonQuery();

                        return result > 0 ? "Success" : "Failed to log error";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }
}
