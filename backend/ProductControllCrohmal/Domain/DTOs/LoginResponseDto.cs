using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class LoginResponseDto
    {
        public UserDTO? User { get; set; }

        public string? Token { get; set; }

        public DateTime? TokenExpiresAt { get; set; }
    }
}
