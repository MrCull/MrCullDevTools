using System.ComponentModel.DataAnnotations;

namespace MrCullDevTools.ViewModels.Home
{
    public class JsonFormatter
    {

        [Display(Name = "Json", Description = "JSON to format.")]
        [Required(ErrorMessage = "{0} is required")]
        [MinLength(1, ErrorMessage = "{0} cannot be less than {1} characters")]
        public string inputJson { get; set; }
        public string outputJson { get; set; }

        public JsonFormatter()
        {
            outputJson = string.Empty;
        }

    }
}