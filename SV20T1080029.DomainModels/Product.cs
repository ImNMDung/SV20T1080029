using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DomainModels
{
    public class Product {
        ///<summary>
        /// Mã mặt hàng
        ///</summary>
        public int ProductID { get; set; }
        ///<summary>
        /// Tên mặt hàng
        ///</summary>
        public string ProductName { get; set; } = "";
        ///<summary>
        /// Mã nhà cung cấp
        ///</summary>
        public int SupplierID { get; set; }
        ///<summary>
        /// Mã loại hàng
        ///</summary>
        public int CategoryID { get; set; }
        ///<summary>
        /// Đơn vị
        ///</summary>
        public string Unit { get; set; } = "";
        ///<summary>
        /// Giá của mặt hàng
        ///</summary>
        public int Price { get; set; }
        ///<summary>
        /// Ảnh của mặt hàngs
        ///</summary>
        public string Photo { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        }
       
}
