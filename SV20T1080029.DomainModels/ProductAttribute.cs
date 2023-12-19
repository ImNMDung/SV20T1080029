using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DomainModels
{
    public class ProductAttribute
    {
        ///<summary>
        /// Mã thuộc tính mặt hàng
        ///</summary>
        public long AttributeID { get; set; }
        ///<summary>
        /// Mã mặt hàng
        ///</summary>
        public int ProductID { get; set; }
        ///<summary>
        /// Tên thuộc tính mặt hàng
        ///</summary>
        public string AttributeName { get; set; }
        ///<summary>
        /// Tên giá trị của thuộc tính
        ///</summary>
        public string AttributeValue { get; set; }
        ///<summary>
        /// Tên hiển thị đặt hàng
        ///</summary>
        public int DisplayOrder { get; set; }
    }
}
