using Dapper;
using SV20T1020313.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"begin
                                insert into Suppliers(SupplierName,ContactName,Provice,Address,Phone,Email)
                                values(@SupplierName, @ContactName, @Provice, @Address, @Phone, @Email);

                                select @@identity;
                            end";
                var parameter = new
                {
                    SupplierName = data.SupplierName ?? "",
                    ContactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""
                };
                id = connection.ExecuteScalar<int>(sql,param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if(!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%"+searchValue + "%";
            }
            using(var connection = OpenConnection())
            {
                string sql = @"select count(*) from Suppliers
                            where (@SearchValue = N'') or (SupplierName like @SearchValue)";
                var parameter = new
                {
                    SearchValue = searchValue ?? ""
                };
                count = connection.ExecuteScalar<int>(sql,param: parameter,commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Suppliers where SupplierID = @SupplierID";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                // nonQuery => trả về số dòng bị tác động trong database (> 0 là có dòng dữ liệu bị xoá, = 0 là không có dòng dl bị xoá)
                connection.Close();
            }
            return result;
        }

        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using(var connection = OpenConnection())
            {
                var sql = @"select * from Suppliers where SupplierID = @SupplierID";
                var parameter = new
                {
                    SupplierID = id,
                };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameter, commandType: System.Data.CommandType.Text);
                connection.Close();              
            }
            return data;
        }

        public bool IsUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierID = @SupplierID)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    SupplierID = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data = new List<Supplier>();
            if(!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using(var connection = OpenConnection())
            {
                var sql = @"select * from
                            (
	                            select	*, row_number() over (order by SupplierName) as RowNumber
	                            from	Suppliers 
	                            where	(@searchValue = N'') or (SupplierName like @searchValue)
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
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }

            return data;
        }

        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                // kiểm tra email có trùng với khách hàng khác không. nếu email đã khác thì chạy lệnh update (CustomerId <> @customerId khách hàng khác khách hàng update)
                var sql = @"update Suppliers 
                                    set SupplierName = @supplierName,
                                        ContactName = @contactName,
                                        Provice = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email
                                    where SupplierID = @supplierID";
                var parameters = new
                {
                    supplierID = data.SupplierID,
                    supplierName = data.SupplierName ?? "",
                    contactName = data.ContactName ?? "",
                    province = data.Provice ?? "",
                    address = data.Address ?? "",
                    phone = data.Phone ?? "",
                    email = data.Email ?? ""
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
