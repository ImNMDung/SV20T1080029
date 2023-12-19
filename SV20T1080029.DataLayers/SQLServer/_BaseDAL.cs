using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DataLayers.SQLServer
{ //abstract dung de ke thua 
    public abstract class _BaseDAL
    {
        protected string _connectionString;
        /// <summary>
        /// Ctor 
        /// </summary>
        /// <param name="connectionString"></param>
        public _BaseDAL(string connectionString)
        {
            this._connectionString = connectionString;
        }
        /// <summary>
        /// Tao va mo ket noi den co so du lieu
        /// </summary>
        /// <returns></returns>
        protected SqlConnection OpenConnection()
        {
            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = _connectionString;
            conn.Open();
            return conn;
        }
    }
}
