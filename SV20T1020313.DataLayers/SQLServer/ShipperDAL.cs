using Dapper;
using SV20T1020313.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.DataLayers.SQLServer
{
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        public ShipperDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Shipper data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"begin
                                    insert into Shippers(ShipperName,Phone)
                                    values(@ShipperName,@Phone);

                                    select @@identity;
                            end";
                // nếu tồn tại email trùng lặp => trả về -1
                // @@identity lấy ra id vừa được sinh ra
                var parameters = new
                {
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                };
                // thực thi câu lệnh
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())   // mở kết nối
            {
                var sql = @"select count(*) from Shippers 
                            where (@searchValue = N'') or (ShipperName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);// thực hiện câu lệnh sql
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                // nonQuery => trả về số dòng bị tác động trong database (> 0 là có dòng dữ liệu bị xoá, = 0 là không có dòng dl bị xoá)
                connection.Close();
            }
            return result;
        }

        public Shipper? Get(int id)
        {
            Shipper? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Shippers where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = id
                };
                data = connection.QueryFirstOrDefault<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                // loại query nhưng chỉ lấy 1 dòng, nhiều cột (first dòng đầu tiên)
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where ShipperID = @ShipperID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    ShipperID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Shipper> data = new List<Shipper>();
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%"; // % để tìm kiếm tương đối
            }
            using (var connection = OpenConnection())
            {   // cte tương ứng với select * from
                var sql = @"select * from
                            (
	                            select	*, row_number() over (order by ShipperName) as RowNumber
	                            from	Shippers 
	                            where	(@searchValue = N'') or (ShipperName like @searchValue)
                            )
                            as t
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Shipper>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Shipper data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"        update Shippers 
                                    set ShipperName = @ShipperName,
                                        Phone = @Phone
                                    where ShipperID = @ShipperID";
                var parameters = new
                {
                    ShipperID = data.ShipperID,
                    ShipperName = data.ShipperName ?? "",
                    Phone = data.Phone ?? "",
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
