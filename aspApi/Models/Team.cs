using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using TodoApi.Models;

namespace aspApi.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }
        public string Name { get; set; }
     
        public ICollection<TodoItem> ? TodoItems { get; set; }


        public ICollection<TeamUser> ? TeamUsers { get; set; }

    }

    
}
