using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Dapper;
using SV20T1020313.DomainModels;

namespace SV20T1020313.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer> // kế thừa _BaseDAL,Customer truyền vào tham số T trong interface **Customer này nằm ở domainModels => cần using domainmodel /// kế thừa interface => xem như 1 implement của Interface
    {
        public CustomerDAL(string connectionString) : base(connectionString)    // đưa vào lỗi đỏ ở CustomerDAL bấm vào bóng đèn => chọn generate constructor, hàm này dùng để chuyển biến cho hàm tạo cha
        {
        }

        public int Add(Customer data)   // đưa vào lỗi đỏ ở interface => bóng đèn => implement interface để tạo code
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Customers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                    values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);

                                    select @@identity;
                                end";
                // nếu tồn tại email trùng lặp => trả về -1
                // @@identity lấy ra id vừa được sinh ra
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                // thực thi câu lệnh
                id = connection.ExecuteScalar<int>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
                return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%"+searchValue+"%";
            }
            using (var connection = OpenConnection())   // mở kết nối
            {
                var sql = @"select count(*) from Customers 
                            where (@searchValue = N'') or (CustomerName like @searchValue)";
                var parameters = new
                {
                    searchValue = searchValue ?? "",
                };
                count = connection.ExecuteScalar<int>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text);// thực hiện câu lệnh sql
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                // nonQuery => trả về số dòng bị tác động trong database (> 0 là có dòng dữ liệu bị xoá, = 0 là không có dòng dl bị xoá)
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)    // thay vì T ở khai báo bên interface sẽ thành Customer
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                // loại query nhưng chỉ lấy 1 dòng, nhiều cột (first dòng đầu tiên)
                connection.Close();
            }
            return data;
        }

        public bool IsUsed(int id)  // vd như khách hàng có đặt đơn hàng (order) => xoá thì sẽ bị lỗi (đơn hàng không xác định đc khách hàng)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Orders where CustomerId = @CustomerId)
                                select 1
                            else 
                                select 0";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: System.Data.CommandType.Text);
                connection.Close();
            }
            return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> data = new List<Customer>();
            if(!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%"+searchValue+"%"; // % để tìm kiếm tương đối
            }
            using ( var connection = OpenConnection())
            {   // cte tương ứng với select * from
                var sql = @"with cte as
                            (
	                            select	*, row_number() over (order by CustomerName) as RowNumber
	                            from	Customers 
	                            where	(@searchValue = N'') or (CustomerName like @searchValue)
                            )
                            select * from cte
                            where  (@pageSize = 0) 
	                            or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                            order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? ""
                };
                data = connection.Query<Customer>(sql: sql,param: parameters, commandType: System.Data.CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }

        public bool Update(Customer data)
        {
            bool result = false;
            using ( var connection = OpenConnection())
            {
                // kiểm tra email có trùng với khách hàng khác không. nếu email đã khác thì chạy lệnh update (CustomerId <> @customerId khách hàng khác khách hàng update)
                var sql = @"if not exists(select * from Customers where CustomerId <> @customerId and Email = @email)
                                begin
                                    update Customers 
                                    set CustomerName = @customerName,
                                        ContactName = @contactName,
                                        Province = @province,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email,
                                        IsLocked = @isLocked
                                    where CustomerId = @customerId
                                end";
                var parameters = new 
                {
                    CustomerId = data.CustomerID,
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: System.Data.CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}
