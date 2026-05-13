using System.Data;

namespace DAL
{
    public class DanhMucDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllDanhMuc");
        }
    }
}
