using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EZTracAdminRSC.Models
{
    public class Dealer
    {
        // Dbs Id can only be up to 5 characters
        [StringLength(5, ErrorMessage = "Dbs Id Must Be At Most 5 Characters Long")]
        public string DbsId { get; set; }

        // Dealer Id can only be up to 5 characters
        [StringLength(5, ErrorMessage = "Dealer Id Must Be At Most 5 Characters Long")]
        public string DealerId { get; set; }

        public string BaseUrl { get; set; }

        public string SubscriptionId { get; set; }
    }
    public class DealerList
    {
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
        public List<Dealer> Dealers { get; set; }
    }
}
