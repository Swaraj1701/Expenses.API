using Azure.Core;
using Expenses.API.Data;
using Expenses.API.DTOs;
using Expenses.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class AuthController : ControllerBase
    {
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly AppDbContext _context;
        public AuthController(AppDbContext dbContext, PasswordHasher<User> passwordHasher)
        {
            _context = dbContext;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto payLoad)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=> x.Email == payLoad.Email);
            if (user == null) return Unauthorized("Invalid Credentials.");
            var password = _passwordHasher.VerifyHashedPassword(user, user.Password, payLoad.Password);
            if(password == PasswordVerificationResult.Failed) return Unauthorized("Invalid Credentials.");
            var token = GenerateJwtToken(user);
            return Ok(new {Token =  token});
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] PostUserDto payLoad)
        {
            if (await _context.Users.AnyAsync(x => x.Email == payLoad.Email)) 
                return BadRequest("This email address is already registered.");
            var hashedPassword = _passwordHasher.HashPassword(null, payLoad.Password);
            var newUser = new User()
            {
                Email = payLoad.Email,
                Password = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            var token = GenerateJwtToken(newUser);

            return Ok( new { Token = token });
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-very-secure-secret-key-32-chars-long"));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "dotnethow.net",
                audience: "dotnethow.net",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
