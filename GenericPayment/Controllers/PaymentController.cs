using System;
using System.Web.Mvc;
using GenericPayment.Database;
using GenericPayment.Models;

namespace GenericPayment.Controllers
{
    public class PaymentController : Controller
    {
        // Generate a paykey for each request
        [HttpPost]
        public JsonResult GeneratePaykey(GenericPaymentRequest request)
        {
            try
            {
                var randomKey = Utilities.Utils.GenerateRandomID(10);
                var cashPayment = new GenericPayments();
                cashPayment.Total = request.total;
                cashPayment.InvoiceNo = request.invoiceno;
                cashPayment.Currency = request.currency;
                cashPayment.PayKey = randomKey;
                cashPayment.Gateway = request.gateway;
                cashPayment.Hashkey = request.hashkey;

                // Save details to db
                var db = new DbContext();
                db.SetDetails(randomKey, cashPayment);

                return Json(randomKey, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }
    }
}