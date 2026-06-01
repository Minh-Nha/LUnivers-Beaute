using System.Data;
using DAL;

namespace BUS
{
    public class LogBUS
    {
        private readonly LogDAL _dal = new LogDAL();

        public int InsertAccessLog(string userName, string ipAddress, string deviceName, string location)
        {
            return _dal.InsertAccessLog(userName, ipAddress, deviceName, location);
        }

        public DataTable GetAllAccessLogs()
        {
            return _dal.GetAllAccessLogs();
        }

        public int InsertEditLog(string userName, string action, string detail, string icon)
        {
            return _dal.InsertEditLog(userName, action, detail, icon);
        }

        public DataTable GetAllEditLogs()
        {
            return _dal.GetAllEditLogs();
        }
    }
}
