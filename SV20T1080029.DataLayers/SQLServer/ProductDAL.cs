using Dapper;

using SV20T1080029.DomainModels;
using SV20T1080029.DataLayers.SQLServer;
using SV20T1080029.DataLayers;
using Microsoft.Data.SqlClient;
using System.Data;
using Azure;
using System.Net.Http.Headers;

namespace SV20T1080029.DataLayers.SQLServer
{
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }
        /// <summary>
        /// Thêm một mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Add(Product data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
               
                var sql = @"	if exists(select * from Products where Productid =  @Productid)
                                select -1
                            else
                                begin
                                   INSERT INTO Products(ProductName, CategoryID, SupplierID, Unit, Price, Photo)
                                    VALUES(@ProductName, @CategoryID, @SupplierID, @Unit, @Price, @Photo);
                                   SELECT @@identity;
                                end";
                var parameters = new
                {
                    Productid = data.ProductID,
                    ProductName = data.ProductName ?? "",
                    CategoryID = data.CategoryID,
                    SupplierID =data.SupplierID ,
                    Unit = data.Unit ,
                    Price = data.Price,
                    Photo = data.Photo ?? "",

                };
                id = connection.ExecuteScalar<int>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            };
            return id;
        }
        public long AddAttribute(ProductAttribute data)
        {
            int result = 0;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                    VALUES (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@AttributeName", data.AttributeName);
                cmd.Parameters.AddWithValue("@AttributeValue", data.AttributeValue);
                cmd.Parameters.AddWithValue("@DisplayOrder", data.DisplayOrder);

                result = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            };
            return result;
        }
        public long AddPhoto(ProductPhoto data)
        {
            int result = 0;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"INSERT INTO ProductPhotos(PhotoID, ProductID, Photo, Description, DisplayOrder, IsHidden)
                                    VALUES(@PhotoID, @ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                    SELECT SCOPE_IDENTITY()";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@PhotoID", data.PhotoID);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@Photo", data.Photo);
                cmd.Parameters.AddWithValue("@Description", data.Description);
                cmd.Parameters.AddWithValue("@DisplayOrder", data.DisplayOrder);
                cmd.Parameters.AddWithValue("@IsHidden", data.IsHidden);

                result = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
            };
            return result;
        }
        /// <summary>
        /// Đếm số lượng của mặt hàng dựa theo tìm kiếm tên mặt hàng,
        /// theo mã loại hàng và theo mã nhà cung cấp
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public int Count(string searchValue = "", int categoryID = 0, int supplierID = 0)
        {
            int count = 0;

            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT  COUNT(*)
                                    FROM    Products
                                    WHERE   ((@SearchValue = N'') OR (ProductName LIKE @SearchValue))";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@SearchValue", searchValue);
                cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                cmd.Parameters.AddWithValue("@SupplierID", supplierID);

                count = Convert.ToInt32(cmd.ExecuteScalar());

                cn.Close();
            }
            return count;
        }
        /// <summary>
        /// Xoá một mặt hàng dựa theo mã mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool Delete(int productID)
        {
            bool result = false;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"DELETE FROM Products WHERE ProductID = @ProductID 
                                        AND NOT EXISTS(SELECT * FROM ProductAttributes WHERE ProductID = @ProductID)
                                        AND NOT EXISTS(SELECT * FROM ProductPhotos WHERE ProductID = @ProductID)
                                        AND NOT EXISTS(SELECT * FROM OrderDetails WHERE ProductID = @ProductID)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                result = cmd.ExecuteNonQuery() > 0;
            };
            return result;
        }
        public bool DeleteAttribute(long attributeID)
        {
            bool result = false;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"DELETE FROM ProductAttributes WHERE AttributeID = @AttributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@AttributeID", attributeID);
                result = cmd.ExecuteNonQuery() > 0;
            };
            return result;
        }

        public bool DeletePhoto(long photoID)
        {
            bool result = false;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"DELETE FROM ProductPhotos WHERE PhotoID = @PhotoID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@PhotoID", photoID);
                result = cmd.ExecuteNonQuery() > 0;
            };
            return result;
        }
        /// <summary>
        /// Lấy thông tin của một mặt hàng dựa vào mã mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Product Get(int productID)
        {
            Product data = null;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM Products WHERE ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (dbReader.Read())
                {
                    data = new Product()
                    {
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        ProductName = Convert.ToString(dbReader["ProductName"]),
                        SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                        CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                        Unit = Convert.ToString(dbReader["Unit"]),
                        Price = Convert.ToInt32(dbReader["Price"]),
                        Photo = Convert.ToString(dbReader["Photo"])
                    };
                }
                dbReader.Close();
                conn.Close();
            };

            return data;
        }

        public ProductAttribute GetAttribute(long attributeID)
        {
            ProductAttribute data = null;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM ProductAttributes WHERE AttributeID = @AttributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@AttributeID", attributeID);
                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (dbReader.Read())
                {
                    data = new ProductAttribute()
                    {
                        AttributeID = Convert.ToInt32(dbReader["AttributeID"]),
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        AttributeName = Convert.ToString(dbReader["AttributeName"]),
                        AttributeValue = Convert.ToString(dbReader["AttributeValue"]),
                        DisplayOrder = Convert.ToInt32(dbReader["DisplayOrder"]),
                    };
                }
                dbReader.Close();
                conn.Close();
            };

            return data;
        }

        public ProductPhoto GetPhoto(long photoID)
        {
            ProductPhoto data = null;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM ProductPhotos WHERE PhotoID = @PhotoID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@PhotoID", photoID);
                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                if (dbReader.Read())
                {
                    data = new ProductPhoto()
                    {
                        PhotoID = Convert.ToInt32(dbReader["PhotoID"]),
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        Photo = Convert.ToString(dbReader["Photo"]),
                        Description = Convert.ToString(dbReader["Description"]),
                        DisplayOrder = Convert.ToInt32(dbReader["DisplayOrder"]),
                        IsHidden = Convert.ToBoolean(dbReader["IsHidden"]),
                    };
                }
                dbReader.Close();
                conn.Close();
            };

            return data;
        }

        /// <summary>
        /// Kiểm tra mặt hàng có thuộc các quan hệ của nó không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool InUsed(int productID)
        {
            bool result = false;

            using (SqlConnection cn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT CASE WHEN EXISTS(
                                                    (SELECT * FROM ProductAttributes WHERE ProductID = @ProductID)
                                                AND (SELECT * FROM ProductPhotos WHERE ProductID = @ProductID)
                                                AND (SELECT * FROM OrderDetails WHERE ProductID = @ProductID)
                                    )
                                                THEN 1 ELSE 0 END";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;
                cmd.Parameters.AddWithValue("@ProductID", productID);

                result = Convert.ToBoolean(cmd.ExecuteScalar());

                cn.Close();
            }

            return result;
        }
        /// <summary>
        /// Hiển thị danh sách tất cả các mặt hàng có tìm kiếm và phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <returns></returns>
        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = "", int categoryID = 0, int supplierID = 0)
        {
            List<Product> data = new List<Product>();

            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT *
                                    FROM 
                                    (
                                        SELECT  *, ROW_NUMBER() OVER (ORDER BY ProductName, CategoryID, SupplierID) AS RowNumber
                                        FROM    Products
                                        WHERE   ((@SearchValue = N'') OR (ProductName LIKE @SearchValue))
                                                AND ((@CategoryID = 0) OR (CategoryID = @CategoryID))
                                                AND ((@SupplierID = 0) OR (SupplierID = @SupplierID))
                                    )   AS t
                                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize)";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@Page", page);
                cmd.Parameters.AddWithValue("@PageSize", pageSize);
                cmd.Parameters.AddWithValue("@SearchValue", searchValue);
                cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                cmd.Parameters.AddWithValue("@SupplierID", supplierID);

                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new Product()
                    {
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        ProductName = Convert.ToString(dbReader["ProductName"]),
                        SupplierID = Convert.ToInt32(dbReader["SupplierID"]),
                        CategoryID = Convert.ToInt32(dbReader["CategoryID"]),
                        Unit = Convert.ToString(dbReader["Unit"]),
                        Price = Convert.ToInt32(dbReader["Price"]),
                        Photo = Convert.ToString(dbReader["Photo"])
                    });
                }
                dbReader.Close();
                conn.Close();
            };
            return data;
        }
        public IList<ProductAttribute> ListAttributes(int productID)
        {
            List<ProductAttribute> data = new List<ProductAttribute>();

            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM ProductAttributes WHERE ProductID = @ProductID ORDER BY DisplayOrder ASC";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new ProductAttribute()
                    {
                        AttributeID = Convert.ToInt32(dbReader["AttributeID"]),
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        AttributeName = Convert.ToString(dbReader["AttributeName"]),
                        AttributeValue = Convert.ToString(dbReader["AttributeValue"]),
                        DisplayOrder = Convert.ToInt32(dbReader["DisplayOrder"])
                    });
                }
                dbReader.Close();
                conn.Close();
            }
            return data;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            List<ProductPhoto> data = new List<ProductPhoto>();
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT * FROM ProductPhotos WHERE ProductID = @ProductID ORDER BY DisplayOrder ASC";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", productID);
                var dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                while (dbReader.Read())
                {
                    data.Add(new ProductPhoto()
                    {
                        PhotoID = Convert.ToInt32(dbReader["PhotoID"]),
                        ProductID = Convert.ToInt32(dbReader["ProductID"]),
                        Photo = Convert.ToString(dbReader["Photo"]),
                        Description = Convert.ToString(dbReader["Description"]),
                        DisplayOrder = Convert.ToInt32(dbReader["DisplayOrder"]),
                        IsHidden = Convert.ToBoolean(dbReader["IsHidden"]),
                    });
                }
                dbReader.Close();
                conn.Close();
            }
            return data;
        }
        public bool Update(Product data)
        {
            bool result = false;

            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE Products
                                    SET ProductName = @ProductName, 
                                        SupplierID  = @SupplierID, 
                                        CategoryID  = @CategoryID, 
                                        Unit        = @Unit, 
                                        Price       = @Price, 
                                        Photo       = @Photo
                                    WHERE ProductID = @ProductID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductName", data.ProductName);
                cmd.Parameters.AddWithValue("@SupplierID", data.SupplierID);
                cmd.Parameters.AddWithValue("@CategoryID", data.CategoryID);
                cmd.Parameters.AddWithValue("@Unit", data.Unit);
                cmd.Parameters.AddWithValue("@Price", data.Price);
                cmd.Parameters.AddWithValue("@Photo", data.Photo);
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);

                result = cmd.ExecuteNonQuery() > 0;
                conn.Close();
            };

            return result;
        }

        //public bool Update(Product data)
        //{
        //    throw new NotImplementedException();
        //}

        public bool UpdateAttribute(ProductAttribute data)
        {
            bool result = false;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE  ProductAttributes
                                    SET     ProductID       = @ProductID, 
                                            AttributeName   = @AttributeName, 
                                            AttributeValue  = @AttributeValue, 
                                            DisplayOrder    = @DisplayOrder
                                    WHERE       AttributeID = @AttributeID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@AttributeName", data.AttributeName);
                cmd.Parameters.AddWithValue("@AttributeValue", data.AttributeValue);
                cmd.Parameters.AddWithValue("@DisplayOrder", data.DisplayOrder);
                cmd.Parameters.AddWithValue("@AttributeID", data.AttributeID);

                result = cmd.ExecuteNonQuery() > 0;
                conn.Close();
            };
            return result;
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            bool result = false;
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = @"UPDATE  ProductPhotos
                                    SET     ProductID   = @ProductID, 
                                            Photo       = @Photo, 
                                            Description = @Description, 
                                            DisplayOrder= @DisplayOrder,
                                            IsHidden    = @IsHidden
                                    WHERE       PhotoID = @PhotoID";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@ProductID", data.ProductID);
                cmd.Parameters.AddWithValue("@Photo", data.Photo);
                cmd.Parameters.AddWithValue("@Description", data.Description);
                cmd.Parameters.AddWithValue("@DisplayOrder", data.DisplayOrder);
                cmd.Parameters.AddWithValue("@IsHidden", data.IsHidden);
                cmd.Parameters.AddWithValue("@PhotoID", data.PhotoID);

                result = cmd.ExecuteNonQuery() > 0;
                conn.Close();
            };
            return result;
        }

        Product IProductDAL.Get(int productID)
        {
           // throw new NotImplementedException();

            Product? data = null;
            using (var connection = OpenConnection())
            {
                var sql = "select * from Products where ProductId = @productId";
                var parameters = new { productId = productID  };
                data = connection.QueryFirstOrDefault<Product>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;

        }
      
        IList<Product> IProductDAL.List(int page, int pageSize, string searchValue, int categoryID, int supplierID )
        {
            List<Product> data;
            if (!string.IsNullOrEmpty(searchValue))
                searchValue = "%" + searchValue + "%";
            using (var connection = OpenConnection())
            {
                var sql = @"WITH cte AS (
    SELECT 
        *, 
        ROW_NUMBER() OVER (ORDER BY ProductName, CategoryID, SupplierID) AS RowNumber
    FROM 
        Products
    WHERE 
        ((@SearchValue = '') OR (ProductName LIKE '%' + @SearchValue + '%'))
        AND ((@CategoryID = 0) OR (CategoryID = @CategoryID))
        AND ((@SupplierID = 0) OR (SupplierID = @SupplierID))
)

SELECT * 
FROM cte
WHERE  
    (@PageSize = 0) OR 
    (RowNumber BETWEEN (@Page - 1) * @PageSize + 1 AND @Page * @PageSize);";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue,
                    categoryID = categoryID,
                    supplierID = supplierID,
                    
                };

                data = connection.Query<Product>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return data;
        }
    }
}
