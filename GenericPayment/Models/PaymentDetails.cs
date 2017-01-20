using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GenericPayment.Models
{
    public class GenericPaymentRequest
    {
        public string invoiceno { get; set; }
        public string total { get; set; }
        public string currency { get; set; }
        public string gateway { get; set; }
        public string hashkey { get; set; }
    }

    public class AgreementViewModel
    {
        public string CashKey { get; set; }
        public string Note { get; set; }
    }

    public class PaymentDetails
    {
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public string CashKey { get; set; }
        public string Note { get; set; }
    }

    public class GenericPayments
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MarketplaceUrl { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Payee> PayeeInfos { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string InvoiceNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PayKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Total { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Gateway { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Hashkey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? AgreedDateTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PayPalId { get; set; }
    }

    public class Payer
    {

    }

    public class Payee
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal Total { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Item> Items { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Reference { get; set; }
    }

    public class Item
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Currency { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Price { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Quantity { get; set; }
    }
}