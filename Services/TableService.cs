using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using ST10296167_CLDV6212_POE.Models;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ST10296167_CLDV6212_POE.Services
{
    public class TableService
    {
        private readonly TableClient _customerTableClient;
        private readonly TableClient _productsTableClient;
        private readonly IConfiguration _configuration;

        public TableService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var serviceClient = new TableServiceClient(connectionString);

            _customerTableClient = serviceClient.GetTableClient("CustomerProfiles");
            _customerTableClient.CreateIfNotExists();

            _productsTableClient = serviceClient.GetTableClient("Products");
            _productsTableClient.CreateIfNotExists();

            _configuration = configuration;
        }

//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task AddCustomerAsync(CustomerProfile profile)
        {
            // Get the current max customer ID from the Azure table
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

            // Add the new profile
            await _customerTableClient.AddEntityAsync(profile);
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task AddProductAsync(Products product)
        {
            // Get the current max Product ID the Azure table
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
//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task InsertCustomerDb(CustomerProfile profile)
        {
            var connectionString = _configuration["ConnectionString:AzureDatabase"];
            var query = @"INSERT INTO CustomerTable (FirstName, LastName, Email, PhoneNumber)
                          VALUES (@FirstName, @LastName, @Email, @PhoneNumber)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", profile.FirstName);
                command.Parameters.AddWithValue("@LastName", profile.LastName);
                command.Parameters.AddWithValue("@Email", profile.Email);
                command.Parameters.AddWithValue("@PhoneNumber", profile.PhoneNumber);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
        public async Task InsertProductDb(Products product)
        {
            var connectionString = _configuration["ConnectionString:AzureDatabase"];
            var query = @"INSERT INTO ProductTable (Name, Price, Category)
                          VALUES (@ProductName, @ProductPrice, @ProductCategory)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@ProductPrice", product.ProductPrice);
                command.Parameters.AddWithValue("@ProductCategory", product.ProductCategory);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
//------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
