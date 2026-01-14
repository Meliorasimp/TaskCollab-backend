using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public ICollection<TeamMember>? Members { get; set; }
    }
}