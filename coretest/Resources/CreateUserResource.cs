using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace coretest.Resources
{
    public class CreateUserResource
    {
        [Required]
        [MaxLength(30)]
        public string username { get; set; }

        [Required]
        [MaxLength(72)]
        [MinLength(8)]
        public string password { get; set; }
    }
}
