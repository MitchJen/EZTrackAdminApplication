using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    // Variables and requirements of Vendors
    public class Vendor
    {
        // Vendor code is a 7 character alpha numeric code
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Use letters and numbers only please.")]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Vendor Code Must Be 7 Characters Long")]
        public String vendorCode { get; set; }

        // Vendor Name max of 50 characters containing letters, spaces, periods, underscores or hyphens
        [RegularExpression(@"^[^\s]+[-a-zA-Z_.\-\s]+([-a-zA-Z]+)*$", ErrorMessage = "Use letters, spaces, periods, underscores or hyphens only please")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Vendor Name Must be Below 50 Characters.")]
        public String vendorName { get; set; }

        // Vendor Pool Id is a valid integer
        [RegularExpression(@"^[1-9][0-9]*", ErrorMessage = "Please provide a valid positive integer.")]
        public String vendorPoolId { get; set; }
    }

    // Variables and requirements of vendor Pools
    public class VendorPool
    {
        // Vendor Name max of 50 characters containing letters, spaces, periods, underscores or hyphens
        [RegularExpression(@"^[^\s]+[-a-zA-Z_.\-\s]+([-a-zA-Z]+)*$", ErrorMessage = "Use letters, spaces, periods, underscores or hyphens only please")]
        public String description { get; set; }

        // Currency Id is already set as a list of values
        public List<String> currencyId { get; set; }

        // Country Id is already set as a defined list of values
        public List<String> countryId { get; set; }

        // Eligibility Expiration Day is a valid positive integer
        [RegularExpression(@"^[1-9][0-9]*", ErrorMessage = "Please provide a valid positive integer.")]
        public String eligibilityExpirationDay { get; set; }
    }
}
