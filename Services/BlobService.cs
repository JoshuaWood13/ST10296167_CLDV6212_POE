using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

using System.Net.Http.Headers;
using System.Data.SqlClient;

namespace ST10296167_CLDV6212_POE.Services
{
    public class BlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureStorage:ConnectionString"]);
            _configuration = configuration;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles getting the correct Azure Blob Container and uploading a blob
        public async Task UploadBlobAsync(string containerName, string blobName, Stream content)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, true);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // This method handles uploading blob data to the SQl db
        public async Task InsertBlobDbAsync(byte[] imageData, string imageName)
        {
            var connectionString = _configuration["ConnectionString:AzureDatabase"];
            var query = @"INSERT INTO ProductImgTable (ImageName, ImageData) VALUES (@Name, @Image)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", imageName);
                command.Parameters.AddWithValue("@Image", imageData);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//