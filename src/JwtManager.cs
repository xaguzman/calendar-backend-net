using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CalendarBackend.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CalendarBackend;

public class JwtManager
{
    private IConfiguration Configuration;

    public JwtManager(IConfiguration configuration){
        this.Configuration = configuration;
    }

    public string GenerateUserJwtToken(UserModel user){
        var issuer = Configuration["JwtSettings:Issuer"];
        var audience = Configuration["JwtSettings:Audience"];
        var signKey = Configuration["JwtSettings:SignKey"];
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey!));
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


        var tokenOptions = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: createUserClaims(user),
            expires: DateTime.Now.AddHours(2),
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return tokenString;
    }

    private List<Claim> createUserClaims(UserModel user){
        var claims = new List<Claim>(){
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("uid", user.Id),
            new Claim("name", user.Name),
            // new Claim("name", user.Name),
        };

        return claims;
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token){
        var tokenValidationParams = new TokenValidationParameters
        {            
            // Validate the Issuer
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SignKey"]!))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParams, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;

    }

    

    // public string GenerateCommunityJwtToken(ClaimsPrincipal user, CommunityModel community, IList<string> roles)
    // {
    //     var issuer = Configuration["JwtSettings:Issuer"];
    //     var signKey = Configuration["JwtSettings:SignKey"];
    //     var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
    //     var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

    //     var audience = issuer;
        
    //     var communityRelatedClaims = new List<Claim>(){
    //         new Claim(CustomClaimTypes.CommunityId, community.CommunityId.ToString()),
    //         new Claim(CustomClaimTypes.CommunityName, community.CommunityName)
    //     }.Concat(roles.Select(r => new Claim(ClaimTypes.Role, r)));

    //     var jwtClaims = user.Claims.Concat(communityRelatedClaims);

    //     var tokenOptions = new JwtSecurityToken(
    //         issuer: issuer,
    //         audience: audience,
    //         claims: jwtClaims,
    //         expires: DateTime.Now.AddMinutes(5),
    //         signingCredentials: signingCredentials
    //     );

    //     var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    //     return tokenString;
    // }
}
