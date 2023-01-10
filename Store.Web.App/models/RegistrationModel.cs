using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Web.App.models
{
    public class RegistrationModel
    {
        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;        

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string PhoneNumber { get; set; } = null!;
    }
}
