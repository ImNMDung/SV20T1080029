using Azure;
using Dapper;
using Microsoft.Data.SqlClient;
using SV20T1080029.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DataLayers.SQLServer
{
    public class OrderDAL : _BaseDAL, IOrderDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public OrderDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Chuyển dữ liệu từ SqlDataReader thành Order
        /// </summary>
        /// <param name="dbReader"></param>
        /// <returns></returns>
        private Order DataReaderToOrder(SqlDataReader dbReader)
        {
            return new Order()
            {


            };
        }
        /// <summary>
        /// Chuyển dữ liệu từ SqlDataReader thành OrderDetail
        /// </summary>
        /// <param name="dbReader"></param>
        /// <returns></returns>
        private OrderDetail DataReaderToOrderDetail(SqlDataReader dbReader)
        {
            return new OrderDetail()
            {

            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public int Add(Order data, IEnumerable<OrderDetail> details)
        {
            int orderID = 0;
            //Tạo đơn hàng mới trong CSDL
            using (var connection = OpenConnection())
            {
                var sqlAddOrder = @"if exists(select * from Orders where OrderID = @OrderID)
                                select -1
                            else
                                begin

                                  INSERT INTO Orders(CustomerID, OrderTime, EmployeeID, AcceptTime, ShipperID, ShippedTime, FinishedTime, Status,DeliveryAddress,DeliveryProvince)
                                         VALUES(@CustomerID, @OrderTime, @EmployeeID, @AcceptTime, @ShipperID, @ShippedTime, @FinishedTime, @Status,@DeliveryAddress,@DeliveryProvince);

                                         SELECT @@identity;  
                                end";
                var parameters = new
                {
                    OrderID = data.OrderID,
                    CustomerID = data.CustomerID,
                    OrderTime = data.OrderTime,
                    EmployeeID = data.EmployeeID,
                    AcceptTime = data.AcceptTime,
                    ShipperID = data.ShipperID,
                    ShippedTime = data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,

                    DeliveryAddress = data.DeliveryAddress,
                    DeliveryProvince = data.DeliveryProvince




                };
                orderID = connection.ExecuteScalar<int>(sql: sqlAddOrder, param: parameters, commandType: CommandType.Text);
                //Bổ sung chi tiết cho đơn hàng có mã là orderID

                var sqlAddOrderDetail = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice) " +
                                         "VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
                foreach (var item in details)
                {
                    var orderDetailsparameters = new
                    {
                        orderID = orderID,
                        productID = item.ProductID,
                        quantity = item.Quantity,
                        salePrice = item.SalePrice,
                    };
                    connection.Execute(sqlAddOrderDetail, orderDetailsparameters);
                }

                connection.Close();
            };
            return orderID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public int Count(int status = 0, string searchValue = "")
        {
            int count = 0;

            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  COUNT(*)
                                    FROM    Orders as o
                                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                    WHERE   (@Status = 0 OR o.Status = @Status)
                                        AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)";
                var parameters = new
                {
                    SearchValue = searchValue,
                    Status = status
                };
                count = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return count;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public bool Delete(int orderID)
        {
     
            bool result = false;
            using (var connection = OpenConnection())
            {

                var sql =  @"DELETE FROM OrderDetails WHERE OrderID = @OrderID
                                        DELETE FROM Orders WHERE OrderID = @OrderID";
                var parameters = new { @OrderID = orderID };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool DeleteDetail(int orderID, int productID)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"

            if exists(select * from Customers where Email = @Email)
                                            select -1
                                        else
                                            begin
                                                insert into Customers(CustomerName,ContactName,Province,Address,Phone,Email,IsLocked)
                                                values(@CustomerName,@ContactName,@Province,@Address,@Phone,@Email,@IsLocked);
                                                select @@identity;
                                            end""


            DELETE FROM OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@ProductID", productID);

                result = cmd.ExecuteNonQuery() > 0;

                connection.Close();
            }
            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order? Get(int id)
        {
            Order? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  o.*,  
                                            c.CustomerName,
                                            c.ContactName as CustomerContactName,
                                            c.Address as CustomerAddress,
                                            c.Email as CustomerEmail,
                                             e.FullName as EmployeeFullName,
                                            s.ShipperName,
                                            s.Phone as ShipperPhone
                                    FROM    Orders as o
                                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                    WHERE   o.OrderID = @OrderID";
                var parameters = new { OrderID = id };
                data = connection.QueryFirstOrDefault<Order>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public OrderDetail? GetDetail(int orderID, int productID)
        {
            OrderDetail? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @OrderID AND od.ProductID = @ProductID";

                var parameters = new { OrderID = orderID , ProductId = productID };
                data = connection.QueryFirstOrDefault<OrderDetail?>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
                
            }
            return data;
        }


        public IList<Order> ListStatus()
        {
            List<Order> data = new List<Order>();
           
            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderStatus";


                data = connection.Query<Order>(sql: sql, commandType: CommandType.Text).ToList();

                connection.Close();
            }
            return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public IList<Order> List(int page = 1, int pageSize = 0, int status = 0, string searchValue = "")
        {

            List<Order> data = new List<Order>();
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using (var connection = OpenConnection())
            {
                var sql = @"SELECT  *
                                    FROM    (
                                            SELECT  o.*,
                                                    c.CustomerName,
                                                    c.ContactName as CustomerContactName,
                                                    c.Address as CustomerAddress,
                                                    c.Email as CustomerEmail,
                                                    e.FullName as EmployeeFullName,
                                                    s.ShipperName,
                                                    s.Phone as ShipperPhone,
                                                    ROW_NUMBER() OVER(ORDER BY o.OrderID DESC) AS RowNumber
                                            FROM    Orders as o
                                                    LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                                    LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                                    LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                                            WHERE   (@Status = 0 OR o.Status = @Status)
                                                AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)
                                            ) AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN(@Page -1)*@PageSize + 1 AND @Page*@PageSize)
                                    ORDER BY t.RowNumber";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    Status = status,
                    searchValue = searchValue
                };
                data = (connection.Query<Order>(sql: sql, param: parameters, commandType: CommandType.Text)).ToList();
                connection.Close();
            }
            if (data == null)
                data = new List<Order>();
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public IList<OrderDetail> ListDetails(int orderID)
        {
            List<OrderDetail> data ;
            using (var connection = OpenConnection())
            {
                var sql = @"SELECT	od.*, p.ProductName, p.Unit, p.Photo		
                                    FROM	OrderDetails AS od
		                                    JOIN Products AS p ON od.ProductID = p.ProductID
                                    WHERE	od.OrderID = @OrderID";
                var parameters = new
                {
                    orderID = orderID,
                    
              };
                data = connection.Query<OrderDetail>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();

                connection.Close();
            }
            return data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int SaveDetail(int orderID, int productID, int quantity, decimal salePrice)
        {
            int result = 0;


            using (var connection = OpenConnection())
            {
                    var sql = @" 
 
                        
                    IF NOT EXISTS (SELECT 1 FROM OrderDetails WHERE OrderID = @OrderID AND ProductID = @ProductID)
                    BEGIN
   
                        INSERT INTO OrderDetails (OrderID, ProductID, Quantity, SalePrice)
                        VALUES (@OrderID, @ProductID, @Quantity, @SalePrice);

                        SELECT SCOPE_IDENTITY() AS NewOrderDetailID;
                    END
                    ELSE
                    BEGIN
   
                        UPDATE OrderDetails
                        SET Quantity = @Quantity, SalePrice = @SalePrice
                        WHERE OrderID = @OrderID AND ProductID = @ProductID;

                        SELECT NULL AS NewOrderDetailID;
                    END                        
                    ";
                var parameters = new
                {
                    OrderID = orderID,
                    ProductID = productID,
                    Quantity = quantity,
                    SalePrice = salePrice
                };

                result = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);


                connection.Close();
            }


            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Update(Order data)
        {
            bool result = false;
            
            using ( var connection = OpenConnection())
            {
                var sql = @"UPDATE Orders
                                    SET     CustomerID = @CustomerID,
                                            OrderTime = @OrderTime,
                                            EmployeeID = @EmployeeID,
                                            AcceptTime = @AcceptTime,
                                            ShipperID = @ShipperID,
                                            ShippedTime = @ShippedTime,
                                            FinishedTime = @FinishedTime,
                                            Status = @Status
                                    WHERE   OrderID = @OrderID";
                var parameters = new
                {
                    
                    CustomerID = data.CustomerID,
                    OrderTime= data.OrderTime,
                    EmployeeID=  data.EmployeeID,
                    AcceptTime=data.AcceptTime,
                    ShipperID= data.ShipperID,
                    ShippedTime=data.ShippedTime,
                    FinishedTime = data.FinishedTime,
                    Status = data.Status,
                    OrderID = data.OrderID



                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;

                connection.Close();
            }
            return result;


        
    }

        public IList<Order> ListOrderStatus()
        {
            List<Order> data = new List<Order>();

            using (var connection = OpenConnection())
            {
                var sql = @"select * from OrderStatus";


                data = connection.Query<Order>(sql: sql, commandType: CommandType.Text).ToList();

                connection.Close();
            }
            return data;
        }
    }
}
