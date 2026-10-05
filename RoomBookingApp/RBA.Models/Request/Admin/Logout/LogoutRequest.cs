using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin.Logout
{
    public class LogoutRequest : RequestBase
    {
        public string UserName { get; set; }
    }
}