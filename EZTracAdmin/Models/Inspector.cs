using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class Inspector
    {
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "3 Characters are required")]
        public string initials { get; set; }
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        public string firstName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        public string lastName { get; set; }
        [RegularExpression(@"^[a-zA-Z]+(?:--?[a-zA-Z]+)*$", ErrorMessage = "Use letters or hyphens only please")]
        public Location location { get; set; }
    }
    public enum Location
    {
        Renton,
        CrossDock,
        SLP,
        ATLANTA,
        Toronto
    }
}
