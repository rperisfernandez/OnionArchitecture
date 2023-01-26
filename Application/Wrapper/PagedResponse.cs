namespace Application.Wrapper
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize)
        {
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
            this.Data = data;
            this.Message = null;
            this.IsSuccess = true;
            this.Errors = null; 
        }
    }
}
