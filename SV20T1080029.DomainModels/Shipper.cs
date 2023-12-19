using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DomainModels
{ /// <summary>
///  neu internal thì chỉ dùng trong project , dùng public   internal class Customer
/// </summary>
    public class Shipper

    {
        public int ShipperID { get; set; }
        public string ShipperName { get; set; } = "";
        public string Phone { get; set; } = "";
    

    }
}
