using Ecommerce.Infrastructure.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class AuthDto
    {
        public class UserLoginDto
        {
            [EmailAddress, Required]
            public string Email { get; set; } = default!;

            [DataType(DataType.Password), Required]
            public string Password { get; set; } = default!;
        }
        public class AuthResponseDto
        {
            public string AccessToken { get; set; } = default!;
            public string RefreshToken { get; set; } = default!;
            public DateTime Expiry { get; set; }
            public UserDto User { get; set; } = default!;
        }
        public class RefreshTokenRequestDto
        {
            [Required]
            public string AccessToken { get; set; } = default!;

            [Required]
            public string RefreshToken { get; set; } = default!;
        }

        public class ResetPasswordDto
        {
            [EmailAddress, Required]
            public string Email { get; set; } = default!;

            [Required]
            public string ResetToken { get; set; } = default!; // Thêm token khi confirm reset

            [DataType(DataType.Password), Required, MinLength(6)]
            public string NewPassword { get; set; } = default!;
        }

        public class ChangePasswordDto
        {
            [DataType(DataType.Password), Required]
            public string CurrentPassword { get; set; } = default!;

            [DataType(DataType.Password), Required, MinLength(6)]
            public string NewPassword { get; set; } = default!;
        }
        public class SocialLoginDto
        {
            [Required]
            public string Provider { get; set; } = default!; // "Google", "Facebook"

            [Required]
            public string IdToken { get; set; } = default!;
        }
        public class VerifyEmailDto
        {
            [Required]
            public string Token { get; set; } = default!;

            [EmailAddress, Required]
            public string Email { get; set; } = default!;
        }
        public class ResendEmailConfirmationDto
        {
            [EmailAddress, Required]
            public string Email { get; set; } = default!;
        }
    }
}
