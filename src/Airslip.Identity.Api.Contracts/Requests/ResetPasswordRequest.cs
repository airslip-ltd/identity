using System.ComponentModel.DataAnnotations;

namespace Airslip.Identity.Api.Contracts.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }

        public ResetPasswordRequest(string password, string confirmPassword, string email, string token)
        {
            Password = password;
            ConfirmPassword = confirmPassword;
            Email = email;
            Token = token;
        }
    }
}