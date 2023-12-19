using SV20T1080029.DataLayers;
using SV20T1080029.DomainModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV20T1080029.DataLayers.SQLServer;

namespace SV20T1080029.BusinessLayers
{
    public static class UserAccountService
    {
        static readonly IUserAccountDAL employeeUserAccountDB;

        static UserAccountService()
        {
          // string connectionString = "Server=DESKTOP-GR9NFKL\\KEITH;User=sa;Password=123456;Database=SpotLightsDB;TrustServerCertificate=true";
        // string connectionString = "Server=mango-rds.maychudns.net,1443;User=spotlights_vn_us;Password=47KLTgLy4QYv;Database=spotlights_vn_db;TrustServerCertificate=true";
            string connectionString = "Server=DESKTOP-GR9NFKL\\KEITH;User=sa;Password=123456;Database=LiteCommerceDB;TrustServerCertificate=true";
            employeeUserAccountDB = new DataLayers.SQLServer.EmployeeUserAccountDAL(connectionString);
        }

        public static UserAccount? Authorize(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.Authorize(username, password);
                default: return null;
            }
        }

       
        public static bool ChangePassword(string username, string password, TypeOfAccount typeOfAccount)
        {
            switch (typeOfAccount)
            {
                case TypeOfAccount.Employee:
                    return employeeUserAccountDB.ChangePassword(username, password);
                default: return false;
            }
        }
    }
    

    public enum TypeOfAccount
    {
        Employee,
        Customer
    }
}