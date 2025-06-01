using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Infrastructure.Entity;
using Microsoft.AspNetCore.Http;
namespace Ecommerce.Infrastructure.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = RoleStatus.CUSTOMER.ToString();
        public bool IsActive { get; set; }
        public string? Token { get; set; }
        public string? Avatar { get; set; }
    }
    public class UserCreateDto
    {
        public Guid Id { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; } = default!;

        [DataType(DataType.Password), Required, MinLength(6)]
        public string Password { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Name { get; set; } = default!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = RoleStatus.CUSTOMER.ToString();

        public bool IsActive { get; set; } = true;
        public IFormFile? Avatar { get; set; }
    }

    public class UpdateInfoDto
    {
        public Guid Id { get; set; }
        [EmailAddress, Required]
        public string Email { get; set; } = default!;
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
