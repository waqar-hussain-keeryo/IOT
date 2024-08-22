namespace IOT.Entities.DTO
{
    public class ResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class LoginResponseDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
