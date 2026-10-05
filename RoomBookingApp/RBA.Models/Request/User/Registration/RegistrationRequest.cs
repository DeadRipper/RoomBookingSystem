using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request.User.Registration
{
    public class RegistrationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
