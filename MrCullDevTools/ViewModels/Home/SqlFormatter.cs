using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MrCullDevTools.ViewModels.Home
{
    public class SqlFormatter
    {
        
        [Display(Name = "SQL", Description = "SQL to format.")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        public string inputSQL { get; set; }
        public string outputSQL { get; set; }

        public SqlFormatter()
        {
            outputSQL = string.Empty;
        }

    }
}