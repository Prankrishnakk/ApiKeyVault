using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos
{
    public class ApiKeyCreateDto
    {
        public string ProjectName { get; set; }
        public string KeyName { get; set; }
        public int RotationMinutes { get; set; }
        public int RotationSeconds { get; set; }
    }

}
