﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;

namespace Talabat.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(AppUser user,UserManager<AppUser> userManager)
        {
            // Payloads
            // 1. Private Clams

            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.DisplayName),
                new Claim(ClaimTypes.Email,user.Email)
            };
            var UserRoles = await userManager.GetRolesAsync(user);
            foreach (var Role in UserRoles)
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));

            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            var Token = new JwtSecurityToken(

                issuer: configuration["JWT:ValidIssure"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                claims:AuthClaims,
                signingCredentials: new SigningCredentials(AuthKey,SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(Token);
           
        }
    }
}
