using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using GenericPayment.Database;
using GenericPayment.Models;
using Newtonsoft.Json;
using PayPal.Api;
using Payer = PayPal.Api.Payer;

namespace GenericPayment.Controllers
{
    public class PayPalController : Controller
    {
        /// <summary>
        /// TO USE PAYPAL REST API:
        /// CONFIG YOUR CLIENTID AND CLIENTSECREAT IN THE WEB.CONFIG
        /// MORE INFORMATION: https://github.com/paypal/PayPal-NET-SDK/
        /// </summary>
        /// <returns></returns>

        public async Task<ActionResult> Index(string paykey)
        {

            var marketplaceUrl = "http://localhost";
            if (Request.UrlReferrer != null)
            {
                marketplaceUrl = Request.UrlReferrer.ToString();
            }
            var uri = new Uri(marketplaceUrl);
            marketplaceUrl = uri.Scheme + Uri.SchemeDelimiter + uri.Authority;
            var db = new DbContext();
            try
            {
                var details = db.GetDetails(paykey);
                if (details != null)
                {
                    // Update details for the valid record
                    PaymentDetails vm = new PaymentDetails();
                    vm.CashKey = details.PayKey;
                    vm.Currency = details.Currency;
                    decimal total;
                    if (!decimal.TryParse(details.Total, out total))
                    {
                        total = 0m;
                    }
                    vm.Total = total;
                    vm.Note = "";

                    // Call Arcadier api to get the details 
                    using (var httpClient = new HttpClient())
                    {
                        var url = marketplaceUrl + "/user/checkout/order-details" + "?gateway=" + details.Gateway + "&invoiceNo=" + details.InvoiceNo + "&paykey=" + paykey + "&hashkey=" + details.Hashkey;
                        HttpResponseMessage tokenResponse = await httpClient.GetAsync(url);
                        tokenResponse.EnsureSuccessStatusCode();
                        string text = await tokenResponse.Content.ReadAsStringAsync();

                        // Set the details to db
                        GenericPayments response = JsonConvert.DeserializeObject<GenericPayments>(text);
                        details.PayeeInfos = response.PayeeInfos;
                        details.MarketplaceUrl = marketplaceUrl;
                        db.SetDetails(paykey, details);
                    }

                    return View("Index", vm);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("Error");
        }

        public JsonResult PayWithPayPal(AgreementViewModel vm)
        {
            string ppurl = "";
            var db = new DbContext();
            try
            {
                var details = db.GetDetails(vm.CashKey);
                if (details != null)
                {
                    details.AgreedDateTime = DateTime.UtcNow;
                    details.Note = vm.Note;

                    // Get a reference to the config
                    var config = ConfigManager.Instance.GetProperties();

                    // Use OAuthTokenCredential to request an access token from PayPal
                    var accessToken = new OAuthTokenCredential(config).GetAccessToken();

                    string host = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority;

                    // Build the PayPal payment
                    var payment = new Payment()
                    {
                        intent = "sale",
                        payer = new Payer()
                        {
                            payment_method = "paypal"
                        },
                        transactions = new List<Transaction>()
                        {
                            new Transaction()
                            {
                                amount = new Amount()
                                {
                                    currency = details.Currency,
                                    total = details.Total
                                },
                                //reference_id = details.InvoiceNo,
                                description = "Invoice " + details.InvoiceNo,
                                invoice_number = vm.CashKey,
                                note_to_payee = vm.Note ?? "",
                                item_list = new ItemList()
                            }
                        },
                        redirect_urls = new RedirectUrls()
                        {
                            return_url = host + "/paypal/return?key=" + vm.CashKey,
                            cancel_url = host + "/paypal/return?cancel=true&key=" + vm.CashKey
                        },
                    };

                    // Create an APIContext
                    var apiContext = new APIContext(accessToken);

                    // Create a payment using a valid APIContext
                    var createdPayment = payment.Create(apiContext);

                    if (createdPayment != null && createdPayment.links.Count > 0)
                    {
                        var id = createdPayment.id;
                        details.PayPalId = id;
                        bool result = db.SetDetails(vm.CashKey, details);
                        if (result)
                        {
                            var links = createdPayment.links.GetEnumerator();
                            while (links.MoveNext())
                            {
                                var link = links.Current;
                                if (link != null && link.rel.ToLower().Trim().Equals("approval_url"))
                                {
                                    ppurl = link.href;
                                }
                            }
                        }
                    }                 
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ppurl = "";
            }

            // Use js to redirect to PayPal with the received approval_url above
            return Json(new {result = ppurl}, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelToPay(AgreementViewModel vm)
        {
            var url = DbContext.CancelUrl(vm.CashKey);
            return Json(new { result = url }, JsonRequestBehavior.AllowGet);
        }


        // PayPal will return to this endpoint
        // If cancel = true, the user has cancelled in PayPal
        [ActionName("return")]
        public ActionResult PayPalReturn(string paymentId, string token, string PayerID, string key, bool cancel = false)
        {
            string result;
            if (cancel || string.IsNullOrEmpty(PayerID))
            {
                result = DbContext.CancelUrl(key);
                return Redirect(result);
            }

            var db = new DbContext();
            var details = db.GetDetails(key);
            if (details != null && details.PayPalId == paymentId)
            {
                var paymentExecution = new PaymentExecution() {payer_id = PayerID };
                var payment = new Payment() {id = details.PayPalId};

                // Get a reference to the config
                var config = ConfigManager.Instance.GetProperties();

                // Use OAuthTokenCredential to request an access token from PayPal
                var accessToken = new OAuthTokenCredential(config).GetAccessToken();

                // Create an APIContext
                var apiContext = new APIContext(accessToken);
                // Execute the payment.
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                // Build the success url and redirect back to Arcadier
                result = DbContext.SuccessUrl(key, "");
                return Redirect(result);
            }
            else
            {
                // Build the failure url and redirect back to Arcadier
                result = DbContext.CancelUrl(key);
                return Redirect(result);
            }
        }
    }
}