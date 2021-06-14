using ModelAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;

namespace BussinessAccessLayer
{
    public class BLAccounts
    {
        private readonly DALAccounts dALAccounts;

        public BLAccounts()
        {
            dALAccounts = new DALAccounts();
        }
        public LoginUser UserLogin(Login loginRequest,out MainResponse response)
        {
            try
            {
                return dALAccounts.UserLogin(loginRequest, out response);
            }
            catch
            {
                throw;
            }
        }

        public MainResponse UserRegister(Register registerRequest)
        {
            try
            {
                return dALAccounts.UserRegister(registerRequest);
            }
            catch
            {
                throw;
            }
        }
        
    }
}
