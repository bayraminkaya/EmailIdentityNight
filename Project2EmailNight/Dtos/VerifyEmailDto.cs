namespace Project2EmailNight.Dtos
{
    public class VerifyEmailDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string ConfirmCode { get; set; }
    }
}