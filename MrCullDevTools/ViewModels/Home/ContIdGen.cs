using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MrCullDevTools.ViewModels.Home
{
    public class ContIdGen
    {
        [Display(Name = "Prefix", Description = "Please provide a valid prefix, this should be 4 letters or *** for random.")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(4, ErrorMessage = "{0} cannot be less than {1} characters")]
        [MaxLength(4, ErrorMessage = "{0} cannot be more than {1} characters")]        
        public string Prefix { get; set; }

        [Required(ErrorMessage = "{0} is required")]        
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        [MaxLength(4, ErrorMessage = "{0} cannot be more than {1} characters")]
        public string Quantity { get; set; }

        public ArrayList ContainerIDs;

    }
}