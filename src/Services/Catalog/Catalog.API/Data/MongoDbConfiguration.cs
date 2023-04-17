using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Data
{
    public class MongoDbConfiguration
    {
        /// <summary>
        /// Gets or sets database name.
        /// </summary>
        [Required]
        public string DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets database connection string.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets database connection string.
        /// </summary>
        [Required]
        public string CollectionName { get; set; }
    }
}
