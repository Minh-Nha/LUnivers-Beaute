using System.Data;

namespace DAL
{
    public class NhanVienDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllNhanVien");
        }
    }
}
