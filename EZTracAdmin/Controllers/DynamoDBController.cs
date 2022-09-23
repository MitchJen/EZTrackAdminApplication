using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using EZTracAdminRSC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLLEZtracAdmin;
using Dealer = EZTracAdminRSC.Models.Dealer;
using Microsoft.Extensions.Logging;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace EZTracAdminRSC.Controllers
{
    public class DynamoDBController : Controller
    {
        // GET: Dealer List (Index of Dealers Table)
        /****************************************************
         * This method sends a request over to the BLL layer
         * to scan the table and return a list of items from
         * the table in paginated form. It also creates a list
         * of Dealers with the DealerList class in Dealer.cs.
         * It then passes the created list to the view to be
         * displayed.
         *
         *
         * Make sure to change the dealerList.Pages Value to be dealerLogic.GetPages()
         * This will ensure that the number of total pages is correct.
         ****************************************************/
        [HttpGet]
        public async Task<IActionResult> DealerList()
        {
            BLLDealers dealerLogic = new();
            var dealerInfoList = await dealerLogic.LoadTable(null, 1);
            // int pages = await dealerLogic.GetPages();
            DealerList dealerList = new();
            List<Dealer> dealers = new();
            dealerList.CurrentPage = 1;
            dealerList.Pages = 1; // await dealerLogic.GetPages();// pages;
            foreach (var line in dealerInfoList)
            {
                Dealer curDealer = new();
                curDealer.DbsId = line.DbsId.ToString();
                curDealer.DealerId = line.DealerId.ToString();
                curDealer.BaseUrl = line.BaseUrl.ToString();
                if (line.SubscriptionId != null)
                {
                    curDealer.SubscriptionId = line.SubscriptionId.ToString();
                }
                dealers.Add(curDealer);
            }
            dealerList.Dealers = dealers;
            return View(dealerList);
        }

        // POST: Dealer List (Search And Pagination of Index Table)
        /****************************************************
         * This method takes in a group of inputs:
         *    searchDbsId (string) - The Dbs Id input field above the table
         *    searchDealerId (string) - The Dealer Id input field above the table
         *    lastKey (string) - the DealerId of the last dealer in the list
         *    page (int) - the current page of the view of the table
         *    search (string) - indicates what search has been done whether that be dbsId or dealerId
         *    searchValue (string) - The searched value passed in by user (either DbsId or DealerId)
         *    
         * Given these inputs this will either call the BLL layer to
         * get the next _pageLimit of dealers from the table to display to the
         * view. Or Query the table from the dbsId or Dealer Id to reduce the
         * lines of the table to display only selected values.
         * 
         * There are two post forms on the view the next button and the
         * search function. Each is interpreted to either go to the next group of
         * lines or to show the results of all those that match the queried key.
         ****************************************************/
        [HttpPost]
        public async Task<IActionResult> DealerList(string searchDbsId, 
                                                    string searchDealerId, 
                                                    string lastKey, 
                                                    int page, 
                                                    string search, 
                                                    string searchValue)
        {
            BLLDealers dealerLogic = new();
            DealerInfo[] dealerInfoList = new DealerInfo[BLLDealers._pageLimit];
            DealerList dealerList = new();
            List<Dealer> dealers = new();
            int numPages = 0;

            // Check the lastKey Value because it's a hidden value that will only be non-null when next is pressed
            if (lastKey != null)
            {
                // If the search value is null means that we haven't searched anything yet so just move to the next set
                if (search == null)
                {
                    dealerInfoList = await dealerLogic.LoadTable(lastKey, page);
                    numPages = await dealerLogic.GetPages();
                }
                // We have searched for the DbsId so pan to the next set of those that match the given DbsId
                else if (search == "DbsId" && searchValue != null)
                {
                    dealerInfoList = await dealerLogic.GetByDbsIdAsync(searchValue, lastKey, page);
                    var dbsPages = await dealerLogic.GetDbsIdPages(searchValue);
                    numPages = dbsPages.Count / BLLDealers._pageLimit;
                }
                // We searched the Dealer Id so display the next set of dealers that match the Dealer Id
                else if (search == "DealerId" && searchValue != null)
                {
                    dealerInfoList = await dealerLogic.GetByDealerIdAsync(searchValue, lastKey, page);
                    var dealerPages = await dealerLogic.GetDealerIdPages(searchValue);
                    numPages = dealerPages.Count / BLLDealers._pageLimit;
                }
                // The search value is null which means we searched for both dealerId and dbsId
                else if (searchValue == null)
                {
                    return View();
                }
                dealerList.CurrentPage = page + 1;  // Current page increments by one
            }
            // This indicates that the search button has been pressed
            else if (lastKey == null)
            {
                // When Searching both Dbs Id and Dealer Id return the first set related
                if ((searchDbsId != null) && (searchDealerId != null))
                {
                    dealerInfoList.SetValue(await dealerLogic.GetAsync(searchDbsId, searchDealerId), 0);
                    TempData["DbsDealerSearch"] = "Searching By Dbs Id And Dealer Id";
                    numPages = 1;
                }
                // When Searching only the Dbs Id return the correlated set
                else if ((searchDbsId != null) && (searchDealerId == null))
                {
                    dealerInfoList = await dealerLogic.GetByDbsIdAsync(searchDbsId, null, 0);
                    TempData["DbsSearch"] = "Searching By Dbs Id";
                    var dbsPages = await dealerLogic.GetDbsIdPages(searchValue);
                    numPages = dbsPages.Count / BLLDealers._pageLimit;
                }
                // When Searching only the Dealer Id return the correlated set
                else if ((searchDbsId == null) && (searchDealerId != null))
                {
                    dealerInfoList = await dealerLogic.GetByDealerIdAsync(searchDealerId, null, 0);
                    TempData["DealerSearch"] = "Searching By Dealer Id";
                    var dealerPages = await dealerLogic.GetDealerIdPages(searchValue);
                    numPages = dealerPages.Count / BLLDealers._pageLimit;
                }
                
                dealerList.CurrentPage = 1; // Current page index is 1 because we are just starting here
            }
            // Loop through the resulting list of dealers returned from BLL layer and format into DealerList class
            int i = 0;
            while (dealerInfoList[i] != null && i < BLLDealers._pageLimit)
            {
                Dealer curDealer = new();
                curDealer.DbsId = dealerInfoList[i].DbsId.ToString();
                curDealer.DealerId = dealerInfoList[i].DealerId.ToString();
                curDealer.BaseUrl = dealerInfoList[i].BaseUrl.ToString();
                if (dealerInfoList[i].SubscriptionId != null)
                {
                    curDealer.SubscriptionId = dealerInfoList[i].SubscriptionId.ToString();
                }

                dealers.Add(curDealer);
                i++;
            }
            dealerList.Pages = numPages;    // This will hold the value of how many pages exist related to the index
            dealerList.Dealers = dealers;   // This will hold a list of all the current dealers being displayed
            return View(dealerList);
        }

        // GET: Add (Create New Dealer Item For Table)
        /****************************************************
         * This method Handles the add request which returns the add view.
         ****************************************************/
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // POST: Add (Create New Dealer Item for Table)
        /****************************************************
         * This method takes the inputted Dealer from the user
         * through the view. Then sends a request to the BLL
         * layer to add the inputted dealer info into the table.
         ****************************************************/
        [HttpPost]
        public async Task<IActionResult> Add(Dealer dealer)
        {
            // Verify that the given input fits the designed structure
            if (!ModelState.IsValid)
            {
                return View(dealer);
            }

            // Send a request to create the dealer in the table if so send a success message else send error
            BLLDealers dealerLogic = new();
            if (await dealerLogic.CreateAsync(dealer.DbsId, 
                                                dealer.DealerId, 
                                                dealer.BaseUrl, 
                                                dealer.SubscriptionId))
            {
                TempData["AlertMessage"] = "Dealer Has Been Added To Database.";
            }
            else
            {
                TempData["AlertMessage"] = "Dealer Already Exists In Database.";
            }

            return RedirectToAction("Add");
        }

        // GET: View (Review Dealer Item and Fields) -- No Post because No submission needed
        /****************************************************
         * This method takes in all values within a Dealer item
         * and displays that information in a table back to the
         * user to easily review the information.
         * 
         * Inputs are:
         *    - Dbs Id (String)
         *    - Dealer Id (string)
         *    - Base Url (string)
         *    - Subscription Id (string)
         ****************************************************/
        [HttpGet]
        public IActionResult View(string dbsId, string dealerId, string baseUrl, string subscriptionId)
        {
            // Initialize the dealer to be viewed and set the given values to their respective fields
            Dealer dealer = new();
            dealer.DbsId = dbsId;
            dealer.DealerId = dealerId;
            dealer.BaseUrl = baseUrl;
            if (subscriptionId != null) dealer.SubscriptionId = subscriptionId;
            return View(dealer);
        }

        // GET: Edit (Update Specified Dealer Item)
        /****************************************************
         * This method takes in the dealer fields related to the
         * selected row to be edited. It then returns a form to the
         * user populated with the given values to be manipulated by
         * the user and updated.
         ****************************************************/
        [HttpGet]
        public IActionResult Edit(string dbsId, string dealerId, string baseUrl, string subscriptionId)
        {
            // Initialize the dealer item and set the given values to their respective fields
            Dealer dealer = new();
            dealer.DbsId = dbsId;
            dealer.DealerId = dealerId;
            dealer.BaseUrl = baseUrl;
            if (subscriptionId != null) dealer.SubscriptionId = subscriptionId;
            return View(dealer);
        }

        // POST: Edit (Update Specified Dealer Item)
        /****************************************************
         * Upon posting the changes this method will take in the
         * input fields provided by the user along with the old dbs id
         * and the old dealer Id to pass along which specific dealer line
         * is to be updated within the table. This method will then pass
         * a request to the BLL layer to update the given dealer line
         * to the inputted values after making sure that the values are valid.
         * 
         * Inputs:
         *    - Dealer (inputted dealer item)
         *    - Old Dbs Id (string: dbsId of line to be updated)
         *    - Old Dealer Id (string: dealerId of line to be updated)
         ****************************************************/
        [HttpPost]
        public async Task<IActionResult> Edit(Dealer dealer, string oldDbsId, string oldDealerId)
        {
            // Create a dealer buisiness logic item to be used
            BLLDealers dealerLogic = new BLLDealers();

            // If the update call to the BLL layer is successful display success message otherwize note it
            if (await dealerLogic.UpdateAsync(oldDbsId, 
                                                oldDealerId, 
                                                dealer.DbsId, 
                                                dealer.DealerId, 
                                                dealer.BaseUrl, 
                                                dealer.SubscriptionId))
            {
                TempData["AlertMessage"] = "Dealer Has Successfully Been Updated.";
            }
            else
            {
                TempData["AlertMessage"] = "Dealer Has Successfully Been Added To The Database.";
            }
            return View();
        }
    }
}