using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [RegularExpression("^([a-zA-Z0-9]{5,20})$", ErrorMessage = "The {0} must contain only alphanumeric characters")]
        [Display(Name = "Username")]
        public string RegisterUsername { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string RegisterEmail { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string RegisterPassword { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Country")]
        public string Country { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "BirthDate (MM/dd/yyyy)")]
        public DateTime BirthDate { get; set; }
    }
}
