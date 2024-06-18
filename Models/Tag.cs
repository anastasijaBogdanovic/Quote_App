using System.ComponentModel.DataAnnotations;
namespace quotes_app.Models;

public class Tag
{
    public int Id { get; set; }
    
    [MaxLength(50)]
    public required string Name { get; set; }
    public List<int> QuoteIds { get; set; } = new List<int>();
}