﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRN232_SU25_SE182614_Repository.Models;
using PRN232_SU25_SE182614_Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN232_SU25_SE182614_LocDpx.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly SystemAccountService _service;
        private readonly IConfiguration _config;
        public SystemAccountController(SystemAccountService service, IConfiguration config) {
            _service = service;
            _config = config;
        }

        public sealed record LoginRequest(string Email, string Password);
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _service.Login(request.Email, request.Password);

            if (user == null)
            {
                return Unauthorized("incorrect Email or Password");
            }

            var token = GenerateJSONWebToken(user);
            return Ok(new
            {
                Token = token,
                Role = GetRoleName(user.Role ?? 0)
            });
        }



        private string GenerateJSONWebToken(SystemAccount systemAccount)
        {
            var key = _config["JwtSettings:Key"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var expiryMinutes = _config["JwtSettings:ExpiryInMinutes"];

            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new InvalidOperationException("JWT settings are not properly configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, systemAccount.Username ?? ""),
                new Claim(ClaimTypes.Email, systemAccount.Email ?? ""),
                new Claim(ClaimTypes.Role, systemAccount.Role.ToString()),

            };

            var expiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(expiryMinutes));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Log token generation for debugging
            Console.WriteLine($"Generated token for user: {systemAccount.Username}");
            Console.WriteLine($"Token expires at: {expiry}");
            Console.WriteLine($"Token first 50 chars: {tokenString.Substring(0, Math.Min(50, tokenString.Length))}...");

            return tokenString;
        }

        public static string GetRoleName(int roleId)
        {
            return roleId switch
            {
                1 => "administrator",
                2 => "moderator",
                3 => "developer",
                4 => "member",
                _ => "unknown"
            };
        }
    }
}
