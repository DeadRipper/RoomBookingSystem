using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Models.AuthModels
{
    public class AuthModel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExparationDate { get; set; }
    }
}