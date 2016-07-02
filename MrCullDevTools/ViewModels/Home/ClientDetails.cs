using System.ComponentModel.DataAnnotations;

namespace MrCullDevTools.ViewModels.Home
{
    public class ClientDetails
    {
        public string browser { get; set; }
        public string os { get; set; }
        public string userHostAddress { get; set; }
    }    
}