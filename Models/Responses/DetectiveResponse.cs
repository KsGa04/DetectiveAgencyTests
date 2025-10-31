using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Models.Responses
{
    public class DetectiveResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Ability { get; set; } = string.Empty;
        public string AbilityId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public int Age { get; set; }
        public string JoinedAt { get; set; } = string.Empty;
    }
}
