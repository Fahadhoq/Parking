using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Data
{
    class Member
    {
        private string MemberName;
        private string password;
        


        
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string EmployeeName
        {
            get { return MemberName; }
            set { MemberName = value; }
        }
 
    }
}
