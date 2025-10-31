using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectiveAgency.Tests.Models.Responses
{
    public class AbilityResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string Type { get; set; } = "special";
        public string Description { get; set; } = string.Empty;
        public int DangerLevel { get; set; } = 1;
        public string Range { get; set; } = string.Empty;
        public string Activation { get; set; } = string.Empty;
    }
}
