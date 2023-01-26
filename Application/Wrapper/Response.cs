namespace Application.Wrapper
{
    public class Response<T>
    {
        public Response()
        {

        }
        public Response(T data, string message = null)
        {
            IsSuccess = true;
            Message = message;  
            Data = data;
        }
        public Response(string message)
        {
            Message = message;
            IsSuccess = false;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; }     
        public T Data { get; set; }
        public List<string> Errors { get; set; }
    }
}
