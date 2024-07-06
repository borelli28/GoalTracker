using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models;

public class Goal
{   
    [Key]
    [StringLength(10)]
    public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 10);

    [Required]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [StringLength(90)]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
