using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.Admin.Login
{
    public class LoginRequest : RequestBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}