using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.DTO.Output;
using app.Data;
using System.Security.Claims;
using app.Models;
using HotChocolate.Types;
using app.DTO.Input;

namespace app.GraphQL.Mutations
{
    [ExtendObjectType(typeof(Mutation))]
    public class TeamMutation
    {
        public async Task<TeamPayload> CreateTeam(
            TeamInput input,
            [Service] AppDbContext context, 
            ClaimsPrincipal user)
        {
            try 
            {
                if(input.Name == null || input.Description == null)
                {
                    return new TeamPayload
                    {
                        Success = false,
                        Error = "All fields are required."
                    };
                }
                
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(string.IsNullOrEmpty(userId))
                {
                    return new TeamPayload
                    {
                        Success = false,
                        Error = "Unauthorized. Please log in."
                    };
                }

                var team = new Team 
                {
                    Name = input.Name,
                    Description = input.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = user.FindFirst(ClaimTypes.Name)?.Value
                };
                
                await context.Teams.AddAsync(team);
                await context.SaveChangesAsync(); // Save to get the team ID
                
                var teamMember = new TeamMember
                {
                    TeamId = team.Id,
                    UserId = int.Parse(userId),
                    Role = "Owner",
                    JoinedAt = DateTime.UtcNow
                };
                
                await context.TeamMembers.AddAsync(teamMember);
                await context.SaveChangesAsync();
                
                return new TeamPayload
                {
                    Name = team.Name,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new TeamPayload
                {
                    Success = false,
                    Error = ex.Message
                };
            }  
        }
    }
}