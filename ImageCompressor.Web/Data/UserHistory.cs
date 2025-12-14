using System.ComponentModel.DataAnnotations;

namespace ImageCompressor.Web.Data;

public class UserHistory
{
    [Key]
    public int Id { get; set; }
    public int UserId { get; set; } // İşlemi kim yaptı?
    public string ActionType { get; set; } = ""; // Örn: "Compress", "Resize"
    public string FileName { get; set; } = ""; // Örn: "manzara.jpg"
    public string Details { get; set; } = ""; // Örn: "5MB -> 2MB"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}