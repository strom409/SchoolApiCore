namespace Examination_Management.Repository
{
    public class ConnectionStringHelper
    {
        private readonly IConfiguration _configuration;
        public ConnectionStringHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId)) return null;

            return clientId.ToLower() switch
            {
                "client1" => _configuration.GetConnectionString("Client1"),
                "client2" => _configuration.GetConnectionString("Client2"),
                _ => null
            };
        }
    }
}
