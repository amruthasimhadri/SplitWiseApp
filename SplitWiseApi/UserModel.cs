using System.ComponentModel.DataAnnotations;

namespace SplitWiseApi
{
    public class UserModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string Number { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
