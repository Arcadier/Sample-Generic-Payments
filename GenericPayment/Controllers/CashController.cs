using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using GenericPayment.Database;
using GenericPayment.Models;
using Newtonsoft.Json;

namespace GenericPayment.Controllers
{
    public class CashController : Controller
    {
        public async Task<ActionResult> Agreement(string paykey)
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

        public JsonResult AgreeToPay(AgreementViewModel vm)
        {
            // Build the success url and redirect back to Arcadier
            var url = DbContext.SuccessUrl(vm.CashKey, vm.Note);
            return Json(new { result = url }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelToPay(AgreementViewModel vm)
        {
            // Build the failure url and redirect back to Arcadier
            var url = DbContext.CancelUrl(vm.CashKey);
            return Json(new { result = url }, JsonRequestBehavior.AllowGet);
        }
    }
}
