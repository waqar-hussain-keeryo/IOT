namespace IOT.Entities.DTO
{
    public class ResponseDTO
    {
        public ResponseDTO(){}
        public ResponseDTO(bool success, string message, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class ApiResponse
    {
        public ApiResponse() { }
        public ApiResponse(bool success, string message, object data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    public class LoginResponseDTO
    {
        public LoginResponseDTO() { }

        public LoginResponseDTO(string email, string roles, string token)
        {
            Email = email;
            Roles = roles;
            Token = token;
        }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string Token { get; set; }
    }
}
