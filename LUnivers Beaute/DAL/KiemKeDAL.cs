using System.Data;

namespace DAL
{
    public class KiemKeDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllKiemKe");
        }
    }
}
