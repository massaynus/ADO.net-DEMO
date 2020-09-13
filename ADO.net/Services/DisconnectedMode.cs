using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace ADO.net.Services
{
    public class DisconnectedMode : BaseService
    {
        string _connStr;
        SqlDataAdapter _adapter;
        DataTable _product;
        SqlConnection _connection;

        Form _form;

        public DisconnectedMode()
        {
            _connStr = ConfigurationManager.ConnectionStrings["ADODB"].ConnectionString;

            _product = new DataTable("Product");
            _connection = new SqlConnection(_connStr);
            _adapter = new SqlDataAdapter("SELECT * FROM Product", _connection);
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder(_adapter);

            _adapter.Fill(_product);

            _form = new Form() 
            { 
                WindowState = FormWindowState.Maximized, 
                Font = new Font("Century Gothic", 17.0f) ,
                Text = "Insert / Update or Delete datat as you wish"
            };
            BindingSource bindingSource = new BindingSource() { DataSource = _product };
            DataGridView dgv1 = new DataGridView()
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                DataSource = bindingSource
            };

            _form.Controls.Add(dgv1);
        }

        public void AddProducts()
        {
            Random random = new Random((int)DateTime.Today.Ticks);
            Func<DateTime> getExpiryDate = () => DateTime.Today.AddDays(random.Next(-30, 60));
            string[] products = new string[] { "Danon", "Chips", "Milk", "Chocolates" };

            for (int i = 0; i < products.Length; i++)
            {
                var row = _product.NewRow();
                row["name"] = products[i];
                row["expirydate"] = getExpiryDate();
                _product.Rows.Add(row);
            }

            WriteInGreen($"Inserted {_adapter.Update(_product)} rows");
            _product.Rows.Clear();
            _adapter.Fill(_product);
        }

        [STAThread]
        public void GetProducts()
        {
            _product.Rows.Clear();
            WriteInGreen($"Got {_adapter.Fill(_product)} rows");
            _form.ShowDialog();
        }

        public void UpdateDatabase()
        {
            WriteInGreen($"Changed {_adapter.Update(_product)} rows");
            _product.Rows.Clear();
            _adapter.Fill(_product);
        }
    }
}
