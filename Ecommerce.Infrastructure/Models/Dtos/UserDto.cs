using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecommerce.Infrastructure.Entity;
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
    }
    public class UserCreateDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = RoleStatus.CUSTOMER.ToString();
        public bool IsActive { get; set; } = true;
    }
    public class UserLoginDto
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

}
