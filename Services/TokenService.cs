using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using aspnet_api_server.Interfaces;
using aspnet_api_server.Models;
using Microsoft.IdentityModel.Tokens;

namespace aspnet_api_server.Services
{

	public class TokenService : ITokenService
	{
		private readonly IConfiguration _config;
		private readonly SymmetricSecurityKey _key;
		public TokenService(IConfiguration config)
		{
			_config = config;
			_key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
		}

		public string CreateToken(AppUser user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
			};
			var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddDays(7),
				SigningCredentials = creds,
				Issuer = _config["JWT:Issuer"],
				Audience = _config["JWT:Audience"]
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);
			return tokenString;
		}

		public string ValidateToken(string token)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = _key,
				ValidateIssuer = true,
				ValidIssuer = _config["JWT:Issuer"],
				ValidateAudience = true,
				ValidAudience = _config["JWT:Audience"],
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal.Identity.Name;
		}
	}
}