using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Models.Responses
{
    public class CaseResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "open";
        public string Priority { get; set; } = "medium";
        public List<string> AssignedTo { get; set; } = new();
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? Reward { get; set; }
    }
}
