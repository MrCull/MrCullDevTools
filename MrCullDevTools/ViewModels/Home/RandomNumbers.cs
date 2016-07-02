using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MrCullDevTools.ViewModels.Home
{
    public class RandomNumbers
    {
        public string ReasonPhrase;

        [Display(Name = "Minimum", Description = "Please provide a valid Minimum.")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        [MaxLength(4, ErrorMessage = "{0} cannot be more than {1} characters")]
        public string Min { get; set; }

        [Display(Name = "Maximum", Description = "Please provide a valid Minumim.")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        [MaxLength(9, ErrorMessage = "{0} cannot be more than {1} characters")]
        public string Max { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        [MaxLength(4, ErrorMessage = "{0} cannot be more than {1} characters")]
        public string Quantity { get; set; }

        public ArrayList Numbers;
    }
}