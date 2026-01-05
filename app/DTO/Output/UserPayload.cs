using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.DTO.Output
{
    public class UserPayload
    {
        public string? Username { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}