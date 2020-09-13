using ADO.net.EF;
using System;
using System.Data.Entity;
using System.Linq;

namespace ADO.net.Services
{
    public class EntityFramwork : BaseService
    {
        ADODBContext _database = new ADODBContext();

        public void AddProducts()
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            Func<DateTime> getExpiryDate = () => DateTime.Today.AddDays(random.Next(-30, 60));

            _database.Products.AddRange(new Product[]
            {
                new Product {name = "Danon", expirydate = getExpiryDate()},
                new Product {name = "Chips", expirydate = getExpiryDate()},
                new Product {name = "Chocolates", expirydate = getExpiryDate()},
                new Product {name = "Milk", expirydate = getExpiryDate()},
            });

            WriteLineInGreen($"Inserted {_database.SaveChanges()} rows");
        }

        public void GetProducts()
        {
            WriteLineInGreen($"Got {_database.Products.ToList().Count} rows");
        }

        public void UpdateProducts()
        {
            _database.Products
                .Where(p => DbFunctions.DiffDays(DateTime.Today, p.expirydate) > 15)
                .ToList()
                .ForEach(product => product.expirydate = DateTime.Today.AddDays(7));

            WriteLineInGreen($"Updated {_database.SaveChanges()} rows");
        }

        public void DeleteProducts()
        {
            _database.Products.RemoveRange(_database.Products);
            WriteLineInGreen($"Deleted {_database.SaveChanges()} rows");
        }
    }
}
