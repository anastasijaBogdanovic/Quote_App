using System.ComponentModel.DataAnnotations;
namespace quotes_app.Models;
public class LoginRequest
{
    [MaxLength(50)]
    public required string Username { get; set; }
    public required string Password { get; set; }
}

