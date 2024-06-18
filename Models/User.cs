using System.ComponentModel.DataAnnotations;
namespace quotes_app.Models;

public class User
{
    public int Id { get; set; }
    
    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public required string Username { get; set; }

    [RegularExpression("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$")]
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Token { get; set; } // token za autentifikaciju
    public string? Role { get; set; } // admin, user itd.
}