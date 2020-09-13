using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace ADO.net.Services
{
    public class ConnectedMode : BaseService
    {
        string _connStr;

        public ConnectedMode()
        {
            _connStr = ConfigurationManager.ConnectionStrings["ADODB"].ConnectionString;
        }

        public void AddProducts()
        {
            Random random = new Random((int)DateTime.Today.Ticks);
            Func<DateTime> getExpiryDate = () => DateTime.Today.AddDays(random.Next(-30, 60));
            string[] products = new string[] { "Danon", "Chips", "Milk", "Chocolates" };

            SqlConnection connection = new SqlConnection(_connStr);
            using (connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Product (name, expirydate) values (@n, @e)";

                command.Parameters.Add("@n", SqlDbType.VarChar);
                command.Parameters.Add("@e", SqlDbType.DateTime);

                command.Transaction = connection.BeginTransaction();

                for (int i = 0; i < products.Length; i++)
                {
                    command.Parameters["@n"].Value = products[i];
                    command.Parameters["@e"].Value = getExpiryDate();
                    command.ExecuteNonQuery();
                }

                command.Transaction.Commit();
            }

            WriteInGreen("Inserted rows");
        }

        public void GetProducts()
        {
            SqlConnection connection = new SqlConnection(_connStr);
            using (connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Product";
                var data = new DataTable();
                data.Load(command.ExecuteReader());
                WriteInGreen($"Got {data.Rows.Count} rows");
            }
        }

        public void UpdateProducts()
        {
            SqlConnection connection = new SqlConnection(_connStr);
            using (connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.Transaction = connection.BeginTransaction();
                command.CommandText = "UPDATE Product set expirydate=@d where datediff(day, GETDATE(), expirydate) > 15";
                command.Parameters.AddWithValue("@d", DateTime.Today.AddDays(7));
                command.ExecuteNonQuery();
                command.Transaction.Commit();
            }

            WriteInGreen("Updated Rows");
        }

        public void DeleteProducts()
        {
            SqlConnection connection = new SqlConnection(_connStr);
            using (connection)
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Product";
                command.ExecuteNonQuery();
            }

            WriteInGreen("Deleted Rows");
        }
    }
}
