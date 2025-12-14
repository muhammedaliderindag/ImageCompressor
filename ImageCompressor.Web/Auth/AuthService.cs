using ImageCompressor.Web.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography; 

namespace ImageCompressor.Web.Auth;

public class AuthService
{
    private readonly AppDbContext _context;
    private const string JWT_SECRET = "super_gizli_ve_uzun_bir_anahtar_kelime_12345";

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
    // -------------------------------------------

    public bool Register(UserAccount user)
    {
        if (_context.Users.Any(u => u.Username == user.Username))
            return false;

        user.Password = HashPassword(user.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
        return true;
    }

    public string? Login(string username, string password)
    {
        var hashedPassword = HashPassword(password);

        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashedPassword);

        if (user == null) return null;

        return GenerateJwtToken(user);
    }

    public bool UpdateUser(UserAccount updatedUser, string? newPassword = null)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == updatedUser.Id);
        if (existingUser == null) return false;

        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Email = updatedUser.Email;
        
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            existingUser.Password = HashPassword(newPassword);
        }

        _context.SaveChanges();
        return true;
    }

    public UserAccount? GetUserById(int id)
    {
        return _context.Users.FirstOrDefault(u => u.Id == id);
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
    public List<UserAccount> GetAllUsers()
    {
        return _context.Users.ToList();
    }

    public bool DeleteUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user == null) return false;

        _context.Users.Remove(user);
        _context.SaveChanges();
        return true;
    }
}