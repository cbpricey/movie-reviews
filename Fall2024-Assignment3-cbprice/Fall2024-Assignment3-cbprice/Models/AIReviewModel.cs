using System.ComponentModel.DataAnnotations;

namespace Fall2024_Assignment3_cbprice.Models;

public class AIReviewModel
{
    [Key] //defines property as primary key
    public int Id { get; set; } // Primary key
    public string ReviewText { get; set; } = string.Empty;
    public string Sentiment { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}