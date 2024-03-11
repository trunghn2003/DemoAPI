namespace aspApi.Models
{
    public class TeamUser
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public bool IsLeader { get; set; }
        public bool IsAdmin { get; set; }
    }
}
