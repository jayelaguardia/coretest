using System.ComponentModel.DataAnnotations;

namespace coretest.Resources
{
    public class LoginResource
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
}
