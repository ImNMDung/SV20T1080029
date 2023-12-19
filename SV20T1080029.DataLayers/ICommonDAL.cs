using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1080029.DataLayers
{
    public interface ICommonDAL<T> where T : class
    { // nen dung bat dong bo
        // viet interface phai mo ta ro rang 
        // IList<T> Data { get; }

        /// <summary>
        /// tim kiem va lay danh sach cac du lieu duoi dang phan trang 
        /// </summary>
        /// <param name="page">trang can hien thi </param>
        /// <param name="pageSize"> so dong tren moi trang (0 neu khong tien hanh phan tran )  </param>
        /// <param name="searchValue"> gia tri tim kiem ( chuoi rong neu lay toan bo du~ lieu ) </param>
        /// <returns></returns>
        IList<T> List(int page = 1 , int pageSize = 0 , string searchValue = "");


        /// <summary>
        /// đếm số dòng dữ liệu thoả điều kiện tìm kiếm 
        /// </summary>
        /// <param name="searchValue">gia tri tim kiem ( chuoi rong neu lay toan bo du~ lieu ) </param>
        /// <returns></returns>
        int Count(string searchValue = "");




        /// <summary>
        /// bổ sung thêm dữ liệu vào database. Hàm trả về Id của dữ liệu 
        /// được bổ sung ( nếu trả về 0 tức là lỗi ) 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(T data);
        /// <summary>
        /// Cap nhat du lieu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        bool Update(T data);
       /// <summary>
       /// xoa du lieu
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        bool Delete(int id );

        /// <summary>
        /// láy bản ghi dữ liệu dựa vào mã 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T? Get(int id);

        /// <summary>
        /// kiem tra xem duw lieu co ma id hien co dang duoc su dung boi cac du lieu khac hay khong
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);







    }
}
