using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NanaFoodDAL.Context;
using NanaFoodDAL.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NanaFoodDAL.IRepository.Repository
{
    internal class TokenService : ITokenService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<User> _userManager;

        public TokenService(ApplicationDbContext db, IConfiguration config, UserManager<User> userManager)
        {
            _db = db;
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApiSettings:JwtOptions:SigningKey"]));
            _userManager = userManager;
        }

        public string CreateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier,user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials,
                Issuer = _config["ApiSettings:JwtOptions:Issuer"],
                Audience = _config["ApiSettings:JwtOptions:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
