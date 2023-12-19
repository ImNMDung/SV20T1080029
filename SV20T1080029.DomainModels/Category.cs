using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DomainModels
{ /// <summary>
///  neu internal thì chỉ dùng trong project , dùng public   internal class Customer
/// </summary>
    public class Category

    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = "";
        public string Description { get; set; } = "";
       

    }
}
