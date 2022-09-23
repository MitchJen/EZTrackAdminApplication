using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BLLEZtracAdmin
{
    public class DealerInfo
    {
        [JsonPropertyName("DbsId")]
        public string DbsId { get; set; } = default;

        [JsonPropertyName("DealerId")]
        public string DealerId { get; set; } = default;

        [JsonPropertyName("BaseUrl")]
        public string BaseUrl { get; set; } = default;

        [JsonPropertyName("SubscriptionId")]
        public string SubscriptionId { get; set; } = default;
    }

    /**************************************************
    * For reference on Querying DynamoDb from .NET I used:
    *   https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/QueryMidLevelDotNet.html
    *   https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/ScanMidLevelDotNet.html
    *
    *   PutItem:
    *       https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/example_dynamodb_PutItem_section.html
    *   Query:
    *       https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/example_dynamodb_Query_section.html
    *   Update:
    *       https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/example_dynamodb_UpdateItem_section.html
    *   Scan:
    *       https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/example_dynamodb_Scan_section.html
    **************************************************/
    public class BLLDealers
    {
        AmazonDynamoDBClient client;
        public BLLDealers() {
            this.client = new AmazonDynamoDBClient("AccessKey", 
                        "SecretKey", Amazon.RegionEndpoint.GetBySystemName("us-west-2"));
        }private IAmazonDynamoDB dynamoDB;
        string _tableName = "DealerTable";
        public const int _pageLimit = 20;



        /****************************************************
         * This method makes a details Request of the table from the
         * dynamoDB connection. From this information it looks at the table
         * row count to determine how many items are there and then divides
         * that by _pageLimit to determine the number of pages.
         ****************************************************/
        public async Task<int> GetPages()
        {
            // Create DescribeTable request
            DescribeTableRequest detailsRequest = new DescribeTableRequest
            {
                TableName = _tableName
            };

            // Issue DescribeTable request and retrieve the table description
            DescribeTableResponse tableResponse = await client.DescribeTableAsync(detailsRequest);
            TableDescription tableDescription = tableResponse.Table;

            // To detemine the number of pages do itemCount/pageLimit
            return ((int)tableDescription.ItemCount / _pageLimit);
        }

        /****************************************************
         * Load Table takes in the paginator and the current page number
         * the paginator is the lastKey called by the load function. Using this information
         * we can load the next set of lines from table.
         * 
         * It will return a list up to size _pageLimit of DealerInfo items
         * to be displayed in the index view.
         ****************************************************/
        public async Task<DealerInfo[]> LoadTable(string paginator, int pageNum)
        {
            // REMOVE EVERYTHING BELOW POINT UNTIL NEXT MARKED!!!!!!!!!!!!!!!!!!!!!!!!!!!


            // Test index items to simulate the index table 
            DealerInfo dealer1 = new DealerInfo();
            dealer1.DbsId = "KWCC";
            dealer1.DealerId = "V525";
            dealer1.BaseUrl = "http://deverp.solucionesde.cloud:50100/kwcc/ecommerce";
            DealerInfo dealer2 = new DealerInfo();
            dealer2.DbsId = "KWCC";
            dealer2.DealerId = "V737";
            dealer2.BaseUrl = "http://deverp.solucionesde.cloud:50100/kwcc/ecommerce";
            DealerInfo dealer3 = new DealerInfo();
            dealer3.DbsId = "KWCC";
            dealer3.DealerId = "V746";
            dealer3.BaseUrl = "http://deverp.solucionesde.cloud:50100/kwcc/ecommerce";
            DealerInfo dealer4 = new DealerInfo();
            dealer4.DbsId = "RUSH";
            dealer4.DealerId = "A264";
            dealer4.BaseUrl = "https://stage.services.rushenterprises.com/api/v1/Paccar";
            DealerInfo dealer5 = new DealerInfo();
            dealer5.DbsId = "RUSH";
            dealer5.DealerId = "A265";
            dealer5.BaseUrl = "https://stage.services.rushenterprises.com/api/v1/Paccar";
            DealerInfo dealer6 = new DealerInfo();
            dealer6.DbsId = "RUSH";
            dealer6.DealerId = "A266";
            dealer6.BaseUrl = "https://stage.services.rushenterprises.com/api/v1/Paccar";

            DealerInfo[] list = { dealer1, dealer2, dealer3, dealer4, dealer5, dealer6};
            return list;

            // REMOVE EVERYTHING ABOVE THIS UNTIL OTHER MARK !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Get the number of pages from the GetPages method
            int pages = await GetPages();

            // Create a dictionary item for the last key evaluated from the paginator
            Dictionary<string, AttributeValue> lastKeyEvaluated = 
                                            new Dictionary<string, AttributeValue>();
            lastKeyEvaluated.Add("DealerId", new AttributeValue { S = paginator });

            // If we haven't reached the end of the table then read the next set of items from the table
            if ((pages) >= pageNum)
            {
                // Create a scan request to get the next set of items
                var request = new ScanRequest
                {
                    TableName = _tableName,
                    Limit = _pageLimit,
                    ExclusiveStartKey = lastKeyEvaluated
                };
                var response = await client.ScanAsync(request);

                // Create a DealerInfo Item for  each item in the response and add the response item values to it
                int i = 0;
                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    DealerInfo dealer = new DealerInfo();
                    dealer.DbsId = item["DbsId"].ToString();
                    dealer.DealerId = item["DealerId"].ToString();
                    dealer.BaseUrl = item["BaseUrl"].ToString();
                    if (item["SubscriptionId"] != null)
                    {
                        dealer.SubscriptionId = item["SubscriptionId"].ToString();
                    }
                    list.SetValue(dealer, i);
                    i ++;
                }
            }
            // return the list of dealerInfo Items
            return list;
        }

        /****************************************************
         * Create Async is a method that takes in items from a dealer
         * class item and inserts it into the dealer list.
         ****************************************************/
        public async Task<bool> CreateAsync(string dbsId, 
                                            string dealerId, 
                                            string basicUrl, 
                                            string subscriptionId)
        {
            // Fill values of dealer info class item with the given values
            DealerInfo dealer = new DealerInfo
            {
                DbsId = dbsId,
                DealerId = dealerId,
                BaseUrl = basicUrl,
                SubscriptionId = subscriptionId
            };
            
            // Convert the fields to json to be read into the database
            var customerAsJson = JsonSerializer.Serialize(dealer);
            var itemAsDocument = Document.FromJson(customerAsJson);
            var itemAsAttributes = itemAsDocument.ToAttributeMap();

            // Create a create Item request for the given item values
            var createItemRequest = new PutItemRequest
            {
                TableName = _tableName,
                Item = itemAsAttributes
            };
            var response = await client.PutItemAsync(createItemRequest);

            // Return the status code indicating if it was successful or not
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        /****************************************************
         * The Update Async method takes in the old dbsId and old Dealer ID
         * along with the new dealer fields and makes an updateitemrequest
         * of the database client to change the specified line to the 
         * given values.
         ****************************************************/
        public async Task<bool> UpdateAsync(string oldDbsId, 
                                            string oldDealerId, 
                                            string dbsId, 
                                            string dealerId, 
                                            string basicUrl, 
                                            string subscriptionId)
        {
           // Create a dealer info class item with the given values
            DealerInfo dealer = new DealerInfo
            {
                DbsId = dbsId,
                DealerId = dealerId,
                BaseUrl = basicUrl,
                SubscriptionId = subscriptionId
            };
            // Convert it to json to be used in the request
            var customerAsJson = JsonSerializer.Serialize(dealer);
            var itemAsDocument = Document.FromJson(customerAsJson);
            var itemAsAttributes = itemAsDocument.ToAttributeMap();

            // Create an update item request and call it on the database
            var updateItemRequest = new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>() 
                { 
                    { "DealerId", new AttributeValue { S = oldDealerId } }, 
                    { "DbsId", new AttributeValue { S = oldDbsId } } 
                },
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    {"#DLI", "DealerId" },
                    {"#DI", "DbsId" },
                    {"#BU", "BaseUrl" },
                    {"#SI", "SubscriptionId"}
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    {":dealerId", new AttributeValue { S = dealer.DealerId } },
                    {":dbsId", new AttributeValue { S = dealer.DbsId} },
                    {":base", new AttributeValue {S = dealer.BaseUrl} },
                    {":sub", new AttributeValue { S = dealer.SubscriptionId} },
                },
                UpdateExpression = "SET #DI = :dbsId, #BU = :base, #SI = :sub, #DLI = :dealerId"
            };
            var response = await client.UpdateItemAsync(updateItemRequest);
            // var response = await dynamoDB.PutItemAsync(updateItemRequest);
            
            // Return the status code to indicate whether it was successful
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        /****************************************************
         * Get Async method takes in the dbsId and dealerId
         * and uses a queryRequest to get the specified item
         * within the table and return it as a class.
         ****************************************************/
        public async Task<DealerInfo[]?> GetAsync(string dbsId, string dealerId)
        {
            // Create Query Request to retrieve the specified item from the table
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "DbsId = :v_dbsId and DealerId = :v_dealerId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_dbsId", new AttributeValue {S = dbsId} },
                    {":v_dealerId", new AttributeValue {S = dealerId} }
                }
            };
            var response = await client.QueryAsync(request);
            var lines = response.Items;

            // If there is not items then the item doesn't exist
            if (response.Items.Count == 0)
            {
                return null;
            }
            // Otherwise fill dealer info class items with the items values
            DealerInfo[] list = new DealerInfo[_pageLimit];
            int j = 0;
            for (int i = 0; i < lines.Count && j < _pageLimit; i++)
            {
                DealerInfo dealerInfo = new DealerInfo();
                dealerInfo.DbsId = lines[i]["DbsId"].ToString();
                dealerInfo.DealerId = lines[i]["DealerId"].ToString();
                dealerInfo.BaseUrl = lines[i]["BaseUrl"].ToString();
                if (lines[i]["SubscriptionId"] != null)
                {
                    dealerInfo.SubscriptionId = lines[i]["SubscriptionId"].ToString();
                }
                list.SetValue(dealerInfo, j);
                j++;
            }
            // Return the resulting list or single item to be displayed
            return list;
        }

        /****************************************************
         * The GetDbsIdPages takes in the dbsId and returns a list
         * of all dictionary items from a query request. This list
         * contains all the items in the list where the dbsId matches
         * the given value.
         ****************************************************/
        public async Task<List<Dictionary<string, AttributeValue>>?> GetDbsIdPages(string dbsId)
        {
            // Create a query request where the dbsId in the table matches the given value
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "DbsId = :v_dbsId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_dbsId", new AttributeValue {S = dbsId} }
                }
            };
            var response = await client.QueryAsync(request);

            // Return the list of items from the response of the query
            return response.Items;
        }

        /****************************************************
         * The Get by Dbs Id Async value takes in the dbsId, paginator,
         * and pageNum to then query the table and return the next set
         * _pageLimit values from the table that match the dbsId. This
         * then puts them into a DealerInfo array of items to be returned
         * to the controller to be interpreted and passed to the view.
         ****************************************************/
        public async Task<DealerInfo[]?> GetByDbsIdAsync(string dbsId, string paginator, int pageNum)
        {
            var lines = await GetDbsIdPages(dbsId);

            // For each item in the response convert the values and input them into the list
            DealerInfo[] list = new DealerInfo[_pageLimit];
            if ((lines.Count / _pageLimit) >= pageNum)
            {
                int j = 0;
                for (int i = _pageLimit * pageNum; i < lines.Count && j < _pageLimit; i++)
                {
                    DealerInfo dealerInfo = new DealerInfo();
                    dealerInfo.DbsId = lines[i]["DbsId"].ToString();
                    dealerInfo.DealerId = lines[i]["DealerId"].ToString();
                    dealerInfo.BaseUrl = lines[i]["BaseUrl"].ToString();
                    if (lines[i]["SubscriptionId"] != null)
                    {
                        dealerInfo.SubscriptionId = lines[i]["SubscriptionId"].ToString();
                    }
                    list.SetValue(dealerInfo, j);
                    j++;
                }
            }
            else return null;

            // Return the list of DealerInfo items
            return list;
        }

        /****************************************************
         * The GetDealerIdPages method takes in the dealerId and returns a list
         * of all dictionary items from a query request. This list
         * contains all the items in the list where the DealerId matches
         * the given value.
         ****************************************************/
        public async Task<List<Dictionary<string, AttributeValue>>?> GetDealerIdPages(string dealerId)
        {
            // Create a query request to return all items with Dealer Id that matches the dealer Id given
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "DealerId = :v_dealerId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    {":v_dealerId", new AttributeValue {S = dealerId} }
                }
            };
            var response = await client.QueryAsync(request);

            // Return the list of items in the response to be used elsewhere
            return response.Items;
        }

        /****************************************************
         * The Get by Dealer Id Async value takes in the dealerId, paginator,
         * and pageNum to then query the table and return the next set
         * _pageLimit values from the table that match the dealerId. This
         * then puts them into a DealerInfo array of items to be returned
         * to the controller to be interpreted and passed to the view.
         ****************************************************/
        public async Task<DealerInfo[]?> GetByDealerIdAsync(string dealerId, string paginator, int pageNum)
        {
            var lines = await GetDealerIdPages(dealerId);

            // For each item in the response convert the values and input them into the list
            DealerInfo[] list = new DealerInfo[_pageLimit];
            if ((lines.Count / _pageLimit) >= pageNum)
            {
                int j = 0;
                for (int i = _pageLimit * pageNum; i < lines.Count && j < _pageLimit; i++)
                {
                    DealerInfo dealerInfo = new DealerInfo();
                    dealerInfo.DbsId = lines[i]["DbsId"].ToString();
                    dealerInfo.DealerId = lines[i]["DealerId"].ToString();
                    dealerInfo.BaseUrl = lines[i]["BaseUrl"].ToString();
                    if (lines[i]["SubscriptionId"] != null)
                    {
                        dealerInfo.SubscriptionId = lines[i]["SubscriptionId"].ToString();
                    }
                    list.SetValue(dealerInfo, j);
                    j++;
                }
            }
            else return null;

            // Return the list of DealerInfo items
            return list;
        }
    }
}
