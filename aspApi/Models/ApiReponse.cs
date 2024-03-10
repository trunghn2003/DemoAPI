namespace aspApi.Models
{
    public class ApiReponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public Object Data { get; set; }
    }
}
