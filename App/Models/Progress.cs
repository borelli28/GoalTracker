using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class Progress
{   
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 10);

    [Required]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    public bool Completed { get; set; } = false;

    [StringLength(90)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    [Required]
    public string GoalId { get; set; }
    public Goal? Goal { get; set; }
}
