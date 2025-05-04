namespace UserCenter.API.DTOs.Auth
{
    public class RegisterRespondDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
