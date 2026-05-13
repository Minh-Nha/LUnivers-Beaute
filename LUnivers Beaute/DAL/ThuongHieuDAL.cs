using System.Data;

namespace DAL
{
    public class ThuongHieuDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllThuongHieu");
        }
    }
}
