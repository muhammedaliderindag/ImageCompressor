using System.ComponentModel.DataAnnotations;

namespace ImageCompressor.Web.Auth;

public class UserAccount
{
    [Key] // Birincil Anahtar
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}