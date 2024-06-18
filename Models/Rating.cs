using System.ComponentModel.DataAnnotations;

namespace quotes_app.Models;

public class Rating
{
    public int Id { get; set; }

    public bool Value { get; set; }

    public int QuoteId { get; set; }
    public int UserId { get; set; }
}