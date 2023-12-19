using Dapper;
using SV20T1080029.DataLayers.SQLServer;
using SV20T1080029.DataLayers;
using SV20T1080029.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DataLayers.SQLServer
{
    /// <summary>
    /// Cai dat cac phep xu ly lien quan den tai khoan cua Employee
    /// </summary>
    public class EmployeeUserAccountDAL : _BaseDAL, IUserAccountDAL
    {
        public EmployeeUserAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount Authorize(string username, string password)
        {
            UserAccount? data = null;

            using (var connection = OpenConnection())
            {
                var sql = @"select EmployeeID as UserId,Email as UserName,FullName,Email,Photo  
                    from Employees 
                where Email = @userName and Password = @password";
                var parameters = new { username = username, password = password };
                data = connection.QueryFirstOrDefault<UserAccount>(sql: sql, param: parameters, commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool ChangePassword(string userName, string password)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"update Employees  
                    set Password = @password 
                    where Email= @userName";
                var parameters = new { username = userName, password = password };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
          return result;
           
        }
    }
}