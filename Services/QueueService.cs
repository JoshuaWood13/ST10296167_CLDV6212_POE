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

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public QueueService(IConfiguration configuration)
        {
            _queueServiceClient = new QueueServiceClient(configuration["AzureStorage:ConnectionString"]);
            _configuration = configuration;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles uploading a message to the correct Azure Queue
        public async Task SendMessageAsync(string queueName, string message)
        {
            var queueClient = _queueServiceClient.GetQueueClient(queueName);
            await queueClient.CreateIfNotExistsAsync();
            await queueClient.SendMessageAsync(message);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles storing select data from the queue message to the SQL db
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
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//