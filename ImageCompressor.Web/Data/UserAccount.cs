using System.ComponentModel.DataAnnotations;

namespace ImageCompressor.Web.Auth;

public class UserAccount
{
    [Key]
    public int Id { get; set; }

    // --- YENİ EKLENEN ALANLAR ---
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    // ----------------------------

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}