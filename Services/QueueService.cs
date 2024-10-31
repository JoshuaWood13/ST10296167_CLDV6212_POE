using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ST10296167_CLDV6212_POE.Services
{
    public class QueueService
    {
        private readonly QueueServiceClient _queueServiceClient;
        private readonly IConfiguration _configuration;

        public QueueService(IConfiguration configuration)
        {
            _queueServiceClient = new QueueServiceClient(configuration["AzureStorage:ConnectionString"]);
            _configuration = configuration;
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task InsertOrderInfoDbAsync(string orderID, string customerID)
        {
            var connectionString = _configuration["ConnectionString:AzureDatabase"];
            var query = @"INSERT INTO OrderInfoTable (OrderID, CustomerID) VALUES (@OrderID, @CustomerID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@CustomerID", customerID);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
