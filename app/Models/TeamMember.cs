using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace app.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public string? Role { get; set; }
        public Team? Team { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}