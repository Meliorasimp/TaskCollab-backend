using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;

namespace app.DTO.Output
{
    public class TeamPayload
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}