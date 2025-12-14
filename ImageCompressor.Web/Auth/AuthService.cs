using ImageCompressor.Web.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ImageCompressor.Web.Auth;

public class AuthService
{
    private readonly AppDbContext _context;
    private const string JWT_SECRET = "super_gizli_ve_uzun_bir_anahtar_kelime_12345";

    // Constructor Injection ile veritabanını alıyoruz
    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public bool Register(UserAccount user)
    {
        // Veritabanında bu kullanıcı var mı?
        if (_context.Users.Any(u => u.Username == user.Username))
            return false;

        _context.Users.Add(user);
        _context.SaveChanges(); // Değişiklikleri kaydet
        return true;
    }

    public string? Login(string username, string password)
    {
        // Kullanıcıyı veritabanında bul
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user == null) return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(UserAccount user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JWT_SECRET);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}