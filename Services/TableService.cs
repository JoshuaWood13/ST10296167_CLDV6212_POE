using Azure;
using Azure.Data.Tables;
using ST10296167_CLDV6212_POE.Models;
using System.Threading.Tasks;

namespace ST10296167_CLDV6212_POE.Services
{
    public class TableService
    {
        private readonly TableClient _tableClient;

        public TableService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var serviceClient = new TableServiceClient(connectionString);
            _tableClient = serviceClient.GetTableClient("CustomerProfiles");
            _tableClient.CreateIfNotExists();
        }

        //public async Task AddEntityAsync(CustomerProfile profile)
        //{
        //    await _tableClient.AddEntityAsync(profile);
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task AddEntityAsync(CustomerProfile profile)
        {
            // Get the current max customer ID from the table
            var existingProfile = _tableClient.Query<CustomerProfile>()
                .OrderByDescending(p => p.CustomerID)
                .FirstOrDefault();

            // Determine the max customer ID
            int maxCustomerID;
            if (existingProfile == null)
            {
                maxCustomerID = 0; 
            }
            else
            {
                maxCustomerID = existingProfile.CustomerID;
            }

            // Increment the ID by 1
            profile.CustomerID = maxCustomerID + 1;

            // Then add the new profile
            await _tableClient.AddEntityAsync(profile);
        }
    }
}
