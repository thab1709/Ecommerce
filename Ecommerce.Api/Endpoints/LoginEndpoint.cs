
namespace Ecommerce.Api.Endpoints;

public class LoginEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", ([FromBody] LoginRequest user, IConfiguration config) =>
        {
          
            if (user.UserName == "admin" && user.Password == "admin")
            {
                var jwtSetting = config.GetSection("jwtSetting");
                var secretKey = jwtSetting["Secrect"]!;
                var issuer = jwtSetting["Issuer"];
                var audience = jwtSetting["Audience"];

               
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("GoTrust", "VietNam") 
                };

               
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

             
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(60),
                    signingCredentials: creds
                );

              
                return Results.Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                });
            }

            return Results.Unauthorized();
        })
        .WithName("Login")
        .WithTags("Auth");
    }
}

public record LoginRequest(string UserName, string Password);