using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Security
{
    public interface IUserService
    {
        IList<IUserInfo> Users { get; }
        LoginResult Login(string userid, string pwd, string host);
        bool CheckPwd(string userid, string pwd);
    }
}
