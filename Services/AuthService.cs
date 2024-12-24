using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string HashPassword(string password)
    {
        // Hash the password with a work factor of 10 (default)
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string enteredPassword, string storedHashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHashedPassword);
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var secretKey = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException(nameof(secretKey), "Secret key is not configured.");
        }
        var key = Encoding.ASCII.GetBytes(secretKey);

        try
        {
            Console.WriteLine("Token to validate: " + token);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

            if (result.IsValid)
            {
                return new ClaimsPrincipal(result.ClaimsIdentity);
            }
            else
            {
                Console.WriteLine("Token validation failed: " + result.Exception?.Message);
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public string GenerateToken(string email, string userId, string username, bool isCoach)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var secretKey = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentNullException(nameof(secretKey), "Secret key is not configured.");
        }
        var key = Encoding.ASCII.GetBytes(secretKey);
        var expirationTime = DateTime.UtcNow.AddHours(1);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("id", userId),
                new Claim("username", username),
                new Claim("is_coach", isCoach.ToString())
            }),
            Expires = expirationTime, // Set the token to expire in 1 hour
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return token;
    }
}
