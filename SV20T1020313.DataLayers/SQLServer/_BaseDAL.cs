using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.DataLayers.SQLServer
{
   /// <summary>
   /// Lớp cha cho các lớp cài đặt các phép xử lý dữ liệu trên sql server
   /// </summary>
    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        public _BaseDAL(string connectionString) // hàm tạo
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Tạo và kết nối đến CSDL
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection() // hàm protected => có thể sử dụng bởi các lớp con (các lớp kết thừa) của nó
        {
            SqlConnection connection = new SqlConnection(); // của thư viện microsoft.data.sqlClient
            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
    }
}
