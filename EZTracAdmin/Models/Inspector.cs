using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class Inspector
    {
        // Letters or hyphens for initials
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "3 Characters are required")]
        public string initials { get; set; }

        // Letters and hyphens for first name
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        public string firstName { get; set; }

        // Letters and hyphens for last name
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        public string lastName { get; set; }

        // Select from location list
        // [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        // public Location location { get; set; }
        public List<string> location { get; set; }
    }
}
