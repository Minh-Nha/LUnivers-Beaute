using System.Data;

namespace DAL
{
    public class ChamCongDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllChamCong");
        }
    }
}
