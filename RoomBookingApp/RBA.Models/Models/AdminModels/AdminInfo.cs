using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models.AdminModels
{
    public class AdminInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}