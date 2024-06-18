using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace quotes_app.Models;
public class Quote
{
    public int Id { get; set; }
    public required string QuoteContent { get; set; }
    
    [MaxLength(50)]
    public required string Author { get; set; }
    public List<Rating> Ratings { get; set; } = new List<Rating>();
}

