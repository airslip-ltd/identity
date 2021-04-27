using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Airslip.Security.Jwt
{
    public class JwtBearerToken
    {
        public string Generate(string privateKey, string audience, string issuer, int expiresTime, string userId)
        {
            if (string.IsNullOrWhiteSpace(privateKey))
                throw new ArgumentNullException(nameof(privateKey), "private key must be set");

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(privateKey));
            if (key.KeySize < 256)
                throw new ArgumentException(
                    "the key must be at least 256 in length -> 32 characters in length at least",
                    nameof(privateKey));

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new()
            {
                new Claim("userid", userId)
            };

            if (claims.All(c => c.Type != "jti"))
                claims.Add(new Claim("jti", Guid.NewGuid().ToString("N")));

            JwtSecurityToken token = new(
                issuer,
                audience,
                claims,
                expires: DateTime.Now.AddSeconds(expiresTime),
                signingCredentials: credentials);

            JwtSecurityTokenHandler tokenHandler = new();
            return tokenHandler.WriteToken(token);
        }
    }
}