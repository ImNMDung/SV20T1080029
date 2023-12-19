using SV20T1080029.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DataLayers
{
    public interface IUserAccountDAL
    {
        /// <summary>
        /// xác thực tài khoản đăng nhập của người dùng (employee,customer)
        /// xác thực thành công thì trả về thông tin người dùng ko thì null
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        UserAccount Authorize(string userName, string password);
        /// <summary>
        /// thay đổi mật khẩu
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool ChangePassword(string userName, string password);
    }
}
