using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {
        public CatalogContext(MongoDbConfiguration dbConfiguration)
        {
            var client = new MongoClient(dbConfiguration.ConnectionString);
            var database = client.GetDatabase(dbConfiguration.DatabaseName);
            Products = database.GetCollection<Product>(dbConfiguration.CollectionName);
            CatalogContextSeed.SeedDataba(Products);
        }

        public IMongoCollection<Product> Products { get; }
    }
}
