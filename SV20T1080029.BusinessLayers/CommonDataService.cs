using Microsoft.Data.SqlClient;
using SV20T1080029.DataLayers;
using SV20T1080029.DataLayers.SQLServer;
using SV20T1080029.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.BusinessLayers
{
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Category> categoryDB;
        /// <summary>
        /// Ctor
        /// </summary>
        static CommonDataService()
        {
           /* SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "DESKTOP-GR9NFKL/e.database.windows.net";
            builder.UserID = "sa";
            builder.Password = "123456";
            builder.InitialCatalog = "LiteCommerceDB ";
            builder.TrustServerCertificate = true;
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))*/
            {
                string connectionString = "Server=DESKTOP-GR9NFKL\\KEITH;User=sa;Password=123456;Database=LiteCommerceDB;TrustServerCertificate=true";
                customerDB = new DataLayers.SQLServer.CustomerDAL(connectionString);
                employeeDB = new DataLayers.SQLServer.EmployeeDAL(connectionString);
                shipperDB = new DataLayers.SQLServer.ShipperDAL(connectionString);
                supplierDB = new DataLayers.SQLServer.SupplierDAL(connectionString);
                categoryDB = new DataLayers.SQLServer.CategoryDAL(connectionString);
                provinceDB = new DataLayers.SQLServer.ProvinceDAL(connectionString);
               
            } 
                
           
                }


        public static int CountCustomer()
        {
            var count = customerDB.Count("");
            return count;
        }
        public static int CountEmployeer()
        {
            var count = employeeDB.Count("");
            return count;
        }
       


        public static bool DeleteCustomer(int id)
        {
            return customerDB.Delete(id);
        }

        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Customer> ListOfCustomers( out int rowCount, int page = 1 , int pageSize = 0 , string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);    
            return customerDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategorys(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }/// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }
        public static List<Category> ListOfCategoryNames()
        {

            return categoryDB.List().ToList();
        }

        public static List<Supplier> ListOfSupplierNames()
        {

            return supplierDB.List().ToList();
        }
        public static List<Shipper> ListOfShippers()
        {

            return shipperDB.List().ToList();
        }
        public static List<Customer> ListOfCustomers()
        {

            return customerDB.List().ToList();
        }
        public static List<Employee> ListOfEmployees()
        {

            return employeeDB.List().ToList();
        }


        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }
        public static bool UpdateCustomer(Customer data)
        {
            return customerDB.Update(data);
        }
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        }
        /// <summary>
        /// Thêm nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// cập nhật thông tin nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedSupplier(int id)
        {
            return supplierDB.InUsed(id);
        }

        // Loại hàng
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        // Shipper
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        // Employee

        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool InUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }


    }
}
