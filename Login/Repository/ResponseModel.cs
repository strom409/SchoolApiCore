namespace login.Repository
{
    public class ResponseModel
    {
        public bool IsSuccess { set; get; }
        public string Message { get; set; }
        public int Status { get; set; }
        public object ResponseData { get; set; } = null;
        public string Error { get; set; }
    }
}
