using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using SV20T1080029.DomainModels;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using SV20T1080029.DataLayers.SQLServer;
using SV20T1080029.DataLayers;

namespace SV20T1080029.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>

    {
        public SupplierDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public int Add(Supplier data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Suppliers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName,ContactName,Provice,Address,Phone,Email)
                                    values(@SupplierName,@ContactName,@Provice,@Address,@Phone,@Email);
                                    select @@identity;
                                end";
                var parameters = new
                {
                    supplierName = data.SupplierName ?? "",
                    contactName = data.ContactName ?? "",
                    Provice = data.Provice ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? ""

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return id;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Count(string searchValue = "")
        {
            int count = 0;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"select count(*) from Suppliers
                    where (@searchValue = N'') or (SupplierName like @searchValue)";
                var parameters = new { searchValue = searchValue };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }

            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = "delete from Suppliers where SupplierId = @supplierId and not exists(select * from Products where SupplierId = @supplierId)";
                var parameters = new { supplierId = id };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Supplier? Get(int id)
        {
            Supplier? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Suppliers where SupplierId = @supplierId";
                var parameters = new { supplierId = id };
                data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if exists(select * from Products where SupplierId = @supplierId)
                                select 1
                            else 
                                select 0";
                var parameters = new { supplierId = id };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Supplier> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as
                (
                    select SupplierID, SupplierName, ContactName, Provice, Address, Phone, Email, 
                    ROW_NUMBER() over (order by SupplierName) as RowNumber
                    from Suppliers
                    where (@searchValue = N'') or (SupplierName like @searchValue)
                )

                select * from cte
                where (@pageSize = 0)
                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue
                };
                data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Update(Supplier data)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"if not exists(select * from Suppliers where SupplierId <> @supplierId and Email = @email)
                                begin
                                    update Suppliers
                                    set SupplierName = @supplierName,
                                        ContactName = @contactName,
                                        Provice = @provice,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email
                                        
                                    where SupplierId = @supplierId
                                end";
                var parameters = new
                {
                    supplierId = data.SupplierID,
                    supplierName = data.SupplierName ?? "",
                    contactName = data.ContactName ?? "",
                    provice = data.Provice ?? "",
                    address = data.Address ?? "",
                    phone = data.Phone ?? "",
                    email = data.Email ?? ""

                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            }
            return result;
        }
    }
}
public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>

{
    public SupplierDAL(string connectionString) : base(connectionString)
    {
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>

    public int Add(Supplier data)
    {
        int id = 0;
        using (var connection = OpenConnection())
        {
            var sql = @"if exists(select * from Suppliers where Email = @Email)
                                select -1
                            else
                                begin
                                    insert into Suppliers(SupplierName,ContactName,Provice,Address,Phone,Email)
                                    values(@SupplierName,@ContactName,@Provice,@Address,@Phone,@Email);
                                    select @@identity;
                                end";
            var parameters = new
            {
                supplierName = data.SupplierName ?? "",
                contactName = data.ContactName ?? "",
                Provice = data.Provice ?? "",
                Address = data.Address ?? "",
                Phone = data.Phone ?? "",
                Email = data.Email ?? ""

            };
            id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            connection.Close();
        }
        return id;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int Count(string searchValue = "")
    {
        int count = 0;
        if (!string.IsNullOrEmpty(searchValue))
            searchValue = "%" + searchValue + "%";
        using (var connection = OpenConnection())
        {
            var sql = @"select count(*) from Suppliers
                    where (@searchValue = N'') or (SupplierName like @searchValue)";
            var parameters = new { searchValue = searchValue };
            count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
            connection.Close();
        }

        return count;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Delete(int id)
    {
        bool result = false;
        using (var connection = OpenConnection())
        {
            var sql = "delete from Suppliers where SupplierId = @supplierId and not exists(select * from Products where SupplierId = @supplierId)";
            var parameters = new { supplierId = id };
            result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
            connection.Close();
        }
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Supplier? Get(int id)
    {
        Supplier? data = null;
        using (var connection = OpenConnection())
        {
            var sql = "select * from Suppliers where SupplierId = @supplierId";
            var parameters = new { supplierId = id };
            data = connection.QueryFirstOrDefault<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text);
            connection.Close();
        }
        return data;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool InUsed(int id)
    {
        bool result = false;
        using (var connection = OpenConnection())
        {
            var sql = @"if exists(select * from Products where SupplierId = @supplierId)
                                select 1
                            else 
                                select 0";
            var parameters = new { supplierId = id };
            result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
            connection.Close();
        }
        return result;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="searchValue"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
    {
        List<Supplier> data;
        if (!string.IsNullOrEmpty(searchValue))
            searchValue = "%" + searchValue + "%";
        using (var connection = OpenConnection())
        {
            var sql = @"with cte as
                (
                    select SupplierID, SupplierName, ContactName, Provice, Address, Phone, Email, 
                    ROW_NUMBER() over (order by SupplierName) as RowNumber
                    from Suppliers
                    where (@searchValue = N'') or (SupplierName like @searchValue)
                )

                select * from cte
                where (@pageSize = 0)
                    or (RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)
                    order by RowNumber";
            var parameters = new
            {
                page = page,
                pageSize = pageSize,
                searchValue = searchValue
            };
            data = connection.Query<Supplier>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

            connection.Close();
        }
        return data;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool Update(Supplier data)
    {
        bool result = false;
        using (var connection = OpenConnection())
        {
            var sql = @"if not exists(select * from Suppliers where SupplierId <> @supplierId and Email = @email)
                                begin
                                    update Suppliers
                                    set SupplierName = @supplierName,
                                        ContactName = @contactName,
                                        Provice = @provice,
                                        Address = @address,
                                        Phone = @phone,
                                        Email = @email
                                        
                                    where SupplierId = @supplierId
                                end";
            var parameters = new
            {
                supplierId = data.SupplierID,
                supplierName = data.SupplierName ?? "",
                contactName = data.ContactName ?? "",
                provice = data.Provice ?? "",
                address = data.Address ?? "",
                phone = data.Phone ?? "",
                email = data.Email ?? ""

            };
            result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
        }
        return result;
    }
}

