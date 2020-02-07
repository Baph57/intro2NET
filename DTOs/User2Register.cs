using System;
using System.ComponentModel.DataAnnotations;

namespace intro2NET.API.DTOs
{
    public class User2Register
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Your password must be at least 8 characters long!")]
        public string Password { get; set; }
    }
}
