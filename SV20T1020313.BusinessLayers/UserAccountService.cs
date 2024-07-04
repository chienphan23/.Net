using SV20T1020313.DataLayers.SQLServer;
using SV20T1020313.DataLayers;
using SV20T1020313.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020313.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;

        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountDAL(Configuration.ConnectionString);
        }
        public static UserAccount? Authorize(string userName, string password)
        {
            return employeeAccountDB.Authorize(userName, password);
        }
        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {

            return employeeAccountDB.ChangePassword(userName, oldPassword, newPassword);

        }
    }
}
