using Azure;
using Azure.Data.Tables;
using System;

namespace ST10296167_CLDV6212_POE.Models
{
    public class Products : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        // Custom properties for product details
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string ProductCategory { get; set; }

        public Products()
        {
            PartitionKey = "Products";
            RowKey = Guid.NewGuid().ToString();
        }
    }
}
