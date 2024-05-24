using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace fashion.Areas.Admin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User name is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]

        public string Password { get; set; }
    }
}
