using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Models.Responses
{
    public class AuthResponse
    {
        public string Message { get; set; } = string.Empty;
        public User User { get; set; } = new();
    }

    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
