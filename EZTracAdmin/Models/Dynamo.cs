using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace EZTracAdminRSC.Models
{
    [DynamoDBTable("students")]
    public class Dynamo
    {
        [DynamoDBHashKey("id")]
        public int? Id { get; set; }

        [DynamoDBProperty("first_name")]
        public string? FirstName { get; set; }

        [DynamoDBProperty("last_name")]
        public string? LastName { get; set; }

        [DynamoDBProperty("class")]
        public int Class { get; set; }

        [DynamoDBProperty("country")]
        public string? Country { get; set; }
    }
}
