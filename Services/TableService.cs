using Azure;
using Azure.Data.Tables;
using ST10296167_CLDV6212_POE.Models;
using System.Threading.Tasks;

namespace ST10296167_CLDV6212_POE.Services
{
    public class TableService
    {
        private readonly TableClient _customerTableClient;
        private readonly TableClient _productsTableClient;

        public TableService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var serviceClient = new TableServiceClient(connectionString);

            _customerTableClient = serviceClient.GetTableClient("CustomerProfiles");
            _customerTableClient.CreateIfNotExists();

            _productsTableClient = serviceClient.GetTableClient("Products");
            _productsTableClient.CreateIfNotExists();
        }

        //public async Task AddEntityAsync(CustomerProfile profile)
        //{
        //    await _tableClient.AddEntityAsync(profile);
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task AddEntityAsync(CustomerProfile profile)
        {
            // Get the current max customer ID from the table
            var existingProfile = _customerTableClient.Query<CustomerProfile>()
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
            await _customerTableClient.AddEntityAsync(profile);
        }

        public async Task AddProductAsync(Products product)
        {
            // Get the current max ProductID from the table
            var existingProduct = _productsTableClient.Query<Products>()
                .OrderByDescending(p => p.ProductID)
                .FirstOrDefault();

            // Determine the max ProductID
            int maxProductID = existingProduct?.ProductID ?? 0;

            // Increment the ID by 1
            product.ProductID = maxProductID + 1;

            // Add the new product
            await _productsTableClient.AddEntityAsync(product);
        }
    }
}
