using System.Data;

namespace DAL
{
    public class TonKhoDAL
    {
        public DataTable GetAll()
        {
            return DatabaseHelpers.GetData("sp_GetAllTonKho");
        }
    }
}
