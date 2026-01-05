using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.DTO.Input;
using app.DTO.Output;
using app.Models;
using Humanizer;

namespace app.GraphQL.Mutations
{
    public class UserMutation
    {
        public async Task<UserPayload> RegisterUser([Service] AppDbContext context, UserInput input) 
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(input.Password);
            var emailexists = context.Users.Any(u => u.Email == input.Email);
            var usernameexists = context.Users.Any(u => u.Username == input.Username);
            if(input.Username == null || input.Email == null || input.Password == null)
            {
                return new UserPayload
                {
                    Success = false,
                    Error = "All fields are required."
                };
            }
            if(usernameexists)
            {
                return new UserPayload
                {
                    Success = false,
                    Error = "Username already taken. Please choose a different username."
                };
            }
            if (emailexists)
            {
                return new UserPayload
                {
                    Success = false,
                    Error = "Email already in use. Please use a different email."
                };
            }

            var user = new User
            {
                Username = input.Username,
                Email = input.Email,
                PasswordHash = hashedPassword,
            };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return new UserPayload
            {
                Username = user.Username,
                Success = true
            };
        }
    }
}