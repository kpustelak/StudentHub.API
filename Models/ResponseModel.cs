namespace StudentHub.API.Models
{
    public class ResponseModel<T>
    {
        public bool Status { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public T Data { get; set; } = default!;

        public Exception? Exception { get; set; }
    }
}
