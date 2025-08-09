using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
 
    public class ApiKey
    {
        public int ConfigId { get; set; }
        public string ProjectName { get; set; } = null!;
        public string? KeyName { get; set; }
        public string KeyValue { get; set; } = null!;
        public DateTime LastRotated { get; set; }
        public int RotationMinutes { get; set; }
        public int RotationSeconds { get; set; }
        public DateTime NextRotation { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
