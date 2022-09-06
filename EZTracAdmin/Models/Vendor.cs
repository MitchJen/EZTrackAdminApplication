using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class Vendor
    {
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Use letters and numbers only please.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Vendor Code Must Be 7 Characters Long")]
        public String vendorCode { get; set; }

        [RegularExpression(@"^[^\s]+[-a-zA-Z_.\-\s]+([-a-zA-Z]+)*$", ErrorMessage = "Use letters, spaces, periods, underscores or hyphens only please")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Vendor Name Must be Below 50 Characters.")]
        public String vendorName { get; set; }

        [RegularExpression(@"^[1-9][0-9]*", ErrorMessage = "Please provide a valid positive integer.")]
        public String vendorPoolId { get; set; }
    }

    public class VendorPool
    {
        [RegularExpression(@"^[^\s]+[-a-zA-Z_.\-\s]+([-a-zA-Z]+)*$", ErrorMessage = "Use letters, spaces, periods, underscores or hyphens only please")]
        public String description { get; set; }

        public List<String> currencyId { get; set; }

        public List<String> countryId { get; set; }
        [RegularExpression(@"^[1-9][0-9]*", ErrorMessage = "Please provide a valid positive integer.")]
        public String eligibilityExpirationDay { get; set; }
    }
}
