using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using app.Data;
using app.DTO.Input;
using app.DTO.Output;
using app.Models;
using Humanizer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<UserPayload> LoginUser([Service] AppDbContext context, [Service] IConfiguration configuration, LoginInput input)
        {
            try 
            {
                var user = context.Users.FirstOrDefault(u => u.Email == input.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash))
                {
                    return new UserPayload
                    {
                        Success = false,
                        Error = "Invalid email or password."
                    };
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email ?? ""),
                    new(ClaimTypes.Name, user.Username ?? ""),
                    new(ClaimTypes.Role, "User")
                };
                
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                
                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(24),
                    signingCredentials: credentials
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return new UserPayload
                {
                    Username = user.Username,
                    Token = tokenString,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new UserPayload
                {
                    Success = false,
                    Error = "An error occurred during login: " + ex.Message
                };
            }
        }
    }
}