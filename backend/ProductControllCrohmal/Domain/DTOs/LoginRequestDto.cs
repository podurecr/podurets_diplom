using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class LoginRequestDto
    {
        public string userLogin { get; set; } = string.Empty;

        public string userPassword { get; set; } = string.Empty;
    }
}
