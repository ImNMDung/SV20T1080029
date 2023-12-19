using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols;
using SV20T1080029.DataLayers;
using SV20T1080029.DataLayers.SQLServer;
using SV20T1080029.DomainModels;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SV20T1080029.BusinessLayers
{/// <summary>
 /// Các nghiệp vụ quản lý hàng hóa
 /// </summary>
    public static class ProductDataService
    {
        private static readonly IProductDAL productDB;

        /// <summary>
        /// 
        /// </summary>
        static ProductDataService()
        {
            //   string connectionString = "Server=DESKTOP-GR9NFKL\\KEITH;User=sa;Password=123456;Database=SpotLightsDB;TrustServerCertificate=true";
            string connectionString = "Server=DESKTOP-GR9NFKL\\KEITH;User=sa;Password=123456;Database=LiteCommerceDB;TrustServerCertificate=true";
          //  string connectionString = "Server=mango-rds.maychudns.net,1443;User=spotlights_vn_us;Password=47KLTgLy4QYv;Database=spotlights_vn_db;TrustServerCertificate=true";
            productDB = new DataLayers.SQLServer.ProductDAL(connectionString);
        }


        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (không phân trang)
        /// </summary>
        /// <param name="searchValue">Tên mặt hàng cần tìm (chuỗi rỗng nếu không tìm kiếm)</param>
        /// <param name="categoryID">Mã loại hàng cần tìm (chuỗi rỗng nếu không tìm kiếm theo loại hàng)</param>
        /// <param name="supplierID">Mã nhà cung cấp cần tìm (chuỗi rỗng nếu không tìm kiếm theo nhà cung cấp)</param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "", int categoryID = 0, int supplierID = 0)
        {
            return productDB.List(1, 0, searchValue, categoryID, supplierID).ToList();
        }
        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(int page, int pageSize, string searchValue, int categoryID, int supplierID, out int rowCount)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID).ToList();
        }
        /// <summary>
        /// Lấy thông tin mặt hàng theo mã hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static Product GetProduct(int productID)
        {
            return productDB.Get(productID);
        }
        /// <summary>
        /// Thêm một mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddProduct(Product data)
        {
            return productDB.Add(data);
        }
        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }
        /// <summary>
        /// Xoá một mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool DeleteProduct(int productID)
        {
            return productDB.Delete(productID);
        }
        /// <summary>
        /// Kiểm tra mã mặt hàng có tồn tại trong các quan hệ
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static bool InUsedProduct(int productID)
        {
            return productDB.InUsed(productID);
        }


        /// <summary>
        /// Hiển thị danh sách ảnh của mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }
        /// <summary>
        /// Lấy thông tin của ảnh mặt hàng theo mã
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static ProductPhoto GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }
        /// <summary>
        /// Thêm ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddPhoto(ProductPhoto data)
        {
            return productDB.AddPhoto(data);
        }
        /// <summary>
        /// Cập nhật thông tin ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }
        /// <summary>
        /// Xoá ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }


        /// <summary>
        /// Hiển thị danh sách thuộc tính của mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }
        /// <summary>
        /// Lấy thông tin thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static ProductAttribute GetAttribute(int attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }
        /// <summary>
        /// Thêm thuộc tính mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }
        /// <summary>
        /// Cập nhật thông tin thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }
        /// <summary>
        /// Xoá thông tin thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}