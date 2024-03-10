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
        public ICollection<User> Users { get; set; }

        public TodoItem TodoItem { get; set; }


    }
}
