using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.Account
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 5)]
        [RegularExpression("^([a-zA-Z0-9]{5,20})$", ErrorMessage = "The {0} must contain only alphanumeric characters")]
        [Display(Name = "Username")]
        public string ExtUsername { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string ExtFirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string ExtLastName { get; set; }

        [Required]
        [Display(Name = "Country")]
        public string ExtCountry { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "BirthDate (MM/dd/yyyy)")]
        public DateTime ExtBirthDate { get; set; }
    }
}
