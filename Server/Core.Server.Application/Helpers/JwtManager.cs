﻿using Core.Server.Common.Attributes;
using Core.Server.Common.Entities.Helpers;
using Core.Server.Shared.Resources;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Core.Server.Application.Helpers
{
    [Inject]
    public class JwtManager : IJwtManager
    {
        private const string UserResourceKey = nameof(UserResource);

        public string GenerateToken(UserResource user, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GetClaims(user),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsIdentity GetClaims(UserResource user)
        {
            var userResourceValue = JsonConvert.SerializeObject(user);
            return new ClaimsIdentity(
                new Claim[]{
                new Claim(UserResourceKey,userResourceValue)});
        }

        public UserResource GetUser(ClaimsPrincipal ClaimsPrincipal)
        {
            var claim = ClaimsPrincipal.Claims.FirstOrDefault();
            if (claim == null) return null;
            return JsonConvert.DeserializeObject<UserResource>(claim.Value);
        }
    }
}
