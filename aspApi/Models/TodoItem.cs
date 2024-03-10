using aspApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace TodoApi.Models;

public class TodoItem
{
    [Key]
    public long TodoItemId { get; set; }
    public string? Name { get; set; }
    public bool IsComplete { get; set; }
    public int TeamId { get; set; } 
    public Team Team { get; set; } = null!;


}