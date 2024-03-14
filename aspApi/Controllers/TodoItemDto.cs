namespace aspApi.Controllers
{
    public class TodoItemDto
    {
        public long TodoItemId { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}