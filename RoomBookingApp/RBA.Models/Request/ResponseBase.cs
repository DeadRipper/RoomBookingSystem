using System;
using System.Collections.Generic;
using System.Text;

namespace RBA.Models.Request
{
    public class ResponseBase
    {
        public Guid RequestId { get; set; } = Guid.NewGuid();
    }
}